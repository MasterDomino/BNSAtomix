
using SmartEngine.Network;
using SagaBNS.Common.Packets;
using SagaBNS.GameServer.Network.Client;

namespace SagaBNS.GameServer.Packets.Client
{
    public class CM_QUEST_LOOT_QUEST_ITEM : Packet<GamePacketOpcode>
    {
        public CM_QUEST_LOOT_QUEST_ITEM()
        {
            ID = GamePacketOpcode.CM_QUEST_LOOT_QUEST_ITEM;
        }

        public ushort QuestID
        {
            get
            {
                return GetUShort(4);
            }
        }

        public byte Step
        {
            get
            {
                return GetByte(6);
            }
        }

        public ulong ActorID
        {
            get
            {
                return GetULong(9);
            }
        }

        public override Packet<GamePacketOpcode> New()
        {
            return new CM_QUEST_LOOT_QUEST_ITEM();
        }

        public override void OnProcess(Session<GamePacketOpcode> client)
        {
            ((GameSession)client).OnQuestLootQuestItem(this);
        }
    }
}
