// // Copyright (c) 2025 xaze_
// //
// // This source code is licensed under the MIT license found in the
// // LICENSE file in the root directory of this source tree.
// //
// // I <3 🦈s :3c

using LabApi.Features.Wrappers;
using XazeAPI.Features.Helpers;

namespace XazeAPI.API.Extensions;

public static class DoorExtensions
{
    extension(BulkheadDoor bulkhead)
    {
        public void SlamShut()
        {
            if (!bulkhead.IsOpened)
                return;
            
            bulkhead.IsOpened = false;
            bulkhead.Base.RpcPryGate();
            bulkhead.Crusher?.DeathCollider.enabled = true;
            ActionQueue.Enqueue(() => bulkhead.Crusher?.Base._enabled = true, 0.5f);
        }
    }
}