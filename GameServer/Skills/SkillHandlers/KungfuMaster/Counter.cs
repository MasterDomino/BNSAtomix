using SagaBNS.Common.Skills;
using SagaBNS.GameServer.Skills.SkillHandlers.Common;

namespace SagaBNS.GameServer.Skills.SkillHandlers.KungfuMaster
{
    public class Counter : DefaultAttack
    {
        #region ISkillHandler 成员

        public override void HandleSkillActivate(SkillArg arg)
        {
            base.HandleSkillActivate(arg);
            foreach (SkillAffectedActor i in arg.AffectedActors)
            {
                if (i.Result == SkillAttackResult.Normal || i.Result == SkillAttackResult.Critical)
                {
                    Common.Additions.Stun stun = new Common.Additions.Stun(arg, i.Target, 3000);
                    stun.Activate();
                }
            }
            Additions.CounterSelf counter = new Additions.CounterSelf(arg, 200);
            counter.Activate();
        }

        #endregion
    }
}
