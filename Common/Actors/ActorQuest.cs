namespace SagaBNS.Common.Actors
{
    public class ActorQuest : ActorExt
    {
        public ActorQuest()
        {
            type = SmartEngine.Network.Map.ActorType.QUEST;
            SightRange = 150;
            Faction = Factions.QuestNPC;
        }
    }
}
