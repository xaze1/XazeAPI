// Copyright (c) 2025 xaze_
//
// This source code is licensed under the MIT license found in the
// LICENSE file in the root directory of this source tree.
// 
// I <3 🦈s :3c

using MEC;
using Mirror;
using PlayerRoles;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using UnityEngine;
using VoiceChat;
using LabApi.Loader.Features.Paths;
using NetworkManagerUtils.Dummies;
using LabApi.Features.Wrappers;
using XazeAPI.API.Extensions;

namespace XazeAPI.API.AudioCore.FakePlayers
{
    public class AudioManager
    {
        public static List<string> RandomNames = ["goober", "silly", "god", "watcher", "the unknown", "super", "sp3c1alN4m3", "NeverGonnaGiveYouUp", "NeverSeeItComing", "alwaysHiding", "furry", "protogen", "MAYHEM!!!!!", "YEAHHHH BABY!!!", "despocito spider"];
        public static AudioManager? Instance { get; private set; }

        public static List<ReferenceHub> ActiveFakes = new();
        public static Dictionary<FakeConnection, ReferenceHub> FakeConnections = new();
        public static Dictionary<int, ReferenceHub> FakeConnectionsIds = new();

        public static Action<CustomAudioPlayer> OnFakeDestroyed;

        private static int _id = 0;

        public static int CurId
        {
            get => _id;
            set => _id = value;
        }

        public static string AudioPath { get; private set; } = Path.Combine(Path.Combine(PathManager.LabApi.FullName, "XazeAPI"), "Audio");

        /// <summary>
        /// Sets the Audio System up
        /// </summary>
        public static void Awake(Assembly audioAssembly)
        {
            Instance ??= new AudioManager();

            Directory.CreateDirectory(AudioPath);
            Logging.Debug($"Resources: {audioAssembly.GetManifestResourceNames().Length}", APILoader.Debug);

            foreach(var resource in audioAssembly.GetManifestResourceNames())
            {
                if (!resource.EndsWith(".ogg"))
                    continue;

                Logging.Debug($"Looking at {resource}", APILoader.Debug);

                int lastDotIndex = resource.LastIndexOf('.');
                int secondLastDotIndex = resource.LastIndexOf('.', lastDotIndex - 1);

                string fileName = resource.Substring(secondLastDotIndex + 1);
                string path = Path.Combine(AudioPath, fileName);
                if (File.Exists(path))
                {
                    continue;
                }

                using var resourceStream = audioAssembly.GetManifestResourceStream(resource);
                using var file = File.Open(path, FileMode.Create);
                resourceStream.CopyTo(file);

                Logging.Debug($"Extracted {fileName} to {path}", APILoader.Debug);
            }

            Logging.Info("Audio System Loaded!");
        }

        /// <summary>
        /// Increases the Value of <see cref="CurId"/> and returns it
        /// </summary>
        /// <returns></returns>
        public static int GetNextId()
        {
            while (FakeConnectionsIds.ContainsKey(CurId))
            {
                CurId += 1;
            }

            return CurId;
        }

        /// <summary>
        /// Creates a Fake Player to play Audio with
        /// </summary>
        /// <returns><see cref="ReferenceHub"/> of the Fake Player</returns>
        public static CustomAudioPlayer createFake(string nickname = null, int id = -1, RoleTypeId role = RoleTypeId.Spectator, bool hidePlayerList = true)
        {
            if (id == -1)
                id = DummyNetworkConnection._idGenerator--;

            var newPlayer = UnityEngine.Object.Instantiate(NetworkManager.singleton.playerPrefab);
            var fakeConnection = new FakeConnection(id);

            var hubPlayer = newPlayer.GetComponent<ReferenceHub>();
            Player fakePlayer = Player.Get(hubPlayer);
            FakeConnections.Add(fakeConnection, hubPlayer);
            FakeConnectionsIds.Add(id, hubPlayer);
            ActiveFakes.Add(hubPlayer);

            NetworkServer.AddPlayerForConnection(fakeConnection, newPlayer);

            hubPlayer.characterClassManager._godMode = true;
            hubPlayer.authManager.InstanceMode = CentralAuth.ClientInstanceMode.Dummy;
            hubPlayer.characterClassManager.Start();
            hubPlayer.serverRoles.Start();

            try
            {
                hubPlayer.authManager._privUserId = $"Dummy{id}@server";

                if (hidePlayerList)
                    hubPlayer.authManager.NetworkSyncedUserId = null;
                else
                    hubPlayer.authManager.NetworkSyncedUserId = hubPlayer.authManager.UserId;
            }
            catch (Exception e)
            {
                Logging.Error($"[AudioSystem] Exception when creating Fake Player\n" + e);
            }

            try
            {
                if (nickname == null)
                    hubPlayer.nicknameSync.Network_myNickSync = $"Dummy player {id}";
                else
                    hubPlayer.nicknameSync.Network_myNickSync = nickname;
            }
            catch (Exception e)
            {
                Logging.Error($"[AudioSystem] Exception when setting nickname for Fake Player\n" + e);
            }

            CustomAudioPlayer cplayer = CustomAudioPlayer.Get(hubPlayer);
            cplayer.BroadcastChannel = VoiceChatChannel.Intercom;
            cplayer.Volume = 15;

            // hubPlayer.playerStats.GetModule<AdminFlagsStat>().SetFlag(AdminFlags.Noclip, true);

            Timing.CallDelayed(0.1f, () =>
            {
                hubPlayer.roleManager.ServerSetRole(role, RoleChangeReason.RemoteAdmin);
                hubPlayer.roleManager.CurrentRole._lastOwner = hubPlayer;

                Timing.CallDelayed(Timing.WaitForOneFrame, () =>
                {
                    Timing.CallDelayed(Timing.WaitForOneFrame, () =>
                    {
                        hubPlayer.SetScale(Vector3.zero);
                    });
                });
                fakePlayer.Gravity = Vector3.zero;
            });

            return cplayer;
        }
        
