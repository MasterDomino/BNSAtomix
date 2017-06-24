
using SmartEngine.Core;
using SmartEngine.Network;

using SagaBNS.Common.Packets;
using SagaBNS.Common.Packets.AccountServer;
using SagaBNS.Common.Account;
using SagaBNS.AccountServer.Cache;
using SagaBNS.AccountServer.Database;
using SagaBNS.AccountServer.Manager;
namespace SagaBNS.AccountServer.Network.Client
{
    public partial class AccountSession : Session<AccountPacketOpcode>
    {
        public void OnAccountInfoRequest(CM_ACCOUNT_INFO_REQUEST p)
        {
            SM_ACCOUNT_INFO p1 = new SM_ACCOUNT_INFO()
            {
                SessionID = p.SessionID
            };
            Logger.Log.Info(string.Format(this + ":Player:{0} is trying to login", p.Username));
            AccountLoginResult res = AccountDB.Instance.GetAccountID(p.Username, out uint accountID);
            switch (res)
            {
                case AccountLoginResult.OK:
                    p1.Result = AccountLoginResult.OK;
                    p1.Account = AccountCache.Instance[accountID];
                    break;
                case AccountLoginResult.NO_SUCH_ACCOUNT:
                    p1.Result = AccountLoginResult.NO_SUCH_ACCOUNT;
                    break;
                case AccountLoginResult.DB_ERROR:
                    p1.Result = AccountLoginResult.DB_ERROR;
                    break;
            }
            Logger.Log.Info(string.Format("Login result:{0}", res));
            Network.SendPacket(p1);
        }

        public void OnAccountInfoRequestId(CM_ACCOUNT_INFO_REQUEST_ID p)
        {
            SM_ACCOUNT_INFO p1 = new SM_ACCOUNT_INFO()
            {
                SessionID = p.SessionID
            };
            Account acc = AccountCache.Instance[p.AccountID];
            if (acc != null)
            {
                p1.Result = AccountLoginResult.OK;
                p1.Account = acc;
                Logger.Log.Info(string.Format("Loading Player:{0}'s account info", acc.UserName));
            }
            else
            {
                p1.Result = AccountLoginResult.NO_SUCH_ACCOUNT;
            }

            Network.SendPacket(p1);
        }

        public void OnAccountLogin(CM_ACCOUNT_LOGIN p)
        {
            Logger.Log.Info(string.Format(this + ":Player:AccountID {0} is trying to login", p.AccountID));
            SM_ACCOUNT_LOGIN_RESULT p1 = new SM_ACCOUNT_LOGIN_RESULT()
            {
                SessionID = p.SessionID,
                Result = AccountManager.Instance.AccountLogin(p.AccountID)
            };
            Network.SendPacket(p1);
        }

        public void OnAccountLogout(CM_ACCOUNT_LOGOUT p)
        {
           AccountManager.Instance.AccountLogout(p.AccountID);
        }

        public void OnAccountSave(CM_ACCOUNT_SAVE p)
        {
            AccountManager.Instance.AccountSave(p.Account);
        }
    }
}
