// // Copyright (c) 2025 xaze_
// //
// // This source code is licensed under the MIT license found in the
// // LICENSE file in the root directory of this source tree.
// //
// // I <3 🦈s :3c

using System;
using System.Collections.Generic;
using LabApi.Features.Wrappers;
using Mirror;
using UnityEngine;
using XazeAPI.API.Extensions;

namespace XazeAPI.Features.Model;

public abstract class ModelBase
{
    public GameObject MainPart { get; protected set; }
    public IReadOnlyList<ModelPart> Parts => _parts.AsReadOnly();

    public Vector3 Position
    {
        get => MainPart.transform.localPosition;
        set => MainPart.transform.localPosition = value;
    }
    public Quaternion Rotation
    {
        get => MainPart.transform.localRotation;
        set => MainPart.transform.localRotation = value;
    }
    public Vector3 Scale
    {
        get => MainPart.transform.localScale;
        set => MainPart.transform.localScale = value;
    }
    
    private readonly List<ModelPart> _parts = new();

    protected void WithPrimitive(Action<ModelPart<PrimitiveObjectToy>> builderAction) => WithPart(builderAction);
    protected void WithLight(Action<ModelPart<LightSourceToy>> builderAction) => WithPart(builderAction);
    protected void WithPart<T>(Action<ModelPart<T>> builderAction) where T : AdminToy
    {
        var part = ModelPart<T>.Create(MainPart.transform, Vector3.zero, Quaternion.identity, Vector3.one, false);
        builderAction?.InvokeSafely(part);
        part.Part.Spawn();
        _parts.Add(part);
    }

    public abstract void Spawn();
    protected abstract void OnDestroy();

    public void Destroy()
    {
        OnDestroy();
        Parts.Do(p => p.Destroy());
        NetworkServer.Destroy(MainPart);
        _parts.Clear();
    }
}