
using SmartEngine.Network;
using SagaBNS.Common.Packets;
using SagaBNS.GameServer.Network.Client;

namespace SagaBNS.GameServer.Packets.Client
{
    public class CM_ACTOR_TURN : Packet<GamePacketOpcode>
    {
        public CM_ACTOR_TURN()
        {
            ID = GamePacketOpcode.CM_ACTOR_TURN;
        }

        public ushort Dir
        {
            get
            {
                return GetUShort(3);
            }
        }

        public override Packet<GamePacketOpcode> New()
        {
            return new CM_ACTOR_TURN();
        }

        public override void OnProcess(Session<GamePacketOpcode> client)
        {
            ((GameSession)client).OnActorTurn(this);
        }
    }
}
