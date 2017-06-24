using SagaBNS.GameServer.Scripting;
using SagaBNS.Common.Actors;
using SagaBNS.Common.Quests;
using SagaBNS.Common.Skills;
namespace Maps.Startzone
{
    public class NPC760 : NPCScriptHandler
    {
        public override ushort NpcID
        {
            get { return 760; }
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
                Delay(13);
                Move(-2342, 10776, 555, 116, 64);
                Move(-2322, 10753, 555, 312, 500);
                Move(-2301, 10731, 555, 312, 500);
                Move(-2288, 10716, 555, 312, 306);
                Move(-2284, 10711, 555, 306, 500, false);
                Move(-2280, 10705, 555, 306, 500, false);
                NPCChat(1559, 306);
                StartTask();
            }
            if (command == "q225.End")
            {
                AI.DueTime = 25000;
                AI.Activate();
                AI.Pause = false;
                BeginTask();
                Delay(180);
                Move(-2282, 10706, 555, 153, 32);
                Move(-2300, 10731, 555, 126, 500);
                Move(-2318, 10756, 555, 126, 500);
                Move(-2330, 10772, 555, 126, 322);
                Move(-2337, 10774, 555, 159, 500, false);
                Move(-2343, 10777, 555, 159, 428, false);
                Move(-2343, 10777, 555, 196, 0);
                NPCChat(2336, 196);
                StartTask();
            }
        }
    }
}
