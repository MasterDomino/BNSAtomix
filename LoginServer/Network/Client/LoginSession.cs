
using SmartEngine.Core;
using SmartEngine.Network;
using SagaBNS.Common.Packets;
using SagaBNS.LoginServer.Network.AccountServer;

namespace SagaBNS.LoginServer.Network.Client
{
    public partial class LoginSession : Session<LoginPacketOpcode>
    {
        public override void OnConnect()
        {
            base.OnConnect();
        }

        public override void OnDisconnect()
        {
            if (account != null)
            {
                if (lastLoginRes == Common.Packets.AccountServer.AccountLoginResult.OK)
                {
                    AccountSession.Instance.AccountSave(account, this);
                    AccountSession.Instance.AccountLogout(account.AccountID, this);
                }
                Logger.Log.Info("Player " + account.UserName + " log out.");
            }
        }
    }
}
