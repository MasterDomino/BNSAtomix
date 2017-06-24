using System;
using SmartEngine.Core;
using SmartEngine.Network;
using SagaBNS.Common;
using SagaBNS.Common.Packets;
using SagaBNS.Common.Account;
using SagaBNS.AuctionServer.Packets.Client;

namespace SagaBNS.AuctionServer.Network.Client
{
    public partial class AuctionSession : Session<AuctionPacketOpcode>
    {
        private Guid accountID;

        private Account acc;

        public void OnLoginAuth(CM_AUTH p)
        {
            accountID = p.AccountID;
            Logger.Log.Info($"Account:{p.AccountID} is loging in");
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
    }
}
