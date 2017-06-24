using System.Collections.Generic;

using SmartEngine.Network;
using SagaBNS.Common.Packets;
using SagaBNS.GameServer.Network.Client;

namespace SagaBNS.GameServer.Packets.Client
{
    public class CM_TELEPORT_REQUEST : Packet<GamePacketOpcode>
    {
        public CM_TELEPORT_REQUEST()
        {
            ID = GamePacketOpcode.CM_TELEPORT_REQUEST;
        }

        public ushort Location
        {
            get
            {
                return GetUShort(2);
            }
        }

        public Dictionary<ushort, ushort> ExchangeItems
        {
            get
            {
                Dictionary<ushort, ushort> items = new Dictionary<ushort, ushort>();
                ushort count = GetUShort(4);
                for (int i = 0; i < count; i++)
                {
                    items.Add(GetUShort(), GetUShort());
                }

                return items;
            }
        }

        public override Packet<GamePacketOpcode> New()
        {
            return new CM_TELEPORT_REQUEST();
        }

        public override void OnProcess(Session<GamePacketOpcode> client)
        {
            ((GameSession)client).OnTeleportRequest(this);
        }
    }
}
