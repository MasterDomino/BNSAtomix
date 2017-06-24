
using SmartEngine.Network;
using SagaBNS.Common.Packets;
using SagaBNS.LoginServer.Network.Client;

namespace SagaBNS.LoginServer.Packets.Client
{
    public class CM_AUTH_GAME_TOKEN : BNSLoginPacket
    {
        public CM_AUTH_GAME_TOKEN()
        {
            ID = LoginPacketOpcode.CM_AUTH_GAME_TOKEN;
        }

        public override Packet<LoginPacketOpcode> New()
        {
            return new CM_AUTH_GAME_TOKEN();
        }

        public override void OnProcess(Session<LoginPacketOpcode> client)
        {
            ((LoginSession)client).OnAuthGameToken(GetInt(2));
        }
    }
}
