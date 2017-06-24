
using SmartEngine.Network;
using SagaBNS.Common.Packets;
using SagaBNS.GameServer.Network.Client;

namespace SagaBNS.GameServer.Packets.Client
{
    public class CM_ACTOR_ITEM_DROP : Packet<GamePacketOpcode>
    {
        public CM_ACTOR_ITEM_DROP()
        {
            ID = GamePacketOpcode.CM_ACTOR_ITEM_DROP;
        }

        public short X
        {
            get
            {
                return GetShort(2);
            }
        }

        public short Y
        {
            get
            {
                return GetShort(4);
            }
        }

        public short Z
        {
            get
            {
                return GetShort(6);
            }
        }

        public override Packet<GamePacketOpcode> New()
        {
            return new CM_ACTOR_ITEM_DROP();
        }

        public override void OnProcess(Session<GamePacketOpcode> client)
        {
            ((GameSession)client).OnActorItemDrop(this);
        }
    }
}
