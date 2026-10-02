// // Copyright (c) 2025 xaze_
// //
// // This source code is licensed under the MIT license found in the
// // LICENSE file in the root directory of this source tree.
// //
// // I <3 🦈s :3c

using LabApi.Features.Wrappers;
using UnityEngine;

namespace XazeAPI.Features.Model.ModelParts;

public class WaypointPart : ModelPart
{
    /// <summary>
    /// Isn't set on creation or during builder action
    /// </summary>
    public new WaypointToy Part => base.Part as WaypointToy;

    /// <summary>
    /// Bounds the waypoint encapsulates along each dimension in meters.
    /// Bounds is effected by position and rotation of the GameObject but not its scale.
    /// Must not exceed <c>Vector3.one * MaxBounds</c>.
    /// </summary>
    /// <remarks>
    /// When <see cref="P:LabApi.Features.Wrappers.AdminToy.IsStatic" /> is <see langword="true" /> rotation and <see cref="P:LabApi.Features.Wrappers.WaypointToy.BoundsSize" /> is not used, instead the bounds is axis aligned and its size is fixed at <see cref="F:LabApi.Features.Wrappers.WaypointToy.MaxBounds" />.
    /// </remarks>
    public Vector3 BoundsSize
    {
        get;
        set
        {
            field = value;
            Part?.BoundsSize = value;
        }
    }

    public bool VisualizeBounds
    {
        get;
        set
        {
            field = value;
            Part?.VisualizeBounds = value;
        }
    }

    public float PriorityBias
    {
        get;
        set
        {
            field = value;
            Part?.PriorityBias = value;
        }
    }

    public override void Spawn()
    {
        base.Part = WaypointToy.Create(Position, Rotation, Scale, Parent, false);
        Part.BoundsSize = BoundsSize;
        Part.VisualizeBounds = VisualizeBounds;
        Part.PriorityBias = PriorityBias;
        Part.Spawn();
    }
}