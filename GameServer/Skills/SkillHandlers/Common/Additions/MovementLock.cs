
using SmartEngine.Network.Map;
using SmartEngine.Network.Tasks;
using SagaBNS.GameServer.Network.Client;
namespace SagaBNS.GameServer.Skills.SkillHandlers.Common.Additions
{
    public class MovementLock : Buff
    {
        private readonly GameSession client;
        public MovementLock(GameSession client,int lockDuration)
            : base(client.Character, "MovementLock", lockDuration)
        {
            this.client = client;
            if (client.Character.Tasks.TryGetValue("MovementLock", out Task task))
            {
                task.Deactivate();
            }

            client.Character.Tasks["MovementLock"] = this;
            OnAdditionStart += new StartEventHandler(MovementLock_OnAdditionStart);
            OnAdditionEnd += new EndEventHandler(MovementLock_OnAdditionEnd);
        }

        private void MovementLock_OnAdditionEnd(Actor actor, Buff skill, bool cancel)
        {
            client.Character.Tasks.TryRemove("MovementLock", out Task removed);
            client.SendPlayerMovementLock(false);
        }

        private void MovementLock_OnAdditionStart(Actor actor, Buff skill)
        {
            client.SendPlayerMovementLock(true);
        }
    }
}
