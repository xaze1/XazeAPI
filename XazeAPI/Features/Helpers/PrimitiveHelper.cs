// Copyright (c) 2025 xaze_
//
// This source code is licensed under the MIT license found in the
// LICENSE file in the root directory of this source tree.
// 
// I <3 🦈s :3c

using Footprinting;
using LabApi.Features.Wrappers;
using Mirror;
using UnityEngine;
using PrimitiveFlags = AdminToys.PrimitiveFlags;

namespace XazeAPI.API.Helpers
{
    public static class PrimitiveHelper
    {
        public static PrimitiveObjectToy spawnPrimitive(Vector3 position, PrimitiveType shape, Color color, Vector3 scale, bool collision = false, ReferenceHub spawner = null, ReferenceHub specificPlayer = null)
        {
            PrimitiveObjectToy primitiveObject = PrimitiveObjectToy.Create(position, Quaternion.identity, scale, null, false);
            primitiveObject.Type = shape;
            primitiveObject.Color = color;
            primitiveObject.Scale = scale;
            primitiveObject.Position = position;

            var box = primitiveObject.GameObject.AddComponent<BoxCollider>();
            box.isTrigger = false;
            box.center = position;
            box.size = scale;
            box.enabled = true;

            primitiveObject.Base.SpawnerFootprint = new Footprint(spawner ?? ReferenceHub.HostHub);

            if (collision)
            {
                primitiveObject.Flags = PrimitiveFlags.Collidable | PrimitiveFlags.Visible;
            }
            else
            {
                primitiveObject.Flags = PrimitiveFlags.Visible;
            }



            if (specificPlayer != null)
            {
                NetworkServer.SendSpawnMessage(primitiveObject.Base.netIdentity, specificPlayer.connectionToClient);
            }
            else
            {
                NetworkServer.Spawn(primitiveObject.GameObject);
            }

            return primitiveObject;
        }

        public static PrimitiveObjectToy spawnPrimitive(Transform transform, PrimitiveType shape, Vector3 scale, Color color, bool collision)
        {
            PrimitiveObjectToy primitiveObject = PrimitiveObjectToy.Create(transform, false);
            primitiveObject.Type = shape;
            primitiveObject.Scale = scale;
            primitiveObject.Color = color;

            if (collision)
            {
                primitiveObject.Flags = PrimitiveFlags.Collidable | PrimitiveFlags.Visible;
            }
            else
            {
                primitiveObject.Flags = PrimitiveFlags.Visible;
            }

            NetworkServer.Spawn(primitiveObject.GameObject);

            return primitiveObject;
        }

        public static PrimitiveObjectToy updatePrimitive(this PrimitiveObjectToy primitive, ReferenceHub specificPlayer, Vector3 position, Vector3 scale)
        {
            PrimitiveObjectToy prim = primitive;
            prim.Position = position;
            prim.Scale = scale;

            float num = Time.deltaTime * prim.MovementSmoothing * 0.3f;
            if (num == 0f)
            {
                num = 1f;
            }

            primitive.Transform.SetPositionAndRotation(Vector3.Lerp(primitive.Transform.position, primitive.Position, num), Quaternion.Lerp(primitive.Transform.rotation, primitive.Rotation, num));
            primitive.Transform.localScale = Vector3.Lerp(primitive.Transform.localScale, primitive.Scale, num);

            NetworkServer.SendSpawnMessage(primitive.Base.netIdentity, specificPlayer.connectionToClient);
            UpdateChildern(primitive, specificPlayer);

            return prim;
        }

        public static PrimitiveObjectToy updatePrimitive(this PrimitiveObjectToy primitive, ReferenceHub specificPlayer, Quaternion rotation)
        {
            primitive.Rotation = rotation;

            float num = Time.deltaTime * (float)(int)primitive.MovementSmoothing * 0.3f;
            if (num == 0f)
            {
                num = 1f;
            }

            primitive.Transform.SetPositionAndRotation(Vector3.Lerp(primitive.Transform.position, primitive.Position, num), Quaternion.Lerp(primitive.Transform.rotation, primitive.Rotation, num));

            NetworkServer.SendSpawnMessage(primitive.Base.netIdentity, specificPlayer.connectionToClient);
            UpdateChildern(primitive, specificPlayer);

            return primitive;
        }
        
        public static PrimitiveObjectToy updatePrimitive(this PrimitiveObjectToy primitive, ReferenceHub specificPlayer, PrimitiveType shape)
        {
            primitive.Type = shape;

            NetworkServer.SendSpawnMessage(primitive.Base.netIdentity, specificPlayer.connectionToClient);
            UpdateChildern(primitive, specificPlayer);

            return primitive;
        }
        
        public static PrimitiveObjectToy updatePrimitive(this PrimitiveObjectToy primitive, ReferenceHub specificPlayer, Color color)
        {
            primitive.Color = color;

            NetworkServer.SendSpawnMessage(primitive.Base.netIdentity, specificPlayer.connectionToClient);
            UpdateChildern(primitive, specificPlayer);

            return primitive;
        }
        
        public static PrimitiveObjectToy updatePrimitive(this PrimitiveObjectToy primitive, ReferenceHub specificPlayer, bool collision)
        {
            if (collision)
            {
                primitive.Flags = PrimitiveFlags.Collidable | PrimitiveFlags.Visible;
            }
            else
            {
                primitive.Flags = PrimitiveFlags.Visible;
            }

            NetworkServer.SendSpawnMessage(primitive.Base.netIdentity, specificPlayer.connectionToClient);
            UpdateChildern(primitive, specificPlayer);

            return primitive;
        }

        public static void DestroyPrimitive(this PrimitiveObjectToy primitive, ReferenceHub specificPlayer)
        {
            specificPlayer.connectionToClient.Send(new ObjectDestroyMessage() { netId = primitive.Base.netId});
            Object.Destroy(primitive.GameObject);
        }

        private static void UpdateChildern(this AdminToy parentObject, ReferenceHub target)
        {
            foreach (var child in parentObject.Base.GetComponentsInChildren<AdminToys.AdminToyBase>())
            {
                NetworkServer.SendSpawnMessage(child.netIdentity, target.connectionToClient);
            }
        }
    }
}
