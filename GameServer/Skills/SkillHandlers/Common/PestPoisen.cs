using SagaBNS.Common.Skills;

namespace SagaBNS.GameServer.Skills.SkillHandlers.Common
{
    public class PestPoisen : DefaultAttack
    {
        public PestPoisen()
        {
        }

        public override void HandleSkillActivate(SkillArg arg)
        {
            base.HandleSkillActivate(arg);
            foreach (SkillAffectedActor i in arg.AffectedActors)
            {
                if (i.Result != SkillAttackResult.Miss && i.Result != SkillAttackResult.Avoid)
                {
                    Common.Additions.Poisen poisen = new Additions.Poisen(arg, i.Target, 52000206, i.Damage, 12000);
                    poisen.Activate();
                }
            }
        }
    }
}
