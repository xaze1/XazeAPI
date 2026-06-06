// // Copyright (c) 2025 xaze_
// //
// // This source code is licensed under the MIT license found in the
// // LICENSE file in the root directory of this source tree.
// //
// // I <3 🦈s :3c

using System;
using System.Collections.Generic;
using MEC;

namespace XazeAPI.API.Helpers;

public static class ActionQueue
{
    private static readonly Queue<Action> _actions = new();
    private static CoroutineHandle _handle;

    public static void Init()
    {
        _handle = Timing.RunCoroutine(DequeueCoroutine());
    }

    public static void Add(Action action)
    {
        _actions.Enqueue(action);
    }

    private static IEnumerator<float> DequeueCoroutine()
    {
        while (true)
        {
            if (!_actions.TryDequeue(out var queuedAction))
            {
                yield return Timing.WaitForOneFrame;
                continue;
            }

            try
            {
                queuedAction();
            }
            catch (Exception ex)
            {
                Logging.Error(ex.ToString());
            }
            
            yield return Timing.WaitForSeconds(0.05f);
        }
    }
}