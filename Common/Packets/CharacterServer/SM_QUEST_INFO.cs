using System.Collections.Generic;

using SmartEngine.Network;
using SagaBNS.Common.Network;

namespace SagaBNS.Common.Packets.CharacterServer
{
    public class SM_QUEST_INFO : Packet<CharacterPacketOpcode>
    {
        private ushort offsetQuest;
        internal class SM_QUEST_INFO_INTERNAL<T> : SM_QUEST_INFO
        {
            public override Packet<CharacterPacketOpcode> New()
            {
                return new SM_QUEST_INFO_INTERNAL<T>();
            }

            public override void OnProcess(Session<CharacterPacketOpcode> client)
            {
                ((CharacterSession<T>)client).OnQuestInfo(this);
            }
        }

        public SM_QUEST_INFO()
        {
            ID = CharacterPacketOpcode.SM_QUEST_INFO;
        }

        public long SessionID
        {
            get
            {
                return GetLong(2);
            }
            set
            {
                PutLong(value, 2);
            }
        }

        public List<Quests.Quest> Quests
        {
            get
            {
                short count = GetShort(10);
                List<Common.Quests.Quest> list = new List<Quests.Quest>();
                for (int i = 0; i < count; i++)
                {
                    Common.Quests.Quest q = new Quests.Quest()
                    {
                        QuestID = GetUShort(),
                        Step = GetByte(),
                        StepStatus = GetByte(),
                        NextStep = GetByte(),
                        Flag1 = GetShort(),
                        Flag2 = GetShort(),
                        Flag3 = GetShort()
                    };
                    for (int j = 0; j < Common.Quests.Quest.Counts; j++)
                    {
                        q.Count[j] = GetInt();
                    }
                    list.Add(q);
                }
                offsetQuest = offset;
                return list;
            }
            set
            {
                PutShort((short)value.Count, 10);
                foreach (Common.Quests.Quest q in value)
                {
                    PutUShort(q.QuestID);
                    PutByte(q.Step);
                    PutByte(q.StepStatus);
                    PutByte(q.NextStep);
                    PutShort(q.Flag1);
                    PutShort(q.Flag2);
                    PutShort(q.Flag3);
                    for (int j = 0; j < Common.Quests.Quest.Counts; j++)
                    {
                        PutInt(q.Count[j]);
                    }
                }
                offsetQuest = offset;
            }
        }

        public List<ushort> QuestCompleted
        {
            get
            {
                short count = GetShort(offsetQuest);
                List<ushort> list = new List<ushort>();
                for (int i = 0; i < count; i++)
                {
                    list.Add(GetUShort());
                }
                return list;
            }
            set
            {
                PutShort((short)value.Count);
                foreach (ushort i in value)
                {
                    PutUShort(i);
                }
            }
        }
    }
}
