
using SmartEngine.Network;

namespace SagaBNS.ChatServer.Packets.Client
{
    public class SM_LOGIN_AUTH_RESULT : Packet<BNSChatOpcodes>
    {
        public SM_LOGIN_AUTH_RESULT()
        {
            ID = BNSChatOpcodes.SM_LOGIN_AUTH_RESULT;

            PutInt(1, 2);
            PutShort(4990);
        }
    }
}
