using SagaBNS.Common.Actors;
using SagaBNS.Common.Skills;
using SagaBNS.GameServer.Skills.SkillHandlers.Common;

namespace SagaBNS.GameServer.Skills.SkillHandlers.ForceMaster
{
    public class FrosenBall : DefaultAttack
    {
        public FrosenBall()
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
                    Additions.Frosen frosen = new Additions.Frosen(arg, i.Target);
                    frosen.Activate();
                }
            }
            arg.Caster.ChangeStance(Stances.Ice, arg.SkillSession, 12000002, 6);
            Additions.FrosenSelf frosenSelf = new Additions.FrosenSelf(arg);
            frosenSelf.Activate();
        }

        #endregion
    }
}
