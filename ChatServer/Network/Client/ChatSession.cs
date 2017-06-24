
using SmartEngine.Core;
using SmartEngine.Network;

using SagaBNS.ChatServer.Manager;
using SagaBNS.ChatServer.Packets;

namespace SagaBNS.ChatServer.Network.Client
{
    public partial class ChatSession : Session<BNSChatOpcodes>
    {
        public override void OnConnect()
        {
            base.OnConnect();
        }

        public override void OnDisconnect()
        {
            if (Authenticated)
            {
                Logger.Log.Info(ToString() + " is logging out");
                ChatClientManager.Instance.Logout(this);
            }
        }

        public override string ToString()
        {
            return string.Format("Player:{0}({1}|0x{2:X})", Name, Email, ActorID);
        }
    }
}
