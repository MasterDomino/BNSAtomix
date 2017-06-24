
using SmartEngine.Network;
using SagaBNS.Common.Packets;
using SagaBNS.GameServer.Network.Client;

namespace SagaBNS.GameServer.Packets.Client
{
    public class CM_ITEM_MOVE : Packet<GamePacketOpcode>
    {
        public CM_ITEM_MOVE()
        {
            ID = GamePacketOpcode.CM_ITEM_MOVE;
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

        public ushort TargetSlot
        {
            get
            {
                return GetUShort(6);
            }
        }

        public ulong ActorID
        {
            get
            {
                return GetULong(8);
            }
        }

        public override Packet<GamePacketOpcode> New()
        {
            return new CM_ITEM_MOVE();
        }

        public override void OnProcess(Session<GamePacketOpcode> client)
        {
            ((GameSession)client).OnItemMove(this);
        }
    }
}
