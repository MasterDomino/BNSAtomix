
using SmartEngine.Network;
using SagaBNS.Common.Packets;

namespace SagaBNS.GameServer.Packets.Client
{
    public class SM_TELEPORT_REQUEST : Packet<GamePacketOpcode>
    {
        public SM_TELEPORT_REQUEST()
        {
            ID = GamePacketOpcode.SM_TELEPORT_REQUEST;
        }

        public ushort Location
        {
            set
            {
                PutUShort(value, 2);
            }
        }

        public ushort Time
        {
            set
            {
                PutUShort(value, 4);
            }
        }
    }
}
