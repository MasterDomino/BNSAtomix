using SmartEngine.Network.Tasks;
using SagaBNS.Common.Skills;
using SagaBNS.GameServer.Skills.SkillHandlers.Common;

namespace SagaBNS.GameServer.Skills.SkillHandlers.ForceMaster
{
    public class ThrowTarget : DefaultAttack
    {
        public ThrowTarget()
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

                //Additions.Throw throws = new Additions.Throw(arg, i.Target);
                //throws.Activate();
                Map.Map map = Map.MapManager.Instance.GetMap(arg.Caster.MapInstanceID);
                Map.MoveArgument argu = new Map.MoveArgument()
                {
                    BNSMoveType = Map.MoveType.PushBack
                };
                bool pushed = map.HeightMapBuilder.GetMaximunPushBackPos(arg.Caster.X, arg.Caster.Y, arg.Caster.Z, arg.Caster.Dir, 150, out argu.X, out argu.Y, out argu.Z);
                argu.PushBackSource = arg.Caster;
                argu.Dir = arg.Caster.Dir;
                argu.SkillSession = arg.SkillSession;
                map.MoveActor(i.Target, argu, true);
                Common.Additions.ActorDown add = new Common.Additions.ActorDown(arg, i.Target, 12311013, i.Damage, 1000);

                i.Target.Tasks["ActorDown"] = add;
                add.Activate();
            }
        }

        #endregion
    }
}
