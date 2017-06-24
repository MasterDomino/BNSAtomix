using System;
using SagaBNS.Common.Skills;
using SagaBNS.GameServer.Skills.SkillHandlers.Common;

namespace SagaBNS.GameServer.Skills.SkillHandlers.Assassin
{
    public class SpineStab : DefaultAttack
    {
        #region ISkillHandler 成员

        public override void  HandleSkillActivate(SkillArg arg)
        {
            if (Math.Abs(arg.Caster.Dir - arg.Target.Dir) <= 90)
            {
                base.HandleSkillActivate(arg);
            }
        }

        #endregion
    }
}
