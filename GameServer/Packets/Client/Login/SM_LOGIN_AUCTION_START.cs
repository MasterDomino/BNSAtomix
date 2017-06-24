
using SmartEngine.Network;
using SagaBNS.Common.Packets;

namespace SagaBNS.GameServer.Packets.Client
{
    public class SM_LOGIN_AUCTION_START : Packet<GamePacketOpcode>
    {
        public SM_LOGIN_AUCTION_START()
        {
            ID = GamePacketOpcode.SM_LOGIN_AUCTION_START;
        }

        public byte[] IP
        {
            set
            {
                PutBytes(value,2);
            }
        }

        public ushort Port
        {
            set
            {
                PutUShort(value,6);
            }
        }

        public byte[] Token
        {
            set
            {
                PutBytes(value,8);
            }
        }
    }
}
