// // Copyright (c) 2025 xaze_
// //
// // This source code is licensed under the MIT license found in the
// // LICENSE file in the root directory of this source tree.
// //
// // I <3 🦈s :3c

using System.Collections.Generic;
using JetBrains.Annotations;
using LabApi.Events.Arguments.PlayerEvents;
using LabApi.Events.Handlers;
using LabApi.Features.Wrappers;
using XazeAPI.API.Extensions;

namespace XazeAPI.Features.AoEs;

public class FollowingAerial<T> : FollowingAerial where T : AerialEffect
{
    private static readonly List<FollowingAerial<T>> _list = new();
    public static IReadOnlyList<FollowingAerial<T>> InternalList => _list.AsReadOnly();

    public new T AoE => (T)base.AoE;
    private readonly bool _destroyOnDeath;

    private void Update()
    {
        if (Target is not { IsAlive: true })
            return;
        
        AoE.SourcePosition = Target.Position;
    }

    private void OnLeft(PlayerLeftEventArgs args)
    {
        if (args.Player != Target)
            return;
        
        Destroy();
    }
    
    private void OnDeath(PlayerDeathEventArgs args)
    {
        if (args.Player != Target)
            return;
        
        Destroy();
    }

    public override void Destroy()
    {
        StaticUnityMethods.OnUpdate -= Update;
        PlayerEvents.Left -= OnLeft;
        if (_destroyOnDeath)
            PlayerEvents.Death -= OnDeath;
        
        AoE.Destroy();
        _list.Remove(this);
        Unregister(this);
    }
    
    private FollowingAerial(Player target, T aerialEffect, bool destroyOnDeath = true) : base(target, aerialEffect)
    {
        _destroyOnDeath = destroyOnDeath;
        
        StaticUnityMethods.OnUpdate += Update;
        PlayerEvents.Left += OnLeft;
        if (_destroyOnDeath)
            PlayerEvents.Death += OnDeath;
        
        _list.Add(this);
        Register(this);
    }
    
    [CanBeNull]
    public static FollowingAerial<T> Create(Player Owner, T aerialEffect)
    {
        if (Owner.GameObject == null || Owner.IsHost)
            return null;
        
        return new FollowingAerial<T>(Owner, aerialEffect);
    }
    
    [CanBeNull]
    public static FollowingAerial<T> Create(ReferenceHub hub, T aerialEffect)
    {
        var Owner = Player.Get(hub);
        if (Owner == null)
            return null;
        
        if (Owner.GameObject == null || Owner.IsHost)
            return null;
        
        return new FollowingAerial<T>(Owner, aerialEffect);
    }

    public static bool Destroy(Player Owner)
    {
        if (!InternalList.TryGetFirst(f => f.Target == Owner, out var following))
            return false;
        
        following.Destroy();
        return true;
    }

    public static bool Destroy(ReferenceHub hub)
    {
        var Owner = Player.Get(hub);
        if (Owner == null)
            return false;
        
        if (!InternalList.TryGetFirst(f => f.Target == Owner, out var following))
            return false;
        
        following.Destroy();
        return true;
    }

    public static void DestroyAll()
    {
        for (int i = InternalList.Count - 1; i >= 0; i--)
            InternalList[i].Destroy();
    }
}

public abstract class FollowingAerial(Player target, AerialEffect aerialEffect)
{
    private static readonly List<FollowingAerial> _allInstances = new();
    public static IReadOnlyList<FollowingAerial> AllInstances => _allInstances.AsReadOnly();

    protected static void Register(FollowingAerial instance) => _allInstances.Add(instance);
    protected static void Unregister(FollowingAerial instance) => _allInstances.Remove(instance);

    public static void DestroyAllGlobal()
    {
        for (int i = _allInstances.Count - 1; i >= 0; i--)
        {
            _allInstances[i].Destroy();
        }
    }

    public static bool DestroyAny(Player player)
    {
        bool removedAny = false;
        for (int i = _allInstances.Count - 1; i >= 0; i--)
        {
            if (_allInstances[i].Target != player) 
                continue;
            _allInstances[i].Destroy();
            removedAny = true;
        }
        return removedAny;
    }

    public Player Target { get; protected set; } = target;
    public AerialEffect AoE { get; protected set; } = aerialEffect;
    
    public abstract void Destroy();
}