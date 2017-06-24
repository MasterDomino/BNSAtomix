
using SmartEngine.Network;
using SagaBNS.Common.Packets;

namespace SagaBNS.GameServer.Packets.Client
{
    public class SM_MAPOBJECT_INVENTORY_OPEN : Packet<GamePacketOpcode>
    {
        public SM_MAPOBJECT_INVENTORY_OPEN()
        {
            ID = GamePacketOpcode.SM_MAPOBJECT_INVENTORY_OPEN;
        }

        public ulong ActorID
        {
            set
            {
                PutULong(value, 2);
                PutInt(0);
                PutShort(0);
            }
        }
    }
}
