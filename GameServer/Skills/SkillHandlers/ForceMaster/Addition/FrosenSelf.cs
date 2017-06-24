
using SmartEngine.Network.Map;
using SmartEngine.Network.Tasks;
using SagaBNS.Common.Actors;
using SagaBNS.Common.Skills;
using SagaBNS.GameServer.Map;
namespace SagaBNS.GameServer.Skills.SkillHandlers.ForceMaster.Additions
{
    public class FrosenSelf : Buff
    {
        private readonly SkillArg arg;
        public FrosenSelf(SkillArg arg)
            : base(arg.Caster, "FrosenSelf", 10000)
        {
            this.arg = arg;
            if (arg.Caster.Tasks.TryGetValue("FrosenSelf", out Task task))
            {
                task.Deactivate();
            }
            arg.Caster.Tasks["FrosenSelf"] = this;
            OnAdditionStart += new StartEventHandler(FrosenSelf_OnAdditionStart);
            OnAdditionEnd += new EndEventHandler(FrosenSelf_OnAdditionEnd);
        }

        private void FrosenSelf_OnAdditionEnd(Actor actor, Buff skill, bool cancel)
        {
            Map.Map map = Map.MapManager.Instance.GetMap(actor.MapInstanceID);
            ((ActorExt)actor).Tasks.TryRemove("FrosenSelf", out Task removed);
            UpdateEvent evt = UpdateEvent.NewActorAdditionExtEvent(arg.Caster, arg.SkillSession, 10, 12000040, 10000, UpdateEvent.ExtraUpdateModes.Cancel);
            map.SendEventToAllActorsWhoCanSeeActor(MapEvents.EVENT_BROADCAST, evt, actor, true);
        }

        private void FrosenSelf_OnAdditionStart(Actor actor, Buff skill)
        {
            Map.Map map = Map.MapManager.Instance.GetMap(actor.MapInstanceID);

            UpdateEvent evt = UpdateEvent.NewActorAdditionExtEvent(arg.Caster, arg.SkillSession, 10, 12000040, 10000, UpdateEvent.ExtraUpdateModes.Activate);
            map.SendEventToAllActorsWhoCanSeeActor(MapEvents.EVENT_BROADCAST, evt, actor, true);
        }
    }
}
