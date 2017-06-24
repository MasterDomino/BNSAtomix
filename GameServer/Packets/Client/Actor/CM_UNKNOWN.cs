
using SmartEngine.Network;
using SagaBNS.Common.Packets;
using SagaBNS.GameServer.Network.Client;

namespace SagaBNS.GameServer.Packets.Client
{
    public class CM_UNKNOWN : Packet<GamePacketOpcode>
    {
        public CM_UNKNOWN()
        {
            ID = GamePacketOpcode.CM_UNKNOWN;
        }

        public override Packet<GamePacketOpcode> New()
        {
            return new CM_UNKNOWN();
        }

        public override void OnProcess(Session<GamePacketOpcode> client)
        {
            ((GameSession)client).OnUnknown(this);
        }
    }
}
