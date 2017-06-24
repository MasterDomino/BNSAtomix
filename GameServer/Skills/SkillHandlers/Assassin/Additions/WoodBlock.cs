
using SmartEngine.Network.Map;
using SmartEngine.Network.Tasks;
using SagaBNS.Common.Actors;
using SagaBNS.Common.Skills;
using SagaBNS.GameServer.Map;
namespace SagaBNS.GameServer.Skills.SkillHandlers.Assassin.Additions
{
    public class WoodBlock : Buff
    {
        private readonly SkillArg arg;
        public uint CounterSkillID { get; set; }
        public WoodBlock(SkillArg arg)
            : base(arg.Caster, "WoodBlock", 1500)
        {
            this.arg = arg;
            if (arg.Caster.Tasks.TryRemove("WoodBlock", out Task removed))
            {
                removed.Deactivate();
            }

            arg.Caster.Tasks["WoodBlock"] = this;
            CounterSkillID = 15116;
            OnAdditionStart += new StartEventHandler(WoodBlock_OnAdditionStart);
            OnAdditionEnd += new EndEventHandler(WoodBlock_OnAdditionEnd);
        }

        private void WoodBlock_OnAdditionEnd(Actor actor, Buff skill, bool cancel)
        {
            ((ActorExt)actor).Status.Dummy = false;
            Map.Map map = MapManager.Instance.GetMap(actor.MapInstanceID);
            ((ActorExt)actor).Tasks.TryRemove("CounterEnemy", out Task removed);
            UpdateEvent evt = UpdateEvent.NewActorAdditionEvent((ActorExt)actor, (ActorExt)actor, arg.SkillSession, 1, 15111010, UpdateEvent.ExtraUpdateModes.Cancel);
            evt.UserData = new byte[] { 9, 1, 0 };
            //evt.AddActorPara(SagaBNS.Common.Packets.GameServer.PacketParameter.BlockingStance, 0);
            map.SendEventToAllActorsWhoCanSeeActor(MapEvents.EVENT_BROADCAST, evt, actor, true);

            evt = UpdateEvent.NewActorAdditionExtEvent((ActorExt)actor, arg.SkillSession, 1, 15111010, 0, UpdateEvent.ExtraUpdateModes.Cancel);
            map.SendEventToAllActorsWhoCanSeeActor(MapEvents.EVENT_BROADCAST, evt, actor, true);
            SkillManager.Instance.BroadcastSkillCast(arg, SkillMode.End);
        }

        private void WoodBlock_OnAdditionStart(Actor actor, Buff skill)
        {
            Map.Map map = MapManager.Instance.GetMap(actor.MapInstanceID);
            ((ActorExt)actor).Status.Dummy = true;
            UpdateEvent evt = UpdateEvent.NewActorAdditionEvent((ActorExt)actor, (ActorExt)actor, arg.SkillSession, 1, 15111010, UpdateEvent.ExtraUpdateModes.Activate);
            //evt.AddActorPara(SagaBNS.Common.Packets.GameServer.PacketParameter.BlockingStance, 1);

            map.SendEventToAllActorsWhoCanSeeActor(MapEvents.EVENT_BROADCAST, evt, actor, true);

            evt = UpdateEvent.NewActorAdditionExtEvent((ActorExt)actor, arg.SkillSession, 1, 15111010, arg.Skill.BaseData.Duration, UpdateEvent.ExtraUpdateModes.Activate);
            map.SendEventToAllActorsWhoCanSeeActor(MapEvents.EVENT_BROADCAST, evt, actor, true);
        }
    }
}
