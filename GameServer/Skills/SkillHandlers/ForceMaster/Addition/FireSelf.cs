
using SmartEngine.Network.Map;
using SmartEngine.Network.Tasks;
using SagaBNS.Common.Actors;
using SagaBNS.Common.Skills;
using SagaBNS.GameServer.Map;
namespace SagaBNS.GameServer.Skills.SkillHandlers.ForceMaster.Additions
{
    public class FireSelf : Buff
    {
        private readonly SkillArg arg;
        public FireSelf(SkillArg arg)
            : base(arg.Caster, "FireSelf", 10000)
        {
            this.arg = arg;
            if (arg.Caster.Tasks.TryGetValue("FireSelf", out Task task))
            {
                task.Deactivate();
            }
            arg.Caster.Tasks["FireSelf"] = this;
            OnAdditionStart += new StartEventHandler(Fire_OnAdditionStart);
            OnAdditionEnd += new EndEventHandler(Fire_OnAdditionEnd);
        }

        private void Fire_OnAdditionEnd(Actor actor, Buff skill, bool cancel)
        {
            Map.Map map = Map.MapManager.Instance.GetMap(actor.MapInstanceID);
            ((ActorExt)actor).Tasks.TryRemove("FireSelf", out Task removed);

            UpdateEvent evt = UpdateEvent.NewActorAdditionExtEvent(arg.Caster, arg.SkillSession, 2, 12000030, 10000, UpdateEvent.ExtraUpdateModes.Cancel);
            map.SendEventToAllActorsWhoCanSeeActor(MapEvents.EVENT_BROADCAST, evt, actor, true);
        }

        private void Fire_OnAdditionStart(Actor actor, Buff skill)
        {
            Map.Map map = Map.MapManager.Instance.GetMap(actor.MapInstanceID);

            UpdateEvent evt = UpdateEvent.NewActorAdditionEvent(arg.Caster, arg.Caster, arg.SkillSession, 0, 12000003, UpdateEvent.ExtraUpdateModes.None);
            map.SendEventToAllActorsWhoCanSeeActor(MapEvents.EVENT_BROADCAST, evt, arg.Caster, true);
            evt = UpdateEvent.NewActorAdditionExtEvent(arg.Caster, arg.SkillSession, 2, 12000030, 10000, UpdateEvent.ExtraUpdateModes.Activate);
            map.SendEventToAllActorsWhoCanSeeActor(MapEvents.EVENT_BROADCAST, evt, actor, true);
        }
    }
}
