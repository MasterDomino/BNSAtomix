
using SmartEngine.Network;
using SagaBNS.Common.Packets;
using SagaBNS.GameServer.Network.Client;

namespace SagaBNS.GameServer.Packets.Client
{
    public class CM_LOCATION_DISCOVERY : Packet<GamePacketOpcode>
    {
        public CM_LOCATION_DISCOVERY()
        {
            ID = GamePacketOpcode.CM_LOCATION_DISCOVERY;
        }

        public override Packet<GamePacketOpcode> New()
        {
            return new CM_LOCATION_DISCOVERY();
        }

        public override void OnProcess(Session<GamePacketOpcode> client)
        {
            ((GameSession)client).SendTeleportAdd();
        }
    }
}
