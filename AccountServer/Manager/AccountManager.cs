using System.Linq;
using System.Collections.Concurrent;
using SmartEngine.Network;

using SagaBNS.Common.Account;
using SagaBNS.Common.Packets.AccountServer;
using SagaBNS.AccountServer.Cache;
using SagaBNS.AccountServer.Network.Client;

namespace SagaBNS.AccountServer.Manager
{
    public class AccountManager : Singleton<AccountManager>
    {
        private readonly ConcurrentDictionary<uint, Account> accountsOnline = new ConcurrentDictionary<uint, Account>();

        public Account this[uint account_id]
        {
            get
            {
                return AccountCache.Instance[account_id];
            }
        }

        public AccountLoginResult AccountLogin(uint accountID)
        {
            Account account = null;
            if (accountsOnline.ContainsKey(accountID))
            {
                return AccountLoginResult.ALREADY_LOG_IN;
            }

            account = AccountCache.Instance[accountID];
            if (account == null)
            {
                return AccountLoginResult.NO_SUCH_ACCOUNT;
            }

            accountsOnline[accountID] = account;
            return AccountLoginResult.OK;
        }

        public AccountLoginResult AccountLogout(uint accountID)
        {
            if (accountsOnline.ContainsKey(accountID))
            {
                accountsOnline.TryRemove(accountID, out Account acc);
                AccountCache.Instance.Save(accountID, acc);
                foreach (AccountSession i in AccountClientManager.Instance.Clients.ToArray())
                {
                    if (i.Authenticated)
                    {
                        SM_ACCOUNT_LOGOUT_NOTIFY p = new SM_ACCOUNT_LOGOUT_NOTIFY()
                        {
                            AccountID = accountID
                        };
                        i.Network.SendPacket(p);
                    }
                }
                return AccountLoginResult.OK;
            }
            else
            {
                return AccountLoginResult.INVALID_PASSWORD;
            }
        }

        public bool IsOnline(uint account_id)
        {
            return accountsOnline.ContainsKey(account_id);
        }

        public void AccountSave(Account account)
        {
            if (accountsOnline.ContainsKey(account.AccountID))
            {
                accountsOnline[account.AccountID] = account;
            }

            AccountCache.Instance.Save(account.AccountID, account);
        }
    }
}
