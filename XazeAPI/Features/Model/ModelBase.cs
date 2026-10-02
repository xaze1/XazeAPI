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

    protected ModelBase WithPrimitive(Action<PrimitivePart> builderAction, out PrimitiveObjectToy toy) => WithPart(builderAction, out toy);
    protected ModelBase WithLight(Action<LightPart> builderAction, out LightSourceToy toy) => WithPart(builderAction, out toy);
    protected ModelBase WithText(Action<TextPart> builderAction, out TextToy toy) => WithPart(builderAction, out toy);
    protected ModelBase WithInteractable(Action<InteractablePart> builderAction, out InteractableToy toy) => WithPart(builderAction, out toy);
    protected ModelBase WithWaypoint(Action<WaypointPart> builderAction, out WaypointToy toy) => WithPart(builderAction, out toy);
    protected ModelBase WithPart<T, Toy>(Action<T> builderAction, out Toy toy) where T : ModelPart where Toy : AdminToy
    {
        var part = Activator.CreateInstance<T>();
        builderAction.InvokeSafely(part);
        part.Spawn();
        _parts.Add(part);
        toy = (Toy)part.Part;
        toy.Parent ??= MainPart.transform;
        return this;
    }

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
        part.Part.Parent ??= MainPart.transform;
        return this;
    }

    protected ModelBase WithMainPart<T>(Action<T> builderAction) where T : ModelPart
    {
        var part = Activator.CreateInstance<T>();
        builderAction.InvokeSafely(part);
        part.Spawn();
        MainPart = part.Part.GameObject;
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
        OnSpawn(position, rotation);
    }

    public void Spawn(GameObject mainPart) => Spawn(mainPart, Vector3.zero, Quaternion.identity);
    public void Spawn(GameObject mainPart, Vector3 position) => Spawn(mainPart, position, Quaternion.identity);
    public void Spawn(GameObject mainPart, Vector3 position, Quaternion rotation)
    {
        IsDestroyed = false;
        MainPart = mainPart;
        OnSpawn(position, rotation);
    }

    protected abstract void OnSpawn(Vector3 position, Quaternion rotation);
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