// // Copyright (c) 2025 xaze_
// //
// // This source code is licensed under the MIT license found in the
// // LICENSE file in the root directory of this source tree.
// //
// // I <3 🦈s :3c

using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using LabApi.Features.Wrappers;
using UnityEngine;
using XazeAPI.API.Extensions;

namespace XazeAPI.Features.LightConfigs;

public abstract class LightConfigBase
{
    [CanBeNull] public static event Action<LightConfigBase> OnAnyDestroyed;
    [CanBeNull] public event Action OnThisDestroyed;
    
    public static IReadOnlyList<LightConfigBase> Lights => _lights.AsReadOnly();
    private static List<LightConfigBase> _lights { get; } = new();

    public float Intensity
    {
        get;
        set
        {
            field = value;
            Light?.Intensity = value;
        }
    } = 0;

    public float Range
    {
        get;
        set
        {
            field = value;
            Light?.Range = value;
        }
    } = 0;
    
    public LightSourceToy Light { get; protected set; }
    
    [CanBeNull] internal Transform _parent;
    internal bool IsSet { get; private set; }

    public abstract void Update(float deltaTime);

    protected virtual void OnCreated()
    {
    }

    protected virtual void OnDestroyed()
    {
    }
    
    public void Destroy()
    {
        if (!IsSet)
            return;
        
        IsSet = false;
        Light.Destroy();
        _lights.Remove(this);
        OnDestroyed();
        OnAnyDestroyed?.InvokeSafely(this);
        OnThisDestroyed?.InvokeSafely();
    }

    public void Create(Vector3 position, Quaternion rotation)
    {
        _lights.Add(this);
        IsSet = true;
        
        Light = LightSourceToy.Create(position, rotation, null, false);
        Light.Color = Color.clear;
        Intensity = 5f;
        Range = 10f;
        
        OnCreated();
        Light.Spawn();
    }

    public void Create(Transform parent)
    {
        _lights.Add(this);
        IsSet = true;
        
        Light = LightSourceToy.Create(parent, false);
        Light.Color = Color.clear;
        Intensity = 5f;
        Range = 10f;
        
        _parent = parent;
        OnCreated();
        Light.Spawn();
    }

    internal static void OnLightRemoved(AdminToys.AdminToyBase obj)
    {
        if (obj is not AdminToys.LightSourceToy light)
            return;
        
        _lights.RemoveAll(c => c.Light.GameObject ==  light.gameObject);
    }
}