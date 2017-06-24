
using SmartEngine.Network.Map;
using SmartEngine.Network.Tasks;
using SagaBNS.Common.Actors;
using SagaBNS.Common.Skills;
using SagaBNS.GameServer.Map;
namespace SagaBNS.GameServer.Skills.SkillHandlers.Common.Additions
{
    public class ActorCatch : Buff
    {
        private readonly SkillArg arg;
        private readonly ActorExt target;
        private readonly uint additionID;
        private readonly int damage;
        private int diffX, diffY;
        public ActorCatch(SkillArg arg, ActorExt target, uint additionID, int damage = 0, int duration = 5000)
            : base(target, "ActorCatch", duration)
        {
            this.arg = arg;
            this.target = target;
            this.additionID = additionID;
            this.damage = damage;
            OnAdditionStart += new StartEventHandler(ActorCatch_OnAdditionStart);
            OnAdditionEnd += new EndEventHandler(ActorCatch_OnAdditionEnd);
        }

        private void ActorCatch_OnAdditionEnd(Actor actor, Buff skill, bool cancel)
        {
            target.Status.Catch = false;
            Map.Map map = MapManager.Instance.GetMap(actor.MapInstanceID);
            target.Status.StanceFlag1.SetValue(StanceU1.Unknown400000000000, false);
            target.Status.StanceFlag1.SetValue(StanceU1.Unknown200000000000, false);
            target.Status.StanceFlag1.SetValue(StanceU1.NoMove, false);
            target.Status.StanceFlag1.SetValue(StanceU1.Unknown4, false);
            target.Status.InteractWith = 0;
            arg.Caster.Status.InteractWith = 0;

            target.Tasks.TryRemove("ActorCatch", out Task removed);
            UpdateEvent evt = UpdateEvent.NewActorAdditionEvent(arg.Caster, target, arg.SkillSession, 4098, additionID, UpdateEvent.ExtraUpdateModes.Cancel);
            evt.UpdateType = UpdateTypes.Actor;
            //evt.AddActorPara(SagaBNS.Common.Packets.GameServer.PacketParameter.Unk7A, target.Status.StanceFlag1.Value);
            map.SendEventToAllActorsWhoCanSeeActor(MapEvents.EVENT_BROADCAST, evt, target, true);

            evt = UpdateEvent.NewActorAdditionEvent(arg.Caster, arg.Caster, 0, 0, 65000, UpdateEvent.ExtraUpdateModes.Cancel);
            //evt.AddActorPara(SagaBNS.Common.Packets.GameServer.PacketParameter.InteractWith, 0);
            //evt.AddActorPara(SagaBNS.Common.Packets.GameServer.PacketParameter.InteractionType, 0);
            //evt.AddActorPara(SagaBNS.Common.Packets.GameServer.PacketParameter.InteractionRelation, 0);
            //evt.AddActorPara(SagaBNS.Common.Packets.GameServer.PacketParameter.Unk74, 0);
            //evt.AddActorPara(SagaBNS.Common.Packets.GameServer.PacketParameter.UnkD7, 0);
            map.SendEventToAllActorsWhoCanSeeActor(MapEvents.EVENT_BROADCAST, evt, actor, true);

            target.Status.StanceFlag1.SetValue(StanceU1.Unknown4, true);
            target.X = arg.Caster.X + diffX;
            target.Y = arg.Caster.Y + diffY;
            target.Z = arg.Caster.Z;

            evt = UpdateEvent.NewActorAdditionEvent(arg.Caster, target, arg.SkillSession, 4099, 12300011, UpdateEvent.ExtraUpdateModes.Cancel);
            evt.AddActorPara(SagaBNS.Common.Packets.GameServer.PacketParameter.X, arg.Caster.X);
            evt.AddActorPara(SagaBNS.Common.Packets.GameServer.PacketParameter.Y, arg.Caster.Y);
            evt.AddActorPara(SagaBNS.Common.Packets.GameServer.PacketParameter.Z, arg.Caster.Z);
            //evt.AddActorPara(SagaBNS.Common.Packets.GameServer.PacketParameter.Unk7A, target.Status.StanceFlag1.Value);
            //evt.AddActorPara(SagaBNS.Common.Packets.GameServer.PacketParameter.InteractWith, 0);
            //evt.AddActorPara(SagaBNS.Common.Packets.GameServer.PacketParameter.InteractionType, 0);
            //evt.AddActorPara(SagaBNS.Common.Packets.GameServer.PacketParameter.InteractionRelation, 0);
            //evt.AddActorPara(SagaBNS.Common.Packets.GameServer.PacketParameter.Unk74, 0);
            //evt.AddActorPara(SagaBNS.Common.Packets.GameServer.PacketParameter.UnkD7, 0);
            evt.UserData = new byte[] { 9, 1, 0 };
            map.SendEventToAllActorsWhoCanSeeActor(MapEvents.EVENT_BROADCAST, evt, target, true);

            evt = UpdateEvent.NewActorAdditionExtEvent(arg.Caster, arg.SkillSession, 4099, 12300011, 5000, UpdateEvent.ExtraUpdateModes.Cancel);
            map.SendEventToAllActorsWhoCanSeeActor(MapEvents.EVENT_BROADCAST, evt, arg.Caster, true);

            SkillManager.Instance.BroadcastSkillCast(arg, SkillMode.End);
        }

