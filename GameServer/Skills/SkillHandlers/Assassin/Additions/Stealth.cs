
using SmartEngine.Network.Map;
using SmartEngine.Network.Tasks;
using SagaBNS.Common.Actors;
using SagaBNS.Common.Skills;
using SagaBNS.GameServer.Map;
namespace SagaBNS.GameServer.Skills.SkillHandlers.Assassin.Additions
{
    public class Stealth : Buff
    {
        private readonly SkillArg arg;
        public Stealth(SkillArg arg)
            : base(arg.Caster, "Stealth", 6000)
        {
            this.arg = arg;
            OnAdditionStart += new StartEventHandler(Stealth_OnAdditionStart);
            OnAdditionEnd += new EndEventHandler(Stealth_OnAdditionEnd);
        }

        private void Stealth_OnAdditionEnd(Actor actor, Buff skill, bool cancel)
        {
            arg.Caster.Status.Stealth = false;
            Map.Map map = Map.MapManager.Instance.GetMap(actor.MapInstanceID);
            ((ActorExt)actor).Tasks.TryRemove("Stealth", out Task removed);
            arg.Caster.Status.StanceFlag1.SetValue(StanceU1.Dash, false);
            UpdateEvent evt = new UpdateEvent()
            {
                Actor = actor,
                Target = actor,
                SkillSession = arg.SkillSession,
                AdditionID = 15119010,
                AdditionSession = 5,
                ExtraActivateMode = UpdateEvent.ExtraUpdateModes.Cancel,
                UpdateType = UpdateTypes.Actor
            };
            //evt.AddActorPara(SagaBNS.Common.Packets.GameServer.PacketParameter.Speed, 62);
            //evt.AddActorPara(SagaBNS.Common.Packets.GameServer.PacketParameter.Unk7A, arg.Caster.Status.StanceFlag1.Value);
            //evt.AddActorPara(SagaBNS.Common.Packets.GameServer.PacketParameter.UnkF4, 0);
            //evt.AddActorPara(SagaBNS.Common.Packets.GameServer.PacketParameter.UnkF0, 0);
            map.SendEventToAllActorsWhoCanSeeActor(MapEvents.EVENT_BROADCAST, evt, actor, true);

            evt = new UpdateEvent()
            {
                Actor = actor,
                Target = actor,
                AdditionSession = 5,
                AdditionID = 15119010,
                RestTime = 6000,
                SkillSession = arg.SkillSession,
                ExtraActivateMode = UpdateEvent.ExtraUpdateModes.Cancel,
                UpdateType = UpdateTypes.ActorExtension
            };
            map.SendEventToAllActorsWhoCanSeeActor(MapEvents.EVENT_BROADCAST, evt, actor, true);

            evt = new UpdateEvent()
            {
                Actor = actor,
                Target = actor,
                SkillSession = 255,
                AdditionID = 15000025,
                AdditionSession = 8194,
                ExtraActivateMode = UpdateEvent.ExtraUpdateModes.Cancel,
                UpdateType = UpdateTypes.Actor
            };
            //evt.AddActorPara(SagaBNS.Common.Packets.GameServer.PacketParameter.UnkEB, 0);
            map.SendEventToAllActorsWhoCanSeeActor(MapEvents.EVENT_BROADCAST, evt, actor, true);

            evt = new UpdateEvent()
            {
                Actor = actor,
                Target = actor,
                AdditionSession = 8194,
                AdditionID = 15000025,
                RestTime = 0,
                SkillSession = arg.SkillSession,
                ExtraActivateMode = UpdateEvent.ExtraUpdateModes.Cancel,
                UpdateType = UpdateTypes.ActorExtension
            };
            map.SendEventToAllActorsWhoCanSeeActor(MapEvents.EVENT_BROADCAST, evt, actor, true);

            arg.Caster.ChangeStance(Stances.Assassin_Normal, arg.SkillSession, 15001011);
        }

        private void Stealth_OnAdditionStart(Actor actor, Buff skill)
        {
            arg.Caster.Status.Stealth = true;
            Map.Map map = Map.MapManager.Instance.GetMap(actor.MapInstanceID);
            arg.Caster.Status.StanceFlag1.SetValue(StanceU1.Dash, true);
            UpdateEvent evt = new UpdateEvent()
            {
                Actor = actor,
                Target = actor,
                SkillSession = arg.SkillSession,
                AdditionID = 15119010,
                AdditionSession = 5,
                ExtraActivateMode = UpdateEvent.ExtraUpdateModes.Activate,
                UpdateType = UpdateTypes.Actor
            };
            //evt.AddActorPara(SagaBNS.Common.Packets.GameServer.PacketParameter.Speed, 124);
            //evt.AddActorPara(SagaBNS.Common.Packets.GameServer.PacketParameter.Unk7A, arg.Caster.Status.StanceFlag1.Value);
            //evt.AddActorPara(SagaBNS.Common.Packets.GameServer.PacketParameter.UnkF4, 1);
            //evt.AddActorPara(SagaBNS.Common.Packets.GameServer.PacketParameter.UnkF0, 1);
            map.SendEventToAllActorsWhoCanSeeActor(MapEvents.EVENT_BROADCAST, evt, actor, true);

            evt = new UpdateEvent()
            {
                Actor = actor,
                Target = actor,
                AdditionSession = 5,
                AdditionID = 15119010,
                RestTime = 6000,
                SkillSession = arg.SkillSession,
                UpdateType = UpdateTypes.ActorExtension
            };
            map.SendEventToAllActorsWhoCanSeeActor(MapEvents.EVENT_BROADCAST, evt, actor, true);

            evt = new UpdateEvent()
            {
                Actor = actor,
                Target = actor,
                SkillSession = 255,
                AdditionID = 15000025,
                AdditionSession = 8194,
                ExtraActivateMode = UpdateEvent.ExtraUpdateModes.Activate,
                UpdateType = UpdateTypes.Actor
            };
            //evt.AddActorPara(SagaBNS.Common.Packets.GameServer.PacketParameter.UnkEB , 1);
            map.SendEventToAllActorsWhoCanSeeActor(MapEvents.EVENT_BROADCAST, evt, actor, true);

            evt = new UpdateEvent()
            {
                Actor = actor,
                Target = actor,
                AdditionSession = 8194,
                AdditionID = 15000025,
                RestTime = 0,
                SkillSession = arg.SkillSession,
                UpdateType = UpdateTypes.ActorExtension
            };
            map.SendEventToAllActorsWhoCanSeeActor(MapEvents.EVENT_BROADCAST, evt, actor, true);

            arg.Caster.ChangeStance(Stances.Assassin_Stealth, arg.SkillSession, 15001011, 10);
        }
    }
}
