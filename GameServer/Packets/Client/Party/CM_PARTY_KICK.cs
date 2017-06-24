using System.Text;

using SmartEngine.Network;
using SagaBNS.Common.Packets;
using SagaBNS.GameServer.Network.Client;

namespace SagaBNS.GameServer.Packets.Client
{
    public class CM_PARTY_KICK : Packet<GamePacketOpcode>
    {
        public CM_PARTY_KICK()
        {
            ID = GamePacketOpcode.CM_PARTY_KICK;
        }

        public string Name
        {
            get
            {
                return Encoding.Unicode.GetString(GetBytes((ushort)(GetUShort(2) * 2)));
            }
        }

        public override Packet<GamePacketOpcode> New()
        {
            return new CM_PARTY_KICK();
        }

        public override void OnProcess(Session<GamePacketOpcode> client)
        {
            ((GameSession)client).OnPartyKick(this);
        }
    }
}
