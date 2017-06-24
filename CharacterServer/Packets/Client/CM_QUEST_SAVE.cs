
using SmartEngine.Network;
using SagaBNS.Common.Packets;
using SagaBNS.CharacterServer.Network.Client;

namespace SagaBNS.CharacterServer.Packets.Client
{
    public class CM_QUEST_SAVE : Common.Packets.CharacterServer.CM_QUEST_SAVE
    {
        public override Packet<CharacterPacketOpcode> New()
        {
            return new CM_QUEST_SAVE();
        }

        public override void OnProcess(Session<CharacterPacketOpcode> client)
        {
            ((CharacterSession)client).OnQuestSave(this);
        }
    }
}
