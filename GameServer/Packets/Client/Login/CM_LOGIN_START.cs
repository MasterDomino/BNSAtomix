
using SmartEngine.Network;
using SagaBNS.Common.Packets;
using SagaBNS.GameServer.Network.Client;

namespace SagaBNS.GameServer.Packets.Client
{
    public class CM_LOGIN_START : Packet<GamePacketOpcode>
    {
        public CM_LOGIN_START()
        {
            ID = GamePacketOpcode.CM_LOGIN_START;
        }

        public override Packet<GamePacketOpcode> New()
        {
            return new CM_LOGIN_START();
        }

        public override void OnProcess(Session<GamePacketOpcode> client)
        {
            ((GameSession)client).OnLoginStart(this);
        }
    }
}
