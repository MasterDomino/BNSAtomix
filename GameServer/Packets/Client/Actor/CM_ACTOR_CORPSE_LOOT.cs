
using SmartEngine.Network;
using SagaBNS.Common.Packets;
using SagaBNS.GameServer.Network.Client;

namespace SagaBNS.GameServer.Packets.Client
{
    public class CM_ACTOR_CORPSE_LOOT : Packet<GamePacketOpcode>
    {
        public CM_ACTOR_CORPSE_LOOT()
        {
            ID = GamePacketOpcode.CM_ACTOR_CORPSE_LOOT;
        }

        public ulong ActorID
        {
            get
            {
                return GetULong(2);
            }
        }

        public byte[] Indices
        {
            get
            {
                return GetBytes(GetUShort(10));
            }
        }

        public override Packet<GamePacketOpcode> New()
        {
            return new CM_ACTOR_CORPSE_LOOT();
        }

        public override void OnProcess(Session<GamePacketOpcode> client)
        {
            ((GameSession)client).OnActorCorpseLoot(this);
        }
    }
}
