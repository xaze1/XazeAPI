// // Copyright (c) 2025 xaze_
// //
// // This source code is licensed under the MIT license found in the
// // LICENSE file in the root directory of this source tree.
// //
// // I <3 🦈s :3c

using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

namespace XazeAPI.API;

public class StopTimer
{
    private readonly TimeSpan _duration;
    private readonly Action _callback;
    private long _startTimestamp;
    private long elapsed;
    private bool _isRunning;
    private CancellationTokenSource cts;

    public bool IsRunning => _isRunning;
    public TimeSpan Elapsed => new TimeSpan(GetElapsedTime());
    
    public StopTimer(TimeSpan duration, Action callback)
    {
        _duration = duration;
        _callback = callback;
        _startTimestamp = Stopwatch.GetTimestamp();
    }

    public void Start()
    {
        Stop();
        _isRunning = true;
        cts = new CancellationTokenSource();
        RunTimerAsync(cts.Token);
    }

    public void Stop()
    {
        cts?.Cancel();
        cts = null;
        elapsed += Stopwatch.GetTimestamp() - _startTimestamp;
        _isRunning = false;
    }

    public void Reset()
    {
        Stop();
        elapsed = 0;
        _startTimestamp = 0L;
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

    private long GetRawElapsedTicks()
    {
        
        long elapsed = this.elapsed;
        if (!_isRunning) return elapsed;
        
        long num = Stopwatch.GetTimestamp() - _startTimestamp;
        elapsed += num;
        
        return elapsed;
    }

    private long GetElapsedTime()
    {
        return GetRawElapsedTicks();
    }
}