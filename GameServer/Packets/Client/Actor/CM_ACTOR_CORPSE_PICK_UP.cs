
using SmartEngine.Network;
using SagaBNS.Common.Packets;
using SagaBNS.GameServer.Network.Client;

namespace SagaBNS.GameServer.Packets.Client
{
    public class CM_ACTOR_CORPSE_PICK_UP : Packet<GamePacketOpcode>
    {
        public CM_ACTOR_CORPSE_PICK_UP()
        {
            ID = GamePacketOpcode.CM_ACTOR_CORPSE_PICK_UP;
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
            return new CM_ACTOR_CORPSE_PICK_UP();
        }

        public override void OnProcess(Session<GamePacketOpcode> client)
        {
            ((GameSession)client).OnActorCorpsePickUp(this);
        }
    }
}
