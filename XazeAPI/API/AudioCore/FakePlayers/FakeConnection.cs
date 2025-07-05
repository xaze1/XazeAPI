// Copyright (c) 2025 xaze_
//
// This source code is licensed under the MIT license found in the
// LICENSE file in the root directory of this source tree.
// 
// I <3 🦈s :3c

using System;
using Mirror;
using NetworkManagerUtils.Dummies;

namespace XazeAPI.API.AudioCore.FakePlayers
{
    public class FakeConnection : NetworkConnectionToClient
    {
        public FakeConnection(int connectionId) : base(connectionId)
        {
            
        }

        public override string address
        {
            get
            {
                return DummyNetworkConnection.DummyAddress;
            }
        }

        public override void Send(ArraySegment<byte> segment, int channelId = 0)
        {
        }

        public override void Disconnect()
        {
        }
    }
}
