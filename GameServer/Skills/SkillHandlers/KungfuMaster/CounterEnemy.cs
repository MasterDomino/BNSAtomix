using SagaBNS.Common.Skills;

namespace SagaBNS.GameServer.Skills.SkillHandlers.KungfuMaster
{
    public class CounterEnemy : ISkillHandler
    {
        #region ISkillHandler 成员

        public void HandleOnSkillCasting(SkillArg arg)
        {

        }

        public void HandleOnSkillCastFinish(SkillArg arg)
        {

        }

        public void HandleSkillActivate(SkillArg arg)
        {
            Additions.CounterEnemy buff = new Additions.CounterEnemy(arg);
            buff.Activate();
        }

        public void OnAfterSkillCast(SkillArg arg)
        {

        }

        #endregion
    }
}
