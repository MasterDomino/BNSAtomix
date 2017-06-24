
using SmartEngine.Network.Map;
using SmartEngine.Network.Tasks;
using SagaBNS.Common.Actors;
using SagaBNS.Common.Skills;
using SagaBNS.GameServer.Map;
namespace SagaBNS.GameServer.Skills.SkillHandlers.ForceMaster.Additions
{
    public class Frosen : Common.Additions.BonusAddition
    {
        private readonly SkillArg arg;
        private readonly ActorExt target;
        public Frosen(SkillArg arg, ActorExt target)
            : base(target, "Frosen", 6000)
        {
            this.arg = arg;
            this.target = target;
            BonusAdditionID = 12000057;
            if (target.Tasks.TryGetValue("Frosen", out Task task))
            {
                task.Deactivate();
                AccumulateCount = ((Frosen)task).AccumulateCount;
            }
            AccumulateCount++;
            if (AccumulateCount > 3)
            {
                AccumulateCount = 3;
            }

            target.Tasks["Frosen"] = this;
            OnAdditionStart += new StartEventHandler(Frosen_OnAdditionStart);
            OnAdditionEnd += new EndEventHandler(Frosen_OnAdditionEnd);
        }

        private void Frosen_OnAdditionEnd(Actor actor, Buff skill, bool cancel)
        {
            Map.Map map = Map.MapManager.Instance.GetMap(actor.MapInstanceID);
            target.Status.StanceFlag1.SetValue(StanceU1.Unknown10000000, false);
            target.Status.StanceFlag1.SetValue(StanceU1.Unknown400000, false);
            target.Status.StanceFlag1.SetValue(StanceU1.Unknown4, false);

            ((ActorExt)actor).Tasks.TryRemove("Frosen", out Task removed);
            target.Speed = 500;
            UpdateEvent evt = UpdateEvent.NewActorAdditionEvent(arg.Caster, (ActorExt)actor, arg.SkillSession, 4101, 12000057, UpdateEvent.ExtraUpdateModes.Cancel);
            //evt.AddActorPara(SagaBNS.Common.Packets.GameServer.PacketParameter.Speed, target.Speed / 10);
            //evt.AddActorPara(SagaBNS.Common.Packets.GameServer.PacketParameter.Unk7A, 0);

            map.SendEventToAllActorsWhoCanSeeActor(MapEvents.EVENT_BROADCAST, evt, actor, true);
            evt = UpdateEvent.NewActorAdditionExtEvent((ActorExt)actor, arg.SkillSession, 12, 12000057, 6000, UpdateEvent.ExtraUpdateModes.Cancel);
            map.SendEventToAllActorsWhoCanSeeActor(MapEvents.EVENT_BROADCAST, evt, actor, true);
        }

        private void Frosen_OnAdditionStart(Actor actor, Buff skill)
        {
            Map.Map map = Map.MapManager.Instance.GetMap(actor.MapInstanceID);

            target.Status.StanceFlag1.SetValue(StanceU1.Unknown10000000, true);
            target.Status.StanceFlag1.SetValue(StanceU1.Unknown400000, true);
            target.Status.StanceFlag1.SetValue(StanceU1.Unknown4, true);

            UpdateEvent evt = UpdateEvent.NewActorAdditionEvent(arg.Caster, (ActorExt)actor, arg.SkillSession, 0, 12256011, UpdateEvent.ExtraUpdateModes.Activate);
            map.SendEventToAllActorsWhoCanSeeActor(MapEvents.EVENT_BROADCAST, evt, actor, true);

            target.Speed = (ushort)(500 * (1 - (0.15 * AccumulateCount)));
            evt = UpdateEvent.NewActorAdditionEvent(arg.Caster, (ActorExt)actor, arg.SkillSession, 4101, 12000057, UpdateEvent.ExtraUpdateModes.Activate);
            //evt.AddActorPara(SagaBNS.Common.Packets.GameServer.PacketParameter.Speed, target.Speed / 10);
            //evt.AddActorPara(SagaBNS.Common.Packets.GameServer.PacketParameter.Unk7A, target.Status.StanceFlag1.Value);
            map.SendEventToAllActorsWhoCanSeeActor(MapEvents.EVENT_BROADCAST, evt, actor, true);

            evt = UpdateEvent.NewActorAdditionExtEvent((ActorExt)actor, arg.SkillSession, 12, 12000057, 6000, UpdateEvent.ExtraUpdateModes.Activate);

            evt.AdditionCount = AccumulateCount;
            map.SendEventToAllActorsWhoCanSeeActor(MapEvents.EVENT_BROADCAST, evt, actor, true);
        }
    }
}
