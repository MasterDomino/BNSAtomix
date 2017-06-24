using SagaBNS.Common.Network;
using SagaBNS.LobbyServer.Network.Client;

namespace SagaBNS.LobbyServer.Network.AccountServer
{
    public partial class AccountSession : AccountSession<LobbySession>
    {
        private static readonly AccountSession instance = new AccountSession();
        public static AccountSession Instance { get { return instance; } }

        public AccountSession()
        {
            Host = Configuration.Instance.AccountHost;
            Port = Configuration.Instance.AccountPort;
            AccountPassword = Configuration.Instance.AccountPassword;
        }

        protected override void OnAccountLoginResult(LobbySession client, Common.Packets.AccountServer.AccountLoginResult result)
        {

        }

        protected override void OnAccountLogoutNotify(uint accountID)
        {

        }

        protected override void OnAccountInfo(LobbySession client, Common.Packets.AccountServer.AccountLoginResult result, Common.Account.Account acc)
        {
            client.OnGotAccountInfo(result, acc);
        }
    }
}
