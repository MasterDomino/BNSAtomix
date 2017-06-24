
using SmartEngine.Network;
using SagaBNS.Common.Packets;

namespace SagaBNS.GameServer.Packets.Client
{
    public class SM_ACTOR_AGGRO : Packet<GamePacketOpcode>
    {
        public SM_ACTOR_AGGRO()
        {
            ID = GamePacketOpcode.SM_ACTOR_AGGRO;
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
