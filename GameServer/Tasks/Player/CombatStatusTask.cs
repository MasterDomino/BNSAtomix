using SmartEngine.Network.Tasks;
using SagaBNS.Common.Actors;
using SagaBNS.GameServer.Network.Client;

namespace SagaBNS.GameServer.Tasks.Player
{
    public class CombatStatusTask : Task
    {
        private readonly GameSession client;
        public CombatStatusTask(int duration, ActorPC actor)
            : base(duration, duration, "CombatStatusTask")
        {
            client = actor.Client();
        }

        public override void CallBack()
        {
            Deactivate();
            if (client?.Character != null)
            {
                client.Character.Tasks.TryRemove("CombatStatusTask", out Task removed);
                client.ChangeCombatStatus(false);
            }
        }
    }
}
