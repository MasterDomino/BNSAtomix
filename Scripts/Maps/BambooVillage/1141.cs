using SagaBNS.GameServer.Scripting;
using SagaBNS.Common.Actors;
using SagaBNS.Common.Quests;

namespace Maps.Startzone
{
    public class NPC1141 : NPCScriptHandler
    {
        public override ushort NpcID
        {
            get { return 1141; }
        }

        public override void OnQuest(ActorPC pc, ushort questID, byte step, Quest quest)
        {
            switch (questID)
            {
                case 305:
                    if (step == 4)
                    {
                        BeginTask();
                        Delay(5);
                        NPCChat(1141, pc);
                        Delay(7);
                        Move(-7606, -12831, -477, 239, 80);
                        Move(-7629, -12850, -477, 220, 467);
                        Move(-7655, -12857, -477, 195, 419);
                        Move(-7668, -12851, -477, 155, 225);
                        Move(-7696, -12836, -477, 151, 500);
                        Move(-7717, -12814, -479, 132, 500);
                        Move(-7733, -12796, -484, 132, 387);
                        StartTask();
                    }
                    break;
            }
        }
    }
}
