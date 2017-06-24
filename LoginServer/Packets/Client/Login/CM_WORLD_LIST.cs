
using SmartEngine.Network;
using SagaBNS.Common.Packets;
using SagaBNS.LoginServer.Network.Client;

namespace SagaBNS.LoginServer.Packets.Client
{
    public class CM_WORLD_LIST : BNSLoginPacket
    {
        public CM_WORLD_LIST()
        {
            ID = LoginPacketOpcode.CM_WORLD_LIST;
        }

        public override Packet<LoginPacketOpcode> New()
        {
            return new CM_WORLD_LIST();
        }

        public override void OnProcess(Session<LoginPacketOpcode> client)
        {
            ((LoginSession)client).OnWorldList(GetInt(2));
        }
    }
}
