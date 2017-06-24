using SmartEngine.Network;
using SagaBNS.Common.Packets;

namespace SagaBNS.LobbyServer.Network.Client
{
    public partial class LobbySession : Session<LobbyPacketOpcode>
    {
        public override string ToString()
        {
            return Network.Socket.RemoteEndPoint.ToString();
        }
    }
}
