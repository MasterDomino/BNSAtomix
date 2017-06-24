using System.Collections.Generic;
using System.Linq;

using SmartEngine.Network.Map;
using SmartEngine.Network.Tasks;
using SagaBNS.Common.Actors;
using SagaBNS.Common.Skills;
using SagaBNS.GameServer.Map;
namespace SagaBNS.GameServer.Skills.SkillHandlers.Common.Additions
{
    public class Teleport : Buff
    {
        private readonly SkillArg arg;
        private readonly Dictionary<SagaBNS.Common.Item.Item, ushort> items = new Dictionary<SagaBNS.Common.Item.Item, ushort>();
        public Teleport(SkillArg arg,Dictionary<SagaBNS.Common.Item.Item, ushort> items)
            : base(arg.Caster, "Teleport", 10000)
        {
            this.arg = arg;
            this.items = items;
            OnAdditionStart += new StartEventHandler(Teleport_OnAdditionStart);
            OnAdditionEnd += new EndEventHandler(Teleport_OnAdditionEnd);
        }

        private void Teleport_OnAdditionEnd(Actor actor, Buff skill, bool cancel)
        {
            Map.Map map = Map.MapManager.Instance.GetMap(actor.MapInstanceID);
            SagaBNS.GameServer.Network.Client.GameSession session = SagaBNS.GameServer.Manager.GameClientManager.Instance.FindClient(actor.Name);
            RespawnPoint sendto = TeleportPoint(arg.Dir);
            bool passed = true;
            ((ActorExt)actor).Tasks.TryRemove("Teleport", out Task removed);
            List<UpdateEvent> update = new List<UpdateEvent>();
            UpdateEvent evt = new UpdateEvent()
            {
                Actor = actor,
                UpdateType = UpdateTypes.Teleport
            };
            if (cancel)
            {
                evt.AdditionCount = 2;
                passed = false;
            }
            else
            {
                evt.AdditionCount = 1;
            }

            map.SendEventToAllActorsWhoCanSeeActor(MapEvents.EVENT_BROADCAST, evt, actor, false);
            update.Add(evt);
            session.Character.Client().SendActorUpdates(update);

            if (passed && session != null && sendto.teleportId != 0)
            {
                foreach (KeyValuePair<SagaBNS.Common.Item.Item, ushort> i in items)
                {
                    session.RemoveItemSlot(i.Key.SlotID, i.Value);
                }

                session.Character.Status.DisappearEffect = 539;
                session.Character.Status.ShouldLoadMap = true;
                Map.Map map2 = MapManager.Instance.GetMap(sendto.MapID, session.Character.CharID, session.Character.PartyID);
                session.Map.SendActorToMap(session.Character, map2, sendto.X, sendto.Y, sendto.Z);
            }
        }

        private RespawnPoint TeleportPoint(ushort location)
        {
            foreach (List<RespawnPoint> points in MapManager.Instance.RespawnPoints.Values.ToArray())
            {
                foreach (RespawnPoint point in points)
                {
                    if (point.teleportId == location)
                    {
                        return point;
                    }
                }
            }
            return new RespawnPoint();
        }

        private void Teleport_OnAdditionStart(Actor actor, Buff skill)
        {
            Map.Map map = Map.MapManager.Instance.GetMap(actor.MapInstanceID);
            UpdateEvent evt = new UpdateEvent()
            {
                Actor = actor,
                UpdateType = UpdateTypes.Teleport,
                AdditionCount = 0
            };
            map.SendEventToAllActorsWhoCanSeeActor(MapEvents.EVENT_BROADCAST, evt, actor, true);
        }
    }
}
