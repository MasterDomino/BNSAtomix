using System.Collections.Generic;
using SmartEngine.Network.Tasks;
using SagaBNS.Common.Skills;
using SagaBNS.GameServer.Skills.SkillHandlers.Common;
using SagaBNS.GameServer.Map;

namespace SagaBNS.GameServer.Skills.SkillHandlers.KungfuMaster
{
    public class JumpPunch : DefaultAttack
    {
        public JumpPunch()
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

        public void HandleSkillActivate(SkillArg arg)
        {
            SkillManager.Instance.DoAttack(arg);
            foreach (SkillAffectedActor i in arg.AffectedActors)
            {
                SkillAttackResult res = i.Result;
                i.NoDamageBroadcast = true;
                if (res != SkillAttackResult.Avoid && res != SkillAttackResult.Miss && res != SkillAttackResult.Parry && res != SkillAttackResult.TotalParry)
                {
                    if (i.Target.Tasks.ContainsKey("ActorDown"))
                    {
                        Buff buff = i.Target.Tasks["ActorDown"] as Buff;
                        buff.Deactivate();
                    }

                    Common.Additions.ActorDown add = new Common.Additions.ActorDown(arg, i.Target, 11118010, i.Damage, 2000);

                    i.Target.Tasks["ActorDown"] = add;
                    add.Activate();
                }
            }
        }
        #endregion
    }
}
