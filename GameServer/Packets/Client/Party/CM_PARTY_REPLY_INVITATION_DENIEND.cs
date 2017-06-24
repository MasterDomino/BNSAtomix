
using SmartEngine.Network;
using SagaBNS.Common.Packets;
using SagaBNS.GameServer.Network.Client;

namespace SagaBNS.GameServer.Packets.Client
{
    public class CM_PARTY_REPLY_INVITATION_DENIED : Packet<GamePacketOpcode>
    {
        public CM_PARTY_REPLY_INVITATION_DENIED()
        {
            ID = GamePacketOpcode.CM_PARTY_REPLY_INVITATION_DENIED;
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
            return new CM_PARTY_REPLY_INVITATION_DENIED();
        }

        public override void OnProcess(Session<GamePacketOpcode> client)
        {
            ((GameSession)client).OnPartyReplyInvitationDenied(this);
        }
    }
}
