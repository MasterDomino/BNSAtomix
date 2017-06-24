using System;
using SmartEngine.Network.Tasks;
using SagaBNS.Common.Skills;
using SagaBNS.GameServer.Skills.SkillHandlers.Common;

namespace SagaBNS.GameServer.Skills.SkillHandlers.KungfuMaster
{
    public class Armbar : DefaultAttack
    {
        #region ISkillHandler 成员

        public override void OnAfterSkillCast(SkillArg arg)
        {
            base.OnAfterSkillCast(arg);
            if (arg.Caster.Tasks.TryGetValue("ActorTakeDown", out Task task))
            {
                Buff buf = task as Buff;
                buf.EndTime = DateTime.Now.AddMilliseconds(1500);
                //buf.Deactivate();
            }
            if (arg.Target.Tasks.TryGetValue("ActorDown", out task))
            {
                task.Deactivate();
            }
        }

        #endregion
    }
}
