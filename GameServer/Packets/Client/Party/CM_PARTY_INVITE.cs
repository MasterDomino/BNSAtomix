
using SmartEngine.Network;
using SagaBNS.Common.Packets;
using SagaBNS.GameServer.Network.Client;

namespace SagaBNS.GameServer.Packets.Client
{
    public class CM_PARTY_INVITE : Packet<GamePacketOpcode>
    {
        public CM_PARTY_INVITE()
        {
            ID = GamePacketOpcode.CM_PARTY_INVITE;
        }

        public ulong ActorID
        {
            get
            {
                return GetULong(2);
            }
        }

        public override Packet<GamePacketOpcode> New()
        {
            return new CM_PARTY_INVITE();
        }

        public override void OnProcess(Session<GamePacketOpcode> client)
        {
            ((GameSession)client).OnPartyInvite(this);
        }
    }
}
