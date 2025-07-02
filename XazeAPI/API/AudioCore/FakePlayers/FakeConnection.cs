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
