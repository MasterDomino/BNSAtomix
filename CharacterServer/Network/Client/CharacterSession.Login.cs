using System.Threading;
using SmartEngine.Core;
using SmartEngine.Network;
using SagaBNS.Common.Packets;
using SagaBNS.Common.Packets.CharacterServer;

namespace SagaBNS.CharacterServer.Network.Client
{
    public partial class CharacterSession : Session<CharacterPacketOpcode>
    {
        private static int session = 1;
        private int curSession;
        public void OnLoginRequest(Packets.Client.CM_LOGIN_REQUEST p)
        {
            SM_LOGIN_RESULT p2 = new SM_LOGIN_RESULT();
            if (p.Password == Configuration.Instance.Password)
            {
                Logger.Log.Info(string.Format("Server({0}) successfully authenticated", Network.Socket.RemoteEndPoint.ToString()));
                p2.Result = SM_LOGIN_RESULT.Results.OK;
                curSession = session;
                Interlocked.Increment(ref session);
            }
            else
            {
                Logger.Log.Info(string.Format("Server({0}) failed authentication with password:{1}", Network.Socket.RemoteEndPoint.ToString(), p.Password));
                p2.Result = SM_LOGIN_RESULT.Results.WRONG_PASSWORD;
            }
            Network.SendPacket(p2);
        }
    }
}
