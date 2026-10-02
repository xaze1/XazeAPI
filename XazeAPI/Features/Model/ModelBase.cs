// // Copyright (c) 2025 xaze_
// //
// // This source code is licensed under the MIT license found in the
// // LICENSE file in the root directory of this source tree.
// //
// // I <3 🦈s :3c

using System;
using System.Collections.Generic;
using Mirror;
using UnityEngine;
using XazeAPI.API.Extensions;
using XazeAPI.Features.Model.ModelParts;

namespace XazeAPI.Features.Model;

public abstract class ModelBase
{
    public GameObject MainPart { get; protected set; }
    public IReadOnlyList<ModelPart> Parts => _parts.AsReadOnly();

    public bool IsDestroyed { get; private set; } = false;
    
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

    protected ModelBase WithPrimitive(Action<PrimitivePart> builderAction) => WithPart(builderAction);
    protected ModelBase WithLight(Action<LightPart> builderAction) => WithPart(builderAction);
    protected ModelBase WithText(Action<TextPart> builderAction) => WithPart(builderAction);
    protected ModelBase WithInteractable(Action<InteractablePart> builderAction) => WithPart(builderAction);
    protected ModelBase WithWaypoint(Action<WaypointPart> builderAction) => WithPart(builderAction);
    protected ModelBase WithPart<T>(Action<T> builderAction) where T : ModelPart
    {
        var part = Activator.CreateInstance<T>();
        builderAction.InvokeSafely(part);
        part.Spawn();
        _parts.Add(part);
        return this;
    }

    public void Spawn() => Spawn(Vector3.zero, Quaternion.identity);
    public void Spawn(Vector3 position) => Spawn(position, Quaternion.identity);

    /// <summary>
    /// Parts MUST be spawned in using this method!
    /// <code>Parts.Do(p => p.Spawn());</code>
    /// </summary>
    public void Spawn(Vector3 position, Quaternion rotation)
    {
        IsDestroyed = false;
        OnSpawn();
    }

    protected abstract void OnSpawn();
    protected abstract void OnDestroy();

    public void Destroy()
    {
        OnDestroy();
        Parts.Do(p => p.Destroy());
        NetworkServer.Destroy(MainPart);
        _parts.Clear();
        IsDestroyed = true;
    }
}