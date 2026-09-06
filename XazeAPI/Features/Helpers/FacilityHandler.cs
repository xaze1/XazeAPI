// Copyright (c) 2025 xaze_
//
// This source code is licensed under the MIT license found in the
// LICENSE file in the root directory of this source tree.
// 
// I <3 🦈s :3c

using System;
using System.Collections.Generic;
using System.Linq;
using Interactables.Interobjects.DoorUtils;
using LabApi.Features.Wrappers;
using MapGeneration;
using MEC;
using PlayerRoles.FirstPersonControl;
using UnityEngine;
using ElevatorDoor = Interactables.Interobjects.ElevatorDoor;

namespace XazeAPI.API.Helpers
{
    public static class FacilityHandler
    {
        public static Color DefaultColor = Color.clear;

        public static void FlickerLights(float duration, FacilityZone zone = FacilityZone.None)
        {
            foreach(var light in Map.RoomLights)
            {
                if (zone == FacilityZone.None || light.Room?.Zone == zone)
                {
                    light.FlickerLights(duration);
                }
            }
        }

        public static void ChangeFacilityLight(Color color)
        {
            foreach (var controller in RoomLightController.Instances)
            {
                if (controller.OverrideColor == color) continue;

                controller.NetworkOverrideColor = color;
            }
        }

        public static void ChangeFacilityLight(float r, float b, float g) => ChangeFacilityLight(new Color(r / 255f, b / 255f, g / 255f));

        public static Room GetApiRoom(this RoomIdentifier room)
        {
            return Room.Get(room);
        }

        public static void ResetFacilityLight(bool flickerLights = false, float flickerDuration = 1f)
        {
            foreach (var controller in RoomLightController.Instances)
            {
                if (flickerLights)
                {
                    controller.ServerFlickerLights(flickerDuration);
                }

                controller.NetworkOverrideColor = DefaultColor;
            }
        }

        public static void FlashLightsRainbow(float rainbowSpeed, float length = 7f)
        {
            float hue = 0;

            Timing.CallContinuously(length, () =>
            {
                hue += rainbowSpeed / 10000f;
                if (hue >= 1) hue = 0;

                ChangeFacilityLight(Color.HSVToRGB(hue, 1, 1));
            }, () =>
            {
                ResetFacilityLight();
                Timing.KillCoroutines(Timing.CurrentCoroutine);
            });
        }

        public static void ChangeLockAndStateOfGates(bool open, bool locked)
        {
            List<string> GateDoors = ["GATE_A", "GATE_B"];

            DoorEventOpenerExtension.TriggerAction(DoorEventOpenerExtension.OpenerEventType.WarheadStart);
            foreach (DoorVariant door in DoorVariant.AllDoors)
            {
                DoorNametagExtension nt;
                bool flag2 = door.TryGetComponent<DoorNametagExtension>(out nt);
                if (!flag2 || !GateDoors.Any((string x) => string.Equals(nt.GetName, x, System.StringComparison.OrdinalIgnoreCase)))
                {
                    continue;
                }
                
                door.NetworkTargetState = open;
                door.ServerChangeLock(DoorLockReason.SpecialDoorFeature, locked);
            }
        }

        public static void TPPlayerToClosestRoom(Player plr)
        {
            Room closestRoom = null;
            foreach(RoomIdentifier room in RoomIdentifier.AllRoomIdentifiers)
            {
                closestRoom ??= Room.Get(room);

                if (Vector3.Distance(plr.Position, room.transform.position) < Vector3.Distance(plr.Position, closestRoom.Position))
                {
                    closestRoom = Room.Get(room);
                }
            }

            if (closestRoom == null)
            {
                return;
            }

            try
            {
                plr.Position = closestRoom.GetDoorPosInRoom();
            }
            catch
            {
                plr.Position = closestRoom.Position;
            }
        }
        
        public static void TPPlayerToClosestRoom(ReferenceHub hub)
        {
            Room closestRoom = null;
            foreach(RoomIdentifier room in RoomIdentifier.AllRoomIdentifiers.Where(room => !(room.Name == RoomName.Pocket)))
            {
                closestRoom ??= Room.Get(room);

                if (Vector3.Distance(hub.gameObject.transform.position, room.transform.position) < Vector3.Distance(hub.gameObject.transform.position, closestRoom.Position))
                {
                    closestRoom = Room.Get(room);
                }
            }

            if (closestRoom == null)
            {
                return;
            }
            
            try
            {
                Vector3 doorPos = closestRoom.GetDoorPosInRoom();
                doorPos += Vector3.forward * 0.4f;
                doorPos += Vector3.up;

                hub.TryOverridePosition(doorPos);
            }
            catch
            {
                Vector3 roompos = closestRoom.Position;
                roompos.y += 2f;
                hub.TryOverridePosition(roompos);
            }
        }
        
        public static RoomIdentifier GetClosestRoom(this ReferenceHub hub)
        {
            RoomIdentifier closestRoom = null;
            foreach(RoomIdentifier room in RoomIdentifier.AllRoomIdentifiers.Where(room => !(room.Name == RoomName.Pocket)))
            {
                if (closestRoom == null)
                    closestRoom = room;

                if (Vector3.Distance(hub.gameObject.transform.position, room.transform.position) < Vector3.Distance(hub.gameObject.transform.position, closestRoom.transform.position))
                {
                    closestRoom = room;
                }
            }

            return closestRoom;
        }
        
        public static RoomIdentifier GetClosestRoom(this TeslaGate gate)
        {
            RoomIdentifier closestRoom = null;
            foreach(RoomIdentifier room in RoomIdentifier.AllRoomIdentifiers.Where(room => !(room.Name == RoomName.Pocket)))
            {
                if (room == gate.Room)
                {
                    continue;
                }

                if (closestRoom == null)
                    closestRoom = room;

                if (Vector3.Distance(gate.gameObject.transform.position, room.transform.position) < Vector3.Distance(gate.gameObject.transform.position, closestRoom.transform.position))
                {
                    closestRoom = room;
                }
            }

            return closestRoom;
        }

        public static void ChangeLightState(bool newState, MapGeneration.FacilityZone zone = MapGeneration.FacilityZone.None, bool invertZone = false)
        {
            foreach (var controller in RoomLightController.Instances.Where(controller => zone != MapGeneration.FacilityZone.None && invertZone ? (controller.Room.Zone != zone) : (controller.Room.Zone == zone)))
            {
                controller.SetLights(newState);
            }
        }

        public static void ChangeAllDoorStates(bool newState)
        {
            foreach(var door in DoorVariant.AllDoors.Where(door => door is not ElevatorDoor))
            {
                door.NetworkTargetState = newState;
            }
        }
        
        public static void ChangeAllDoorStates(bool newState, Func<DoorVariant, bool> doorCheck)
        {
            foreach(var door in DoorVariant.AllDoors.Where(doorCheck))
            {
                door.NetworkTargetState = newState;
            }
        }
        
        public static void ChangeAllDoorLocks(DoorLockReason reason, bool newState)
        {
            foreach(var door in DoorVariant.AllDoors.Where(door => door is not ElevatorDoor))
            {
                door.ServerChangeLock(reason, newState);
            }
        }public static void ChangeAllDoorLocks(DoorLockReason reason, bool newState, Func<DoorVariant, bool> doorCheck)
        {
            foreach(var door in DoorVariant.AllDoors.Where(doorCheck))
            {
                door.ServerChangeLock(reason, newState);
            }
        }
    }
}
