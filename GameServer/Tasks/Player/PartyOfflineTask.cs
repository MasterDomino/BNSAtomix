using SmartEngine.Network.Tasks;
using SagaBNS.Common.Actors;
namespace SagaBNS.GameServer.Tasks.Player
{
    public class PartyOfflineTask : Task
    {
        private readonly ActorPC pc;
        public PartyOfflineTask(ActorPC pc)
            : base(300000, 300000, "PartyOfflineTask")
        {
            this.pc = pc;
            pc.Tasks["PartyOfflineTask"] = this;
        }

        protected override void OnDeactivate()
        {
            pc.Tasks.TryRemove("PartyOfflineTask", out Task removed);
        }

        public override void CallBack()
        {
            Deactivate();
            if (pc.Party != null)
            {
                Party.PartyManager.Instance.PartyMemberQuit(pc.Party, pc);
            }
        }
    }
}
