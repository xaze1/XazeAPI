// // Copyright (c) 2025 xaze_
// //
// // This source code is licensed under the MIT license found in the
// // LICENSE file in the root directory of this source tree.
// //
// // I <3 🦈s :3c

using System.Collections.Generic;
using UserSettings.ServerSpecific;
using XazeAPI.API.Extensions;

namespace XazeAPI.Features.SSS.Components;

public class SettingsList(int priority = 5) : SettingComponent(priority)
{
    public IReadOnlyList<ServerSpecificSettingBase> Settings => _settings.AsReadOnly();
    private readonly List<ServerSpecificSettingBase> _settings = new();
    
    public override ServerSpecificSettingBase[] GetSettings()
    {
        return _settings.ToArray();
    }

    public void AddSetting(ServerSpecificSettingBase setting)
    {
        _settings.Add(setting);
    }

    public void AddSettings(IEnumerable<ServerSpecificSettingBase> settings)
    {
        foreach (var setting in settings)
            AddSetting(setting);
    }

    public void RemoveSetting(ServerSpecificSettingBase setting)
    {
        _settings.Remove(setting);
    }

    public void RemoveSettings(IEnumerable<ServerSpecificSettingBase> settings)
    {
        foreach (var setting in settings)
            AddSetting(setting);
    }

    public override void Update()
    {
        Settings.Do(s => s.OnUpdate());
    }
}