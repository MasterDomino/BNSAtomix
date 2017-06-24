
using SmartEngine.Network;
using SagaBNS.Common.Packets;

namespace SagaBNS.GameServer.Packets.Client
{
    public class SM_LOCATION_ADD_TELEPORT : Packet<GamePacketOpcode>
    {
        public SM_LOCATION_ADD_TELEPORT()
        {
            ID = GamePacketOpcode.SM_LOCATION_ADD_TELEPORT;
        }

        public ushort LocationId
        {
            set
            {
                PutUShort(value, 2);
            }
        }
    }
}
