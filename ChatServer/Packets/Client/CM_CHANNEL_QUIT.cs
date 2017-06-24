
using SmartEngine.Network;
using SagaBNS.ChatServer.Network.Client;

namespace SagaBNS.ChatServer.Packets.Client
{
    public class CM_CHANNEL_QUIT : Packet<BNSChatOpcodes>
    {
        public CM_CHANNEL_QUIT()
        {
            ID = BNSChatOpcodes.CM_CHANNEL_QUIT;
        }

        public uint ChannelID
        {
            get
            {
                return GetUInt(4);
            }
        }

        public override Packet<BNSChatOpcodes> New()
        {
            return new CM_CHANNEL_QUIT();
        }

        public override void OnProcess(Session<BNSChatOpcodes> client)
        {
            ((ChatSession)client).OnChannelQuit(this);
        }
    }
}
