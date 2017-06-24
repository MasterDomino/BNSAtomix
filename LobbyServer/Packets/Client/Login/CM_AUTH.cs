using System;

using SmartEngine.Network;
using SagaBNS.Common.Packets;
using SagaBNS.LobbyServer.Network.Client;

namespace SagaBNS.LobbyServer.Packets.Client
{
    public class CM_AUTH : Packet<LobbyPacketOpcode>
    {
        public CM_AUTH()
        {
            ID = LobbyPacketOpcode.CM_AUTH;
        }

        public Guid Token
        {
            get
            {
                return new Guid(GetBytes(16, 2));
            }
        }

        public Guid AccountID
        {
            get
            {
                return new Guid(GetBytes(16, 18));
            }
        }

        public override Packet<LobbyPacketOpcode> New()
        {
            return new CM_AUTH();
        }

        public override void OnProcess(Session<LobbyPacketOpcode> client)
        {
            ((LobbySession)client).OnLoginAuth(this);
        }
    }
}
