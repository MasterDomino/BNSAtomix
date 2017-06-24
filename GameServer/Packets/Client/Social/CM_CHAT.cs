using System.Text;

using SmartEngine.Network;
using SagaBNS.Common.Packets;
using SagaBNS.GameServer.Network.Client;

namespace SagaBNS.GameServer.Packets.Client
{
    public class CM_CHAT : Packet<GamePacketOpcode>
    {
        private long offsetAfter;
        public CM_CHAT()
        {
            ID = GamePacketOpcode.CM_CHAT;
        }

        public ChatType Type
        {
            get
            {
                return (ChatType)GetByte(2);
            }
        }

        public string Recipient
        {
            get
            {
                ushort len = GetUShort(3);
                string name = len != 0 ? Encoding.Unicode.GetString(GetBytes((ushort)(len * 2))) : null;
                offsetAfter = offset;
                return name;
            }
        }

        public string Text
        {
            get
            {
                ushort len = GetUShort((ushort)(offsetAfter + 4));

                return Encoding.Unicode.GetString(GetBytes((ushort)(len * 2))).Trim('\0');
            }
        }

        public override Packet<GamePacketOpcode> New()
        {
            return new CM_CHAT();
        }

        public override void OnProcess(Session<GamePacketOpcode> client)
        {
            ((GameSession)client).OnChat(this);
        }
    }
}
