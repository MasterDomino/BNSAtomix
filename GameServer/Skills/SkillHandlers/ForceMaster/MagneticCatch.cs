using System.Collections.Generic;
using SmartEngine.Network.Tasks;
using SagaBNS.Common.Skills;
using SagaBNS.GameServer.Skills.SkillHandlers.Common;

namespace SagaBNS.GameServer.Skills.SkillHandlers.ForceMaster
{
    public class MagneticCatch : DefaultAttack
    {
        private readonly uint additionID;
        public MagneticCatch(uint additionID)
        {
            this.additionID = additionID;
        }
        #region ISkillHandler 成员

        public override void HandleSkillActivate(SkillArg arg)
        {
            SkillManager.Instance.DoAttack(arg);
            foreach (SkillAffectedActor i in arg.AffectedActors)
            {
                SkillAttackResult res = i.Result;
                if (res != SkillAttackResult.Avoid && res != SkillAttackResult.Miss && res != SkillAttackResult.Parry && res != SkillAttackResult.TotalParry)
                {
                    if (i.Target.Tasks.ContainsKey("ActorCatch"))
                    {
                        Buff buff = i.Target.Tasks["ActorCatch"] as Buff;
                        buff.Deactivate();
                    }

                    Common.Additions.ActorCatch add = new Common.Additions.ActorCatch(arg, i.Target, additionID, i.Damage, 5000);

                    i.Target.Tasks["ActorCatch"] = add;
                    add.Activate();
                }
            }
        }

        #endregion
    }
}
