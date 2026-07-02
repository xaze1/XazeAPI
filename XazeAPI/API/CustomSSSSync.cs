using System;
using System.Collections.Generic;
using System.Linq;
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
        private static readonly HashSet<int> _settingIds = new();
        private static readonly List<ServerSpecificSettingBase> _globalSettings = new();
        public static IReadOnlyList<ServerSpecificSettingBase> GlobalDefinedSettings => _globalSettings.AsReadOnly();
        public static readonly Dictionary<ReferenceHub, List<ServerSpecificSettingBase>> DefinedSettings = new();
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


        /// <summary>
        /// Use BEFORE any player has joined the Server!
        /// Setting isn't synced, only added to the global settings list
        /// </summary>
        /// <param name="setting">Setting to add to the global settings list</param>
        public static void AddGlobalSetting(ServerSpecificSettingBase setting)
        {
            if (!_settingIds.Add(setting.SettingId))
                throw new ArgumentException("ServerSpecificSettingBase already exists with the specified SettingId.");
            
            _globalSettings.Add(setting);
        }

        /// <summary>
        /// Use BEFORE any player has joined the Server!
        /// Settings aren't synced, only added to the global settings list
        /// </summary>
        /// <param name="settings">Settings to add to the global settings list</param>
        public static void AddGlobalSettings(IEnumerable<ServerSpecificSettingBase> settings)
        {
            foreach (var setting in settings)
            {
                AddGlobalSetting(setting);
            }
        }
        
        /// <summary>
        /// Don't use duplicate Settings IDs for different settings on different players, may crash/kick them
        /// </summary>
        /// <param name="User">Player to add a setting to</param>
        /// <param name="setting">Setting to add to the player's settings list</param>
        public static void AddLocalSetting(Player User, ServerSpecificSettingBase setting)
        {
            if (!User.IsPlayer) return;
            
            var hub = User.ReferenceHub;
            if (!DefinedSettings.TryGetValue(hub, out var settings))
            {
                settings = GlobalDefinedSettings.ToList();
            }
            
            if (_settingIds.Contains(setting.SettingId) || settings.Any(s => s.SettingId == setting.SettingId))
                throw new ArgumentException("ServerSpecificSettingBase already exists with the specified SettingId.");
            
            settings.AddItem(setting);
            DefinedSettings[hub] = settings;
            
            if (!NetworkServer.active)
                return;
            
            hub.connectionToClient.Send(new SSSEntriesPack(settings.ToArray(), ServerSpecificSettingsSync.Version));
        }
        
        /// <summary>
        /// Don't use duplicate Settings IDs for different settings on different players, may crash/kick them
        /// </summary>
        /// <param name="User">Player to add a setting to</param>
        /// <param name="settings">Settings to add to the player's settings list</param>
        public static void AddLocalSettings(Player User, IEnumerable<ServerSpecificSettingBase> settings)
        {
            if (!User.IsPlayer) 
                return;
            
            foreach (var setting in settings)
            {
                AddLocalSetting(User, setting);
            }
        }

        /// <summary>
        /// Use BEFORE any player has joined the Server!
        /// Setting isn't synced, only removed from the global settings list
        /// </summary>
        /// <param name="setting">Setting to removed from the global settings list</param>
        public static void RemoveGlobalSetting(ServerSpecificSettingBase setting)
        {
            if (!_settingIds.Remove(setting.SettingId))
                throw new ArgumentException("ServerSpecificSettingBase doesn't exist with the specified SettingId.");
            
            _globalSettings.Remove(setting);
        }
        
        /// <summary>
        /// Use BEFORE any player has joined the Server!
        /// Settings aren't synced, only removed from the global settings list
        /// </summary>
        /// <param name="settings">Settings to removed from the global settings list</param>
        public static void RemoveGlobalSettings(IEnumerable<ServerSpecificSettingBase> settings)
        {
            foreach (var setting in settings)
            {
                RemoveGlobalSetting(setting);
            }
        }
        
        /// <summary>
        /// </summary>
        /// <param name="User">Player to remove a setting from</param>
        /// <param name="setting">Setting to remove fromt the player's settings list</param>
        public static void RemoveLocalSetting(Player User, ServerSpecificSettingBase setting)
        {
            if (!User.IsPlayer) return;
            
            if (_settingIds.Contains(setting.SettingId))
                throw new ArgumentException("ServerSpecificSettingBase is a global setting, can't remove locally");
            
            var hub = User.ReferenceHub;
            if (!DefinedSettings.TryGetValue(hub, out var settings))
                return;
            
            if(settings.All(s => s.SettingId != setting.SettingId))
                throw new ArgumentException("ServerSpecificSettingBase doesn't exist with the specified SettingId.");
            
            settings.Remove(setting);
            DefinedSettings[hub] = settings;
            
            if (!NetworkServer.active)
                return;
            
            hub.connectionToClient.Send(new SSSEntriesPack(settings.ToArray(), ServerSpecificSettingsSync.Version));
        }
        
        /// <summary>
        /// </summary>
        /// <param name="User">Player to remove a setting from</param>
        /// <param name="settings">Settings to remove from the player's settings list</param>
        public static void RemoveLocalSettings(Player User, IEnumerable<ServerSpecificSettingBase> settings)
        {
            if (!User.IsPlayer) 
                return;
            
            foreach (var setting in settings)
            {
                RemoveLocalSetting(User, setting);
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
                settings = GlobalDefinedSettings.ToList();
                DefinedSettings[hub] = settings;
            }

            hub.connectionToClient.Send(new SSSEntriesPack(settings.ToArray(), ServerSpecificSettingsSync.Version));
        }

        public static void UpdateDefinedSettings()
        {
            try
            {
                if (StaticUnityMethods.IsPlaying)
                {
                    DefinedSettings.ForEach(delegate (KeyValuePair<ReferenceHub, List<ServerSpecificSettingBase>> x)
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
