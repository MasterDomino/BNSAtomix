using SagaBNS.GameServer.Scripting;

namespace Maps.BambooVillageNight
{
    public class NPC1126 : NPCScriptHandler
    {
        public override ushort NpcID
        {
            get { return 1126; }
        }

        public override void OnCreate(bool success)
        {
            if (success)
            {
                BeginTask();
                Delay(5);
                Dash(-13283, -16631, -834, 60, 1424, 55);
                Delay(56);
                Dash(-13125, -16384, -827, 57, 0, 0);
                StartTask();
            }
        }

        public override void OnSkillDamage(SagaBNS.Common.Skills.SkillArg arg, SagaBNS.Common.Skills.SkillAttackResult result, int dmg, int bonusCount)
        {
            base.OnSkillDamage(arg, result, dmg, bonusCount);
            if (NPC.HP <= 80)
            {
                Disappear(1270);
                SendNPCCommand("q303_end");
            }
        }
    }
}
