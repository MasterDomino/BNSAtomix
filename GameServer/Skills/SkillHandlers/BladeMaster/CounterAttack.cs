using SmartEngine.Core.Math;
using SagaBNS.Common.Skills;
using SagaBNS.GameServer.Skills.SkillHandlers.Common;
using SagaBNS.GameServer.Map;

namespace SagaBNS.GameServer.Skills.SkillHandlers.BladeMaster
{
    public class CounterAttack : DefaultAttack
    {
        public CounterAttack()
            : base(true)
        {
        }
        #region ISkillHandler 成员

        public override void HandleOnSkillCasting(SkillArg arg)
        {
            Map.Map map = MapManager.Instance.GetMap(arg.Caster.MapInstanceID);

            Map.MoveArgument argu = new Map.MoveArgument()
            {
                BNSMoveType = MoveType.StepForward
            };
            int distance = arg.Caster.DistanceToActor(arg.Target);
            int forward = 20;
            argu.SkillSession = arg.SkillSession;
            Vec3 vec = -arg.Target.Dir.DirectionToVector();

            argu.X = arg.Target.X + (int)(vec.X * forward);
            argu.Y = arg.Target.Y + (int)(vec.Y * forward);
            argu.Z = (short)arg.Target.Z;
            argu.Dir = arg.Target.Dir;
            map.MoveActor(arg.Caster, argu, true);
        }

        public override void HandleSkillActivate(SkillArg arg)
        {

        }
        #endregion
    }
}
