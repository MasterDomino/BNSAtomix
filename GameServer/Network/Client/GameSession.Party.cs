using System.Collections.Generic;
using System.Collections.Concurrent;
using SmartEngine.Network;
using SagaBNS.Common.Actors;
using SagaBNS.Common.Party;
using SagaBNS.Common.Packets;
using SagaBNS.GameServer.Map;
using SagaBNS.GameServer.Party;
using SagaBNS.GameServer.Packets.Client;

namespace SagaBNS.GameServer.Network.Client
{
    public partial class GameSession : Session<GamePacketOpcode>
    {
        private readonly ConcurrentDictionary<ActorPC, ulong> partyInviteTable = new ConcurrentDictionary<ActorPC, ulong>();
        public void OnPartyInvite(CM_PARTY_INVITE p)
        {
            if (map.GetActor(p.ActorID) is ActorPC partyTarget)
            {
                if (!partyInviteTable.ContainsKey(partyTarget))
                {
                    if (partyTarget != null)
                    {
                        if (partyTarget.Party == null)
                        {
                            ulong newPartyID;
                            if (chara.Party != null)
                            {
                                newPartyID = chara.Party.PartyID;
                            }
                            else
                            {
                                newPartyID = PartyManager.Instance.GetNewPartyID();
                            }

                            partyInviteTable[partyTarget] = newPartyID;
                            SM_PARTY_SEND_INVITATION p1 = new SM_PARTY_SEND_INVITATION()
                            {
                                PartyID = newPartyID,
                                ActorID = chara.ActorID,
                                Name = chara.Name
                            };
                            GameSession target = partyTarget.Client();
                            target.partyInviteTable[chara] = newPartyID;
                            target.Network.SendPacket(p1);
                        }
                        else
                        {
                            SM_PARTY_INVITE_FAILED p1 = new SM_PARTY_INVITE_FAILED()
                            {
                                Reason = SM_PARTY_INVITE_FAILED.Reasons.ALREADY_SENT//TODO: already in party
                            };
                            Network.SendPacket(p1);
                        }
                    }
                }
                else
                {
                    SM_PARTY_INVITE_FAILED p1 = new SM_PARTY_INVITE_FAILED()
                    {
                        Reason = SM_PARTY_INVITE_FAILED.Reasons.ALREADY_SENT
                    };
                    Network.SendPacket(p1);
                }
            }
        }

        public void OnPartyReplyInvitation(CM_PARTY_REPLY_INVITATION p)
        {
            foreach (KeyValuePair<ActorPC, ulong> i in partyInviteTable)
            {
                if (i.Value == p.PartyID)
                {
                    Common.Party.Party newParty;
                    ActorPC partyTarget = i.Key;
                    if (chara.Party == null)
                    {
                        if (partyTarget.Party != null)
                        {
                            newParty = partyTarget.Party;
                            if (newParty.Members.Count < 5)
                            {
                                newParty.Members.Add(chara);
                                chara.Party = newParty;
                                chara.PartyID = i.Value;
                            }
                            else
                            {
                                //TODO: party full
                                partyInviteTable.TryRemove(i.Key, out ulong removed);
                                GameSession target = partyTarget.Client();
                                target.partyInviteTable.TryRemove(chara, out removed);
                            }
                        }
                        else
                        {
                            newParty = PartyManager.Instance.NewParty(i.Value, partyTarget, chara);
                        }

                        foreach (ActorPC pc in newParty.Members)
                        {
                            GameSession client = pc.Client();
                            client?.SendPartyInfo();
                        }
                    }
                    {
                        partyInviteTable.TryRemove(i.Key, out ulong removed);
                        GameSession target = partyTarget.Client();
                        target.partyInviteTable.TryRemove(chara, out removed);
                    }
                }
            }
        }

        public void OnPartyReplyInvitationDenied(CM_PARTY_REPLY_INVITATION_DENIED p)
        {
            foreach (KeyValuePair<ActorPC, ulong> i in partyInviteTable)
            {
                if (i.Value == p.PartyID)
                {
                    ActorPC partyTarget = i.Key;
                    if (partyTarget != null)
                    {
                        partyInviteTable.TryRemove(i.Key, out ulong removed);
                        GameSession target = partyTarget.Client();
                        target.partyInviteTable.TryRemove(chara, out removed);
                    }
                }
            }
        }

        public void OnPartyGiveLeadership(CM_PARTY_GIVE_LEADERSHIP p)
        {
            ulong actorID = p.ActorID;
            if (chara.Party != null)
            {
                foreach (ActorPC pc in chara.Party.Members)
                {
                    if (pc.ActorID == actorID)
                    {
                        PartyManager.Instance.PartyChangeLeader(pc.Party, pc);
                        return;
                    }
                }
            }
        }

        public void OnPartyQuit(CM_PARTY_QUIT p)
        {
            if (chara.Party != null)
            {
                Party.PartyManager.Instance.PartyMemberQuit(chara.Party, chara);
            }
        }

        public void OnPartyKick(CM_PARTY_KICK p)
        {
            if (chara.Party != null && chara.Party.Leader == chara)
            {
                PartyManager.Instance.PartyMemberKick(chara.Party, p.Name);
            }
        }

        public void OnPartyChangeLootMode(CM_PARTY_CHANGE_LOOT_MODE p)
        {
            if (chara.Party != null && chara.Party.Leader == chara)
            {
                PartyManager.Instance.PartyChangeLootMode(chara.Party, p.LootMode, p.ItemRank, p.ActorID);
            }
        }

        public void SendPartyChangeLootMode(PartyLootMode mode, byte rank, ulong actorID)
        {
            SM_PARTY_LOOT_MODE_CHANGED p1 = new SM_PARTY_LOOT_MODE_CHANGED()
            {
                LootMode = mode,
                ItemRank = rank,
                ActorID = actorID
            };
            Network.SendPacket(p1);
        }

        public void SendPartyMemberUpdate(ActorPC member,UpdateEvent evt)
        {
            SM_PARTY_MEMBER_UPDATE p = new SM_PARTY_MEMBER_UPDATE()
            {
                ActorID = member.ActorID,
                Updates = evt
            };
            Network.SendPacket(p);
        }

        public void SendPartyLeaderChange()
        {
            if (chara.Party != null)
            {
                SM_PARTY_LEADER_CHANGE p = new SM_PARTY_LEADER_CHANGE()
                {
                    ActorID = chara.Party.Leader.ActorID
                };
                Network.SendPacket(p);
            }
        }

        public void SendPartyInfo()
        {
            SM_PARTY_INFO p1 = new SM_PARTY_INFO()
            {
                PartyID = chara.PartyID,
                Members = chara.Party.Members
            };
            Network.SendPacket(p1);
        }

        public void SendPartyMemberOnlineStatusChange(ActorPC pc)
        {
            SM_PARTY_MEMBER_ONLINE_STATUS p = new SM_PARTY_MEMBER_ONLINE_STATUS()
            {
                ActorID = pc.ActorID,
                Status = pc.Offline ? SM_PARTY_MEMBER_ONLINE_STATUS.Statuses.Offline : SM_PARTY_MEMBER_ONLINE_STATUS.Statuses.Online
            };
            Network.SendPacket(p);
        }

        public void SendPartyMemberLeave(string name)
        {
            SM_PARTY_MEMBER_LEAVE p = new SM_PARTY_MEMBER_LEAVE()
            {
                MemberName = name
            };
            Network.SendPacket(p);
        }
    }
}
