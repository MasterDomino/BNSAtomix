
using SmartEngine.Network;
using SagaBNS.Common.Packets;

namespace SagaBNS.GameServer.Packets.Client
{
    public class SM_COMPOSE_BAGUA_FAIL : Packet<GamePacketOpcode>
    {
        public SM_COMPOSE_BAGUA_FAIL()
        {
            ID = GamePacketOpcode.SM_COMPOSE_BAGUA_FAIL;
        }

        public ushort Code
        {
            set
            {
                PutUShort(value, 2);
            }
        }
    }
}
