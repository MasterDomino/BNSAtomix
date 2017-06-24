
using SmartEngine.Network;

namespace SagaBNS.Common.Packets.CharacterServer
{
    public class CM_ITEM_INVENTORY_GET : Packet<CharacterPacketOpcode>
    {
        public CM_ITEM_INVENTORY_GET()
        {
            ID = CharacterPacketOpcode.CM_ITEM_INVENTORY_GET;
        }

        public long SessionID
        {
            get
            {
                return GetLong(2);
            }
            set
            {
                PutLong(value, 2);
            }
        }

        public uint CharID
        {
            get
            {
                return GetUInt(10);
            }
            set
            {
                PutUInt(value, 10);
            }
        }
    }
}
