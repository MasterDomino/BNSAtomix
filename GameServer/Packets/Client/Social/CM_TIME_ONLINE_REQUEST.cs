
using SmartEngine.Network;
using SagaBNS.Common.Packets;
using SagaBNS.GameServer.Network.Client;

namespace SagaBNS.GameServer.Packets.Client
{
    public class CM_TIME_ONLINE_REQUEST : Packet<GamePacketOpcode>
    {
        public CM_TIME_ONLINE_REQUEST()
        {
            ID = GamePacketOpcode.CM_TIME_ONLINE_REQUEST;
        }

        public override Packet<GamePacketOpcode> New()
        {
            return new CM_TIME_ONLINE_REQUEST();
        }

        public override void OnProcess(Session<GamePacketOpcode> client)
        {
            ((GameSession)client).OnGetTime();
        }
    }
}
