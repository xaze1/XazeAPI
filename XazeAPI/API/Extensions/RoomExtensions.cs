// // Copyright (c) 2025 xaze_
// //
// // This source code is licensed under the MIT license found in the
// // LICENSE file in the root directory of this source tree.
// //
// // I <3 🦈s :3c

using LabApi.Features.Wrappers;

namespace XazeAPI.API.Extensions;

public static class RoomExtensions
{
    extension(Room room)
    {
        public void Blackout(float duration)
        {
            room.AllLightControllers.Do(c => c.FlickerLights(duration));
        }
    }
}