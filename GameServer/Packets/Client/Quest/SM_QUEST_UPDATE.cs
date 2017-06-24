
using SmartEngine.Network;
using SagaBNS.Common.Packets;

namespace SagaBNS.GameServer.Packets.Client
{
    public class SM_QUEST_UPDATE : Packet<GamePacketOpcode>
    {
        public SM_QUEST_UPDATE()
        {
            ID = GamePacketOpcode.SM_QUEST_UPDATE;
            PutUInt(0, 13);
            PutShort(0);
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

        public byte NextStep
        {
            set
            {
                PutByte(value, 6);
            }
        }

        public short Flag1
        {
            set
            {
                PutShort(value, 7);
            }
        }

        public short Flag2
        {
            set
            {
                PutShort(value, 9);
            }
        }

        public short Flag3
        {
            set
            {
                PutShort(value, 11);
            }
        }
    }
}
