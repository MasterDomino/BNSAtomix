using SagaBNS.Common.Skills;
using SagaBNS.GameServer.Skills.SkillHandlers.Common;

namespace SagaBNS.GameServer.Skills.SkillHandlers.Assassin
{
    public class AssassinAttack : DefaultAttack
    {
        public AssassinAttack()
        {
        }

        public AssassinAttack(bool addmana)
            : base(addmana)
        {
        }
        #region ISkillHandler 成员

        public override void  HandleSkillActivate(SkillArg arg)
        {
            base.HandleSkillActivate(arg);
            if (arg.Caster.Tasks.ContainsKey("Poisening"))
            {
                foreach (SkillAffectedActor i in arg.AffectedActors)
                {
                    if (i.Result.IsHit())
                    {
                        Additions.PoisenCharge poisen = new Additions.PoisenCharge(arg, i.Target);
                        poisen.Activate();
                    }
                }
            }
        }

        #endregion
    }
}
