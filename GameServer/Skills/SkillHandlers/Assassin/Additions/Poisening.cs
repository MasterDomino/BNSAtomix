
using SmartEngine.Network.Map;
using SmartEngine.Network.Tasks;
using SagaBNS.Common.Skills;
using SagaBNS.GameServer.Map;
namespace SagaBNS.GameServer.Skills.SkillHandlers.Assassin.Additions
{
    public class Poisening : Buff
    {
        private readonly SkillArg arg;
        public Poisening(SkillArg arg)
            : base(arg.Caster, "Poisening", 30000)
        {
            this.arg = arg;
            if (arg.Caster.Tasks.TryRemove("Poisening", out Task removed))
            {
                removed.Deactivate();
            }

            arg.Caster.Tasks["Poisening"] = this;
            OnAdditionStart += new StartEventHandler(Poisening_OnAdditionStart);
            OnAdditionEnd += new EndEventHandler(Poisening_OnAdditionEnd);
        }

        private void Poisening_OnAdditionEnd(Actor actor, Buff skill, bool cancel)
        {
            arg.Caster.Tasks.TryRemove("Poisening", out Task removed);
            Map.Map map = Map.MapManager.Instance.GetMap(actor.MapInstanceID);
            UpdateEvent evt = UpdateEvent.NewActorAdditionEvent(arg.Caster, arg.Caster, arg.SkillSession, 3, 15000016, UpdateEvent.ExtraUpdateModes.Cancel);
            //evt.AddActorPara(SagaBNS.Common.Packets.GameServer.PacketParameter.UnkF9, 0);
            map.SendEventToAllActorsWhoCanSeeActor(MapEvents.EVENT_BROADCAST, evt, actor, true);

            evt = UpdateEvent.NewActorAdditionExtEvent(arg.Caster, arg.SkillSession, 3, 15000016, 3000, UpdateEvent.ExtraUpdateModes.Cancel);
            map.SendEventToAllActorsWhoCanSeeActor(MapEvents.EVENT_BROADCAST, evt, actor, true);
        }

        private void Poisening_OnAdditionStart(Actor actor, Buff skill)
        {
            Map.Map map = Map.MapManager.Instance.GetMap(actor.MapInstanceID);
            UpdateEvent evt = UpdateEvent.NewActorAdditionEvent(arg.Caster, arg.Caster, arg.SkillSession, 3, 15000016, UpdateEvent.ExtraUpdateModes.Activate);
            //evt.AddActorPara(SagaBNS.Common.Packets.GameServer.PacketParameter.UnkF9, 1);
            map.SendEventToAllActorsWhoCanSeeActor(MapEvents.EVENT_BROADCAST, evt, actor, true);

            evt = UpdateEvent.NewActorAdditionExtEvent(arg.Caster, arg.SkillSession, 3, 15000016, 3000, UpdateEvent.ExtraUpdateModes.Activate);
            map.SendEventToAllActorsWhoCanSeeActor(MapEvents.EVENT_BROADCAST, evt, actor, true);
        }
    }
}
