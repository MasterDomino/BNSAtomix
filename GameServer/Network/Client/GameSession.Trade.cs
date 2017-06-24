using SmartEngine.Network;
using SagaBNS.Common.Packets;
using SagaBNS.GameServer.Packets.Client;

namespace SagaBNS.GameServer.Network.Client
{
    public partial class GameSession : Session<GamePacketOpcode>
    {
        public void OnTradeRequest(CM_TRADE_INIT p)
        {
        }
    }
}