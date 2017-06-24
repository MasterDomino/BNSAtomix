
using SmartEngine.Network;
using SagaBNS.Common.Packets;
using SagaBNS.GameServer.Network.Client;

namespace SagaBNS.GameServer.Packets.Client
{
    public class CM_LOCATION_LOAD_TELEPORT : Packet<GamePacketOpcode>
    {
        public CM_LOCATION_LOAD_TELEPORT()
        {
            ID = GamePacketOpcode.CM_LOCATION_LOAD_TELEPORT;
        }

        public override Packet<GamePacketOpcode> New()
        {
            return new CM_LOCATION_LOAD_TELEPORT();
        }

        public override void OnProcess(Session<GamePacketOpcode> client)
        {
            ((GameSession)client).SendTeleportLoad();
        }
    }
}
