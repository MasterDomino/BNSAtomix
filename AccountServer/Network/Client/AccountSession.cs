
using SmartEngine.Network;

using SagaBNS.Common.Packets;
namespace SagaBNS.AccountServer.Network.Client
{
    public partial class AccountSession : Session<AccountPacketOpcode>
    {
        public override void OnConnect()
        {

        }

        public override void OnDisconnect()
        {

        }

        public override string ToString()
        {
            return "Session " + curSession.ToString();
        }
    }
}
