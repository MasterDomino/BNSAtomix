
using SmartEngine.Network;
using SagaBNS.Common.Packets;
using SagaBNS.CharacterServer.Network.Client;

namespace SagaBNS.CharacterServer.Packets.Client
{
    public class CM_SKILL_GET : Common.Packets.CharacterServer.CM_SKILL_GET
    {
        public override Packet<CharacterPacketOpcode> New()
        {
            return new CM_SKILL_GET();
        }

        public override void OnProcess(Session<CharacterPacketOpcode> client)
        {
            ((CharacterSession)client).OnSkillGet(this);
        }
    }
}
