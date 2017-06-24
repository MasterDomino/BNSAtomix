using SagaBNS.GameServer.Scripting;
using SagaBNS.Common.Actors;
using SagaBNS.Common.Quests;
namespace Maps.Startzone
{
    public class NPC770 : NPCScriptHandler
    {
        public override ushort NpcID
        {
            get { return 770; }
        }

        public override void OnQuest(ActorPC pc, ushort questID, byte step, Quest quest)
        {
            switch (questID)
            {
                case 227:
                case 230:
                case 236:
                case 239:
                    {
                        if (quest.NextStep == 4)
                        {
                            Disappear(2339);
                            BeginTask();
                            Delay(20);
                            SpawnNPCTask(675, 1542, -5326, 11039, 253, 5, 404);
                            StartTask();
                        }
                    }
                    break;
            }
        }
    }
}