        /// <summary>
        /// Creates a Fake Player to play Audio with
        /// </summary>
        /// <returns><see cref="CustomAudioPlayer"/> of the Fake Player</returns>
        public static CustomAudioPlayer createFake(string nickname = null, int id = -1, VoiceChatChannel broadcastChannel = VoiceChatChannel.Proximity, bool hidePlayerList = true)
        {
            var player = createFake(nickname, id, RoleTypeId.Tutorial, hidePlayerList);
            player.BroadcastChannel = broadcastChannel;

            return player;
        }
        
        /// <summary>
        /// Creates a Fake Player to play Audio with
        /// </summary>
        /// <returns><see cref="CustomAudioPlayer"/> of the Fake Player</returns>
        public static CustomAudioPlayer createFake(string nickname = null, RoleTypeId role = RoleTypeId.Tutorial, VoiceChatChannel broadcastChannel = VoiceChatChannel.Proximity, bool hidePlayerList = true)
        {
            var player = createFake(nickname, -1, role, hidePlayerList);
            player.BroadcastChannel = broadcastChannel;

            return player;
        }

        /// <summary>
        /// Uses a random Username instead of a given one
        /// </summary>
        /// <param name="id"></param>
        /// <param name="role"></param>
        /// <param name="broadcastChannel"></param>
        /// <param name="hidePlayerList"></param>
        /// <returns></returns>
        public static CustomAudioPlayer createFake(int id = -1, VoiceChatChannel broadcastChannel = VoiceChatChannel.Proximity, bool hidePlayerList = true) => 
            createFake(RandomNames.RandomItem(), id, broadcastChannel, hidePlayerList);

        /// <summary>
        /// Puts a audio into the Queue
        /// </summary>
        /// <param name="dummyId">Id of the Fake Player</param>
        /// <param name="audio">Path to the audio File</param>
        /// <param name="audioPos">Index of where to put the Audio in the Queue</param>
        public static void Enqueue(int dummyId, string audio, int audioPos = -1)
        {
            if (FakeConnectionsIds.TryGetValue(dummyId, out ReferenceHub hub))
            {
                var audioPlayer = CustomAudioPlayer.Get(hub);
                audioPlayer.Enqueue(audio, audioPos);
            }
        }

        /// <summary>
        /// Plays a Audio file that's in the Queue
        /// </summary>
        /// <param name="dummyId">Fake Player Id</param>
        /// <param name="queuePos">Queue Index</param>
        public static void Play(int dummyId, int queuePos, VoiceChatChannel broadcastChannel = VoiceChatChannel.None)
        {
            if (FakeConnectionsIds.TryGetValue(dummyId, out ReferenceHub hub))
            {
                var audioPlayer = CustomAudioPlayer.Get(hub);
                if (broadcastChannel != VoiceChatChannel.None)
                    audioPlayer.BroadcastChannel = broadcastChannel;

                audioPlayer.Play(queuePos);
            }
        }
        
        public static void Play(ReferenceHub dummy, int queuePos, VoiceChatChannel broadcastChannel = VoiceChatChannel.None)
        {
            var audioPlayer = CustomAudioPlayer.Get(dummy);

            if (broadcastChannel != VoiceChatChannel.None)
                audioPlayer.BroadcastChannel = broadcastChannel;

            audioPlayer.Play(queuePos);
        }

