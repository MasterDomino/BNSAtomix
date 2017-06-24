
using SmartEngine.Network;
using SagaBNS.Common.Packets;
using SagaBNS.GameServer.Network.Client;

namespace SagaBNS.GameServer.Packets.Client
{
    public class CM_LOGIN_AUTH : Packet<GamePacketOpcode>
    {
        public CM_LOGIN_AUTH()
        {
            ID = GamePacketOpcode.CM_LOGIN_AUTH;
        }

        public ulong Unknown
        {
            get
            {
                return GetULong(2);
            }
        }

        public ulong CharID
        {
            get
            {
                return GetULong(10);
            }
        }

        public override Packet<GamePacketOpcode> New()
        {
            return new CM_LOGIN_AUTH();
        }

        public override void OnProcess(Session<GamePacketOpcode> client)
        {
            ((GameSession)client).OnLoginAuth(this);
        }
    }
}
