// // Copyright (c) 2025 xaze_
// //
// // This source code is licensed under the MIT license found in the
// // LICENSE file in the root directory of this source tree.
// //
// // I <3 🦈s :3c

using System.Collections.Generic;
using LabApi.Events.Arguments.PlayerEvents;
using LabApi.Events.Handlers;
using LabApi.Features.Wrappers;
using NorthwoodLib.Pools;
using PlayerRoles.FirstPersonControl;
using UnityEngine;

namespace XazeAPI.Features.AoEs;

public abstract class AerialEffect
{
    public static List<AerialEffect> List { get; } = new();
    public List<Player> AffectedPlayers { get; } = new();

    public virtual float MaxDistance { get; set; } = 10;
    public virtual float MaxHeightDistance { get; set; } = 10;
    public virtual Vector3 SourceOffset { get; protected set; } = Vector3.zero;
    public virtual bool IsActive { get; set; } = true;

    public virtual Vector3 SourcePosition
    {
        get => field + SourceOffset;
        set;
    } = Vector3.zero;
    
    public virtual bool OnEnter(Player player)
    {
        AffectedPlayers.Add(player);
        return true;
    }
    
    public virtual bool OnStay(Player player)
    {
        return true;
    }

    public virtual bool OnExit(Player player)
    {
        AffectedPlayers.Remove(player);
        return true;
    }

    public virtual bool IsInArea(Vector3 sourcePos, Vector3 targetPos)
    {
        return Mathf.Abs(targetPos.y - sourcePos.y) <= MaxHeightDistance && (sourcePos - targetPos).SqrMagnitudeIgnoreY() <= MaxDistance * MaxDistance;
    }

    protected virtual void UpdateTargets()
    {
        var list = ListPool<Player>.Shared.Rent();
        foreach (var plr in Player.ReadyList)
        {
            if (plr.RoleBase is not IFpcRole fpcRole)
                continue;
            
            bool affected = AffectedPlayers.Contains(plr);
            if (IsInArea(SourcePosition, fpcRole.FpcModule.Position))
            {
                if (!affected)
                    OnEnter(plr);
                else
                    list.Add(plr);
            }
            else if (affected)
                OnExit(plr);
        }

        if (list.Count == 0)
        {
            ListPool<Player>.Shared.Return(list);
            return;
        }

        foreach (var plr in list)
            OnStay(plr);
        ListPool<Player>.Shared.Return(list);
    }

    protected virtual void Update()
    {
        UpdateTargets();
    }

    private void OnRoleChanged(PlayerChangedRoleEventArgs args)
    {
        if (!AffectedPlayers.Contains(args.Player))
            return;
        OnExit(args.Player);
    }

    public virtual void Destroy()
    {
        IsActive = false;
        PlayerEvents.ChangedRole -= OnRoleChanged;
        StaticUnityMethods.OnUpdate -= Update;
        for (int i = AffectedPlayers.Count - 1; i >= 0; i--)
        {
            OnExit(AffectedPlayers[i]);
        }

        List.Remove(this);
    }

    internal static void DestroyAll()
    {
        for (int i = List.Count - 1; i >= 0; i--)
        {
            List[i].Destroy();
        }
    }
    
    protected AerialEffect(Vector3 sourcePos)
    {
        SourcePosition = sourcePos;
        
        PlayerEvents.ChangedRole += OnRoleChanged;
        StaticUnityMethods.OnUpdate += Update;
        List.Add(this);
    }
}