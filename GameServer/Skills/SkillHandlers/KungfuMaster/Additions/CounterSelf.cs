
using SmartEngine.Network.Map;
using SmartEngine.Network.Tasks;
using SagaBNS.Common.Actors;
using SagaBNS.Common.Skills;
using SagaBNS.GameServer.Map;
namespace SagaBNS.GameServer.Skills.SkillHandlers.KungfuMaster.Additions
{
    public class CounterSelf : Buff
    {
        private readonly SkillArg arg;
        public CounterSelf(SkillArg arg,int lifeTime)
            : base(arg.Caster, "CounterSelf", lifeTime)
        {
            this.arg = arg;
            if (arg.Caster.Tasks.TryGetValue("CounterSelf", out Task task))
            {
                task.Deactivate();
            }

            arg.Caster.Tasks["CounterSelf"] = this;

            OnAdditionStart += new StartEventHandler(CounterEnemy_OnAdditionStart);
            OnAdditionEnd += new EndEventHandler(CounterEnemy_OnAdditionEnd);
        }

        private void CounterEnemy_OnAdditionEnd(Actor actor, Buff skill, bool cancel)
        {
            ((ActorExt)actor).Status.Counter = false;
            Map.Map map = MapManager.Instance.GetMap(actor.MapInstanceID);
            ((ActorExt)actor).Tasks.TryRemove("CounterSelf", out Task removed);
            arg.Caster.Status.StanceFlag1.SetValue(StanceU1.Unknown40000, false);
            UpdateEvent evt = UpdateEvent.NewActorAdditionEvent(arg.Caster, arg.Caster, arg.SkillSession, 4098, 11000018, UpdateEvent.ExtraUpdateModes.Cancel);
            //evt.AddActorPara(SagaBNS.Common.Packets.GameServer.PacketParameter.Unk7A, arg.Caster.Status.StanceFlag1.Value);
            map.SendEventToAllActorsWhoCanSeeActor(MapEvents.EVENT_BROADCAST, evt, arg.Caster, true);

            evt = UpdateEvent.NewActorAdditionExtEvent(arg.Caster, arg.SkillSession, 4098, 11000018, TotalLifeTime, UpdateEvent.ExtraUpdateModes.Cancel);
            map.SendEventToAllActorsWhoCanSeeActor(MapEvents.EVENT_BROADCAST, evt, arg.Caster, true);
        }

        private void CounterEnemy_OnAdditionStart(Actor actor, Buff skill)
        {
            Map.Map map = MapManager.Instance.GetMap(actor.MapInstanceID);
            ((ActorExt)actor).Status.Counter = true;
            arg.Caster.Status.StanceFlag1.SetValue(StanceU1.Unknown40000, true);
            UpdateEvent evt = UpdateEvent.NewActorAdditionEvent(arg.Caster, arg.Caster, arg.SkillSession, 4098, 11000018, UpdateEvent.ExtraUpdateModes.Activate);
            //evt.AddActorPara(SagaBNS.Common.Packets.GameServer.PacketParameter.Unk7A, arg.Caster.Status.StanceFlag1.Value);
            map.SendEventToAllActorsWhoCanSeeActor(MapEvents.EVENT_BROADCAST, evt, arg.Caster, true);

            evt = UpdateEvent.NewActorAdditionExtEvent(arg.Caster, arg.SkillSession, 4098, 11000018, TotalLifeTime, UpdateEvent.ExtraUpdateModes.Activate);
            map.SendEventToAllActorsWhoCanSeeActor(MapEvents.EVENT_BROADCAST, evt, arg.Caster, true);
        }
    }
}
