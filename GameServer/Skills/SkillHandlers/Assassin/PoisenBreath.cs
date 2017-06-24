using SagaBNS.Common.Skills;
using SagaBNS.GameServer.Skills.SkillHandlers.Common;

namespace SagaBNS.GameServer.Skills.SkillHandlers.Assassin
{
    public class PoisenBreath : DefaultAttack
    {
        #region ISkillHandler 成员

        public override void  HandleSkillActivate(SkillArg arg)
        {
            base.HandleSkillActivate(arg);
            Additions.Poisening poisening = new Additions.Poisening(arg);
            poisening.Activate();
        }

        #endregion
    }
}
