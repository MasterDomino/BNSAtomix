
using SmartEngine.Network;
using SagaBNS.Common.Packets;
using SagaBNS.GameServer.Network.Client;

namespace SagaBNS.GameServer.Packets.Client
{
    public class CM_ITEM_UNEQUIP : Packet<GamePacketOpcode>
    {
        public CM_ITEM_UNEQUIP()
        {
            ID = GamePacketOpcode.CM_ITEM_UNEQUIP;
        }

        public ushort SlotID
        {
            get
            {
                return GetUShort(2);
            }
        }

        public override Packet<GamePacketOpcode> New()
        {
            return new CM_ITEM_UNEQUIP();
        }

        public override void OnProcess(Session<GamePacketOpcode> client)
        {
            ((GameSession)client).OnItemUnequip(this);
        }
    }
}
