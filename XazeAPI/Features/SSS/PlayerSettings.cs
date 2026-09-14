// // Copyright (c) 2025 xaze_
// //
// // This source code is licensed under the MIT license found in the
// // LICENSE file in the root directory of this source tree.
// //
// // I <3 🦈s :3c

using System;
using System.Collections.Generic;
using LabApi.Features.Wrappers;
using UserSettings.ServerSpecific;
using XazeAPI.Features.SSS.Components;

namespace XazeAPI.Features.SSS;

public class PlayerSettings
{
    public uint CurrentPage
    {
        get;
        set
        {
            if (field == value)
                return;
            field = value;
            SyncSettings();
        }
    } = 0;
    public IReadOnlyDictionary<uint, SettingPage> Pages => _pages;
    public SettingPage DefaultPage { get; }
    
    private Dictionary<uint, SettingPage> _pages { get; } = new();
    private Player Owner { get; }

    private bool ValidateSettings(SettingPage page)
    {
        var settingIds = new HashSet<int>();
        foreach (var setting in page.GetSettings(true))
        {
            if (settingIds.Add(setting.SettingId))
                continue;
            Logging.Error(setting.Label, "has a already existing SettingId:", setting.SettingId);
            return false;
        }

        return true;
    }

    public void SyncSettings()
    {
        if (!Pages.TryGetValue(CurrentPage, out var page) || !ValidateSettings(page))
            return;
        
        Owner.ConnectionToClient.Send(new SSSEntriesPack(page.GetSettings(), ServerSpecificSettingsSync.Version));
    }

    public SettingPage AddPage(uint pageIndex)
    {
        if (pageIndex == 0)
            throw new ArgumentException("Page Index cannot be 0");
        
        if (Pages.ContainsKey(pageIndex))
            throw new ArgumentException("Page Index already exists");
        
        var page = new SettingPage(this, pageIndex);
        _pages.Add(pageIndex, page);
        return page;
    }

    public void AddComponent(SettingComponent component)
    {
        if (!Pages.TryGetValue(CurrentPage, out var page))
            return;
        page.AddComponent(component);
    }

    public void AddComponents(IEnumerable<SettingComponent> components)
    {
        if (!Pages.TryGetValue(CurrentPage, out var page))
            return;
        page.AddComponents(components);
    }

    public ServerSpecificSettingBase[] GetSettings()
    {
        if (!Pages.TryGetValue(CurrentPage, out var page))
            return [];
        return page.GetSettings();
    }

    internal void Update()
    {
        if (!Pages.TryGetValue(CurrentPage, out var page))
            return;
        page.Update();
    }
    
    internal PlayerSettings(Player User)
    {
        Owner = User;
        DefaultPage = new SettingPage(this, 0);
        _pages.Add(0, DefaultPage);
    }
}