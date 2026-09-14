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

public abstract class SettingComponent(int priority = 5)
{
    public int Priority { get; } = priority;
    public abstract ServerSpecificSettingBase[] GetSettings();

    public virtual void Update()
    {
        var settings = GetSettings();
        settings.Do(s => s.OnUpdate());
    }
}