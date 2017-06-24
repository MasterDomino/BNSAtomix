using System.Collections.Generic;

using SmartEngine.Network;
using SagaBNS.Common.Packets;

namespace SagaBNS.GameServer.Packets.Client
{
    public class SM_ITEM_BUYBACK_LIST : Packet<GamePacketOpcode>
    {
        public SM_ITEM_BUYBACK_LIST()
        {
            ID = GamePacketOpcode.SM_ITEM_BUYBACK_LIST;
        }

        public List<Common.Item.Item> Items
        {
            set
            {
                PutUShort((ushort)value.Count, 6);//Count
                byte index = 1;
                foreach (Common.Item.Item i in value)
                {
                    PutByte(index++);
                    i.ToPacket(this);
                }

                PutInt((int)Length - 6, 2);
            }
        }
    }
}
