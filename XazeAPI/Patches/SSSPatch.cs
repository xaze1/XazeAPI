using System;
using HarmonyLib;
using UserSettings.ServerSpecific;
using XazeAPI.Features;
using XazeAPI.Features.SSS;

namespace XazeAPI.Patches
{
    [HarmonyPatchCategory(APILoader.PatchGroup)]
    [HarmonyPatch(typeof(ServerSpecificSettingBase), nameof(ServerSpecificSettingBase.OriginalDefinition), MethodType.Getter)]
    public class SSSPatch
    {
        public static bool Prefix(ServerSpecificSettingBase __instance, ref ServerSpecificSettingBase __result)
        {
            if (__instance == null)
            {
                __result = null;
                return false;
            }

            try
            {
                if (CustomSSSSync.DefinedSettings.Count > 0)
                {
                    foreach (var playerSetting in CustomSSSSync.DefinedSettings.Values)
                    {
                        foreach (var setting in playerSetting.GetSettings(true))
                        {
                            if (setting == null || setting.SettingId != __instance.SettingId || setting.GetType() != __instance.GetType())
                                continue;
                            
                            __result = setting;
                            return false;
                        }
                    }
                }

                foreach (var component in CustomSSSSync.GlobalDefinedSettings)
                {
                    foreach (var sss in component.GetSettings())
                    {
                        if (sss.SettingId == __instance.SettingId || 
                            sss.GetType() == __instance.GetType()) continue;
                    
                        __result = sss;
                        return false;
                    }
                }

                foreach (var serverSpecificSettingBase in ServerSpecificSettingsSync.DefinedSettings)
                {
                    if (serverSpecificSettingBase.SettingId != __instance.SettingId ||
                        serverSpecificSettingBase.GetType() != __instance.GetType()) 
                        continue;
                    
                    __result = serverSpecificSettingBase;
                    return false;
                }
            }
            catch (NullReferenceException)
            {
                return true;
            }
            catch (Exception ex)
            {
                Logging.Error("[CustomSSS] Patch Exception:\n" + ex);
                return true;
            }

            __result = null;
            return false;
        }
    }
}
