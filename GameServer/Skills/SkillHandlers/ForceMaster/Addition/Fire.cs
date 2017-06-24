
using SmartEngine.Network.Map;
using SmartEngine.Network.Tasks;
using SagaBNS.Common.Actors;
using SagaBNS.Common.Skills;
using SagaBNS.GameServer.Map;
namespace SagaBNS.GameServer.Skills.SkillHandlers.ForceMaster.Additions
{
    public class Fire : Common.Additions.BonusAddition
    {
        private readonly SkillArg arg;
        public Fire(SkillArg arg, ActorExt target)
            : base(target, "Fire", 10000)
        {
            this.arg = arg;
            BonusAdditionID = 12000036;
            if (target.Tasks.TryGetValue("Fire", out Task task))
            {
                task.Deactivate();
                AccumulateCount = ((Fire)task).AccumulateCount;
            }
            AccumulateCount++;
            if (AccumulateCount > 5)
            {
                AccumulateCount = 5;
            }

            target.Tasks["Fire"] = this;
            OnAdditionStart += new StartEventHandler(Fire_OnAdditionStart);
            OnAdditionEnd += new EndEventHandler(Fire_OnAdditionEnd);
        }

        private void Fire_OnAdditionEnd(Actor actor, Buff skill, bool cancel)
        {
            Map.Map map = Map.MapManager.Instance.GetMap(actor.MapInstanceID);
            ((ActorExt)actor).Tasks.TryRemove("Fire", out Task removed);
            UpdateEvent evt = UpdateEvent.NewActorAdditionEvent(arg.Caster, (ActorExt)actor, arg.SkillSession, 4105, 12000035, UpdateEvent.ExtraUpdateModes.Cancel);
            //evt.AddActorPara(SagaBNS.Common.Packets.GameServer.PacketParameter.Unk7A, 0);
            //evt.AddActorPara(SagaBNS.Common.Packets.GameServer.PacketParameter.UnkF7, 0);
            evt.UserData = new byte[] { 9, 4, 0 };
            map.SendEventToAllActorsWhoCanSeeActor(MapEvents.EVENT_BROADCAST, evt, actor, true);

            evt = UpdateEvent.NewActorAdditionExtEvent((ActorExt)actor, arg.SkillSession, 4105, 12000035, 10000, UpdateEvent.ExtraUpdateModes.Cancel);
            map.SendEventToAllActorsWhoCanSeeActor(MapEvents.EVENT_BROADCAST, evt, actor, true);
        }

        private void Fire_OnAdditionStart(Actor actor, Buff skill)
        {
            Map.Map map = Map.MapManager.Instance.GetMap(actor.MapInstanceID);

            UpdateEvent evt = UpdateEvent.NewActorAdditionEvent(arg.Caster, (ActorExt)actor, arg.SkillSession, 4105, 12000035, UpdateEvent.ExtraUpdateModes.Activate);
            //evt.AddActorPara(SagaBNS.Common.Packets.GameServer.PacketParameter.Unk7A, 4);
            //evt.AddActorPara(SagaBNS.Common.Packets.GameServer.PacketParameter.UnkF7, 1);
            map.SendEventToAllActorsWhoCanSeeActor(MapEvents.EVENT_BROADCAST, evt, actor, true);

            evt = UpdateEvent.NewActorAdditionExtEvent((ActorExt)actor, arg.SkillSession, 4105, 12000035, 10000, UpdateEvent.ExtraUpdateModes.Activate);
            evt.AdditionCount = AccumulateCount;
            map.SendEventToAllActorsWhoCanSeeActor(MapEvents.EVENT_BROADCAST, evt, actor, true);
        }
    }
}
