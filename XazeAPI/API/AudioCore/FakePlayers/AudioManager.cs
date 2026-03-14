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
using PlayerRoles.Spectating;
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

        public static event Action<FakeLoader> OnFakeDestroying;

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
        public static FakeLoader createFake(string nickname = null, int id = -1, RoleTypeId role = RoleTypeId.Spectator, bool hidePlayerList = true)
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

                hubPlayer.authManager.NetworkSyncedUserId = hidePlayerList ? null : hubPlayer.authManager.UserId;
            }
            catch (Exception e)
            {
                Logging.Error($"[AudioSystem] Exception when creating Fake Player\n" + e);
            }

            try
            {
                hubPlayer.nicknameSync.Network_myNickSync = nickname ?? $"Dummy player {id}";
            }
            catch (Exception e)
            {
                Logging.Error($"[AudioSystem] Exception when setting nickname for Fake Player\n" + e);
            }
            
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
                    SpectatableVisibilityManager.SetHidden(hubPlayer, true);
                });
                fakePlayer.Gravity = Vector3.zero;
            });

            return hubPlayer.gameObject.AddComponent<FakeLoader>();
        }
        
        /// <summary>
        /// Creates a Fake Player to play Audio with
        /// </summary>
        /// <returns><see cref="CustomAudioPlayer"/> of the Fake Player</returns>
        public static FakeLoader createFake(string nickname = null, int id = -1, VoiceChatChannel broadcastChannel = VoiceChatChannel.Proximity, bool hidePlayerList = true)
        {
            var player = createFake(nickname, id, RoleTypeId.Tutorial, hidePlayerList);
            player.Channel = broadcastChannel;

            return player;
        }
        
        /// <summary>
        /// Creates a Fake Player to play Audio with
        /// </summary>
        /// <returns><see cref="CustomAudioPlayer"/> of the Fake Player</returns>
        public static FakeLoader createFake(string nickname = null, RoleTypeId role = RoleTypeId.Tutorial, VoiceChatChannel broadcastChannel = VoiceChatChannel.Proximity, bool hidePlayerList = true)
        {
            var player = createFake(nickname, -1, role, hidePlayerList);
            player.Channel = broadcastChannel;

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
        public static FakeLoader createFake(int id = -1, VoiceChatChannel broadcastChannel = VoiceChatChannel.Proximity, bool hidePlayerList = true) => 
            createFake(RandomNames.RandomItem(), id, broadcastChannel, hidePlayerList);

        public static void Play(FakeLoader fake, string fileName)
        {
            fake.Play(Path.Combine(AudioPath, fileName));
        }

        public static void Destroy(FakeLoader fake)
        {
            int dummyId = FakeConnectionsIds.First(conn => conn.Value == fake.Dummy.ReferenceHub).Key;
            fake.Dummy.SetRole(RoleTypeId.Spectator);
            
            FakeConnections.Remove(FakeConnections.FirstOrDefault(s => s.Value == fake.Dummy.ReferenceHub).Key);
            FakeConnectionsIds.Remove(dummyId);
            ActiveFakes.Remove(fake.Dummy.ReferenceHub);
            
            OnFakeDestroying?.Invoke(fake);
            fake.Destroy();
            Timing.CallDelayed(1, () =>
            {
                NetworkServer.RemovePlayerForConnection(fake.Dummy.Connection, true);
            });
        }

        public static void Destroy(ReferenceHub fake)
        {
            int dummyId = FakeConnectionsIds.First(conn => conn.Value == fake).Key;
            
            FakeConnections.Remove(FakeConnections.FirstOrDefault(s => s.Value == fake).Key);
            FakeConnectionsIds.Remove(dummyId);
            ActiveFakes.Remove(fake);
            
            if (!FakeLoader.AudioPlayers.TryGetValue(fake, out var fakeLoader))
            {
                return;
            }
            
            fakeLoader.Dummy.SetRole(RoleTypeId.Spectator);
            
            OnFakeDestroying?.Invoke(fakeLoader);
            fakeLoader.Destroy();
            Timing.CallDelayed(1, () =>
            {
                NetworkServer.RemovePlayerForConnection(fakeLoader.Dummy.Connection, true);
            });
        }

        public static void FakePlayerFollowTransform(FakeLoader fake, ReferenceHub target)
        {
            fake.SetTarget(target);
        }

        public static void FakePlayerFollowTransform(FakeLoader fake, Player target)
        {
            fake.Target = target;
        }
    }
}
