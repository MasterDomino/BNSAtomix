using SmartEngine.Network.Tasks;
using SagaBNS.Common.Skills;
using SagaBNS.GameServer.Skills.SkillHandlers.Common;

namespace SagaBNS.GameServer.Skills.SkillHandlers.ForceMaster
{
    public class EnergyMissile : DefaultAttack
    {
        public EnergyMissile()
            : base(false)
        {
        }
        #region ISkillHandler 成员

        public override void HandleSkillActivate(SkillArg arg)
        {
            base.HandleSkillActivate(arg);
            foreach (SkillAffectedActor i in arg.AffectedActors)
            {
                if (arg.Target.Tasks.TryGetValue("ActorCatch", out Task removed))
                {
                    removed.Deactivate();
                }
            }
        }

        #endregion
    }
}
