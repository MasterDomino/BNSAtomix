using System.Text;

using SmartEngine.Network;

namespace SagaBNS.ChatServer.Packets.Client
{
    public class SM_CHAT : Packet<BNSChatOpcodes>
    {
        public SM_CHAT()
        {
            ID = BNSChatOpcodes.SM_CHAT;
        }

        public uint ChannelID
        {
            set
            {
                PutUInt(value, 2);
            }
        }

        public ulong ActorID
        {
            set
            {
                PutULong(value, 6);
            }
        }

        public void PutMessage(string name, string msg)
        {
            PutShort((short)name.Length);
            PutBytes(Encoding.Unicode.GetBytes(name));
            PutShort((short)msg.Length);
            PutBytes(Encoding.Unicode.GetBytes(msg));
        }
    }
}
