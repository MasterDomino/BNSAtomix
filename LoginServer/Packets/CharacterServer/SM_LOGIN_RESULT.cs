
using SmartEngine.Network;
using SagaBNS.Common.Packets;
using SagaBNS.LoginServer.Network.CharacterServer;

namespace SagaBNS.LoginServer.Packets.CharacterServer
{
    public class SM_LOGIN_RESULT : Common.Packets.CharacterServer.SM_LOGIN_RESULT
    {
        public override Packet<CharacterPacketOpcode> New()
        {
            return new SM_LOGIN_RESULT();
        }

        public override void OnProcess(Session<CharacterPacketOpcode> client)
        {
            ((CharacterSession)client).OnLoginResult(this);
        }
    }
}
