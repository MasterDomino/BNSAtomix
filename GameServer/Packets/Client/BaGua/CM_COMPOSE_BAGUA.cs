using System.Collections.Generic;

using SmartEngine.Network;
using SagaBNS.Common.Packets;
using SagaBNS.GameServer.Network.Client;

namespace SagaBNS.GameServer.Packets.Client
{
    public class CM_COMPOSE_BAGUA : Packet<GamePacketOpcode>
    {
        public CM_COMPOSE_BAGUA()
        {
            ID = GamePacketOpcode.CM_COMPOSE_BAGUA;
        }

        public ushort PrimarySlot
        {
            get
            {
                return GetUShort(2);
            }
        }

        public ushort SecondarySlot
        {
            get
            {
                return GetUShort(4);
            }
        }

        public byte Stat
        {
            get
            {
                return GetByte(6);
            }
        }

        public Dictionary<ushort, ushort> ExchangeItems
        {
            get
            {
                Dictionary<ushort, ushort> items = new Dictionary<ushort, ushort>();
                ushort count = GetUShort(7);
                for (int i = 0; i < count; i++)
                {
                    items.Add(GetUShort(), GetUShort());
                }

                return items;
            }
        }

        public override Packet<GamePacketOpcode> New()
        {
            return new CM_COMPOSE_BAGUA();
        }

        public override void OnProcess(Session<GamePacketOpcode> client)
        {
            ((GameSession)client).ComposeBaGua(this);
        }
    }
}
