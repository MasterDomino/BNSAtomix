
using SmartEngine.Network;

using SagaBNS.Common.Packets;
using SagaBNS.AccountServer.Network.Client;

namespace SagaBNS.AccountServer.Manager
{
    public class AccountClientManager : ClientManager<AccountPacketOpcode>
    {
        private static AccountClientManager instance;

        public static AccountClientManager Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new AccountClientManager();
                }

                return instance;
            }
        }

        public AccountClientManager()
        {
            Encrypt = false;

            RegisterPacketHandler(AccountPacketOpcode.CM_LOGIN_REQUEST, new Packets.Client.CM_LOGIN_REQUEST());
            RegisterPacketHandler(AccountPacketOpcode.CM_ACCOUNT_INFO_REQUEST, new Packets.Client.CM_ACCOUNT_INFO_REQUEST());
            RegisterPacketHandler(AccountPacketOpcode.CM_ACCOUNT_INFO_REQUEST_ID, new Packets.Client.CM_ACCOUNT_INFO_REQUEST_ID());
            RegisterPacketHandler(AccountPacketOpcode.CM_ACCOUNT_LOGIN, new Packets.Client.CM_ACCOUNT_LOGIN());
            RegisterPacketHandler(AccountPacketOpcode.CM_ACCOUNT_LOGOUT, new Packets.Client.CM_ACCOUNT_LOGOUT());
            RegisterPacketHandler(AccountPacketOpcode.CM_ACCOUNT_SAVE, new Packets.Client.CM_ACCOUNT_SAVE());
        }

        protected override Session<AccountPacketOpcode> NewSession()
        {
            return new AccountSession();
        }
    }
}
