using SmartEngine.Network.Tasks;
using SagaBNS.Common.Actors;

namespace SagaBNS.GameServer.Tasks.Actor
{
    public class RespawnTask : Task
    {
        private readonly ActorNPC actor;
        private readonly bool already;
        public RespawnTask(int respawnTime, ActorNPC actor)
            : base(respawnTime, respawnTime, "Respawn")
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
            if (map != null)
            {
                actor.X = actor.X_Ori;
                actor.Y = actor.Y_Ori;
                actor.Z = actor.Z_Ori;
                actor.Status.Dead = false;
                actor.HP = actor.MaxHP;
                map.RegisterActor(actor);
                actor.Invisible = false;
                map.OnActorVisibilityChange(actor);
                map.SendVisibleActorsToActor(actor);
            }
        }
    }
}
