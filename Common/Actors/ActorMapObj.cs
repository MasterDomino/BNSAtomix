using System.Collections.Generic;

namespace SagaBNS.Common.Actors
{
    public class ActorMapObj : ActorCorpse
    {
        public ushort ObjectID { get; set; }
        public bool Available { get; set; }
        public int RespawnTime { get; set; }
        public bool Special { get; set; }
        public bool DragonStream { get; set; }
        public uint SpecialMapID { get; set; }
        public Dictionary<uint, int> ItemIDs { get { return itemIds; } }
        public int MinGold { get; set; }
        public int MaxGold { get; set; }

        private readonly Dictionary<uint, int> itemIds = new Dictionary<uint, int>();
        private readonly List<Item.Item> items = new List<Item.Item>();
        public ActorMapObj(ushort objID)
            :base(null)
        {
            ObjectID = objID;
            type = SmartEngine.Network.Map.ActorType.MAPOBJECT;
            SightRange = 2000;
            RespawnTime = 60000;
            Available = true;
        }
    }
}
