// // Copyright (c) 2025 xaze_
// //
// // This source code is licensed under the MIT license found in the
// // LICENSE file in the root directory of this source tree.
// //
// // I <3 🦈s :3c

using System.Collections.Generic;
using UserSettings.ServerSpecific;

namespace XazeAPI.Features.SSS.Components;

public class SettingsTab(int priority = 5) : SettingComponent(priority)
{
    public uint CurrentTab
    {
        get;
        set
        {
            if (value > Tabs.Count - 1)
            {
                field = (uint)Tabs.Count - 1;
                return;
            }
            field = value;
        }
    }
    public List<SettingsList> Tabs { get; } = new();
    
    public override ServerSpecificSettingBase[] GetSettings()
    {
        if (Tabs.Count == 0)
            return [];
        
        var settings = Tabs[(int)CurrentTab];
        return settings.GetSettings();
    }
}