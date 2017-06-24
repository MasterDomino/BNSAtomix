using System.Collections.Generic;
using SmartEngine.Network.Tasks;
using SagaBNS.Common.Skills;

namespace SagaBNS.GameServer.Skills.SkillHandlers.Common
{
    public class Teleport : ISkillHandler
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
        }

        public void HandleSkillActivate(SkillArg arg, Dictionary<SagaBNS.Common.Item.Item, ushort> items)
        {
            if (arg.Caster.Tasks.ContainsKey("Teleport"))
            {
                Buff buff = arg.Caster.Tasks["Teleport"] as Buff;
                buff.Deactivate();
            }

            Additions.Teleport add = new Additions.Teleport(arg, items);

            arg.Caster.Tasks["Teleport"] = add;
            add.Activate();
        }

        public void OnAfterSkillCast(SkillArg arg)
        {
        }
        #endregion
    }
}
