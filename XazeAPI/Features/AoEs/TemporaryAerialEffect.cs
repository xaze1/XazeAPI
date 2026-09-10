// // Copyright (c) 2025 xaze_
// //
// // This source code is licensed under the MIT license found in the
// // LICENSE file in the root directory of this source tree.
// //
// // I <3 🦈s :3c

using UnityEngine;

namespace XazeAPI.Features.AoEs;

public abstract class TemporaryAerialEffect : AerialEffect
{
    public abstract float Duration { get; }
    public virtual float DecaySpeed { get; set; } = 1;
    public float Elapsed;

    protected override void Update()
    {
        base.Update();
        if (!IsActive)
            return;
        if (Elapsed > Duration)
        {
            Destroy();
            return;
        }
        Elapsed += Time.deltaTime;
    }
    
    protected TemporaryAerialEffect(Vector3 sourcePos) : base(sourcePos)
    {
    }
}