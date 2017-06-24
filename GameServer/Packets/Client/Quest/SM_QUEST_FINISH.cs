
using SmartEngine.Network;
using SagaBNS.Common.Packets;

namespace SagaBNS.GameServer.Packets.Client
{
    public class SM_QUEST_FINISH : Packet<GamePacketOpcode>
    {
        public SM_QUEST_FINISH()
        {
            ID = GamePacketOpcode.SM_QUEST_FINISH;
        }

        public ushort QuestID
        {
            set
            {
                PutUShort(value, 2);
            }
        }

        public byte Step
        {
            set
            {
                PutByte(value, 4);
            }
        }

        public byte StepStatus
        {
            set
            {
                PutByte(value, 5);
            }
        }
    }
}
