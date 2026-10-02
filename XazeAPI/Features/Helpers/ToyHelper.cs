// // Copyright (c) 2025 xaze_
// //
// // This source code is licensed under the MIT license found in the
// // LICENSE file in the root directory of this source tree.
// //
// // I <3 🦈s :3c

namespace XazeAPI.Features.Helpers;

using AdminToys;
using JetBrains.Annotations;
using LabApi.Features.Wrappers;
using Mirror;
using UnityEngine;

public static class ToyHelper
{
    public static AdminToy Create<T>(Vector3 position, Quaternion rotation, Vector3 scale, [CanBeNull] Transform parent, params Player[] targets) where T : AdminToyBase
    {
        var hasTargets = targets.Length == 0;
        var toy = AdminToy.Create<T>(position, rotation, scale, parent);
        if (!hasTargets)
        {
            NetworkServer.Spawn(toy.gameObject);
            return AdminToy.Get(toy);
        }
        
        foreach (var target in targets)
            NetworkServer.SendSpawnMessage(toy.netIdentity, target.ConnectionToClient);

        return AdminToy.Get(toy);
    }
    
    extension<T>(T toy) where T : AdminToy
    {
        public void Spawn(params Player[] targets) => toy.Update(targets);
        
        public void Update(params Player[] targets)
        {
            if (toy.IsDestroyed || targets.Length == 0)
                return;
        
            foreach (var target in targets)
                NetworkServer.SendSpawnMessage(toy.Base.netIdentity, target.ConnectionToClient);
        }
        
        public void Update(Vector3 position, params Player[] targets)
        {
            if (toy.IsDestroyed || targets.Length == 0)
                return;
        
            toy.Position = position;
            foreach (var target in targets)
                NetworkServer.SendSpawnMessage(toy.Base.netIdentity, target.ConnectionToClient);
        }

        public void Update(Quaternion rotation, params Player[] targets)
        {
            if (toy.IsDestroyed || targets.Length == 0)
                return;
        
            toy.Rotation = rotation;
            foreach (var target in targets)
                NetworkServer.SendSpawnMessage(toy.Base.netIdentity, target.ConnectionToClient);
        }

        public void Update(Vector3? scale, params Player[] targets)
        {
            if (toy.IsDestroyed || targets.Length == 0 || !scale.HasValue)
                return;
        
            toy.Scale = scale.Value;
            foreach (var target in targets)
                NetworkServer.SendSpawnMessage(toy.Base.netIdentity, target.ConnectionToClient);
        }
        
        public void Destroy(params Player[] targets)
        {
            if (toy.IsDestroyed || targets.Length == 0)
                return;
        
            foreach (var target in targets)
                target.ConnectionToClient.Send(new ObjectDestroyMessage { netId = toy.Base.netId});
        }
    }
}