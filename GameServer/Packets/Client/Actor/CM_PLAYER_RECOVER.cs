
using SmartEngine.Network;
using SagaBNS.Common.Packets;
using SagaBNS.GameServer.Network.Client;

namespace SagaBNS.GameServer.Packets.Client
{
    public class CM_PLAYER_RECOVER : Packet<GamePacketOpcode>
    {
        public CM_PLAYER_RECOVER()
        {
            ID = GamePacketOpcode.CM_PLAYER_RECOVER;
        }

        public override Packet<GamePacketOpcode> New()
        {
            return new CM_PLAYER_RECOVER();
        }

        public override void OnProcess(Session<GamePacketOpcode> client)
        {
            ((GameSession)client).OnPlayerRecover(this);
        }
    }
}
