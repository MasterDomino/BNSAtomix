using System.Text;

using SmartEngine.Network;
using SmartEngine.Network.Utils;
using SagaBNS.ChatServer.Network.Client;

namespace SagaBNS.ChatServer.Packets.Client
{
    public class CM_LOGIN_AUTH : Packet<BNSChatOpcodes>
    {
        public CM_LOGIN_AUTH()
        {
            ID = BNSChatOpcodes.CM_LOGIN_AUTH;
        }

        public ulong ActorID
        {
            get
            {
                return GetULong(26);
            }
        }

        public void GetInfo(out string name, out string email,out string token)
        {
            name = Encoding.Unicode.GetString(GetBytes((ushort)(GetShort(42) * 2)));
            email = Encoding.Unicode.GetString(GetBytes((ushort)(GetShort() * 2)));
            token = Conversions.bytes2HexString(GetBytes((ushort)(GetShort())));
        }

        public override Packet<BNSChatOpcodes> New()
        {
            return new CM_LOGIN_AUTH();
        }

        public override void OnProcess(Session<BNSChatOpcodes> client)
        {
            ((ChatSession)client).OnLoginAuth(this);
        }
    }
}
