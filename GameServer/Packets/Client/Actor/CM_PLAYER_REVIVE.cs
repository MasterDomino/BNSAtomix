
using SmartEngine.Network;
using SagaBNS.Common.Packets;
using SagaBNS.GameServer.Network.Client;

namespace SagaBNS.GameServer.Packets.Client
{
    public class CM_PLAYER_REVIVE : Packet<GamePacketOpcode>
    {
        public CM_PLAYER_REVIVE()
        {
            ID = GamePacketOpcode.CM_PLAYER_REVIVE;
        }

        public override Packet<GamePacketOpcode> New()
        {
            return new CM_PLAYER_REVIVE();
        }

        public override void OnProcess(Session<GamePacketOpcode> client)
        {
            ((GameSession)client).OnPlayerRevive(this);
        }
    }
}
