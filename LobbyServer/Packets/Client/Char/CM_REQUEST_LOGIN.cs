
using SmartEngine.Network;
using SagaBNS.Common.Packets;
using SagaBNS.LobbyServer.Network.Client;

namespace SagaBNS.LobbyServer.Packets.Client
{
    public class CM_REQUEST_LOGIN : Packet<LobbyPacketOpcode>
    {
        public CM_REQUEST_LOGIN()
        {
            ID = LobbyPacketOpcode.CM_REQUEST_LOGIN;
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
            return new CM_REQUEST_LOGIN();
        }

        public override void OnProcess(Session<LobbyPacketOpcode> client)
        {
            ((LobbySession)client).OnRequestLogin(this);
        }
    }
}
