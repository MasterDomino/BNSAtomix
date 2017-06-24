
using SmartEngine.Network;
using SagaBNS.Common.Packets;
using SagaBNS.GameServer.Network.Client;

namespace SagaBNS.GameServer.Packets.Client
{
    public class CM_LOGIN_UNKNOWN2 : Packet<GamePacketOpcode>
    {
        public CM_LOGIN_UNKNOWN2()
        {
            ID = GamePacketOpcode.CM_LOGIN_UNKNOWN2;
        }

        public int UnknownDWORD
        {
            get
            {
                return GetInt(2);
            }
        }

        public override Packet<GamePacketOpcode> New()
        {
            return new CM_LOGIN_UNKNOWN2();
        }

        public override void OnProcess(Session<GamePacketOpcode> client)
        {
            ((GameSession)client).OnLoginUnknown2(this);
        }
    }
}
