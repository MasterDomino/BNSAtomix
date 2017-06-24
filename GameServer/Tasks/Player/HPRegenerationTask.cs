using System.Threading;
using SmartEngine.Network.Tasks;
using SagaBNS.Common.Actors;
using SagaBNS.GameServer.Network.Client;

namespace SagaBNS.GameServer.Tasks.Player
{
    public class HPRegenerationTask : Task
    {
        private readonly GameSession client;
        public HPRegenerationTask(ActorPC actor)
            : base(0, 1000, "HPRegeneration")
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
            if (!client.Character.Status.IsInCombat && !client.Character.Status.Dead && client.Character.HP < client.Character.MaxHP)
            {
                int amount = client.Character.MaxHP / 120;
                if (amount == 0)
                {
                    amount = 1;
                }

                Interlocked.Add(ref client.Character.HP, amount);
                if (client.Character.HP > client.Character.MaxHP)
                {
                    Interlocked.Exchange(ref client.Character.HP, client.Character.MaxHP);
                }

                client.SendPlayerHP();
            }
        }
    }
}
