// // Copyright (c) 2025 xaze_
// //
// // This source code is licensed under the MIT license found in the
// // LICENSE file in the root directory of this source tree.
// //
// // I <3 🦈s :3c

using System;
using SecretLabNAudio.Core;

namespace XazeAPI.API.AudioCore.Speakers;

public abstract class SpeakerOutputMonitor : IAudioPacketMonitor
{
    protected abstract void OnRead(float[] buffer);
    
    public void OnRead(ReadOnlySpan<float> buffer)
    {
        OnRead(buffer.ToArray());
    }

    public abstract void OnEmpty();
}