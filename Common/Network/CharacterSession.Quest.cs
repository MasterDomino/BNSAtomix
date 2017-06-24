using System.Collections.Generic;
using System.Linq;
using SmartEngine.Network;

using SagaBNS.Common.Packets;
using SagaBNS.Common.Actors;
using SagaBNS.Common.Packets.CharacterServer;

namespace SagaBNS.Common.Network
{
    public abstract partial class CharacterSession<T> : DefaultClient<CharacterPacketOpcode>
    {
        public void GetQuests(uint charID, T client)
        {
            long session = Global.PacketSession;
            packetSessions[session] = client;

            CM_QUEST_GET p = new CM_QUEST_GET()
            {
                SessionID = session,
                CharID = charID
            };
            Network.SendPacket(p);
        }

        public void SaveQuest(ActorPC pc)
        {
            CM_QUEST_SAVE p = new CM_QUEST_SAVE()
            {
                CharID = pc.CharID,
                Quests = pc.Quests.Values.ToList(),
                QuestCompleted = pc.QuestsCompleted
            };
            Network.SendPacket(p);
        }

        public void OnQuestInfo(SM_QUEST_INFO p)
        {
            long session = p.SessionID;
            if (packetSessions.ContainsKey(session))
            {
                if (packetSessions.TryRemove(session, out T client))
                {
                    OnQuestInfo(client, p.Quests, p.QuestCompleted);
                }
            }
        }

        protected abstract void OnQuestInfo(T client, List<Quests.Quest> quests, List<ushort> completed);
    }
}
