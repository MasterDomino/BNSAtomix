using System.Collections.Concurrent;
using System.Threading;

using SmartEngine.Network;
using SagaBNS.Common.Actors;
using SagaBNS.Common.Party;
using SagaBNS.GameServer.Map;
using SagaBNS.GameServer.Network.Client;
namespace SagaBNS.GameServer.Party
{
    public class PartyManager : Singleton<PartyManager>
    {
        private long partyID = 0x80000000000000;
        private readonly ConcurrentDictionary<ulong, Common.Party.Party> parties = new ConcurrentDictionary<ulong, Common.Party.Party>();

        public ConcurrentDictionary<ulong, Common.Party.Party> Parties { get { return parties; } }
        public ulong GetNewPartyID()
        {
            Interlocked.Increment(ref partyID);
            return (ulong)partyID;
        }

        public Common.Party.Party NewParty(ulong id, ActorPC leader, params  ActorPC[] members)
        {
            if (!parties.ContainsKey(id))
            {
                Common.Party.Party party = new Common.Party.Party();
                leader.PartyID = id;
                leader.Party = party;
                foreach (ActorPC i in members)
                {
                    i.PartyID = id;
                    i.Party = party;
                }
                party.PartyID = id;
                party.Leader = leader;
                party.Members.Add(leader);
                party.Members.AddRange(members);
                parties[id] = party;
                return party;
            }
            else
            {
                return null;
            }
        }

        public bool PartyCanLoot(ActorPC looter, ActorPC owner)
        {
            if (looter != null && owner != null)
            {
                if (looter.Party == owner.Party)
                {
                    if (looter.Party != null)
                    {
                        return looter.Party.LootMode == PartyLootMode.Free;
                    }
                    else
                    {
                        return false;
                    }
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return false;
            }
        }

        public ActorPC PartyGetMember(Common.Party.Party party, uint charID)
        {
            if (party != null)
            {
                foreach (ActorPC i in party.Members)
                {
                    if (i.CharID == charID)
                    {
                        return i;
                    }
                }
            }
            return null;
        }

        public void PartyMemberOfflineChange(Common.Party.Party party, ActorPC pc, bool offline)
        {
            pc.Offline = offline;
            if (party != null)
            {
                if (party.Leader == pc)
                {
                    PartyChangeLeader(party, party.Members[1]);
                }
                foreach (ActorPC i in party.Members)
                {
                    if (i == pc && !offline)
                    {
                        i.Client().SendPartyInfo();
                    }
                    else
                    {
                        GameSession client = i.Client();
                        client?.SendPartyMemberOnlineStatusChange(pc);
                    }
                }
            }
        }

        public void PartyMemberKick(Common.Party.Party party, string name)
        {
            if (party != null)
            {
                foreach (ActorPC i in party.Members)
                {
                    if (i.Name == name)
                    {
                        PartyMemberQuit(party, i);
                        return;
                    }
                }
            }
        }

        public void PartyMemberQuit(Common.Party.Party party, ActorPC pc)
        {
            if (party?.Members.Contains(pc) == true)
            {
                pc.Party = null;
                pc.PartyID = 0;
                foreach (ActorPC i in party.Members)
                {
                    GameSession client = i.Client();
                    client?.SendPartyMemberLeave(i.Name);
                }
                party.Members.Remove(pc);
                if (party.Members.Count <= 1)
                {
                    if (party.Members.Count > 0)
                    {
                        GameSession client = party.Members[0].Client();
                        client?.SendPartyMemberLeave(client.Character.Name);
                        client.Character.PartyID = 0;
                        client.Character.Party = null;
                    }
                    party.Leader = null;
                    party.Members.Clear();
                    parties.TryRemove(party.PartyID, out party);
                }
                else
                {
                    if (party.Leader == pc)
                    {
                        PartyChangeLeader(party, party.Members[0]);
                    }
                    if (party.SpecifiedLooter == pc)
                    {
                        PartyChangeLootMode(party, party.LootMode, party.AuctionItemRank, party.Leader.ActorID);
                    }
                }
            }
        }

        public void PartyMemberPositionUpdate(Common.Party.Party party, ActorPC pc)
        {
            if (party != null)
            {
                UpdateEvent evt = new UpdateEvent();
                evt.AddActorPara(Common.Packets.GameServer.PacketParameter.X, pc.X);
                evt.AddActorPara(Common.Packets.GameServer.PacketParameter.Y, pc.Y);
                evt.AddActorPara(Common.Packets.GameServer.PacketParameter.Z, pc.Z);
                evt.AddActorPara(Common.Packets.GameServer.PacketParameter.Dir, pc.Dir);
                PartyMemberUpdate(party, evt, pc);
            }
        }

        public void PartyMemberDirUpdate(Common.Party.Party party, ActorPC pc)
        {
            if (party != null)
            {
                UpdateEvent evt = new UpdateEvent();
                evt.AddActorPara(Common.Packets.GameServer.PacketParameter.Dir, pc.Dir);
                PartyMemberUpdate(party, evt, pc);
            }
        }

        private void PartyMemberUpdate(Common.Party.Party party, UpdateEvent evt, ActorPC pc)
        {
            foreach (ActorPC i in party.Members)
            {
                if (i != pc)
                {
                    GameSession client = i.Client();
                    client?.SendPartyMemberUpdate(pc, evt);
                }
            }
        }

        public void PartyChangeLootMode(Common.Party.Party party, PartyLootMode mode, byte rank, ulong actorID)
        {
            if (party != null)
            {
                party.LootMode = mode;
                party.AuctionItemRank = rank;
                party.SpecifiedLooter = null;
                if (actorID != 0)
                {
                    foreach (ActorPC i in party.Members)
                    {
                        if (i.ActorID == actorID)
                        {
                            party.SpecifiedLooter = i;
                            break;
                        }
                    }
                }

                foreach (ActorPC i in party.Members)
                {
                    GameSession client = i.Client();
                    client?.SendPartyChangeLootMode(mode, rank, actorID);
                }
            }
        }

        public void PartyChangeLeader(Common.Party.Party party, ActorPC newLeader)
        {
            if (party?.Members.Contains(newLeader) == true)
            {
                party.Leader = newLeader;
                ActorPC backup = party.Members[0];
                party.Members.Remove(newLeader);
                party.Members.Insert(0, newLeader);
                foreach (ActorPC i in party.Members)
                {
                    GameSession client = i.Client();
                    client?.SendPartyLeaderChange();
                }
            }
        }

        public ActorPC GetPartyLootOwner(ActorPC pc)
        {
            if (pc.Party != null)
            {
                switch(pc.Party.LootMode)
                {
                    case PartyLootMode.Ordered:
                        Interlocked.CompareExchange(ref pc.Party.PartyOrderIndex, 0, pc.Party.Members.Count);
                        ActorPC res = pc.Party.Members[pc.Party.PartyOrderIndex];
                        Interlocked.Increment(ref pc.Party.PartyOrderIndex);
                        Interlocked.CompareExchange(ref pc.Party.PartyOrderIndex, 0, pc.Party.Members.Count);
                        return res;
                    case PartyLootMode.Specified:
                        return pc.Party.SpecifiedLooter;
                    case PartyLootMode.Free:
                    default:
                        return pc;
                }
            }
            else
            {
                return pc;
            }
        }
    }
}
