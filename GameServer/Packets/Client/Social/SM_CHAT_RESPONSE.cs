
using SmartEngine.Network;
using SagaBNS.Common.Packets;

namespace SagaBNS.GameServer.Packets.Client
{
    public class SM_CHAT_RESPONSE : Packet<GamePacketOpcode>
    {
        public SM_CHAT_RESPONSE()
        {
            ID = GamePacketOpcode.SM_CHAT_RESPONSE;
        }

        public ushort MessageId
        {
            set
            {
                PutUShort(value, 2);
            }
        }
    }
}
