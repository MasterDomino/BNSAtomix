
using SmartEngine.Network;
using SagaBNS.Common.Packets;
using SagaBNS.GameServer.Network.Client;

namespace SagaBNS.GameServer.Packets.Client
{
    public class CM_MAP_PORTAL_ENTER : Packet<GamePacketOpcode>
    {
        public CM_MAP_PORTAL_ENTER()
        {
            ID = GamePacketOpcode.CM_MAP_PORTAL_ENTER;
        }

        public override Packet<GamePacketOpcode> New()
        {
            return new CM_MAP_PORTAL_ENTER();
        }

        public override void OnProcess(Session<GamePacketOpcode> client)
        {
            ((GameSession)client).OnMapPortalEnter(this);
        }
    }
}
