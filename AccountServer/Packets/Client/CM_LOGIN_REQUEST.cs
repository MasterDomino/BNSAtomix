
using SmartEngine.Network;
using SagaBNS.Common.Packets;
using SagaBNS.AccountServer.Network.Client;

namespace SagaBNS.AccountServer.Packets.Client
{
    public class CM_LOGIN_REQUEST : Common.Packets.AccountServer.CM_LOGIN_REQUEST
    {
        public override Packet<AccountPacketOpcode> New()
        {
            return new CM_LOGIN_REQUEST();
        }

        public override void OnProcess(Session<AccountPacketOpcode> client)
        {
            ((AccountSession)client).OnLoginRequest(this);
        }
    }
}
