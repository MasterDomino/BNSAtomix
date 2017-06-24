using System.Collections.Generic;

using SagaBNS.Common.Actors;

namespace SagaBNS.GameServer.NPC
{
    public class FactionRelation
    {
        private readonly Dictionary<Factions, Relations> relations = new Dictionary<Factions, Relations>();
        public Factions Faction { get; set; }

        public Relations this[Factions target]
        {
            get
            {
                if (relations.ContainsKey(target))
                {
                    return relations[target];
                }
                else
                {
                    return Relations.Friendly;
                }
            }
            set
            {
                relations[target] = value;
            }
        }
    }
}
