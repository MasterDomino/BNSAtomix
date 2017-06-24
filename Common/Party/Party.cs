﻿using System.Collections.Generic;

using SagaBNS.Common.Actors;

namespace SagaBNS.Common.Party
{
    public enum PartyLootMode
    {
        Free,
        Ordered,
        Specified,
    }

    public class Party
    {
        private readonly List<ActorPC> members = new List<ActorPC>();
        public ulong PartyID { get; set; }
        public ActorPC Leader { get; set; }
        public PartyLootMode LootMode { get; set; }
        public byte AuctionItemRank { get; set; }
        public ActorPC SpecifiedLooter { get; set; }
        public List<ActorPC> Members { get { return members; } }
        public int PartyOrderIndex;
    }
}
