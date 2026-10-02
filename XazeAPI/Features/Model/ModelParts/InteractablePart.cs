// // Copyright (c) 2025 xaze_
// //
// // This source code is licensed under the MIT license found in the
// // LICENSE file in the root directory of this source tree.
// //
// // I <3 🦈s :3c

using AdminToys;
using LabApi.Features.Wrappers;

namespace XazeAPI.Features.Model.ModelParts;

public class InteractablePart : ModelPart
{
    /// <summary>
    /// Isn't set on creation or during builder action
    /// </summary>
    public new InteractableToy Part => base.Part as InteractableToy;

    public InvisibleInteractableToy.ColliderShape Shape
    {
        get;
        set
        {
            field = value;
            Part?.Shape = value;
        }
    } = InvisibleInteractableToy.ColliderShape.Box;

    public float InteractionDuration
    {
        get;
        set
        {
            field = value;
            Part?.InteractionDuration = value;
        }
    } = 0.0f;

    public bool IsLocked
    {
        get;
        set
        {
            field = value;
            Part?.IsLocked = value;
        }
    } = false;

    public override void Spawn()
    {
        base.Part = InteractableToy.Create(Position, Rotation, Scale, Parent, false);
        Part.Shape = Shape;
        Part.InteractionDuration = InteractionDuration;
        Part.IsLocked = IsLocked;
        Part.Spawn();
    }
}