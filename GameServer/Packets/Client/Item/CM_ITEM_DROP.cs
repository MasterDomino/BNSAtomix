
using SmartEngine.Network;
using SagaBNS.Common.Packets;
using SagaBNS.GameServer.Network.Client;

namespace SagaBNS.GameServer.Packets.Client
{
    public class CM_ITEM_DROP : Packet<GamePacketOpcode>
    {
        public CM_ITEM_DROP()
        {
            ID = GamePacketOpcode.CM_ITEM_DROP;
        }

        public ushort SlotID
        {
            get
            {
                return GetUShort(2);
            }
        }

        public ushort Count
        {
            get
            {
                return GetUShort(4);
            }
        }

        public override Packet<GamePacketOpcode> New()
        {
            return new CM_ITEM_DROP();
        }

        public override void OnProcess(Session<GamePacketOpcode> client)
        {
            ((GameSession)client).OnItemDrop(this);
        }
    }
}
