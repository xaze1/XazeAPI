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

public class StopTimer(TimeSpan duration, Action callback = null)
{
    private long _startTimestamp;
    private CancellationTokenSource cts;
    private readonly TimeSpan _duration = duration;
    private bool _finished;

    public bool IsRunning { get; private set; }
    public TimeSpan Elapsed => GetElapsedTime();
    public TimeSpan Remaining => _duration - Elapsed > TimeSpan.Zero? _duration - Elapsed : TimeSpan.Zero;

    public void Start()
    {
        if (IsRunning)
            return;
        
        Stop();
        _finished = false;
        _startTimestamp = Stopwatch.GetTimestamp();
        IsRunning = true;
        cts = new CancellationTokenSource();
        _ = RunTimerAsync(cts.Token);
    }

    public void Stop()
    {
        if (IsRunning)
        {
            IsRunning = false;
        }

        cts?.Cancel();
        cts = null;
    }

    public void Reset()
    {
        Stop();
        _startTimestamp = 0L;
    }

    private async Task RunTimerAsync(CancellationToken token)
    {
        try
        {
            await Task.Delay(_duration, token);

            if (!token.IsCancellationRequested)
            {
                _finished = true;
                Stop();
                callback?.Invoke();
            }
        }
        catch
        {
            // Ignored
        }
    }

    private TimeSpan GetElapsedTime()
    {
        if (_finished)
        {
            return _duration;
        }
        
        if (!IsRunning)
        {
            return TimeSpan.Zero;
        }
        
        return TimeSpan.FromSeconds((Stopwatch.GetTimestamp() - _startTimestamp) / (double)Stopwatch.Frequency);
    }
}