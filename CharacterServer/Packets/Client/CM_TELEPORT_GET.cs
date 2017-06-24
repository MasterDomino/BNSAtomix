
using SmartEngine.Network;
using SagaBNS.Common.Packets;
using SagaBNS.CharacterServer.Network.Client;

namespace SagaBNS.CharacterServer.Packets.Client
{
    public class CM_TELEPORT_GET : Common.Packets.CharacterServer.CM_TELEPORT_GET
    {
        public override Packet<CharacterPacketOpcode> New()
        {
            return new CM_TELEPORT_GET();
        }

        public override void OnProcess(Session<CharacterPacketOpcode> client)
        {
            ((CharacterSession)client).OnTeleportGet(this);
        }
    }
}
