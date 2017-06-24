namespace SagaBNS.Common.Actors
{
    public class ActorItem : ActorExt
    {
        public uint ObjectID { get; set; }
        public ActorExt Creator { get; set; }
        public ulong CorpseID { get; set; }
        public int DisappearTime { get; set; }
        public ActorItem(uint objID)
        {
            ObjectID = objID;
            DisappearTime = 20000;
            type = SmartEngine.Network.Map.ActorType.ITEM;
            SightRange = 0;
        }
    }
}
