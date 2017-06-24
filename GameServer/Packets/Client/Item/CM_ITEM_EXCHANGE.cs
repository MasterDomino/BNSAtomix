using System.Collections.Generic;

using SmartEngine.Network;
using SagaBNS.Common.Packets;
using SagaBNS.GameServer.Network.Client;

namespace SagaBNS.GameServer.Packets.Client
{
    public class CM_ITEM_EXCHANGE : Packet<GamePacketOpcode>
    {
        public CM_ITEM_EXCHANGE()
        {
            ID = GamePacketOpcode.CM_ITEM_EXCHANGE;
        }

        public ulong ActorID
        {
            get
            {
                return GetULong(2);
            }
        }

        public byte BuySlot
        {
            get
            {
                return GetByte(10);
            }
        }

        public ushort BuyCount
        {
            get
            {
                return GetUShort(11);
            }
        }

        public Dictionary<ushort, ushort> ExchangeItems
        {
            get
            {
                Dictionary<ushort, ushort> items = new Dictionary<ushort, ushort>();
                ushort count = GetUShort(13);
                for (int i = 0; i < count; i++)
                {
                    items.Add(GetUShort(), GetUShort());
                }

                return items;
            }
        }

        public override Packet<GamePacketOpcode> New()
        {
            return new CM_ITEM_EXCHANGE();
        }

        public override void OnProcess(Session<GamePacketOpcode> client)
        {
            ((GameSession)client).OnItemBuy(this);
        }
    }
}
