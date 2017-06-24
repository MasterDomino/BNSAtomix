
using SmartEngine.Network;
using SagaBNS.Common.Packets;
using SagaBNS.LoginServer.Network.CharacterServer;

namespace SagaBNS.LoginServer.Packets.CharacterServer
{
    public class SM_CHAR_CREATE_RESULT : Common.Packets.CharacterServer.SM_CHAR_CREATE_RESULT
    {
        public override Packet<CharacterPacketOpcode> New()
        {
            return new SM_CHAR_CREATE_RESULT();
        }

        public override void OnProcess(Session<CharacterPacketOpcode> client)
        {
            ((CharacterSession)client).OnCharCreateResult(this);
        }
    }
}
