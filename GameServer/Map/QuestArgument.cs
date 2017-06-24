
using SmartEngine.Network.Map;
using SagaBNS.Common.Actors;
using SagaBNS.Common.Quests;

namespace SagaBNS.GameServer.Map
{
    public class QuestArgument : MapEventArgs
    {
        public ActorPC Player { get; set; }
        public uint OriginNPC { get; set; }
        public Quest Quest { get; set; }
        public byte Step { get; set; }
    }
}
