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
    private long _startTimestamp = 0L;
    private long elapsed = 0;
    private bool _isRunning = false;
    private CancellationTokenSource cts;

    public bool IsRunning => _isRunning;
    public TimeSpan Elapsed => GetElapsedTime();
    public TimeSpan Remaining
    {
        get
        {
            double remainingSeconds = Math.Max(0, field.TotalSeconds - Elapsed.TotalSeconds);
            return TimeSpan.FromSeconds(remainingSeconds);
        }
    } = duration;

    public void Start()
    {
        if (_isRunning)
            return;
        
        Stop();
        _startTimestamp = Stopwatch.GetTimestamp();
        _isRunning = true;
        cts = new CancellationTokenSource();
        RunTimerAsync(cts.Token);
    }

    public void Stop()
    {
        if (_isRunning)
        {
            elapsed += Stopwatch.GetTimestamp() - _startTimestamp;
            _isRunning = false;
        }

        cts?.Cancel();
        cts = null;
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
            await Task.Delay(duration, token);

            if (!token.IsCancellationRequested)
            {
                Stop();
                callback?.Invoke();
            }
        }
        catch
        {
            // Ignored
        }
    }

    private long GetRawElapsedTicks()
    {
        long raw = elapsed;
        if (_isRunning)
        {
            raw += Stopwatch.GetTimestamp() - _startTimestamp;
        }
        return raw;
    }

    private TimeSpan GetElapsedTime()
    {
        long rawTicks = GetRawElapsedTicks();
        double seconds = (double)rawTicks / Stopwatch.Frequency;
        return TimeSpan.FromSeconds(seconds);
    }
}