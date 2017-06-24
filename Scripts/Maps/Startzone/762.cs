using SagaBNS.GameServer.Scripting;
using SagaBNS.Common.Actors;
using SagaBNS.Common.Quests;
using SagaBNS.Common.Skills;
namespace Maps.Startzone
{
    public class NPC762 : NPCScriptHandler
    {
        public override ushort NpcID
        {
            get { return 762; }
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
                BeginTask();
                AI.Pause = true;
                Delay(10);
                Move(-2156, 10566, 555, 350, 274);
                Move(-2177, 10588, 555, 132, 500);
                Move(-2197, 10610, 555, 132, 500);
                Move(-2208, 10622, 557, 132, 258);
                Move(-2216, 10624, 557, 166, 444, false);
                NPCChat(1557, 166);
                StartTask();
            }
            if (command == "q225.End")
            {
                AI.DueTime = 25000;
                AI.Activate();
                AI.Pause = false;
                BeginTask();
                Delay(180);
                Move(-2214, 10621, 557, 304, 48);
                Move(-2194, 10598, 555, 312, 500);
                Move(-2174, 10575, 555, 312, 500);
                Move(-2169, 10570, 555, 312, 112);
                Move(-2160, 10569, 555, 348, 500, false);
                Move(-2155, 10567, 555, 348, 277, false);
                Move(-2155, 10567, 555, 337, 0);
                NPCChat(2334, 337);
                StartTask();
            }
        }
    }
}
