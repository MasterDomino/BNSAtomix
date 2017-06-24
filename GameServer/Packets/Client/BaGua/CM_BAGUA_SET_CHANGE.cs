using System.Collections.Generic;

using SmartEngine.Network;
using SagaBNS.Common.Packets;
using SagaBNS.GameServer.Network.Client;

namespace SagaBNS.GameServer.Packets.Client
{
    public class CM_BAGUA_SET_CHANGE : Packet<GamePacketOpcode>
    {
        public CM_BAGUA_SET_CHANGE()
        {
            ID = GamePacketOpcode.CM_BAGUA_SET_CHANGE;
        }

        public Dictionary<ushort, byte> EquipItems
        {
            get
            {
                Dictionary<ushort, byte> items = new Dictionary<ushort, byte>();
                ushort count = GetUShort(2);
                for (int i = 0; i < count; i++)
                {
                    items.Add(GetUShort(), GetByte());
                }

                return items;
            }
        }

        public override Packet<GamePacketOpcode> New()
        {
            return new CM_BAGUA_SET_CHANGE();
        }

        public override void OnProcess(Session<GamePacketOpcode> client)
        {
            ((GameSession)client).ChangeBaGua(this);
        }
    }
}
