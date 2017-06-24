
using SmartEngine.Network;
using SagaBNS.Common.Packets;

namespace SagaBNS.GameServer.Packets.Client
{
    public class SM_LOGIN_CHAT_AUTH : Packet<GamePacketOpcode>
    {
        public SM_LOGIN_CHAT_AUTH()
        {
            ID = GamePacketOpcode.SM_LOGIN_CHAT_AUTH;
        }

        public ulong Server
        {
            set
            {
                PutULong(value,2);
            }
        }

        public byte[] Token
        {
            set
            {
                PutUShort((ushort)value.Length, 10);
                PutBytes(value);
            }
        }
    }
}
