
using SmartEngine.Network;
using SagaBNS.Common.Packets;
using SagaBNS.GameServer.Network.Client;

namespace SagaBNS.GameServer.Packets.Client
{
    public class CM_ITEM_USE : Packet<GamePacketOpcode>
    {
        public CM_ITEM_USE()
        {
            ID = GamePacketOpcode.CM_ITEM_USE;
        }

        public ushort SlotID
        {
            get
            {
                return GetUShort(2);
            }
        }

        public ulong ActorID
        {
            get
            {
                return GetULong(4);
            }
        }

        public override Packet<GamePacketOpcode> New()
        {
            return new CM_ITEM_USE();
        }

        public override void OnProcess(Session<GamePacketOpcode> client)
        {
            ((GameSession)client).OnItemUse(this);
        }
    }
}
