
using SmartEngine.Network;
using SagaBNS.Common.Packets;
using SagaBNS.GameServer.Network.Client;

namespace SagaBNS.GameServer.Packets.Client
{
    public class CM_LOGIN_UNKNOWN1 : Packet<GamePacketOpcode>
    {
        public CM_LOGIN_UNKNOWN1()
        {
            ID = GamePacketOpcode.CM_LOGIN_UNKNOWN1;
        }

        public override Packet<GamePacketOpcode> New()
        {
            return new CM_LOGIN_UNKNOWN1();
        }

        public override void OnProcess(Session<GamePacketOpcode> client)
        {
            ((GameSession)client).OnLoginUnknown1(this);
        }
    }
}
