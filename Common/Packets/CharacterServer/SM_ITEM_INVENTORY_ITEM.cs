
using SmartEngine.Network;
using SagaBNS.Common.Network;
namespace SagaBNS.Common.Packets.CharacterServer
{
    public class SM_ITEM_INVENTORY_ITEM : Packet<CharacterPacketOpcode>
    {
        internal class SM_ITEM_INVENTORY_ITEM_INTERNAL<T> : SM_ITEM_INVENTORY_ITEM
        {
            public override Packet<CharacterPacketOpcode> New()
            {
                return new SM_ITEM_INVENTORY_ITEM_INTERNAL<T>();
            }

            public override void OnProcess(Session<CharacterPacketOpcode> client)
            {
                ((CharacterSession<T>)client).OnItemInventoryItem(this);
            }
        }

        public SM_ITEM_INVENTORY_ITEM()
        {
            ID = CharacterPacketOpcode.SM_ITEM_INVENTORY_ITEM;
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

        public bool End
        {
            get
            {
                return GetByte(10) == 1;
            }
            set
            {
                PutByte(value ? (byte)1 : (byte)0, 10);
            }
        }

        public Item.Item Item
        {
            get
            {
                uint itemID = GetUInt(11);
                if (itemID == 0)
                {
                    return null;
                }

                Item.ItemData data = new Item.ItemData()
                {
                    ItemID = itemID
                };
                Common.Item.Item item = new Common.Item.Item(data)
                {
                    ID = GetUInt(),
                    CharID = GetUInt(),
                    SlotID = GetUShort(),
                    Container = (SagaBNS.Common.Item.Containers)GetByte(),
                    Count = GetUShort(),
                    Synthesis = GetByte()
                };
                return item;
            }
            set
            {
                if (value != null)
                {
                    PutUInt(value.ItemID, 11);
                    PutUInt(value.ID);
                    PutUInt(value.CharID);
                    PutUShort(value.SlotID);
                    PutByte((byte)value.Container);
                    PutUShort(value.Count);
                    PutByte(value.Synthesis);
                }
                else
                {
                    PutUInt(0, 11);
                }
            }
        }
    }
}
