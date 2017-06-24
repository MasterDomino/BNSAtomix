
using SmartEngine.Network;
using SagaBNS.Common.Packets;
using SagaBNS.GameServer.Network.Client;

namespace SagaBNS.GameServer.Packets.Client
{
    public class CM_TRADE_INIT : Packet<GamePacketOpcode>
    {
        public CM_TRADE_INIT()
        {
            ID = GamePacketOpcode.CM_TRADE_INIT;
        }

        public override Packet<GamePacketOpcode> New()
        {
            return new CM_TRADE_INIT();
        }

        public ulong ActorID
        {
            get
            {
                return GetULong(8);
            }
        }

        public override void OnProcess(Session<GamePacketOpcode> client)
        {
            ((GameSession)client).OnTradeRequest(this);
        }
    }
}
