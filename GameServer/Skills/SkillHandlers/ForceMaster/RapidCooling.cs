using SagaBNS.Common.Skills;
using SagaBNS.GameServer.Skills.SkillHandlers.Common;

namespace SagaBNS.GameServer.Skills.SkillHandlers.ForceMaster
{
    public class RapidCooling : DefaultAttack
    {
        public RapidCooling()
            : base(false)
        {
        }
        #region ISkillHandler 成员

        public override void HandleSkillActivate(SkillArg arg)
        {
            base.HandleSkillActivate(arg);
            for (int idx = 0; idx < arg.AffectedActors.Count; idx++)
            {
                SkillAffectedActor i = arg.AffectedActors[idx];
                if (i.Result.IsHit())
                {
                    Additions.Frost frost = new Additions.Frost(arg, i.Target);
                    frost.Activate();
                }
            }
        }

        #endregion
    }
}
