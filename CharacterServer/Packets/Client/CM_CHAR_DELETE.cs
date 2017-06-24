
using SmartEngine.Network;
using SagaBNS.Common.Packets;
using SagaBNS.CharacterServer.Network.Client;

namespace SagaBNS.CharacterServer.Packets.Client
{
    public class CM_CHAR_DELETE : Common.Packets.CharacterServer.CM_CHAR_DELETE
    {
        public override Packet<CharacterPacketOpcode> New()
        {
            return new CM_CHAR_DELETE();
        }

        public override void OnProcess(Session<CharacterPacketOpcode> client)
        {
            ((CharacterSession)client).OnCharDelete(this);
        }
    }
}
