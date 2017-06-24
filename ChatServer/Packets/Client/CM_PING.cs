
using SmartEngine.Network;
using SagaBNS.ChatServer.Network.Client;

namespace SagaBNS.ChatServer.Packets.Client
{
    public class CM_PING : Packet<BNSChatOpcodes>
    {
        public CM_PING()
        {
            ID = BNSChatOpcodes.CM_PING;
        }

        public override Packet<BNSChatOpcodes> New()
        {
            return new CM_PING();
        }

        public override void OnProcess(Session<BNSChatOpcodes> client)
        {
            ((ChatSession)client).OnPing(this);
        }
    }
}
