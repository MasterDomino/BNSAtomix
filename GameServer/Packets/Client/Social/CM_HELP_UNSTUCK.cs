using System.Text;

using SmartEngine.Network;
using SagaBNS.Common.Packets;
using SagaBNS.GameServer.Network.Client;

namespace SagaBNS.GameServer.Packets.Client
{
    public class CM_HELP_UNSTUCK : Packet<GamePacketOpcode>
    {
        public CM_HELP_UNSTUCK()
        {
            ID = GamePacketOpcode.CM_HELP_UNSTUCK;
        }

        public int Unknown
        {
            get
            {
                return GetInt(2);
            }
        }

        public string Unknown2
        {
            get
            {
                ushort len = GetUShort(6);
                return len != 0 ? Encoding.Unicode.GetString(GetBytes(len)).Trim('\0') : null;
            }
        }

        public override Packet<GamePacketOpcode> New()
        {
            return new CM_HELP_UNSTUCK();
        }

        public override void OnProcess(Session<GamePacketOpcode> client)
        {
            ((GameSession)client).OnHelpUnstuck(this);
        }
    }
}
