using System;

using SmartEngine.Network;
using SagaBNS.Common.Packets;
using SagaBNS.AuctionServer.Network.Client;

namespace SagaBNS.AuctionServer.Packets.Client
{
    public class CM_AUTH : Packet<AuctionPacketOpcode>
    {
        public CM_AUTH()
        {
            ID = AuctionPacketOpcode.CM_AUTH;
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

        public override Packet<AuctionPacketOpcode> New()
        {
            return new CM_AUTH();
        }

        public override void OnProcess(Session<AuctionPacketOpcode> client)
        {
            ((AuctionSession)client).OnLoginAuth(this);
        }
    }
}
