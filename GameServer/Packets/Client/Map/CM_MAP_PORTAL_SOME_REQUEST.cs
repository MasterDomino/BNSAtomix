
using SmartEngine.Network;
using SagaBNS.Common.Packets;
using SagaBNS.GameServer.Network.Client;

namespace SagaBNS.GameServer.Packets.Client
{
    public class CM_MAP_PORTAL_SOME_REQUEST : Packet<GamePacketOpcode>
    {
        public CM_MAP_PORTAL_SOME_REQUEST()
        {
            ID = GamePacketOpcode.CM_MAP_PORTAL_SOME_REQUEST;
        }

        public override Packet<GamePacketOpcode> New()
        {
            return new CM_MAP_PORTAL_SOME_REQUEST();
        }

        public override void OnProcess(Session<GamePacketOpcode> client)
        {
            ((GameSession)client).OnMapPortalSomeRequest(this);
        }
    }
}
