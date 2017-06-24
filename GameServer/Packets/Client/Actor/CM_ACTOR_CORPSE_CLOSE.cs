
using SmartEngine.Network;
using SagaBNS.Common.Packets;
using SagaBNS.GameServer.Network.Client;

namespace SagaBNS.GameServer.Packets.Client
{
    public class CM_ACTOR_CORPSE_CLOSE : Packet<GamePacketOpcode>
    {
        public CM_ACTOR_CORPSE_CLOSE()
        {
            ID = GamePacketOpcode.CM_ACTOR_CORPSE_CLOSE;
        }

        public ulong ActorID
        {
            get
            {
                return GetULong(2);
            }
        }

        public override Packet<GamePacketOpcode> New()
        {
            return new CM_ACTOR_CORPSE_CLOSE();
        }

        public override void OnProcess(Session<GamePacketOpcode> client)
        {
            ((GameSession)client).OnActorCorpseClose(this);
        }
    }
}
