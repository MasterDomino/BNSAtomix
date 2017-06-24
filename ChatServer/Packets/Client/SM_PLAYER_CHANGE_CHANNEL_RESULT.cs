
using SmartEngine.Network;

namespace SagaBNS.ChatServer.Packets.Client
{
    public class SM_PLAYER_CHANGE_CHANNEL_RESULT : Packet<BNSChatOpcodes>
    {
        public SM_PLAYER_CHANGE_CHANNEL_RESULT()
        {
            ID = BNSChatOpcodes.SM_PLAYER_CHANGE_CHANNEL_RESULT;
        }

        public int Unknown
        {
            set
            {
                PutInt(value, 2);
            }
        }

        public uint ChannelID
        {
            set
            {
                PutUInt(value, 6);
            }
        }
    }
}
