using System.Collections.Generic;

using SmartEngine.Network;
using SagaBNS.Common.Packets;
using SagaBNS.GameServer.Network.Client;

namespace SagaBNS.GameServer.Packets.Client
{
    public class CM_BAGUA_SET_REMOVE : Packet<GamePacketOpcode>
    {
        public CM_BAGUA_SET_REMOVE()
        {
            ID = GamePacketOpcode.CM_BAGUA_SET_REMOVE;
        }

        public List<byte> RemoveItems
        {
            get
            {
                List<byte> items = new List<byte>();
                ushort count = GetUShort(2);
                for (int i = 0; i < count; i++)
                {
                    items.Add(GetByte());
                }

                return items;
            }
        }

        public override Packet<GamePacketOpcode> New()
        {
            return new CM_BAGUA_SET_REMOVE();
        }

        public override void OnProcess(Session<GamePacketOpcode> client)
        {
            ((GameSession)client).RemoveBaGua(this);
        }
    }
}
