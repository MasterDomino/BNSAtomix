using System.Collections.Generic;
using SmartEngine.Network.Tasks;
using SagaBNS.Common.Skills;

namespace SagaBNS.GameServer.Skills.SkillHandlers.Common
{
    public class Repair : ISkillHandler
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
            if (arg.Caster.Tasks.ContainsKey("Repair"))
            {
                Buff buff = arg.Caster.Tasks["Repair"] as Buff;
                buff.Deactivate();
            }

            Additions.Repair add = new Additions.Repair(arg, items);

            arg.Caster.Tasks["Repair"] = add;
            add.Activate();
        }

        public void OnAfterSkillCast(SkillArg arg)
        {
        }
        #endregion
    }
}
