using System.Threading;

using SmartEngine.Core;
using SmartEngine.Network;

using SagaBNS.Common.Packets;
using SagaBNS.Common.Packets.AccountServer;
namespace SagaBNS.AccountServer.Network.Client
{
    public partial class AccountSession : Session<AccountPacketOpcode>
    {
        public bool Authenticated { get; set; }
        private static int session = 1;
        private int curSession;
        public void OnLoginRequest(Packets.Client.CM_LOGIN_REQUEST p)
        {
            SM_LOGIN_RESULT p2 = new SM_LOGIN_RESULT();
            if (p.Password == Configuration.Instance.Password)
            {
                Logger.Log.Info(string.Format("Server({0}) successfully authenticated", Network.Socket.RemoteEndPoint.ToString()));
                p2.Result = SM_LOGIN_RESULT.Results.OK;
                Authenticated = true;
                curSession = session;
                Interlocked.Increment(ref session);
            }
            else
            {
                Logger.Log.Info(string.Format("Server({0}) failed authentication with password:{1}", Network.Socket.RemoteEndPoint.ToString(), p.Password));
                p2.Result = SM_LOGIN_RESULT.Results.WRONG_PASSWORD;
                Authenticated = false;
            }
            Network.SendPacket(p2);
        }
    }
}
