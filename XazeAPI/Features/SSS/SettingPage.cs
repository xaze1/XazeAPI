// // Copyright (c) 2025 xaze_
// //
// // This source code is licensed under the MIT license found in the
// // LICENSE file in the root directory of this source tree.
// //
// // I <3 🦈s :3c

using System;
using System.Collections.Generic;
using System.Linq;
using UserSettings.ServerSpecific;
using XazeAPI.Features.SSS.Components;

namespace XazeAPI.Features.SSS;

public class SettingPage
{
    public uint Page { get; } = 0;
    public bool IsDisplayed
    {
        get => _player.CurrentPage == Page;
        set
        {
            if (value)
                _player.CurrentPage = Page;
            else if (Page != 0)
            {
                _player.CurrentPage = 0;
            }
        }
    }

    public IReadOnlyList<SettingComponent> Components => _components.AsReadOnly();

    private List<SettingComponent> _components { get; } = new();
    internal HashSet<int> _settingIds { get; } = new();
    internal bool _settingsDirty = false;
    private PlayerSettings _player { get; }
    private ServerSpecificSettingBase[] _cachedSettings;

    public void AddComponent(SettingComponent comp)
    {
        if (comp.GetSettings().Any(setting => !CheckSetting(setting)))
            return;

        _components.Add(comp);
        _settingsDirty = true;
    }

    public void AddComponents(IEnumerable<SettingComponent> components)
    {
        foreach (var setting in components)
            AddComponent(setting);
    }

    public void ClearSettings()
    {
        _components.Clear();
        _settingIds.Clear();
        _settingsDirty = true;
    }

    public bool CheckSetting(ServerSpecificSettingBase setting)
    {
        if (_settingIds.Add(setting.SettingId))
            return true;
        Logging.Error(setting.Label, "has a already existing SettingId:", setting.SettingId);
        return false;
    }

    public ServerSpecificSettingBase[] GetSettings(bool ignoreCache = false)
    {
        if (!_settingsDirty && !ignoreCache)
            return _cachedSettings;
        
        try
        {
            List<ServerSpecificSettingBase> settings = new();
            _components.Sort((a, b) => a.Priority.CompareTo(b.Priority));
            foreach (var component in Components)
            {
                settings.AddRange(component.GetSettings());
            }

            _cachedSettings = settings.ToArray();
            _settingsDirty = false;
            return _cachedSettings;
        }
        catch (Exception ex)
        {
            return [new SSTextArea(0, "Server Exception:\n" + ex)];
        }
    }

    internal void Update()
    {
        foreach (var component in Components)
            component.Update();
    }
    
    internal SettingPage(PlayerSettings player, uint page)
    {
        _player = player;
        Page = page;
    }
}