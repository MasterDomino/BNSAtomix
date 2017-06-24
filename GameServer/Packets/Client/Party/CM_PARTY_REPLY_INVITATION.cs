
using SmartEngine.Network;
using SagaBNS.Common.Packets;
using SagaBNS.GameServer.Network.Client;

namespace SagaBNS.GameServer.Packets.Client
{
    public class CM_PARTY_REPLY_INVITATION : Packet<GamePacketOpcode>
    {
        public CM_PARTY_REPLY_INVITATION()
        {
            ID = GamePacketOpcode.CM_PARTY_REPLY_INVITATION;
        }

        public ulong PartyID
        {
            get
            {
                return GetULong(2);
            }
        }

        public int Result
        {
            get
            {
                return GetInt(10);
            }
        }

        public override Packet<GamePacketOpcode> New()
        {
            return new CM_PARTY_REPLY_INVITATION();
        }

        public override void OnProcess(Session<GamePacketOpcode> client)
        {
            ((GameSession)client).OnPartyReplyInvitation(this);
        }
    }
}
