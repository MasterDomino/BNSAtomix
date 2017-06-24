
using SmartEngine.Network;
using SagaBNS.Common.Packets;
using SagaBNS.Common.Inventory;
using SagaBNS.Common;
using SagaBNS.GameServer.Network.Client;

namespace SagaBNS.GameServer.Packets.Client
{
    public class CM_ITEM_EQUIP : Packet<GamePacketOpcode>
    {
        public CM_ITEM_EQUIP()
        {
            ID = GamePacketOpcode.CM_ITEM_EQUIP;
        }

        public ushort SlotID
        {
            get
            {
                return GetUShort(2);
            }
        }

        public InventoryEquipSlot EquipSlot
        {
            get
            {
                return ((ushort)GetByte(4)).ToInventoryEquipSlot();
            }
        }

        public override Packet<GamePacketOpcode> New()
        {
            return new CM_ITEM_EQUIP();
        }

        public override void OnProcess(Session<GamePacketOpcode> client)
        {
            ((GameSession)client).OnItemEquip(this);
        }
    }
}
