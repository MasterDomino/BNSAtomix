
using SmartEngine.Network;
using SagaBNS.Common.Packets;
using SagaBNS.CharacterServer.Network.Client;

namespace SagaBNS.CharacterServer.Packets.Client
{
    public class CM_TELEPORT_SAVE : Common.Packets.CharacterServer.CM_TELEPORT_SAVE
    {
        public override Packet<CharacterPacketOpcode> New()
        {
            return new CM_TELEPORT_SAVE();
        }

        public override void OnProcess(Session<CharacterPacketOpcode> client)
        {
            ((CharacterSession)client).OnTeleportSave(this);
        }
    }
}
