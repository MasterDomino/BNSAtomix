using System;
using System.Linq;

using SmartEngine.Core;
using SmartEngine.Network;
using SmartEngine.Network.Utils;
using SagaBNS.Common.Actors;
using SagaBNS.Common.Packets;
using SagaBNS.Common.Account;
using SagaBNS.GameServer.Map;
using SagaBNS.GameServer.Packets.Client;
using SagaBNS.GameServer.Network.AccountServer;
using SagaBNS.GameServer.Network.CharacterServer;
using SagaBNS.GameServer.Services;

namespace SagaBNS.GameServer.Network.Client
{
    public partial class GameSession : Session<GamePacketOpcode>
    {
        private static ulong nextSessionID = 0x1000000000001;
        private Account account;
        private readonly string username;
        private Guid accountGUID, slotGUID, tokenGUID;
        private ActorPC chara;
        private DateTime loginTime;
        private Map.Map map;
        private BroadcastService broadcastService;

        public bool Authenticated { get; set; }
        public ulong SessionID { get; set; }

        public BroadcastService BroadcastService { get { return broadcastService; } }

        public Account Account { get { return account; } }

        public Map.Map Map { get { return map; } }

        public ActorPC Character { get { return chara; } }

        public void CharacterSelect()
        {
            SM_CHARACTER_SELECT p1 = new SM_CHARACTER_SELECT()
            {
                Token = Conversions.HexStr2Bytes("0000000000000000000000000000")
            };
            Network.SendPacket(p1);
        }

        public void OnLoginAuth(Packets.Client.CM_LOGIN_AUTH p)
        {
            CharacterSession.Instance.RequestCharInfo((uint)p.CharID, this);
            //Logger.Log.Info(username + " is trying to login");
        }

        public void OnActorInfo(ActorPC pc)
        {
            if (pc != null)
            {
                chara = pc;
                AccountSession.Instance.RequestAccountInfo(pc.AccountID, this);
                return;
            }
            Logger.Log.Info("Login result: No such character");
            Network.Disconnect();
        }

        private void SendAuthFinish()
        {
            SM_LOGIN_AUTH_RESULT p = new SM_LOGIN_AUTH_RESULT();
            Network.SendPacket(p);
        }

        public void OnAccountInfo(Common.Packets.AccountServer.AccountLoginResult result, Account acc)
        {
            if (acc?.UserName != null && acc.UserName != string.Empty)
            {
                Logger.Log.Info(string.Format("Login result for player {0}:{1}", username, result));
                if (result == Common.Packets.AccountServer.AccountLoginResult.OK)
                {
                    foreach (GameSession i in Manager.GameClientManager.Instance.Clients.ToArray())
                    {
                        if (i.account != null && i.account.AccountID == acc.AccountID)
                        {
                            i.Network.Disconnect();
                            AccountSession.Instance.RequestAccountInfo(username, this);
                            return;
                        }
                    }
                    account = acc;
                    OnGotChar(chara);
                    return;
                }
                else
                {
                    Logger.Log.Info("Login result for " + username + ": No such account");
                    Network.Disconnect();
                }
            }
            Logger.Log.Info("Login result: No such account");
            Network.Disconnect();
        }

        public void OnGotChar(ActorPC pc)
        {
            Logger.Log.Info(string.Format("Finishing login for player {0}", username));
            if (pc.Offline && pc.PartyID > 0)
            {
                if (Party.PartyManager.Instance.Parties.TryGetValue(pc.PartyID, out Common.Party.Party party))
                {
                    chara = Party.PartyManager.Instance.PartyGetMember(party, pc.CharID);
                    if (chara != null)
                    {
                        if (chara.Tasks.TryGetValue("PartyOfflineTask", out SmartEngine.Network.Tasks.Task task))
                        {
                            task.Deactivate();
                        }

                        SendAuthFinish();
                        return;
                    }
                }
            }
            chara = pc;
            Logger.Log.Info("Login result for " + username + ": OK");
            Logger.Log.Info("Player " + account.UserName + " selected character:" + pc.Name);
            CharacterSession.Instance.GetInventory(chara.CharID, this);

            return;
        }

        public void OnLoginStart(CM_LOGIN_START p)
        {
            loginTime = DateTime.Now;

            map = MapManager.Instance.GetMap(chara.MapID, chara.CharID, chara.PartyID);
            if (chara.Party != null && chara.Offline)
            {
                SessionID = chara.ActorID;
            }
            else
            {
                SessionID = nextSessionID++;
            }

            chara.EventHandler = new ActorEventHandlers.PCEventHandler(this);

            broadcastService = new Services.BroadcastService(this);
            broadcastService.Activate();
            map.RegisterActor(chara, SessionID);
        }

        public void OnLoginChatAuth(CM_LOGIN_CHAT_AUTH p)
        {
            SM_LOGIN_CHAT_AUTH p1 = new SM_LOGIN_CHAT_AUTH()
            {
                Server = chara.WorldID,
                Token = Conversions.HexStr2Bytes("4165E727FDE5F82DDA7BB8977CF8A3FBBD5C8A61EF90ED81EDAA98B6ED180740131ABD2EDAEBAB6EDE7B6B2579F3BCF3CDB257E17165D38171FD7632C1F7CC31")
            };
            Network.SendPacket(p1);
        }

        public void OnAuctionHandshake(CM_LOGIN_AUCTION_START p)
        {
            if (Configuration.Instance.AuctionEnabled)
            {
                SM_LOGIN_AUCTION_START r = new SM_LOGIN_AUCTION_START()
                {
                    IP = Configuration.Instance.AuctionServer,
                    Port = 11010,
                    Token = Conversions.HexStr2Bytes("6D2BB06122A6ACADC0A80167")
                };
                Network.SendPacket(r);
            }
        }

        public void OnLoginUnknown1(CM_LOGIN_UNKNOWN1 p)
        {
            SM_LOGIN_UNKNOWN1 p1 = new SM_LOGIN_UNKNOWN1()
            {
                Unknown1 = 0x1ED16,
                Unknown2 = 0x4A2C,
                Unknown3 = 0x4056
            };
            Network.SendPacket(p1);
        }

        public void OnLoginUnknown2(CM_LOGIN_UNKNOWN2 p)
        {
            SM_LOGIN_UNKNOWN2 p1 = new SM_LOGIN_UNKNOWN2()
            {
                Unknown1 = 0x2C4
            };
            Network.SendPacket(p1);
        }

        public void SendLoginInit()
        {
            Authenticated = true;
            SM_LOGIN_INIT p1 = new SM_LOGIN_INIT();
            PC.Status.CalcStatus(chara,false);
            p1.Player = chara;
            Network.SendPacket(p1);
            SendQuestList();
            SendChangeMap();
        }
    }
}
