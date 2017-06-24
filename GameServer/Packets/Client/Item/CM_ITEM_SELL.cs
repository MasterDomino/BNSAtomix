using System.Collections.Generic;

using SmartEngine.Network;
using SagaBNS.Common.Packets;
using SagaBNS.GameServer.Network.Client;

namespace SagaBNS.GameServer.Packets.Client
{
    public class CM_ITEM_SELL : Packet<GamePacketOpcode>
    {
        public CM_ITEM_SELL()
        {
            ID = GamePacketOpcode.CM_ITEM_SELL;
        }

        public ulong ActorID
        {
            get
            {
                return GetULong(2);
            }
        }

        public Dictionary<ushort, ushort> Items
        {
            get
            {
                Dictionary<ushort, ushort> items = new Dictionary<ushort, ushort>();
                short count = GetShort(10);
                for (int i = 0; i < count; i++)
                {
                    items.Add(GetUShort(), GetUShort());
                }

                return items;
            }
        }

        public override Packet<GamePacketOpcode> New()
        {
            return new CM_ITEM_SELL();
        }

        public override void OnProcess(Session<GamePacketOpcode> client)
        {
            ((GameSession)client).OnItemSell(this);
        }
    }
}
