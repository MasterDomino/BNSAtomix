using System.Collections.Generic;

using SmartEngine.Network;
using SagaBNS.Common.Packets;
using SagaBNS.GameServer.Item;

namespace SagaBNS.GameServer.Packets.Client
{
    public class SM_ITEM_INFO : Packet<GamePacketOpcode>
    {
        private ItemUpdateMethod curMethod;
        public SM_ITEM_INFO()
        {
            ID = GamePacketOpcode.SM_ITEM_INFO;
        }

        public ItemUpdateMethod Reason
        {
            set
            {
                PutByte((byte)value, 6);
                curMethod = value;
            }
        }

        public List<Common.Item.Item> Items
        {
            set
            {
                PutUShort((ushort)value.Count, 7);//Count
                foreach (Common.Item.Item i in value)
                {
                    i.ToPacket(this);
                }

                PutInt((int)Length - 6, 2);
            }
        }
    }
}
