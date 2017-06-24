using System.Threading;
using SmartEngine.Network.Tasks;
using SagaBNS.Common.Actors;
using SagaBNS.GameServer.Network.Client;

namespace SagaBNS.GameServer.Tasks.Player
{
    public class MPRegenerationTask : Task
    {
        private readonly GameSession client;
        public MPRegenerationTask(ActorPC actor)
            : base(0, 3000, "MPRegeneration")
        {
            client = actor.Client();
        }

        public override void CallBack()
        {
            if (client.Character == null)
            {
                Deactivate();
                return;
            }
            if (!client.Character.Status.IsInCombat && !client.Character.Status.Dead && client.Character.MP < client.Character.MaxMP)
            {
                Interlocked.Increment(ref client.Character.MP);
                if (client.Character.MP > client.Character.MaxMP)
                {
                    Interlocked.Exchange(ref client.Character.MP, client.Character.MaxMP);
                }

                client.SendPlayerMP();
            }
        }
    }
}
