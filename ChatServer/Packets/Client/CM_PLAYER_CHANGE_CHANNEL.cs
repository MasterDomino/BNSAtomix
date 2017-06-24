using System.Text;

using SmartEngine.Network;
using SagaBNS.ChatServer.Network.Client;

namespace SagaBNS.ChatServer.Packets.Client
{
    public class CM_PLAYER_CHANGE_CHANNEL : Packet<BNSChatOpcodes>
    {
        public CM_PLAYER_CHANGE_CHANNEL()
        {
            ID = BNSChatOpcodes.CM_PLAYER_CHANGE_CHANNEL;
        }

        public short Type
        {
            get
            {
                return GetShort(4);
            }
        }

        public string Channel
        {
            get
            {
                return Encoding.Unicode.GetString(GetBytes((ushort)(GetShort(6) * 2)));
            }
        }

        public override Packet<BNSChatOpcodes> New()
        {
            return new CM_PLAYER_CHANGE_CHANNEL();
        }

        public override void OnProcess(Session<BNSChatOpcodes> client)
        {
            ((ChatSession)client).OnPlayerChangeChannel(this);
        }
    }
}
