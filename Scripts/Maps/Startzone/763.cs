using SagaBNS.GameServer.Scripting;
using SagaBNS.Common.Actors;
using SagaBNS.Common.Quests;
using SagaBNS.Common.Skills;
namespace Maps.Startzone
{
    public class NPC763 : NPCScriptHandler
    {
        public override ushort NpcID
        {
            get { return 763; }
        }

        public override void OnQuest(ActorPC pc, ushort questID, byte step, Quest quest)
        {

        }

        public override void OnSkillDamage(SkillArg arg, SkillAttackResult result, int dmg, int bonusCount)
        {
            base.OnSkillDamage(arg, result, dmg, bonusCount);
            System.Threading.Interlocked.Exchange(ref NPC.HP, NPC.MaxHP);
        }

        public override void OnReceiveNPCCommand(ActorNPC npc, string command)
        {
            if (command == "q225.Start")
            {
                AI.Deactivate();
                AI.Pause = true;
                BeginTask();
                Delay(10);
                Move(-2386, 10641, 555, 270, 48);
                Move(-2355, 10645, 555, 8, 500);
                Move(-2324, 10650, 555, 8, 500);
                Move(-2306, 10653, 557, 8, 290);
                Move(-2298, 10652, 557, 350, 500, false);
                Move(-2295, 10651, 557, 350, 187, false);
                NPCChat(1558, 350);
                StartTask();
            }
            if (command == "q225.End")
            {
                AI.DueTime = 25000;
                AI.Activate();
                AI.Pause = false;
                BeginTask();
                Delay(180);
                Move(-2299, 10651, 557, 180, 64);
                Move(-2329, 10645, 555, 192, 500);
                Move(-2360, 10639, 555, 192, 500);
                Move(-2373, 10636, 555, 192, 209);
                Move(-2381, 10638, 555, 164, 500, false);
                Move(-2387, 10640, 555, 164, 375, false);
                Move(-2387, 10640, 555, 147, 0);
                NPCChat(2335, 147);
                StartTask();
            }
        }
    }
}
