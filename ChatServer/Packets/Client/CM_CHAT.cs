using System.Text;

using SmartEngine.Network;
using SagaBNS.ChatServer.Network.Client;

namespace SagaBNS.ChatServer.Packets.Client
{
    public class CM_CHAT : Packet<BNSChatOpcodes>
    {
        public CM_CHAT()
        {
            ID = BNSChatOpcodes.CM_CHAT;
        }

        public uint ChannelID
        {
            get
            {
                return GetUInt(12);
            }
        }

        public string Message
        {
            get
            {
                return Encoding.Unicode.GetString(GetBytes((ushort)(GetShort(16) * 2)));
            }
        }

        public override Packet<BNSChatOpcodes> New()
        {
            return new CM_CHAT();
        }

        public override void OnProcess(Session<BNSChatOpcodes> client)
        {
            ((ChatSession)client).OnChat(this);
        }
    }
}
