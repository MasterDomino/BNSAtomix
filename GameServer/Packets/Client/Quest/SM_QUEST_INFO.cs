using System.Collections.Generic;

using SmartEngine.Network;
using SagaBNS.Common.Quests;
using SagaBNS.Common.Packets;

namespace SagaBNS.GameServer.Packets.Client
{
    public class SM_QUEST_INFO : Packet<GamePacketOpcode>
    {
        public SM_QUEST_INFO()
        {
            ID = GamePacketOpcode.SM_QUEST_INFO;
        }

        public List<Quest> Quests
        {
            set
            {
                PutShort((short)value.Count, 2);
                foreach (Quest i in value)
                {
                    PutUShort(i.QuestID);
                    PutByte(i.NextStep);
                    PutShort(i.Flag1);
                    PutShort(i.Flag2);
                    PutShort(i.Flag3);
                    PutShort(0);
                    PutShort(0);
                    PutShort(0);
                }
            }
        }
    }
}
