using System.Linq;

using SmartEngine.Network;
using SagaBNS.Common.Packets;
using SagaBNS.Common.Packets.CharacterServer;
using SagaBNS.Common.Actors;
using SagaBNS.CharacterServer.Cache;

namespace SagaBNS.CharacterServer.Network.Client
{
    public partial class CharacterSession : Session<CharacterPacketOpcode>
    {
        public void OnQuestGet(Packets.Client.CM_QUEST_GET p)
        {
            ActorPC pc = CharacterCache.Instance[p.CharID];
            SM_QUEST_INFO p1 = new SM_QUEST_INFO()
            {
                SessionID = p.SessionID,
                Quests = pc.Quests.Values.ToList(),
                QuestCompleted = pc.QuestsCompleted
            };
            Network.SendPacket(p1);
        }

        public void OnQuestSave(Packets.Client.CM_QUEST_SAVE p)
        {
            ActorPC pc = CharacterCache.Instance[p.CharID];
            if (pc != null)
            {
                lock (pc.Quests)
                {
                    pc.Quests.Clear();
                    pc.QuestsCompleted.Clear();
                    foreach (Common.Quests.Quest q in p.Quests)
                    {
                        pc.Quests[q.QuestID] = q;
                    }

                    foreach (ushort i in p.QuestCompleted)
                    {
                        pc.QuestsCompleted.Add(i);
                    }
                }
            }
        }
    }
}
