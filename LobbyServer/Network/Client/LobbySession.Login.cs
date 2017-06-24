using System;
using SmartEngine.Core;
using SmartEngine.Network;
using SagaBNS.Common;
using SagaBNS.Common.Packets;
using SagaBNS.Common.Account;
using SagaBNS.LobbyServer.Manager;
using SagaBNS.LobbyServer.Packets.Client;
using SagaBNS.LobbyServer.Network.AccountServer;

namespace SagaBNS.LobbyServer.Network.Client
{
    public partial class LobbySession : Session<LobbyPacketOpcode>
    {
        private Guid accountID;
        private Account acc;
        public void OnLoginAuth(CM_AUTH p)
        {
            accountID = p.AccountID;
            Logger.Log.Info(string.Format("Account:{0} is loging in", p.AccountID));
            AccountSession.Instance.RequestAccountInfo(accountID.ToUInt(), this);
        }

        public void OnGotAccountInfo(Common.Packets.AccountServer.AccountLoginResult result, Account acc)
        {
            Logger.Log.Info(string.Format("Load Account info for {0}({1}):{2}", accountID, accountID.ToUInt(), result));
            if (result == Common.Packets.AccountServer.AccountLoginResult.OK)
            {
                this.acc = acc;
                SM_AUTH_RESULT p1 = new SM_AUTH_RESULT();
                Network.SendPacket(p1);
            }
        }

        public void OnWorldListRequest(CM_SERVER_LIST p)
        {
            SM_SERVER_LIST p1 = new SM_SERVER_LIST()
            {
                Worlds = WorldManager.Instance.Worlds.Values
            };
            Network.SendPacket(p1);
        }
    }
}