        /// <summary>
        /// Destroys a Fake Player
        /// </summary>
        /// <param name="dummyId">Fake Player Id</param>
        public static void Destroy(int dummyId)
        {
            if (FakeConnectionsIds.TryGetValue(dummyId, out ReferenceHub hub))
            {
                FakeConnections.Remove(FakeConnections.FirstOrDefault(s => s.Value == hub).Key);
                FakeConnectionsIds.Remove(dummyId);
                ActiveFakes.Remove(hub);
                CustomAudioPlayer cplayer = CustomAudioPlayer.Get(hub);
                cplayer.ClearOnFinish = true;
                OnFakeDestroyed.Invoke(cplayer);
                cplayer.OnDestroy();
            }
        }
        
        /// <summary>
        /// Destroys a Fake Player
        /// </summary>
        /// <param name="cplayer">CustomAudioPlayer</param>
        public static void Destroy(CustomAudioPlayer cplayer)
        {
            int dummyId = FakeConnectionsIds.First(conn => conn.Value == cplayer.Owner).Key;

            if (cplayer.Owner.IsAlive())
            {
                cplayer.Owner.roleManager.ServerSetRole(RoleTypeId.Spectator, RoleChangeReason.Died);
            }

            if (FakeConnectionsIds.TryGetValue(dummyId, out ReferenceHub hub))
            {
                FakeConnections.Remove(FakeConnections.FirstOrDefault(s => s.Value == hub).Key);
                FakeConnectionsIds.Remove(dummyId);
                ActiveFakes.Remove(hub);
            }

            cplayer.ClearOnFinish = true;
            OnFakeDestroyed.Invoke(cplayer);
            cplayer.OnDestroy();
        }
        
        /// <summary>
        /// Destroys a Fake Player
        /// </summary>
        /// <param name="hub">Fake Player ReferenceHub</param>
        public static void Destroy(ReferenceHub hub)
        {
            int dummyId = FakeConnectionsIds.First(conn => conn.Value == hub).Key;

            if (hub.IsAlive())
            {
                hub.roleManager.ServerSetRole(RoleTypeId.Spectator, RoleChangeReason.Died);
            }

            FakeConnections.Remove(FakeConnections.FirstOrDefault(s => s.Value == hub).Key);
            FakeConnectionsIds.Remove(dummyId);
            ActiveFakes.Remove(hub);

            CustomAudioPlayer cplayer = CustomAudioPlayer.Get(hub);
            cplayer.ClearOnFinish = true;
            OnFakeDestroyed.Invoke(cplayer);
            cplayer.OnDestroy();
        }

        /// <summary>
        /// Sets te Volume of the Fake Player
        /// </summary>
        /// <param name="dummyId">Fake Player Id</param>
        /// <param name="volume">Volume of the Fake Player (1f = 100%)</param>
        public static void SetVolume(int dummyId, float volume)
        {
            if (FakeConnectionsIds.TryGetValue(dummyId, out ReferenceHub hub))
            {
                var audioPlayer = CustomAudioPlayer.Get(hub);
                audioPlayer.Volume = volume;
            }
        }
        
        /// <summary>
        /// Sets te Volume of the Fake Player
        /// </summary>
        /// <param name="dummyId">Fake Player Id</param>
        /// <param name="volume">Volume of the Fake Player (1f = 100%)</param>
        public static void SetVolume(CustomAudioPlayer audioPlayer, float volume)
        {
            audioPlayer.Volume = volume;
        }

        /// <summary>
        /// Sets the VoiceChannel that the AudioPlayer plays in
        /// </summary>
        /// <param name="dummyId">Fake Player Id</param>
        /// <param name="newChannel">The Voice Channel to switch to</param>
        public static void SetAudioChannel(int dummyId, VoiceChatChannel newChannel)
        {
            if (FakeConnectionsIds.TryGetValue(dummyId, out ReferenceHub hub))
            {
                var audioPlayer = CustomAudioPlayer.Get(hub);
                audioPlayer.BroadcastChannel = newChannel;
            }
        }

        public static void SetSpeed(int dummyId, int speed = 48000)
        {
            if (FakeConnectionsIds.TryGetValue(dummyId, out ReferenceHub hub))
            {
                var audioPlayer = CustomAudioPlayer.Get(hub);
                audioPlayer.samplesPerSecond = speed;
            }
        }
        
        public static void SetSpeed(CustomAudioPlayer audioPlayer, int speed = 48000)
        {
            audioPlayer.samplesPerSecond = speed;
        }

        public static void FakePlayerFollowTransform(CustomAudioPlayer fake, ReferenceHub target)
        {
            fake.TargetHub = target;
        }

        public static void FakePlayerFollowTransform(CustomAudioPlayer fake, Player target) =>
            FakePlayerFollowTransform(fake, target);
    }
}
