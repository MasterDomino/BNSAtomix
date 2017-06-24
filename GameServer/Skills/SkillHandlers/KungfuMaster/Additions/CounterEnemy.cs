
using SmartEngine.Network.Map;
using SmartEngine.Network.Tasks;
using SagaBNS.Common.Actors;
using SagaBNS.Common.Skills;
using SagaBNS.GameServer.Map;
namespace SagaBNS.GameServer.Skills.SkillHandlers.KungfuMaster.Additions
{
    public class CounterEnemy : Buff
    {
        private readonly SkillArg arg;
        public uint CounterSkillID { get; set; }
        public CounterEnemy(SkillArg arg)
            : base(arg.Caster, "CounterEnemy", arg.Skill.BaseData.Duration)
        {
            this.arg = arg;
            CounterSkillID = 11104;
            if (arg.Caster.Tasks.TryGetValue("CounterEnemy", out Task task))
            {
                task.Deactivate();
            }

            arg.Caster.Tasks["CounterEnemy"] = this;

            OnAdditionStart += new StartEventHandler(CounterEnemy_OnAdditionStart);
            OnAdditionEnd += new EndEventHandler(CounterEnemy_OnAdditionEnd);
        }

        private void CounterEnemy_OnAdditionEnd(Actor actor, Buff skill, bool cancel)
        {
            ((ActorExt)actor).Status.Counter = false;
            Map.Map map = MapManager.Instance.GetMap(actor.MapInstanceID);
            ((ActorExt)actor).Tasks.TryRemove("CounterEnemy", out Task removed);
            UpdateEvent evt = UpdateEvent.NewActorAdditionEvent((ActorExt)actor, (ActorExt)actor, arg.SkillSession, 1, 11103010, UpdateEvent.ExtraUpdateModes.Cancel);
            evt.UserData = new byte[] { 9, 1, 0 };
            //evt.AddActorPara(SagaBNS.Common.Packets.GameServer.PacketParameter.BlockingStance, 0);
            map.SendEventToAllActorsWhoCanSeeActor(MapEvents.EVENT_BROADCAST, evt, actor, true);

            evt = UpdateEvent.NewActorAdditionExtEvent((ActorExt)actor, arg.SkillSession, 1, 11103010, 0, UpdateEvent.ExtraUpdateModes.Cancel);
            map.SendEventToAllActorsWhoCanSeeActor(MapEvents.EVENT_BROADCAST, evt, actor, true);
            SkillManager.Instance.BroadcastSkillCast(arg, SkillMode.End);
        }

        private void CounterEnemy_OnAdditionStart(Actor actor, Buff skill)
        {
            Map.Map map = MapManager.Instance.GetMap(actor.MapInstanceID);
            ((ActorExt)actor).Status.Counter = true;
            UpdateEvent evt = UpdateEvent.NewActorAdditionEvent((ActorExt)actor, (ActorExt)actor, arg.SkillSession, 1, 11103010, UpdateEvent.ExtraUpdateModes.Activate);
            //evt.AddActorPara(SagaBNS.Common.Packets.GameServer.PacketParameter.BlockingStance, 1);

            map.SendEventToAllActorsWhoCanSeeActor(MapEvents.EVENT_BROADCAST, evt, actor, true);

            evt = UpdateEvent.NewActorAdditionExtEvent((ActorExt)actor, arg.SkillSession, 1, 11103010, arg.Skill.BaseData.Duration, UpdateEvent.ExtraUpdateModes.Activate);
            map.SendEventToAllActorsWhoCanSeeActor(MapEvents.EVENT_BROADCAST, evt, actor, true);
        }
    }
}
