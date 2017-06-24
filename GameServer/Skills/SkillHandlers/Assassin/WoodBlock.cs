using SagaBNS.Common.Skills;

namespace SagaBNS.GameServer.Skills.SkillHandlers.Assassin
{
    public class WoodBlock : ISkillHandler
    {
        public WoodBlock()
        {

        }

        #region ISkillHandler 成员

        public void HandleOnSkillCasting(SkillArg arg)
        {

        }

        public void HandleOnSkillCastFinish(SkillArg arg)
        {

        }

        public void HandleSkillActivate(SkillArg arg)
        {
            Additions.WoodBlock buff = new Additions.WoodBlock(arg);
            buff.Activate();
        }

        public void OnAfterSkillCast(SkillArg arg)
        {
        }
        #endregion
    }
}
