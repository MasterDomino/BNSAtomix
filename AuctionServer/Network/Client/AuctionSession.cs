using SmartEngine.Network;
using SagaBNS.Common.Packets;

namespace SagaBNS.AuctionServer.Network.Client
{
    public partial class AuctionSession : Session<AuctionPacketOpcode>
    {
        public override string ToString()
        {
            return Network.Socket.RemoteEndPoint.ToString();
        }
    }
}
