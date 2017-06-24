using System.Text;

using SmartEngine.Network;
using SagaBNS.Common.Packets;

namespace SagaBNS.GameServer.Packets.Client
{
    public class SM_CHAT : Packet<GamePacketOpcode>
    {
        private long offsetAfter;
        public SM_CHAT()
        {
            ID = GamePacketOpcode.SM_CHAT;
        }

        public void Recipient (string recipient, ulong actorId)
        {
            if (recipient != string.Empty && recipient != null)
            {
                PutUShort((ushort)recipient.Length, 2);
                byte[] buf = Encoding.Unicode.GetBytes(recipient);
                PutBytes(buf);
            }
            else
            {
                PutUShort(0);
            }
            PutULong(actorId);
            offsetAfter = offset;
        }

        public void PutMessage(string name, byte type, string content)
        {
            byte[] buf = Encoding.Unicode.GetBytes(name);
            PutUShort((ushort)name.Length, (ushort)offsetAfter);
            PutBytes(buf);
            PutByte(type);
            PutUInt(0);
            buf = Encoding.Unicode.GetBytes(content);
            PutUShort((ushort)content.Length);
            PutBytes(buf);
        }
    }
}
