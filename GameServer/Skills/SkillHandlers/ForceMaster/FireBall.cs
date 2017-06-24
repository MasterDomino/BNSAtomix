using SagaBNS.Common.Actors;
using SagaBNS.Common.Skills;
using SagaBNS.GameServer.Skills.SkillHandlers.Common;

namespace SagaBNS.GameServer.Skills.SkillHandlers.ForceMaster
{
    public class FireBall : DefaultAttack
    {
        public FireBall()
            : base(true)
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
                    Additions.Fire fire = new Additions.Fire(arg, i.Target);
                    fire.Activate();
                }
            }
            arg.Caster.ChangeStance(Stances.Fire, arg.SkillSession, 12000002, 6);
            Additions.FireSelf fireSelf = new Additions.FireSelf(arg);
            fireSelf.Activate();
        }

        #endregion
    }
}
