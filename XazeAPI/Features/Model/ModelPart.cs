// // Copyright (c) 2025 xaze_
// //
// // This source code is licensed under the MIT license found in the
// // LICENSE file in the root directory of this source tree.
// //
// // I <3 🦈s :3c

using System;
using System.Collections.Generic;
using LabApi.Features.Wrappers;
using UnityEngine;

namespace XazeAPI.Features.Model;

public class ModelPart<T> : ModelPart where T : AdminToy
{
    public uint Id { get; internal set; }
    public T Part { get; init; }

    public Transform Parent
    {
        get => Part.Parent;
        set => Part.Parent = value;
    }
    
    public Vector3 Position
    {
        get => Part.Position;
        set => Part.Position = value;
    }
    public Quaternion Rotation
    {
        get => Part.Rotation;
        set => Part.Rotation = value;
    }
    public Vector3 Scale
    {
        get => Part.Scale;
        set => Part.Scale = value;
    }

    public override void Destroy()
    {
        if (Part.IsDestroyed)
            return;
        Part.Destroy();
    }

    public ModelPart<T> WithPosition(Vector3 position)
    {
        Position = position;
        return this;
    }

    public ModelPart<T> WithRotation(Quaternion rotation)
    {
        Rotation = rotation;
        return this;
    }

    public ModelPart<T> WithScale(Vector3 scale)
    {
        Scale = scale;
        return this;
    }

    public ModelPart<T> WithParent(AdminToy parent) => WithParent(parent.Transform);
    public ModelPart<T> WithParent(GameObject parent) => WithParent(parent.transform);
    public ModelPart<T> WithParent(Transform parent)
    {
        Parent = parent;
        return this;
    }

    
    public static ModelPart<T> Create(Transform parent, bool spawn = true) => Create(parent, Vector3.zero, Quaternion.identity, Vector3.one, spawn);
    public static ModelPart<T> Create(Transform parent, Vector3 position, bool spawn = true) => Create(parent, position, Quaternion.identity, Vector3.one, spawn);
    public static ModelPart<T> Create(Transform parent, Vector3 position, Vector3 euler, bool spawn = true) => Create(parent, position, Quaternion.Euler(euler), Vector3.one, spawn);
    public static ModelPart<T> Create(Transform parent, Vector3 position, Quaternion rotation, bool spawn = true) => Create(parent, position, rotation, Vector3.one, spawn);
    public static ModelPart<T> Create(Transform parent, Vector3 position, Vector3 euler, Vector3 scale, bool spawn = true) => Create(parent, position, Quaternion.Euler(euler), scale, spawn);
    public static ModelPart<T> Create(Transform parent, Vector3 position, Quaternion rotation, Vector3 scale, bool spawn = true)
    {
        var toy = GetCreator<T>();
        var wrapper = (T)AdminToy.Get(toy);
        wrapper.Parent = parent;
        wrapper.Position = position;
        wrapper.Rotation = rotation;
        wrapper.Scale = scale;
        var part = new ModelPart<T> { Part = wrapper };
        if (spawn)
            part.Part.Spawn();
        return part;
    }

    private ModelPart()
    {
    }
}

public abstract class ModelPart
{
    private static readonly Dictionary<Type, Func<AdminToys.AdminToyBase>> _creators = new()
    {
        [typeof(PrimitiveObjectToy)] = () => AdminToy.Create<AdminToys.PrimitiveObjectToy>(Vector3.zero, Quaternion.identity, Vector3.one, null),
        [typeof(LightSourceToy)] = () => AdminToy.Create<AdminToys.LightSourceToy>(Vector3.zero, Quaternion.identity, Vector3.one, null),
        [typeof(ShootingTargetToy)] = () => AdminToy.Create<AdminToys.ShootingTarget>(Vector3.zero, Quaternion.identity, Vector3.one, null),
        [typeof(ShootingTargetToy)] = () => AdminToy.Create<AdminToys.ShootingTarget>(Vector3.zero, Quaternion.identity, Vector3.one, null),
        [typeof(SpeakerToy)] = () => AdminToy.Create<AdminToys.SpeakerToy>(Vector3.zero, Quaternion.identity, Vector3.one, null),
        [typeof(InteractableToy)] = () => AdminToy.Create<AdminToys.InvisibleInteractableToy>(Vector3.zero, Quaternion.identity, Vector3.one, null),
        [typeof(CameraToy)] = () => AdminToy.Create<AdminToys.Scp079CameraToy>(Vector3.zero, Quaternion.identity, Vector3.one, null),
        [typeof(CapybaraToy)] = () => AdminToy.Create<AdminToys.CapybaraToy>(Vector3.zero, Quaternion.identity, Vector3.one, null),
        [typeof(TextToy)] = () => AdminToy.Create<AdminToys.TextToy>(Vector3.zero, Quaternion.identity, Vector3.one, null),
        [typeof(WaypointToy)] = () => AdminToy.Create<AdminToys.WaypointToy>(Vector3.zero, Quaternion.identity, Vector3.one, null),
    };

    protected static AdminToys.AdminToyBase GetCreator<T>() where T : AdminToy
    {
        if (!_creators.TryGetValue(typeof(T), out var creator))
            throw new ArgumentException($"The creator {typeof(T).FullName} for was not found.");
        return creator();
    }
    
    public abstract void Destroy();
}