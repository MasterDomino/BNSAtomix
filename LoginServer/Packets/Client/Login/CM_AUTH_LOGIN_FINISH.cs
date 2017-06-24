
using SmartEngine.Network;
using SagaBNS.Common.Packets;
using SagaBNS.LoginServer.Network.Client;

namespace SagaBNS.LoginServer.Packets.Client
{
    public class CM_AUTH_LOGIN_FINISH : BNSLoginPacket
    {
        public CM_AUTH_LOGIN_FINISH()
        {
            ID = LoginPacketOpcode.CM_AUTH_LOGIN_FINISH;
        }

        public override Packet<LoginPacketOpcode> New()
        {
            return new CM_AUTH_LOGIN_FINISH();
        }

        public override void OnProcess(Session<LoginPacketOpcode> client)
        {
            ((LoginSession)client).OnAuthLoginFinish(GetInt(2));
        }
    }
}
