
using SmartEngine.Network;
using SagaBNS.Common.Packets;
using SagaBNS.LoginServer.Network.Client;

namespace SagaBNS.LoginServer.Packets.Client
{
    public class CM_ACCOUNT_LIST : BNSLoginPacket
    {
        public CM_ACCOUNT_LIST()
        {
            ID = LoginPacketOpcode.CM_ACCOUNT_LIST;
        }

        public override Packet<LoginPacketOpcode> New()
        {
            return new CM_ACCOUNT_LIST();
        }

        public override void OnProcess(Session<LoginPacketOpcode> client)
        {
            ((LoginSession)client).OnAccountList(GetInt(2));
        }
    }
}
