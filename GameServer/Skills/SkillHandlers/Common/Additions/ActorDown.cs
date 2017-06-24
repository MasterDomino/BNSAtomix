
using SmartEngine.Network.Map;
using SmartEngine.Network.Tasks;
using SagaBNS.Common.Actors;
using SagaBNS.Common.Skills;
using SagaBNS.GameServer.Map;
namespace SagaBNS.GameServer.Skills.SkillHandlers.Common.Additions
{
    public class ActorDown : Buff
    {
        private readonly SkillArg arg;
        private readonly ActorExt target;
        private readonly uint additionID;
        private readonly int damage;
        public ActorDown(SkillArg arg, ActorExt target, uint additionID, int damage = 0, int duration = 5000)
            : base(target, "ActorDown", duration)
        {
            this.arg = arg;
            this.target = target;
            this.additionID = additionID;
            this.damage = damage;
            OnAdditionStart += new StartEventHandler(ActorDown_OnAdditionStart);
            OnAdditionEnd += new EndEventHandler(ActorDown_OnAdditionEnd);
        }

        private void ActorDown_OnAdditionEnd(Actor actor, Buff skill, bool cancel)
        {
            target.Status.Down = false;
            target.Tasks.TryRemove("ActorDown", out Task removed);
            target.Status.StanceFlag1.SetValue(StanceU1.Down1, false);
            target.Status.StanceFlag1.SetValue(StanceU1.NoMove, false);
            UpdateEvent evt = new UpdateEvent()
            {
                Actor = arg.Caster,
                AdditionID = additionID,
                Target = target,
                Skill = arg.Skill,
                ExtraActivateMode = UpdateEvent.ExtraUpdateModes.Cancel,
                SkillSession = arg.SkillSession,
                UpdateType = UpdateTypes.Actor,
                //evt.AddActorPara(SagaBNS.Common.Packets.GameServer.PacketParameter.Unk7A, target.Status.StanceFlag1.Value);
                //evt.AddActorPara(SagaBNS.Common.Packets.GameServer.PacketParameter.UnkDF, 0);
                UserData = new byte[] { 9, 1, 0 }
            };
            MapManager.Instance.GetMap(actor.MapInstanceID).SendEventToAllActorsWhoCanSeeActor(MapEvents.EVENT_BROADCAST, evt, target, true);
        }

        private unsafe void ActorDown_OnAdditionStart(Actor actor, Buff skill)
        {
            if (!target.Status.Down)
            {
                target.Status.Down = true;
                target.Status.StanceFlag1.SetValue(StanceU1.Down1, true);
                target.Status.StanceFlag1.SetValue(StanceU1.NoMove, true);
                UpdateEvent evt = new UpdateEvent()
                {
                    Actor = arg.Caster,
                    AdditionID = additionID,
                    Target = target,
                    Skill = arg.Skill,
                    SkillSession = arg.SkillSession,
                    ExtraActivateMode = UpdateEvent.ExtraUpdateModes.Activate,
                    UpdateType = UpdateTypes.Actor
                };
                //evt.AddActorPara(SagaBNS.Common.Packets.GameServer.PacketParameter.Unk7A, target.Status.StanceFlag1.Value);
                //evt.AddActorPara(SagaBNS.Common.Packets.GameServer.PacketParameter.UnkDF, 1);
                byte[] buf = new byte[7];
                fixed (byte* res = buf)
                {
                    res[0] = 3;
                    *(int*)&res[2] = damage;
                }
                evt.UserData = buf;
                MapManager.Instance.GetMap(actor.MapInstanceID).SendEventToAllActorsWhoCanSeeActor(MapEvents.EVENT_BROADCAST, evt, target, true);
            }
        }
    }
}
