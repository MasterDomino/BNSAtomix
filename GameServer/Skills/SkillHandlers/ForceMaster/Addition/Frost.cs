
using SmartEngine.Network.Map;
using SmartEngine.Network.Tasks;
using SagaBNS.Common.Actors;
using SagaBNS.Common.Skills;
using SagaBNS.GameServer.Map;
namespace SagaBNS.GameServer.Skills.SkillHandlers.ForceMaster.Additions
{
    public class Frost : Common.Additions.BonusAddition
    {
        private readonly SkillArg arg;
        private readonly ActorExt target;
        public Frost(SkillArg arg, ActorExt target)
            : base(target, "Frost", 6400)
        {
            this.arg = arg;
            this.target = target;

            OnAdditionStart += new StartEventHandler(Frost_OnAdditionStart);
            OnAdditionEnd += new EndEventHandler(Frost_OnAdditionEnd);
        }

        private void Frost_OnAdditionEnd(Actor actor, Buff skill, bool cancel)
        {
            target.Status.Invincible = false;

            target.Status.StanceFlag1.SetValue(StanceU1.Unknown10000000, false);
            target.Status.StanceFlag1.SetValue(StanceU1.NoMove, false);
            target.Status.StanceFlag1.SetValue(StanceU1.Unknown200, false);
            Map.Map map = Map.MapManager.Instance.GetMap(actor.MapInstanceID);
            ((ActorExt)actor).Tasks.TryRemove("Frost", out Task removed);

            UpdateEvent evt = UpdateEvent.NewActorAdditionEvent(arg.Caster, (ActorExt)actor, arg.SkillSession, 4104, 12237012, UpdateEvent.ExtraUpdateModes.Cancel);
            //evt.AddActorPara(SagaBNS.Common.Packets.GameServer.PacketParameter.Unk7A, 0);
            //evt.AddActorPara(SagaBNS.Common.Packets.GameServer.PacketParameter.UnkE9, 0);

            map.SendEventToAllActorsWhoCanSeeActor(MapEvents.EVENT_BROADCAST, evt, actor, true);
        }

        private void Frost_OnAdditionStart(Actor actor, Buff skill)
        {
            target.Status.Invincible = true;

            Map.Map map = Map.MapManager.Instance.GetMap(actor.MapInstanceID);

            target.Status.StanceFlag1.SetValue(StanceU1.Unknown10000000, true);
            target.Status.StanceFlag1.SetValue(StanceU1.NoMove, true);
            target.Status.StanceFlag1.SetValue(StanceU1.Unknown200, true);

            UpdateEvent evt = UpdateEvent.NewActorAdditionEvent(arg.Caster, (ActorExt)actor, arg.SkillSession, 4104, 12237012, UpdateEvent.ExtraUpdateModes.Activate);

            //evt.AddActorPara(SagaBNS.Common.Packets.GameServer.PacketParameter.Unk7A, target.Status.StanceFlag1.Value);
            //evt.AddActorPara(SagaBNS.Common.Packets.GameServer.PacketParameter.UnkE9, 1);
            map.SendEventToAllActorsWhoCanSeeActor(MapEvents.EVENT_BROADCAST, evt, actor, true);
        }
    }
}
