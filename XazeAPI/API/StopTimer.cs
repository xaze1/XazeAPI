// // Copyright (c) 2025 xaze_
// //
// // This source code is licensed under the MIT license found in the
// // LICENSE file in the root directory of this source tree.
// //
// // I <3 🦈s :3c

using System;
using System.Threading;
using System.Threading.Tasks;

namespace XazeAPI.API;

public class StopTimer
{
    private readonly TimeSpan _duration;
    private readonly Action _callback;
    private CancellationTokenSource cts;

    public StopTimer(TimeSpan duration, Action callback)
    {
        _duration = duration;
        _callback = callback;
    }

    public void Start()
    {
        Stop();
        cts = new CancellationTokenSource();
        RunTimerAsync(cts.Token);
    }

    public void Stop()
    {
        cts?.Cancel();
        cts = null;
    }

    private async void RunTimerAsync(CancellationToken token)
    {
        try
        {
            await Task.Delay(_duration, token);
            if (!token.IsCancellationRequested)
                _callback?.Invoke();
        }
        catch
        {
            // Ignored
        }
    }
}