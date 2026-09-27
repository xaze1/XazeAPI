// // Copyright (c) 2025 xaze_
// //
// // This source code is licensed under the MIT license found in the
// // LICENSE file in the root directory of this source tree.
// //
// // I <3 🦈s :3c

using System;
using System.Collections.Generic;
using MEC;
using Mirror;
using XazeAPI.API.Extensions;

namespace XazeAPI.Features.Helpers;

public static class ActionQueue
{
    private class QueuedAction(Action action, double delay = 0.0)
    {
        public Action Action { get; } = action;
        public double Delay { get; } = delay;
        public double StartTime { get; } = NetworkTime.time;
    }
    
    private static readonly List<QueuedAction> _actions = new();
    private static CoroutineHandle _handle;

    public static void Init()
    {
        _handle = Timing.RunCoroutine(DequeueCoroutine());
    }

    public static void Enqueue(Action action)
    {
        _actions.Add(new QueuedAction(action));
    }

    public static void Enqueue(Action action, double delay)
    {
        _actions.Add(new QueuedAction(action, delay));
    }

    private static IEnumerator<float> DequeueCoroutine()
    {
        while (true)
        {
            for (int i = _actions.Count - 1; i >= 0; i--)
            {
                var queuedAction = _actions[i];
                if (queuedAction.StartTime + queuedAction.Delay >= NetworkTime.time)
                {
                    yield return Timing.WaitForOneFrame;
                    continue;
                }
                
                queuedAction.Action.InvokeSafely();
                _actions.RemoveAt(i);
            }
            
            yield return Timing.WaitForSeconds(0.05f);
        }
    }
}