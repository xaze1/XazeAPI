using System;
using System.Collections.Generic;
using System.Linq;
using LabApi.Events.Handlers;
using LabApi.Features.Wrappers;
using Mirror;
using UnityEngine;
using UserSettings.ServerSpecific;
using XazeAPI.API.Extensions;
using XazeAPI.Features.SSS.Components;

namespace XazeAPI.Features.SSS
{
    public static class CustomSSSSync
    {
        public static List<SettingComponent> GlobalDefinedSettings { get; } = new();
        public static readonly Dictionary<string, PlayerSettings> DefinedSettings = new();
        
        public static void Init()
        {
            PlayerEvents.Joined += args =>
            {
                var plr = args.Player;
                if (!plr.IsPlayer)
                    return;
                
                if (DefinedSettings.TryGetValue(plr.UserId, out var settings))
                    return;
                
                settings = new PlayerSettings(plr);
                settings.DefaultPage.AddComponents(GlobalDefinedSettings);
                DefinedSettings[plr.UserId] = settings;
                settings.SyncSettings();
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
        public static void AddGlobalSetting(SettingComponent component)
        {
            GlobalDefinedSettings.Add(component);
        }

        /// <summary>
        /// Use BEFORE any player has joined the Server!
        /// Settings aren't synced, only added to the global settings list
        /// </summary>
        public static void AddGlobalSettings(IEnumerable<SettingComponent> components)
        {
            foreach (var component in components)
                AddGlobalSetting(component);
        }
        
        /// <summary>
        /// Don't use duplicate Settings IDs for different settings on different players, may crash/kick them
        /// </summary>
        /// <param name="User">Player to add a setting to</param>
        /// <param name="component">Setting to add to the player's settings list</param>
        public static void AddLocalSetting(Player User, SettingComponent component)
        {
            if (!User.IsPlayer) 
                return;
            
            if (!DefinedSettings.TryGetValue(User.UserId, out var settings))
            {
                settings = new PlayerSettings(User);
                settings.DefaultPage.AddComponents(GlobalDefinedSettings);
                DefinedSettings[User.UserId] = settings;
            }
            
            settings.AddComponent(component);
            
            if (!NetworkServer.active)
                return;
            
            User.ConnectionToClient.Send(new SSSEntriesPack(settings.GetSettings(), ServerSpecificSettingsSync.Version));
        }
        
        /// <summary>
        /// Don't use duplicate Settings IDs for different settings on different players, may crash/kick them
        /// </summary>
        /// <param name="User">Player to add a setting to</param>
        /// <param name="components">Settings to add to the player's settings list</param>
        public static void AddLocalSettings(Player User, IEnumerable<SettingComponent> components)
        {
            if (!User.IsPlayer) 
                return;
            
            foreach (var component in components)
                AddLocalSetting(User, component);
        }

        /// <summary>
        /// Use BEFORE any player has joined the Server!
        /// Setting isn't synced, only removed from the global settings list
        /// </summary>
        public static void RemoveGlobalSetting(SettingComponent component)
        {
            GlobalDefinedSettings.Remove(component);
        }
        
        /// <summary>
        /// Use BEFORE any player has joined the Server!
        /// Settings aren't synced, only removed from the global settings list
        /// </summary>
        public static void RemoveGlobalSettings(IEnumerable<SettingComponent> components)
        {
            foreach (var component in components)
                RemoveGlobalSetting(component);
        }

        public static void SendToPlayer(Player User)
        {
            if (!NetworkServer.active)
                return;

            if (!DefinedSettings.TryGetValue(User.UserId, out var settings))
            {
                settings = new PlayerSettings(User);
                settings.DefaultPage.AddComponents(GlobalDefinedSettings);
                DefinedSettings[User.UserId] = settings;
            }

            User.ConnectionToClient.Send(new SSSEntriesPack(settings.GetSettings(), ServerSpecificSettingsSync.Version));
        }

        public static void UpdateDefinedSettings()
        {
            try
            {
                if (StaticUnityMethods.IsPlaying)
                    DefinedSettings.Values.ForEach(s => s.Update());
            }
            catch (Exception exception)
            {
                Debug.LogException(exception);
            }
        }

        public static bool ServerPrevalidateClientResponse(SSSClientResponse msg, ReferenceHub user)
        {
            if (DefinedSettings == null)
                return false;

            if (DefinedSettings.TryGetValue(user.authManager.UserId, out var playerSettings))
                return playerSettings.GetSettings().Any(sss => sss.SettingId == msg.Id &&
                                                               !(sss.GetType() !=
                                                                 msg.SettingType));
            if (GlobalDefinedSettings.IsEmpty())
                return false;

            return GlobalDefinedSettings.Any(component => component.GetSettings().Any(sss => sss.SettingId == msg.Id && !(sss.GetType() != msg.SettingType)));
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
