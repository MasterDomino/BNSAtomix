
using SmartEngine.Network;
using SagaBNS.Common.Packets;
using SagaBNS.LobbyServer.Network.Client;

namespace SagaBNS.LobbyServer.Packets.Client
{
    public class CM_CHAR_DELETE : Packet<LobbyPacketOpcode>
    {
        public CM_CHAR_DELETE()
        {
            ID = LobbyPacketOpcode.CM_CHAR_DELETE;
        }

        public byte[] Guid
        {
            get
            {
                return GetBytes(16, 2);
            }
        }

        public override Packet<LobbyPacketOpcode> New()
        {
            return new CM_CHAR_DELETE();
        }

        public override void OnProcess(Session<LobbyPacketOpcode> client)
        {
            ((LobbySession)client).OnDeleteChar(this);
        }
    }
}
