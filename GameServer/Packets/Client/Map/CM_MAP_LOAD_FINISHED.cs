
using SmartEngine.Network;
using SagaBNS.Common.Packets;
using SagaBNS.GameServer.Network.Client;

namespace SagaBNS.GameServer.Packets.Client
{
    public class CM_MAP_LOAD_FINISHED : Packet<GamePacketOpcode>
    {
        public CM_MAP_LOAD_FINISHED()
        {
            ID = GamePacketOpcode.CM_MAP_LOAD_FINISHED;
        }

        public override Packet<GamePacketOpcode> New()
        {
            return new CM_MAP_LOAD_FINISHED();
        }

        public override void OnProcess(Session<GamePacketOpcode> client)
        {
            ((GameSession)client).OnMapLoadFinished(this);
        }
    }
}
