
using SmartEngine.Network;
using SagaBNS.Common.Packets;
using SagaBNS.CharacterServer.Network.Client;

namespace SagaBNS.CharacterServer.Packets.Client
{
    public class CM_ACTOR_INFO_REQUEST : Common.Packets.CharacterServer.CM_ACTOR_INFO_REQUEST
    {
        public override Packet<CharacterPacketOpcode> New()
        {
            return new CM_ACTOR_INFO_REQUEST();
        }

        public override void OnProcess(Session<CharacterPacketOpcode> client)
        {
            ((CharacterSession)client).OnActorInfoRequest(this);
        }
    }
}
