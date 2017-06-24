using SmartEngine.Network.Tasks;
using SagaBNS.Common.Actors;

namespace SagaBNS.GameServer.Tasks.Actor
{
    public class ItemDeleteTask : Task
    {
        private readonly ActorItem actor;
        public ItemDeleteTask(ActorItem actor)
            : base(actor.DisappearTime,actor.DisappearTime, "ItemDelete")
        {
            this.actor = actor;
            if (actor.Tasks.TryRemove("ItemDelete", out Task removed))
            {
                removed.Deactivate();
            }
            /*if (actor.NPC.BaseData.QuestIDs.Count > 0)
{
   actor.QuestID = actor.NPC.BaseData.QuestIDs[0];
}*/
            actor.Tasks["ItemDelete"] = this;
        }

        public override void CallBack()
        {
            Map.Map map = Map.MapManager.Instance.GetMap(actor.MapInstanceID);
            actor.Tasks.TryRemove("ItemDelete", out Task task);
            Deactivate();
            map.DeleteActor(actor);
        }
    }
}
