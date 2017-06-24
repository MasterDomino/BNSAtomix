
using SmartEngine.Network.Map;
using SmartEngine.Network.Tasks;
using SagaBNS.Common.Actors;
using SagaBNS.Common.Skills;
using SagaBNS.GameServer.Map;
namespace SagaBNS.GameServer.Skills.SkillHandlers.Common.Additions
{
    public class ActorFrosen : Buff
    {
        private readonly SkillArg arg;
        private readonly ActorExt target;
        private readonly uint additionID;
        private readonly int damage;
        public ActorFrosen(SkillArg arg, ActorExt target, uint additionID, int damage = 0, int duration = 5000)
            : base(target, "ActorFrosen", duration)
        {
            this.arg = arg;
            this.target = target;
            this.additionID = additionID;
            this.damage = damage;
            OnAdditionStart += new StartEventHandler(ActorFrosen_OnAdditionStart);
            OnAdditionEnd += new EndEventHandler(ActorFrosen_OnAdditionEnd);
        }

        private void ActorFrosen_OnAdditionEnd(Actor actor, Buff skill, bool cancel)
        {
            target.Status.Frosen = false;
            target.Status.StanceFlag1.SetValue(StanceU1.TakenDown, false);
            target.Status.StanceFlag1.SetValue(StanceU1.Unknown40000, false);
            target.Status.StanceFlag1.SetValue(StanceU1.Unknown4, false);
            target.Tasks.TryRemove("ActorFrosen", out Task removed);
            UpdateEvent evt = UpdateEvent.NewActorAdditionEvent(arg.Caster, target, arg.SkillSession, 4100, additionID, UpdateEvent.ExtraUpdateModes.Cancel);
            evt.UpdateType = UpdateTypes.Actor;
            //evt.AddActorPara(SagaBNS.Common.Packets.GameServer.PacketParameter.Unk7A, target.Status.StanceFlag1.Value);
            //evt.AddActorPara(SagaBNS.Common.Packets.GameServer.PacketParameter.UnkE7, 0);
            MapManager.Instance.GetMap(actor.MapInstanceID).SendEventToAllActorsWhoCanSeeActor(MapEvents.EVENT_BROADCAST, evt, target, true);
        }

        private unsafe void ActorFrosen_OnAdditionStart(Actor actor, Buff skill)
        {
            if (!target.Status.Frosen)
            {
                target.Status.Frosen = true;
                target.Status.StanceFlag1.SetValue(StanceU1.TakenDown, true);
                target.Status.StanceFlag1.SetValue(StanceU1.Unknown40000, true);
                target.Status.StanceFlag1.SetValue(StanceU1.Unknown4, true);
                UpdateEvent evt = UpdateEvent.NewActorAdditionEvent(arg.Caster, target, arg.SkillSession, 4100, additionID, UpdateEvent.ExtraUpdateModes.Activate);
                evt.UpdateType = UpdateTypes.Actor;
                //evt.AddActorPara(SagaBNS.Common.Packets.GameServer.PacketParameter.Unk7A, target.Status.StanceFlag1.Value);
                //evt.AddActorPara(SagaBNS.Common.Packets.GameServer.PacketParameter.UnkE7, 1);
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
