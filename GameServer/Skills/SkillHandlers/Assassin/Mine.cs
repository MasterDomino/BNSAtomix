using SmartEngine.Network.Tasks;
using SagaBNS.Common.Skills;

namespace SagaBNS.GameServer.Skills.SkillHandlers.Assassin
{
    public class Mine : ISkillHandler
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
            if (arg.Caster.Tasks.ContainsKey("Mine"))
            {
                Buff buff = arg.Caster.Tasks["Mine"] as Buff;
                buff.Deactivate();
            }

            Additions.Mine add = new Additions.Mine(arg);

            arg.Caster.Tasks["Mine"] = add;
            add.Activate();
        }

        public void OnAfterSkillCast(SkillArg arg)
        {
        }
        #endregion
    }
}
