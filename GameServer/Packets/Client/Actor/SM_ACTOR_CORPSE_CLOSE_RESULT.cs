
using SmartEngine.Network;
using SagaBNS.Common.Packets;

namespace SagaBNS.GameServer.Packets.Client
{
    public class SM_ACTOR_CORPSE_CLOSE_RESULT : Packet<GamePacketOpcode>
    {
        public SM_ACTOR_CORPSE_CLOSE_RESULT()
        {
            ID = GamePacketOpcode.SM_ACTOR_CORPSE_CLOSE_RESULT;
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
