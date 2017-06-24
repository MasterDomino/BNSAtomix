using System.Collections.Generic;

using SmartEngine.Network;
using SagaBNS.Common.Packets;
using SagaBNS.GameServer.Network.Client;

namespace SagaBNS.GameServer.Packets.Client
{
    public class CM_HAMMER_REPAIR : Packet<GamePacketOpcode>
    {
        public CM_HAMMER_REPAIR()
        {
            ID = GamePacketOpcode.CM_HAMMER_REPAIR;
        }

        public ulong CampfireID
        {
            get
            {
                return GetULong(2);
            }
        }

        public Dictionary<ushort, ushort> ExchangeItems
        {
            get
            {
                Dictionary<ushort, ushort> items = new Dictionary<ushort, ushort>();
                ushort count = GetUShort(10);
                for (int i = 0; i < count; i++)
                {
                    items.Add(GetUShort(), GetUShort());
                }

                return items;
            }
        }

        public override Packet<GamePacketOpcode> New()
        {
            return new CM_HAMMER_REPAIR();
        }

        public override void OnProcess(Session<GamePacketOpcode> client)
        {
            ((GameSession)client).OnHammerRepair(this);
        }
    }
}
