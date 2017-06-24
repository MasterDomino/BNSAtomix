
using SmartEngine.Network;
using SagaBNS.Common.Packets;

namespace SagaBNS.GameServer.Packets.Client
{
    public class SM_PARTY_LEADER_CHANGE : Packet<GamePacketOpcode>
    {
        public SM_PARTY_LEADER_CHANGE()
        {
            ID = GamePacketOpcode.SM_PARTY_LEADER_CHANGE;
        }

        public ulong ActorID
        {
            set
            {
                PutULong(value, 2);
            }
        }
    }
}
