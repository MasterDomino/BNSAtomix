
using SmartEngine.Network;
using SagaBNS.Common.Packets;
using SagaBNS.LoginServer.Network.Client;

namespace SagaBNS.LoginServer.Packets.Client
{
    public class CM_SLOT_LIST : BNSLoginPacket
    {
        public CM_SLOT_LIST()
        {
            ID = LoginPacketOpcode.CM_SLOT_LIST;
        }

        public override Packet<LoginPacketOpcode> New()
        {
            return new CM_SLOT_LIST();
        }

        public override void OnProcess(Session<LoginPacketOpcode> client)
        {
            ((LoginSession)client).SendSlotList(GetInt(2));
        }
    }
}
