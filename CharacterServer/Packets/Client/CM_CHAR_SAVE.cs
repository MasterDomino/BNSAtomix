
using SmartEngine.Network;
using SagaBNS.Common.Packets;
using SagaBNS.CharacterServer.Network.Client;

namespace SagaBNS.CharacterServer.Packets.Client
{
    public class CM_CHAR_SAVE : Common.Packets.CharacterServer.CM_CHAR_SAVE
    {
        public override Packet<CharacterPacketOpcode> New()
        {
            return new CM_CHAR_SAVE();
        }

        public override void OnProcess(Session<CharacterPacketOpcode> client)
        {
            ((CharacterSession)client).OnCharSave(this);
        }
    }
}
