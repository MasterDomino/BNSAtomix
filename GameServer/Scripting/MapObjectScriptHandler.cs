using SagaBNS.Common.Actors;
using SagaBNS.Common.Quests;

namespace SagaBNS.GameServer.Scripting
{
    public abstract class MapObjectScriptHandler
    {
        #region Properties

        public abstract uint MapID { get; }

        public abstract uint ObjectID { get; }

        #endregion

        #region Methods

        public virtual void OnMapObjectNPCCommnd(ActorPC npc, SmartEngine.Network.Map.Map<Map.MapEvents> map)
        {
            // do nothing
        }

        public abstract void OnOperate(ActorPC pc, Map.Map map);

        protected Quest CreateNewQuest(ActorPC pc, ushort questID)
        {
            Quest q = new Quest()
            {
                QuestID = questID
            };
            if (pc.Quests.ContainsKey(questID))
            {
                return null;
            }
            else
            {
                pc.Quests[questID] = q;
                return q;
            }
        }

        protected void GiveItem(ActorPC pc, uint itemID)
        {
            ((ActorEventHandlers.PCEventHandler)pc.EventHandler).Client.AddItem(itemID);
        }

        protected void SpawnNPC(Map.Map map, ushort npcID, short x, short y, short z, ushort dir, ushort motion)
        {
            Utils.SpawnNPC(map, npcID, x, y, z, dir, motion);
        }

        protected void UpdateQuest(ActorPC pc, Quest quest)
        {
            ((ActorEventHandlers.PCEventHandler)pc.EventHandler).Client.SendQuestUpdate(quest);
        }

        #endregion
    }
}