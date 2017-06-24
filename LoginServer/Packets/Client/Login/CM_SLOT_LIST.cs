
using SmartEngine.Network;
using SagaBNS.Common.Packets;
using SagaBNS.LoginServer.Network.Client;

namespace SagaBNS.LoginServer.Packets.Client
{
    public class CM_CHAR_LIST : BNSLoginPacket
    {
        public CM_CHAR_LIST()
        {
            ID = LoginPacketOpcode.CM_CHAR_LIST;
        }

        public override Packet<LoginPacketOpcode> New()
        {
            return new CM_CHAR_LIST();
        }

        public override void OnProcess(Session<LoginPacketOpcode> client)
        {
            ((LoginSession)client).OnCharList(GetInt(2));
        }
    }
}
