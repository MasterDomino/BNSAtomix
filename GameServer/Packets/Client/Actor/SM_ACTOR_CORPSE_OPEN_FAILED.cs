
using SmartEngine.Network;
using SagaBNS.Common.Packets;

namespace SagaBNS.GameServer.Packets.Client
{
    public class SM_ACTOR_CORPSE_OPEN_FAILED : Packet<GamePacketOpcode>
    {
        public SM_ACTOR_CORPSE_OPEN_FAILED()
        {
            ID = GamePacketOpcode.SM_ACTOR_CORPSE_OPEN_FAILED;
            PutUShort(0x11D, 2);
        }
    }
}
