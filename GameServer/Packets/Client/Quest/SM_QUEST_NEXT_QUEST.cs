
using SmartEngine.Network;
using SagaBNS.Common.Packets;

namespace SagaBNS.GameServer.Packets.Client
{
    public class SM_QUEST_NEXT_QUEST : Packet<GamePacketOpcode>
    {
        public SM_QUEST_NEXT_QUEST()
        {
            ID = GamePacketOpcode.SM_QUEST_NEXT_QUEST;
        }

        public ushort QuestID
        {
            set
            {
                PutUShort(value, 2);
                PutByte(0);
            }
        }
    }
}
