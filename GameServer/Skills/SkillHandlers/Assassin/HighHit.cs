using SmartEngine.Network.Tasks;
using SagaBNS.Common.Skills;
using SagaBNS.GameServer.Skills.SkillHandlers.Common;
using SagaBNS.GameServer.Map;

namespace SagaBNS.GameServer.Skills.SkillHandlers.Assassin
{
    public class HighHit : DefaultAttack
    {
        public int oldX, oldY, oldZ ,distance;
        public override void HandleOnSkillCasting(SkillArg arg)
        {
            Map.Map map = MapManager.Instance.GetMap(arg.Caster.MapInstanceID);
            UpdateEvent evt = UpdateEvent.NewActorAdditionEvent(arg.Caster, arg.Caster, arg.SkillSession, 28673, 65004, UpdateEvent.ExtraUpdateModes.Activate);
            oldX = arg.Caster.X;
            oldY = arg.Caster.Y;
            oldZ = arg.Caster.Z;
            Map.MoveArgument argu = new Map.MoveArgument()
            {
                BNSMoveType = MoveType.StepForward
            };
            distance = arg.Caster.DistanceToActor(arg.Target);
            argu.SkillSession = arg.SkillSession;
            float deltaX = (float)(arg.Target.X - arg.Caster.X) / distance;
            float deltaY = (float)(arg.Target.Y - arg.Caster.Y) / distance;

            argu.X = arg.Caster.X + (int)(deltaX * distance);
            argu.Y = arg.Caster.Y + (int)(deltaY * distance);
            argu.Z = (short)arg.Target.Z;
            argu.Dir = arg.Caster.Dir;
            map.MoveActor(arg.Caster, argu, true);

            //UpdateEvent evt1 = UpdateEvent.NewActorAdditionExtEvent(arg.Caster, arg.SkillSession, 28673, 15000017, 5000, UpdateEvent.ExtraUpdateModes.Activate);
            //map.SendEventToAllActorsWhoCanSeeActor(MapEvents.EVENT_BROADCAST, evt1, arg.Caster, true);
        }

        public override void HandleOnSkillCastFinish(SkillArg arg)
        {

        }

        public override void HandleSkillActivate(SkillArg arg)
        {
            base.HandleSkillActivate(arg);
        }

        public override void OnAfterSkillCast(SkillArg arg)
        {
            SimpleTask task = new SimpleTask("HighHitDelay", 200, (t) =>
                {
                    Map.Map map = MapManager.Instance.GetMap(arg.Caster.MapInstanceID);
                    UpdateEvent evt = UpdateEvent.NewActorAdditionEvent(arg.Caster, arg.Caster, arg.SkillSession, 28673, 65004, UpdateEvent.ExtraUpdateModes.Cancel);
                    Map.MoveArgument argu = new Map.MoveArgument()
                    {
                        BNSMoveType = MoveType.StepForward,
                        SkillSession = arg.SkillSession,
                        X = oldX,
                        Y = oldY,
                        Z = oldZ,
                        Dir = arg.Caster.Dir
                    };
                    map.MoveActor(arg.Caster, argu, true);
                   // UpdateEvent evt1 = UpdateEvent.NewActorAdditionExtEvent(arg.Caster, arg.SkillSession, 28673, 65004, 1300, UpdateEvent.ExtraUpdateModes.Cancel);
                    //map.SendEventToAllActorsWhoCanSeeActor(MapEvents.EVENT_BROADCAST, evt1, arg.Caster, true);
                });
            task.Activate();
        }
    }
}