        private unsafe void ActorCatch_OnAdditionStart(Actor actor, Buff skill)
        {
            if (!target.Status.Catch)
            {
                #region move
                {
                    target.Status.Catch = true;
                    target.Status.StanceFlag1.SetValue(StanceU1.Unknown400000000000, true);
                    target.Status.StanceFlag1.SetValue(StanceU1.Unknown200000000000, true);
                    target.Status.StanceFlag1.SetValue(StanceU1.NoMove, true);
                    target.Status.StanceFlag1.SetValue(StanceU1.Unknown4, true);
                    UpdateEvent evt = UpdateEvent.NewActorAdditionEvent(arg.Caster, target, arg.SkillSession, 4098, additionID, UpdateEvent.ExtraUpdateModes.Activate);
                    evt.UpdateType = UpdateTypes.Actor;
                    //evt.AddActorPara(SagaBNS.Common.Packets.GameServer.PacketParameter.Unk7A, target.Status.StanceFlag1.Value);
                    MapManager.Instance.GetMap(actor.MapInstanceID).SendEventToAllActorsWhoCanSeeActor(MapEvents.EVENT_BROADCAST, evt, target, true);

                    Map.MoveArgument argu = new Map.MoveArgument()
                    {
                        BNSMoveType = Map.MoveType.PushBack,
                        PushBackSource = arg.Caster
                    };
                    int distance = arg.Target.DistanceToActor(arg.Caster);
                    int forward = distance < 40 ? 0 : distance - 20;
                    argu.SkillSession = arg.SkillSession;
                    float deltaX = (float)(arg.Caster.X - arg.Target.X) / distance;
                    float deltaY = (float)(arg.Caster.Y - arg.Target.Y) / distance;

                    argu.X = arg.Target.X + (int)(deltaX * forward);
                    argu.Y = arg.Target.Y + (int)(deltaY * forward);
                    argu.Z = (short)arg.Caster.Z;
                    argu.Dir = arg.Target.Dir;
                    diffX = argu.X - arg.Caster.X;
                    diffY = argu.Y - arg.Caster.Y;

                    MapManager.Instance.GetMap(actor.MapInstanceID).MoveActor(arg.Target, argu, true);//*/

                    target.Status.StanceFlag1.SetValue(StanceU1.Unknown400000000000, false);
                    target.Status.StanceFlag1.SetValue(StanceU1.Unknown200000000000, false);
                    target.Status.StanceFlag1.SetValue(StanceU1.NoMove, false);
                    //evt.AddActorPara(SagaBNS.Common.Packets.GameServer.PacketParameter.Unk7A, target.Status.StanceFlag1.Value);
                    MapManager.Instance.GetMap(actor.MapInstanceID).SendEventToAllActorsWhoCanSeeActor(MapEvents.EVENT_BROADCAST, evt, target, true);
                }
                #endregion
                {
                    Map.Map map = Map.MapManager.Instance.GetMap(actor.MapInstanceID);

                    target.Status.StanceFlag1.SetValue(StanceU1.Unknown4, true);
                    target.Status.StanceFlag1.SetValue(StanceU1.NoMove, true);
                    target.Status.StanceFlag1.SetValue(StanceU1.Unknown400000000000, true);
                    target.Status.InteractWith = arg.Caster.ActorID;
                    arg.Caster.Status.InteractWith = target.ActorID;

                    UpdateEvent evt = UpdateEvent.NewActorAdditionEvent(arg.Caster, arg.Caster, 0, 0, 65000, UpdateEvent.ExtraUpdateModes.Activate);
                    //evt.AddActorPara(SagaBNS.Common.Packets.GameServer.PacketParameter.InteractWith, (long)target.ActorID);
                    //evt.AddActorPara(SagaBNS.Common.Packets.GameServer.PacketParameter.InteractionType, 7);
                    //evt.AddActorPara(SagaBNS.Common.Packets.GameServer.PacketParameter.InteractionRelation, 1);
                    //evt.AddActorPara(SagaBNS.Common.Packets.GameServer.PacketParameter.Unk74, 1);
                    //evt.AddActorPara(SagaBNS.Common.Packets.GameServer.PacketParameter.UnkD7, 1);
                    map.SendEventToAllActorsWhoCanSeeActor(MapEvents.EVENT_BROADCAST, evt, actor, true);

                    evt = UpdateEvent.NewActorAdditionEvent(arg.Caster, target, arg.SkillSession, 4099, 12300011, UpdateEvent.ExtraUpdateModes.Activate);
                    //evt.AddActorPara(SagaBNS.Common.Packets.GameServer.PacketParameter.Unk7A, target.Status.StanceFlag1.Value);
                    //evt.AddActorPara(SagaBNS.Common.Packets.GameServer.PacketParameter.Dir, actor.Dir > 180 ? actor.Dir - 180 : 360 - actor.Dir);
                    //evt.AddActorPara(SagaBNS.Common.Packets.GameServer.PacketParameter.InteractWith, (long)arg.Caster.ActorID);
                    //evt.AddActorPara(SagaBNS.Common.Packets.GameServer.PacketParameter.InteractionType, 7);
                    //evt.AddActorPara(SagaBNS.Common.Packets.GameServer.PacketParameter.InteractionRelation, 2);
                    //evt.AddActorPara(SagaBNS.Common.Packets.GameServer.PacketParameter.Unk74, 1);
                    //evt.AddActorPara(SagaBNS.Common.Packets.GameServer.PacketParameter.UnkD7, 1);
                    map.SendEventToAllActorsWhoCanSeeActor(MapEvents.EVENT_BROADCAST, evt, actor, true);

                    evt = UpdateEvent.NewActorAdditionExtEvent(target, arg.SkillSession, 4099, 12300011, 5000, UpdateEvent.ExtraUpdateModes.Activate);
                    map.SendEventToAllActorsWhoCanSeeActor(MapEvents.EVENT_BROADCAST, evt, actor, true);
                }
            }
        }
    }
}
