using SagaBNS.Common.Skills;
using SagaBNS.GameServer.Map;

namespace SagaBNS.GameServer.Skills.SkillHandlers.Common
{
    public class Dash : DefaultAttack
    {
        public Dash()
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
            int forward = distance < 40 ? 0 : distance - 20;
            argu.SkillSession = arg.SkillSession;
            float deltaX = (float)(arg.Target.X - arg.Caster.X) / distance;
            float deltaY = (float)(arg.Target.Y - arg.Caster.Y) / distance;

            argu.X = arg.Caster.X + (int)(deltaX * forward);
            argu.Y = arg.Caster.Y + (int)(deltaY * forward);
            argu.Z = (short)arg.Target.Z;
            argu.Dir = arg.Caster.Dir;
            map.MoveActor(arg.Caster, argu, true);
        }
        #endregion
    }
}
