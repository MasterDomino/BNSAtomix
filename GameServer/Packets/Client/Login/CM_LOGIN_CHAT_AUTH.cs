
using SmartEngine.Network;
using SagaBNS.Common.Packets;
using SagaBNS.GameServer.Network.Client;

namespace SagaBNS.GameServer.Packets.Client
{
    public class CM_LOGIN_CHAT_AUTH : Packet<GamePacketOpcode>
    {
        public CM_LOGIN_CHAT_AUTH()
        {
            ID = GamePacketOpcode.CM_LOGIN_CHAT_AUTH;
        }

        public override Packet<GamePacketOpcode> New()
        {
            return new CM_LOGIN_CHAT_AUTH();
        }

        public override void OnProcess(Session<GamePacketOpcode> client)
        {
            ((GameSession)client).OnLoginChatAuth(this);
        }
    }
}
