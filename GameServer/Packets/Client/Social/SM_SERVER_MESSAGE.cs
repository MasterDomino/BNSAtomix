using System.Text;

using SmartEngine.Network;
using SagaBNS.Common.Packets;

namespace SagaBNS.GameServer.Packets.Client
{
    public class SM_SERVER_MESSAGE : Packet<GamePacketOpcode>
    {
        public enum Positions
        {
            Top,
            ChatWindow,
        }

        public SM_SERVER_MESSAGE()
        {
            ID = GamePacketOpcode.SM_SERVER_MESSAGE;
        }

        public Positions MessagePosition
        {
            set
            {
                PutByte((byte)value, 15);
            }
        }

        public string Message
        {
            set
            {
                PutUShort((ushort)value.Length, 16);
                PutBytes(Encoding.Unicode.GetBytes(value));
            }
        }
    }
}
