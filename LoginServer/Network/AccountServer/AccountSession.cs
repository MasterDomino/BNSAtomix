using System.Linq;
using SagaBNS.Common.Account;
using SagaBNS.Common.Packets.AccountServer;
using SagaBNS.Common.Network;
using SagaBNS.LoginServer.Network.Client;

namespace SagaBNS.LoginServer.Network.AccountServer
{
    public class AccountSession : AccountSession<LoginSession>
    {
        private static readonly AccountSession instance = new AccountSession();

        public static AccountSession Instance { get { return instance; } }
        public AccountSession()
        {
            Host = Configuration.Instance.AccountHost;
            Port = Configuration.Instance.AccountPort;
            AccountPassword = Configuration.Instance.AccountPassword;
        }

        protected override void OnAccountInfo(LoginSession client, AccountLoginResult result, Account acc)
        {
            client.OnAccountInfo(result, acc);
        }

        protected override void OnAccountLoginResult(LoginSession client, AccountLoginResult result)
        {
            client.OnAccountLoginResult(result);
        }

        protected override void OnAccountLogoutNotify(uint accountID)
        {
            foreach (LoginSession i in Manager.LoginClientManager.Instance.Clients.ToArray())
            {
                if (i.Account != null && i.Account.AccountID == accountID)
                {
                    i.Network.Disconnect();
                }
            }
        }
    }
}
