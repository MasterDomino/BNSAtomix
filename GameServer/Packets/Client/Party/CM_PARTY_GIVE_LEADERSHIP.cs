
using SmartEngine.Network;
using SagaBNS.Common.Packets;
using SagaBNS.GameServer.Network.Client;

namespace SagaBNS.GameServer.Packets.Client
{
    public class CM_PARTY_GIVE_LEADERSHIP : Packet<GamePacketOpcode>
    {
        public CM_PARTY_GIVE_LEADERSHIP()
        {
            ID = GamePacketOpcode.CM_PARTY_GIVE_LEADERSHIP;
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
            return new CM_PARTY_GIVE_LEADERSHIP();
        }

        public override void OnProcess(Session<GamePacketOpcode> client)
        {
            ((GameSession)client).OnPartyGiveLeadership(this);
        }
    }
}
