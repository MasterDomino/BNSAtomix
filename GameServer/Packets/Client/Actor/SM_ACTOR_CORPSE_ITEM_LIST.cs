using System.Collections.Generic;

using SmartEngine.Network;
using SagaBNS.Common.Packets;

namespace SagaBNS.GameServer.Packets.Client
{
    public class SM_ACTOR_CORPSE_ITEM_LIST : Packet<GamePacketOpcode>
    {
        public SM_ACTOR_CORPSE_ITEM_LIST()
        {
            ID = GamePacketOpcode.SM_ACTOR_CORPSE_ITEM_LIST;
        }

        public ulong ActorID
        {
            set
            {
                PutULong(value, 2);
            }
        }

        public int Gold
        {
            set
            {
                PutInt(value, 10);
            }
        }

        public List<Common.Item.Item> Items
        {
            set
            {
                PutShort((short)value.Count, 14);
                byte index = 0;
                foreach (Common.Item.Item i in value)
                {
                    PutUInt(i.ItemID);
                    PutByte(index++);
                    PutByte((byte)i.Count);
                }
            }
        }
    }
}
