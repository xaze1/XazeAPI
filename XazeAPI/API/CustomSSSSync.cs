using System;
using System.Collections.Generic;
using System.Linq;
using CentralAuth;
using HarmonyLib;
using LabApi.Events.Handlers;
using LabApi.Features.Wrappers;
using Mirror;
using UnityEngine;
using UserSettings.ServerSpecific;
using XazeAPI.API.Extensions;

namespace XazeAPI.API
{
    public static class CustomSSSSync
    {
        private static readonly List<ServerSpecificSettingBase> _globalSettings = new();
        public static IReadOnlyList<ServerSpecificSettingBase> GlobalDefinedSettings => _globalSettings.AsReadOnly();
        public static readonly Dictionary<ReferenceHub, ServerSpecificSettingBase[]> DefinedSettings = new();
        public static Predicate<Player> SendOnJoinFilter { get; set; } = null;
        
        public static void Init()
        {
            PlayerEvents.Joined += args =>
            {
                var plr = args.Player;
                if (!plr.IsPlayer)
                {
                    return;
                }
                
                if (SendOnJoinFilter == null || SendOnJoinFilter(plr))
                {
                    SendToPlayer(plr.ReferenceHub);
                }
            };
            CustomNetworkManager.OnClientReady += delegate
            {
                NetworkServer.ReplaceHandler<SSSClientResponse>(ServerProcessClientResponseMsg);
            };
            StaticUnityMethods.OnUpdate += UpdateDefinedSettings;
        }

        public static void AddGlobalSettings(ServerSpecificSettingBase setting)
        {
            if (_globalSettings.Any(s => s.SettingId == setting.SettingId))
                throw new ArgumentException("ServerSpecificSettingBase already exists with the specified SettingId.");
            
            _globalSettings.Add(setting);
        }

        public static void AddGlobalSettings(IEnumerable<ServerSpecificSettingBase> settings)
        {
            foreach (var setting in settings)
            {
                AddGlobalSettings(setting);
            }
        }

        public static void SendToPlayer(ReferenceHub hub)
        {
            if (!NetworkServer.active)
            {
                return;
            }

            if (!DefinedSettings.TryGetValue(hub, out var settings))
            {
                settings = GlobalDefinedSettings.ToArray();
            }

            DefinedSettings[hub] = settings;
            hub.connectionToClient.Send(new SSSEntriesPack(settings, ServerSpecificSettingsSync.Version));
        }

        public static void UpdateDefinedSettings()
        {
            try
            {
                if (StaticUnityMethods.IsPlaying)
                {
                    DefinedSettings.ForEach(delegate (KeyValuePair<ReferenceHub, ServerSpecificSettingBase[]> x)
                    {
                        DictionaryExtensions.ForEach(x.Value, y => y.OnUpdate());
                    });
                }
            }
            catch (Exception exception)
            {
                Debug.LogException(exception);
            }
        }

        public static bool ServerPrevalidateClientResponse(SSSClientResponse msg, ReferenceHub user)
        {
            if (DefinedSettings == null)
            {
                return false;
            }

            if (DefinedSettings.TryGetValue(user, out var definedSettings))
                return definedSettings.Any(serverSpecificSettingBase => serverSpecificSettingBase.SettingId == msg.Id &&
                                                                        !(serverSpecificSettingBase.GetType() !=
                                                                          msg.SettingType));
            if (GlobalDefinedSettings.IsEmpty())
            {
                return false;
            }

            return GlobalDefinedSettings.Any(serverSpecificSettingBase => serverSpecificSettingBase.SettingId == msg.Id && !(serverSpecificSettingBase.GetType() != msg.SettingType));
        }

        public static void ServerProcessClientResponseMsg(NetworkConnection conn, SSSClientResponse msg)
        {
            if (!ReferenceHub.TryGetHub(conn, out var hub) || !ServerPrevalidateClientResponse(msg, hub))
            {
                ServerSpecificSettingsSync.ServerProcessClientResponseMsg(conn, msg);
                return;
            }

            var orAdd = ServerSpecificSettingsSync.ReceivedUserSettings.GetOrAdd(hub, () => new List<ServerSpecificSettingBase>());
            NetworkReaderPooled reader = NetworkReaderPool.Get(msg.Payload);
            foreach (var item in orAdd.Where(item => item.SettingId == msg.Id && !(item.GetType() != msg.SettingType)))
            {
                ServerSpecificSettingsSync.ServerDeserializeClientResponse(hub, item, reader);
                return;
            }

            ServerSpecificSettingBase serverSpecificSettingBase = ServerSpecificSettingsSync.CreateInstance(msg.SettingType);
            orAdd.Add(serverSpecificSettingBase);
            serverSpecificSettingBase.SetId(msg.Id, null);
            serverSpecificSettingBase.ApplyDefaultValues();
            ServerSpecificSettingsSync.ServerDeserializeClientResponse(hub, serverSpecificSettingBase, reader);
        }
    }
}
