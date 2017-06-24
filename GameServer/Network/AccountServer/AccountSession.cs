using System.Linq;
using SagaBNS.Common.Packets.AccountServer;
using SagaBNS.Common.Network;
using SagaBNS.GameServer.Network.Client;

namespace SagaBNS.GameServer.Network.AccountServer
{
    public class AccountSession : AccountSession<GameSession>
    {
        private static AccountSession instance = new AccountSession();

        /// <summary>
        /// 单实例
        /// </summary>
        public static AccountSession Instance { get { return instance; } set { instance = value; } }

        public AccountSession()
        {
            Host = Configuration.Instance.AccountHost;
            Port = Configuration.Instance.AccountPort;
            AccountPassword = Configuration.Instance.AccountPassword;
        }

        protected override void OnAccountLoginResult(GameSession client, AccountLoginResult result)
        {

        }

        protected override void OnAccountLogoutNotify(uint accountID)
        {
            foreach (GameSession i in Manager.GameClientManager.Instance.Clients.ToArray())
            {
                if (i?.Account != null && i.Account.AccountID == accountID)
                {
                    i.Network.Disconnect();
                }
            }
        }

        protected override void OnAccountInfo(GameSession client, AccountLoginResult result, Common.Account.Account acc)
        {
            client.OnAccountInfo(result, acc);
        }
    }
}
