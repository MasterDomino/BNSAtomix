using System.Collections.Generic;

using SmartEngine.Network;
using SagaBNS.Common.Item;

namespace SagaBNS.Common.Packets.CharacterServer
{
    public class CM_ITEM_LIST_SAVE : Packet<CharacterPacketOpcode>
    {
        public CM_ITEM_LIST_SAVE()
        {
            ID = CharacterPacketOpcode.CM_ITEM_LIST_SAVE;
        }

        public List<Item.Item> Items
        {
            get
            {
                List<Item.Item> list = new List<Item.Item>();
                ushort count = GetUShort(2);
                for (int i = 0; i < count; i++)
                {
                    uint itemID = GetUInt();
                    ItemData data = new ItemData()
                    {
                        ItemID = itemID
                    };
                    Common.Item.Item item = new Common.Item.Item(data)
                    {
                        ID = GetUInt(),
                        CharID = GetUInt(),
                        SlotID = GetUShort(),
                        Container = (Item.Containers)GetByte(),
                        Count = GetUShort(),
                        Synthesis = GetByte()
                    };
                    list.Add(item);
                }
                return list;
            }
            set
            {
                ushort offset = this.offset;
                ushort count = 0;
                PutUShort((ushort)value.Count);
                foreach (Item.Item i in value)
                {
                    if (i == null)
                    {
                        continue;
                    }

                    count++;
                    PutUInt(i.ItemID);
                    PutUInt(i.ID);
                    PutUInt(i.CharID);
                    PutUShort(i.SlotID);
                    PutByte((byte)i.Container);
                    PutUShort(i.Count);
                    PutByte(i.Synthesis);
                }
                this.offset = offset;
                PutUShort(count);
            }
        }
    }
}
