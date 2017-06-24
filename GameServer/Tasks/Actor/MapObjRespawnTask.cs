using SmartEngine.Network.Tasks;
using SagaBNS.Common.Actors;
using SagaBNS.GameServer.Map;

namespace SagaBNS.GameServer.Tasks.Actor
{
    public class MapObjRespawnTask : Task
    {
        private readonly ActorMapObj actor;
        private readonly bool already;
        public MapObjRespawnTask(ActorMapObj actor)
            : base(actor.RespawnTime, actor.RespawnTime, "Respawn")
        {
            this.actor = actor;
            if (actor.Tasks.TryRemove("Respawn", out Task removed))
            {
                removed.Deactivate();
            }

            actor.Tasks["Respawn"] = this;
        }

        public override void CallBack()
        {
            Deactivate();
            actor.Tasks.TryRemove("Respawn", out Task task);
            Map.Map map = Map.MapManager.Instance.GetMap(actor.MapInstanceID);

            actor.Available = true;
            actor.Items = null;
            UpdateEvent evt = new UpdateEvent()
            {
                Actor = actor,
                UpdateType = UpdateTypes.MapObjectVisibilityChange
            };
            map.SendEventToAllActors(MapEvents.EVENT_BROADCAST, evt, actor, true);
        }
    }
}
