using SagaBNS.GameServer.Scripting;
using SagaBNS.Common.Actors;
using SagaBNS.Common.Quests;
using SagaBNS.Common.Skills;
namespace Maps.Startzone
{
    public class NPC767 : NPCScriptHandler
    {
        public override ushort NpcID
        {
            get { return 767; }
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
                AI.HateTable.Clear();
                NPC.Faction = Factions.FriendlyNPC;
                AI.Deactivate();
                AI.Pause = true;
            }
            if (command == "q225.End")
            {
                NPC.Faction = Factions.Training;
                AI.DueTime = 25000;
                AI.Activate();
                AI.Pause = false;
            }
        }
    }
}
