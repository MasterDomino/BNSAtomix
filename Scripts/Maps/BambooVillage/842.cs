using SagaBNS.GameServer.Scripting;
using SagaBNS.Common.Actors;
using SagaBNS.Common.Quests;

namespace Maps.BambooVillageNight
{
    public class NPC842 : NPCScriptHandler
    {
        public override ushort NpcID
        {
            get { return 842; }
        }

        public override void OnQuest(ActorPC pc, ushort questID, byte step, Quest quest)
        {
            switch (questID)
            {
                case 303:
                    if (step == 10)
                    {
                        SpawnNPC(82, -12588, -13840, -665, 140, 0);
                    }
                    break;
            }
        }
    }
}
