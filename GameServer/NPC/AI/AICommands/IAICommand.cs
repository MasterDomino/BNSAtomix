using SagaBNS.Common.Actors;

namespace SagaBNS.GameServer.NPC.AI.AICommands
{
    public interface IAICommand
    {
        CommandTypes Type { get; }
        CommandStatus Status { get; }
        ActorExt Target { get; set; }
        void Update();
    }
}
