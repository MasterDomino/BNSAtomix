
using SmartEngine.Network;
using SagaBNS.Common.Packets;
using SagaBNS.CharacterServer.Network.Client;

namespace SagaBNS.CharacterServer.Packets.Client
{
    public class CM_LOGIN_REQUEST : Common.Packets.CharacterServer.CM_LOGIN_REQUEST
    {
        public override Packet<CharacterPacketOpcode> New()
        {
            return new CM_LOGIN_REQUEST();
        }

        public override void OnProcess(Session<CharacterPacketOpcode> client)
        {
            ((CharacterSession)client).OnLoginRequest(this);
        }
    }
}
