
using SmartEngine.Core.Math;
using SmartEngine.Network.Tasks;
using SagaBNS.Common.Skills;
using SagaBNS.GameServer.Skills.SkillHandlers.Common;
using SagaBNS.GameServer.Map;

namespace SagaBNS.GameServer.Skills.SkillHandlers.Assassin
{
    public class WoodBlockCounter : DefaultAttack
    {
        #region ISkillHandler 成员

        public override void  HandleSkillActivate(SkillArg arg)
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
            if (arg.Caster.Tasks.ContainsKey("Stealth"))
            {
                Buff buff = arg.Caster.Tasks["Stealth"] as Buff;
                buff.Deactivate();
            }

            Additions.Stealth add = new Additions.Stealth(arg);

            arg.Caster.Tasks["Stealth"] = add;
            add.Activate();
        }

        #endregion
    }
}
