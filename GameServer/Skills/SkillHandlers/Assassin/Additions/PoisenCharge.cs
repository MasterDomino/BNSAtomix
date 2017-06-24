
using SmartEngine.Network.Map;
using SmartEngine.Network.Tasks;
using SagaBNS.Common.Actors;
using SagaBNS.Common.Skills;
using SagaBNS.GameServer.Map;
namespace SagaBNS.GameServer.Skills.SkillHandlers.Assassin.Additions
{
    public class PoisenCharge : Common.Additions.BonusAddition
    {
        private readonly SkillArg arg;
        public PoisenCharge(SkillArg arg, ActorExt target)
            : base(target, "PoisenCharge", 5000)
        {
            this.arg = arg;
            BonusAdditionID = 15000017;
            if (target.Tasks.TryGetValue("PoisenCharge", out Task task))
            {
                task.Deactivate();
                AccumulateCount = ((PoisenCharge)task).AccumulateCount;
            }
            AccumulateCount++;
            if (AccumulateCount > 5)
            {
                if (arg.Caster.Tasks.TryGetValue("Poisening", out task))
                {
                    task.Deactivate();
                }

                AccumulateCount = 5;
            }
            target.Tasks["PoisenCharge"] = this;
            OnAdditionStart += new StartEventHandler(Poisening_OnAdditionStart);
            OnAdditionEnd += new EndEventHandler(Poisening_OnAdditionEnd);
        }

        private void Poisening_OnAdditionEnd(Actor actor, Buff skill, bool cancel)
        {
            Map.Map map = Map.MapManager.Instance.GetMap(actor.MapInstanceID);
            UpdateEvent evt = UpdateEvent.NewActorAdditionEvent(arg.Caster, (ActorExt)actor, arg.SkillSession, 3, 15000017, UpdateEvent.ExtraUpdateModes.Cancel);
            map.SendEventToAllActorsWhoCanSeeActor(MapEvents.EVENT_BROADCAST, evt, actor, true);

            evt = UpdateEvent.NewActorAdditionExtEvent((ActorExt)actor, arg.SkillSession, 3, 15000017, 3000, UpdateEvent.ExtraUpdateModes.Cancel);
            map.SendEventToAllActorsWhoCanSeeActor(MapEvents.EVENT_BROADCAST, evt, actor, true);
        }

        private void Poisening_OnAdditionStart(Actor actor, Buff skill)
        {
            Map.Map map = Map.MapManager.Instance.GetMap(actor.MapInstanceID);
            UpdateEvent evt = UpdateEvent.NewActorAdditionEvent(arg.Caster, (ActorExt)actor, arg.SkillSession, 3, 15000017, UpdateEvent.ExtraUpdateModes.Activate);
            map.SendEventToAllActorsWhoCanSeeActor(MapEvents.EVENT_BROADCAST, evt, actor, true);

            evt = UpdateEvent.NewActorAdditionExtEvent((ActorExt)actor, arg.SkillSession, 3, 15000017, 3000, UpdateEvent.ExtraUpdateModes.Activate);
            evt.AdditionCount = AccumulateCount;
            map.SendEventToAllActorsWhoCanSeeActor(MapEvents.EVENT_BROADCAST, evt, actor, true);
        }
    }
}
