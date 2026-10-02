// // Copyright (c) 2025 xaze_
// //
// // This source code is licensed under the MIT license found in the
// // LICENSE file in the root directory of this source tree.
// //
// // I <3 🦈s :3c

using JetBrains.Annotations;
using LabApi.Features.Wrappers;
using UnityEngine;

namespace XazeAPI.Features.Model.ModelParts;

public abstract class ModelPart
{
    /// <summary>
    /// Isn't set on creation or during builder action
    /// </summary>
    public AdminToy Part { get; protected set; }
    public uint Id { get; internal set; }

    [CanBeNull]
    public Transform Parent
    {
        get;
        set
        {
            field = value;
            Part?.Parent = value;
        }
    } = null;

    public Vector3 Position
    {
        get;
        set
        {
            field = value;
            Part?.Position = value;
        }
    } = Vector3.zero;
    public Quaternion Rotation
    {
        get;
        set
        {
            field = value;
            Part?.Rotation = value;
        }
    } = Quaternion.identity;
    public Vector3 Scale
    {
        get;
        set
        {
            field = value;
            Part?.Scale = value;
        }
    } = Vector3.one;

    public byte MovementSmoothing
    {
        get;
        set
        {
            field = value;
            Part?.MovementSmoothing = value;
        }
    }
    public float SyncInterval
    {
        get;
        set
        {
            field = value;
            Part?.SyncInterval = value;
        }
    }

    public abstract void Spawn();
    public virtual void Destroy()
    {
        if (Part.IsDestroyed)
            return;
        Part.Destroy();
    }

    public ModelPart WithPosition(float x, float y, float z) => WithPosition(new Vector3(x, y, z));
    public ModelPart WithPosition(Vector3 position)
    {
        Position = position;
        return this;
    }

    public ModelPart WithRotation(float x = 0, float y = 0, float z = 0) => WithRotation(Quaternion.Euler(x, y, z));
    public ModelPart WithRotation(Vector3 euler) => WithRotation(Quaternion.Euler(euler));
    public ModelPart WithRotation(Quaternion rotation)
    {
        Rotation = rotation;
        return this;
    }

    public ModelPart WithScale(float x, float y, float z) => WithScale(new Vector3(x, y, z));
    public ModelPart WithScale(Vector3 scale)
    {
        Scale = scale;
        return this;
    }

    public ModelPart WithParent(ModelPart part) => WithParent(part.Part.Transform);
    public ModelPart WithParent(AdminToy parent) => WithParent(parent.Transform);
    public ModelPart WithParent(GameObject parent) => WithParent(parent.transform);
    public ModelPart WithParent(Transform parent)
    {
        Parent = parent;
        return this;
    }
}