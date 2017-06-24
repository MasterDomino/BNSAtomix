using System;
using SmartEngine.Core;
using System.Threading;

using SmartEngine.Network.Map;
using SmartEngine.Network.Tasks;
using SagaBNS.Common.Actors;
using SagaBNS.Common.Skills;
using SagaBNS.GameServer.Map;
namespace SagaBNS.GameServer.Skills.SkillHandlers.Common.Additions
{
    public class Poisen : Buff
    {
        private readonly SkillArg arg;
        private readonly ActorExt target;
        private readonly uint additionID;
        private readonly int damage;
        public Poisen(SkillArg arg, ActorExt target, uint additionID, int damage = 0, int duration = 5000)
            : base(target, "Poisen", duration,2000)
        {
            this.arg = arg;
            this.target = target;
            this.additionID = additionID;
            this.damage = damage;
            if (target.Tasks.TryGetValue("Poisen", out Task task))
            {
                task.Deactivate();
            }
            target.Tasks["Poisen"] = this;
            OnAdditionStart += new StartEventHandler(ActorFrosen_OnAdditionStart);
            OnAdditionEnd += new EndEventHandler(ActorFrosen_OnAdditionEnd);
            OnUpdate += new UpdateEventHandler(Poisen_OnUpdate);
        }

        private unsafe void Poisen_OnUpdate(Actor actor, Buff skill)
        {
            Map.Map map = MapManager.Instance.GetMap(actor.MapInstanceID);
            if (map != null)
            {
                Interlocked.Add(ref target.HP, -damage);
                if (target.HP < 0)
                {
                    Interlocked.Exchange(ref target.HP, 0);
                    Deactivate();
                }
                UpdateEvent evt = UpdateEvent.NewActorAdditionEvent(arg.Caster, target, arg.SkillSession, 4098, additionID, UpdateEvent.ExtraUpdateModes.Update);
                evt.AddActorPara(SagaBNS.Common.Packets.GameServer.PacketParameter.HP, target.HP);
                if (target.HP <= 0 && !(target.Status.Dead && !target.Status.Dying))
                {
                    target.Status.Dead = true;
                    //evt.AddActorPara(SagaBNS.Common.Packets.GameServer.PacketParameter.Dead, target.ActorType == ActorType.PC ? (target.Status.Dying ? 1 : 2) : 1);
                    ((BNSActorEventHandler)target.EventHandler).OnDie(arg.Caster);
                }
                byte[] buf = new byte[6];
                fixed (byte* res = buf)
                {
                    res[0] = 7;
                    *(int*)&res[2] = -damage;
                }
                evt.UserData = buf;

                map.SendEventToAllActorsWhoCanSeeActor(MapEvents.EVENT_BROADCAST, evt, target, true);
            }
            else
            {
                Deactivate();
            }
        }

        private void ActorFrosen_OnAdditionEnd(Actor actor, Buff skill, bool cancel)
        {
            try
            {
                Map.Map map = MapManager.Instance.GetMap(actor.MapInstanceID);
                target.Tasks.TryRemove("Poisen", out Task removed);
                target.Status.StanceFlag1.SetValue(StanceU1.Poisen, false);
                if (map != null)
                {
                    UpdateEvent evt = UpdateEvent.NewActorAdditionEvent(arg.Caster, target, arg.SkillSession, 4098, additionID, UpdateEvent.ExtraUpdateModes.Cancel);
                    //evt.AddActorPara(SagaBNS.Common.Packets.GameServer.PacketParameter.Unk7A, target.Status.StanceFlag1.Value);
                    map.SendEventToAllActorsWhoCanSeeActor(MapEvents.EVENT_BROADCAST, evt, target, true);
                    evt = UpdateEvent.NewActorAdditionExtEvent(target, arg.SkillSession, 4098, additionID, TotalLifeTime, UpdateEvent.ExtraUpdateModes.Cancel);
                    map.SendEventToAllActorsWhoCanSeeActor(MapEvents.EVENT_BROADCAST, evt, target, true);
                }
            }
            catch (Exception ex)
            {
                Logger.Log.Error(ex);
            }
        }

        private unsafe void ActorFrosen_OnAdditionStart(Actor actor, Buff skill)
        {
            Map.Map map = MapManager.Instance.GetMap(actor.MapInstanceID);
            target.Status.StanceFlag1.SetValue(StanceU1.Poisen, true);
            UpdateEvent evt = UpdateEvent.NewActorAdditionEvent(arg.Caster, target, arg.SkillSession, 4098, additionID, UpdateEvent.ExtraUpdateModes.Activate);
            //evt.AddActorPara(SagaBNS.Common.Packets.GameServer.PacketParameter.Unk7A, target.Status.StanceFlag1.Value);
            byte[] buf = new byte[6];
            fixed (byte* res = buf)
            {
                res[0] = 7;
                *(int*)&res[2] = -damage;
            }
            evt.UserData = buf;
            map.SendEventToAllActorsWhoCanSeeActor(MapEvents.EVENT_BROADCAST, evt, target, true);

            evt = UpdateEvent.NewActorAdditionExtEvent(target, arg.SkillSession, 4098, additionID, TotalLifeTime, UpdateEvent.ExtraUpdateModes.Activate);
            map.SendEventToAllActorsWhoCanSeeActor(MapEvents.EVENT_BROADCAST, evt, target, true);
        }
    }
}
