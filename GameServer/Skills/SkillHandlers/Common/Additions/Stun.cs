
using SmartEngine.Network.Map;
using SmartEngine.Network.Tasks;
using SagaBNS.Common.Actors;
using SagaBNS.Common.Skills;
using SagaBNS.GameServer.Map;
namespace SagaBNS.GameServer.Skills.SkillHandlers.Common.Additions
{
    public class Stun : Buff
    {
        private readonly SkillArg arg;
        private readonly ActorExt target;
        public Stun(SkillArg arg, ActorExt target, int lifeTime)
            : base(target, "Stun", lifeTime)
        {
            this.arg = arg;
            this.target = target;
            if (target.Tasks.TryGetValue("Stun", out Task task))
            {
                task.Deactivate();
            }

            target.Tasks["Stun"] = this;
            OnAdditionStart += new StartEventHandler(ActorDown_OnAdditionStart);
            OnAdditionEnd += new EndEventHandler(ActorDown_OnAdditionEnd);
        }

        private void ActorDown_OnAdditionEnd(Actor actor, Buff skill, bool cancel)
        {
            target.Status.Stun = false;
            Map.Map map = MapManager.Instance.GetMap(actor.MapInstanceID);
            target.Tasks.TryRemove("Stun", out Task removed);
            target.Status.StanceFlag1.SetValue(StanceU1.Stun, false);
            target.Status.StanceFlag1.SetValue(StanceU1.NoMove, false);
            target.Status.StanceFlag1.SetValue(StanceU1.Unknown2000, false);

            UpdateEvent evt = UpdateEvent.NewActorAdditionEvent(arg.Caster, target, arg.SkillSession, 4098, 11104011, UpdateEvent.ExtraUpdateModes.Cancel);
            //evt.AddActorPara(SagaBNS.Common.Packets.GameServer.PacketParameter.Unk7A, target.Status.StanceFlag1.Value);
            map.SendEventToAllActorsWhoCanSeeActor(MapEvents.EVENT_BROADCAST, evt, target, true);

            evt = UpdateEvent.NewActorAdditionExtEvent(target, arg.SkillSession, 4098, 11104011, TotalLifeTime, UpdateEvent.ExtraUpdateModes.Cancel);
            map.SendEventToAllActorsWhoCanSeeActor(MapEvents.EVENT_BROADCAST, evt, target, true);

            evt = UpdateEvent.NewActorAdditionEvent(arg.Caster, target, arg.SkillSession, 4099, 11104012, UpdateEvent.ExtraUpdateModes.Cancel);
            //evt.AddActorPara(SagaBNS.Common.Packets.GameServer.PacketParameter.UnkD3, 0);
            map.SendEventToAllActorsWhoCanSeeActor(MapEvents.EVENT_BROADCAST, evt, target, true);

            evt = UpdateEvent.NewActorAdditionExtEvent(target, arg.SkillSession, 4099, 11104012, TotalLifeTime, UpdateEvent.ExtraUpdateModes.Cancel);
            map.SendEventToAllActorsWhoCanSeeActor(MapEvents.EVENT_BROADCAST, evt, target, true);
        }

        private unsafe void ActorDown_OnAdditionStart(Actor actor, Buff skill)
        {
            Map.Map map = MapManager.Instance.GetMap(actor.MapInstanceID);
            target.Status.Stun = true;
            target.Status.StanceFlag1.SetValue(StanceU1.Stun, true);
            target.Status.StanceFlag1.SetValue(StanceU1.NoMove, true);
            UpdateEvent evt = UpdateEvent.NewActorAdditionEvent(arg.Caster, target, arg.SkillSession, 4098, 11104011, UpdateEvent.ExtraUpdateModes.Activate);
            //evt.AddActorPara(SagaBNS.Common.Packets.GameServer.PacketParameter.Unk7A, target.Status.StanceFlag1.Value);
            map.SendEventToAllActorsWhoCanSeeActor(MapEvents.EVENT_BROADCAST, evt, target, true);

            evt = UpdateEvent.NewActorAdditionExtEvent(target, arg.SkillSession, 4098, 11104011, TotalLifeTime, UpdateEvent.ExtraUpdateModes.Activate);
            map.SendEventToAllActorsWhoCanSeeActor(MapEvents.EVENT_BROADCAST, evt, target, true);

            evt = UpdateEvent.NewActorAdditionEvent(arg.Caster, target, arg.SkillSession, 4099, 11104012, UpdateEvent.ExtraUpdateModes.Activate);
            //evt.AddActorPara(SagaBNS.Common.Packets.GameServer.PacketParameter.UnkD3, 1);
            map.SendEventToAllActorsWhoCanSeeActor(MapEvents.EVENT_BROADCAST, evt, target, true);

            evt = UpdateEvent.NewActorAdditionExtEvent(target, arg.SkillSession, 4099, 11104012, TotalLifeTime, UpdateEvent.ExtraUpdateModes.Activate);
            map.SendEventToAllActorsWhoCanSeeActor(MapEvents.EVENT_BROADCAST, evt, target, true);
        }
    }
}
