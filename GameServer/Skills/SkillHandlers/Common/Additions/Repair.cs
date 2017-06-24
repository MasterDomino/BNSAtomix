using System.Collections.Generic;

using SmartEngine.Network.Map;
using SmartEngine.Network.Tasks;
using SagaBNS.Common.Actors;
using SagaBNS.Common.Skills;
using SagaBNS.GameServer.Map;
namespace SagaBNS.GameServer.Skills.SkillHandlers.Common.Additions
{
    public class Repair : Buff
    {
        private readonly SkillArg arg;
        private readonly Dictionary<SagaBNS.Common.Item.Item, ushort> items = new Dictionary<SagaBNS.Common.Item.Item, ushort>();
        public Repair(SkillArg arg,Dictionary<SagaBNS.Common.Item.Item, ushort> items)
            : base(arg.Caster, "Repair", 7500)
        {
            this.arg = arg;
            this.items = items;
            OnAdditionStart += new StartEventHandler(Repair_OnAdditionStart);
            OnAdditionEnd += new EndEventHandler(Repair_OnAdditionEnd);
        }

        private void Repair_OnAdditionEnd(Actor actor, Buff skill, bool cancel)
        {
            Map.Map map = Map.MapManager.Instance.GetMap(actor.MapInstanceID);
            SagaBNS.GameServer.Network.Client.GameSession session = SagaBNS.GameServer.Manager.GameClientManager.Instance.FindClient(actor.Name);
            ((ActorExt)actor).Tasks.TryRemove("Repair", out Task removed);

            foreach (KeyValuePair<SagaBNS.Common.Item.Item, ushort> i in items)
            {
                session.RemoveItemSlot(i.Key.SlotID, i.Value);
            }

            //TODO: Set Durability to Max for equipted weapon

            UpdateEvent evt = new UpdateEvent()
            {
                UpdateType = UpdateTypes.Repair,
                Actor = actor,
                Target = arg.Target,
                AdditionCount = 1
            };
            map.SendEventToAllActorsWhoCanSeeActor(MapEvents.EVENT_BROADCAST, evt, actor, true);
        }

        private void Repair_OnAdditionStart(Actor actor, Buff skill)
        {
            Map.Map map = Map.MapManager.Instance.GetMap(actor.MapInstanceID);
            UpdateEvent evt = new UpdateEvent()
            {
                UpdateType = UpdateTypes.Repair,
                Actor = actor,
                Target = arg.Target,
                AdditionCount = 0
            };
            map.SendEventToAllActorsWhoCanSeeActor(MapEvents.EVENT_BROADCAST, evt, actor, true);
        }
    }
}
