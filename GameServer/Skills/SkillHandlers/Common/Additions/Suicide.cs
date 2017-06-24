
using SmartEngine.Network.Map;
using SmartEngine.Network.Tasks;
using SagaBNS.Common.Actors;
using SagaBNS.Common.Skills;
using SagaBNS.GameServer.Map;
namespace SagaBNS.GameServer.Skills.SkillHandlers.Common.Additions
{
    public class Suicide : Buff
    {
        private readonly SkillArg arg;
        private readonly ActorExt target;
        private readonly uint additionID;
        public Suicide(SkillArg arg, ActorExt target, uint additionID)
            : base(target, "Suicide", 1000)
        {
            this.arg = arg;
            this.target = target;
            this.additionID = additionID;
            if (target.Tasks.TryGetValue("Suicide", out Task task))
            {
                task.Deactivate();
            }
            target.Tasks["Suicide"] = this;
            OnAdditionEnd += new EndEventHandler(ActorFrosen_OnAdditionEnd);
        }

        private void ActorFrosen_OnAdditionEnd(Actor actor, Buff skill, bool cancel)
        {
            Map.Map map = MapManager.Instance.GetMap(actor.MapInstanceID);
            target.Tasks.TryRemove("Suicide", out Task removed);
            UpdateEvent evt = UpdateEvent.NewActorAdditionEvent(arg.Caster, target, arg.SkillSession, 0, additionID, UpdateEvent.ExtraUpdateModes.None);
            //evt.AddActorPara(SagaBNS.Common.Packets.GameServer.PacketParameter.Dead, 1);
            target.Status.Dead = true;
            target.HP = 0;
            ((BNSActorEventHandler)target.EventHandler).OnDie(target);
            map.SendEventToAllActorsWhoCanSeeActor(MapEvents.EVENT_BROADCAST, evt, target, true);
        }
    }
}
