using System.Collections.Generic;

using SmartEngine.Network;
using SagaBNS.Common.Packets;
using SagaBNS.GameServer.Network.Client;

namespace SagaBNS.GameServer.Packets.Client
{
    public class CM_ITEM_BUY : Packet<GamePacketOpcode>
    {
        public CM_ITEM_BUY()
        {
            ID = GamePacketOpcode.CM_ITEM_BUY;
        }

        public ulong ActorID
        {
            get
            {
                return GetULong(2);
            }
        }

        public Dictionary<byte, ushort> Items
        {
            get
            {
                Dictionary<byte, ushort> items = new Dictionary<byte, ushort>();
                short count = GetShort(10);
                for (int i = 0; i < count; i++)
                {
                    items.Add(GetByte(), GetUShort());
                }

                return items;
            }
        }

        public override Packet<GamePacketOpcode> New()
        {
            return new CM_ITEM_BUY();
        }

        public override void OnProcess(Session<GamePacketOpcode> client)
        {
            ((GameSession)client).OnItemBuy(this);
        }
    }
}
