using SmartEngine.Network;
using SagaBNS.Common.Packets;

namespace SagaBNS.Common.Item
{
    public class Item
    {
        private readonly ItemData data;

        public ItemData BaseData { get { return data; } }
        public uint ID { get; set; }
        public uint CharID { get; set; }
        public uint ItemID { get { return data.ItemID; } }
        public ushort SlotID { get; set; }
        public Containers Container { get; set; }
        public ushort Count { get; set; }
        public byte Synthesis { get; set; }
        public int MaxStackableCount
        {
            get
            {
                switch (BaseData.ItemType)
                {
                    case ItemType.Usable_Item:
                    case ItemType.Trash:
                    case ItemType.Material:
                    case ItemType.Potion:
                    case ItemType.Food:
                    case ItemType.RepairKit:
                    case ItemType.Teleport_Ticket:
                    case ItemType.QuestItem:
                        return BaseData.MaxStackableCount != 0 ? BaseData.MaxStackableCount : 1;
                    default:
                        return 1;
                }
            }
        }

        public Item(ItemData data)
        {
            this.data = data;
        }

        public void ToPacket(Packet<GamePacketOpcode> p)
        {
            switch (BaseData.ItemType)
            {
                case ItemType.Costume:
                    if (Count <= 0)
                    {
                        p.PutByte(0xB);
                        p.PutByte(4);
                        p.PutUShort(SlotID);
                        //p.PutByte((byte)this.InventoryEquipSlot);
                        p.PutUInt(0);
                        p.PutByte(0);
                        p.PutUShort(0);
                    }
                    else
                    {
                        p.PutByte(0xA);
                        p.PutByte(3);
                        p.PutByte((byte)Container);
                        p.PutUShort((ushort)(SlotID + 1));
                        //p.PutUShort((ushort)this.InventoryEquipSlot);
                        p.PutUInt(ItemID);
                        p.PutByte(1);
                        //p.PutByte(8);
                    }
                    break;
                case ItemType.Trash:
                case ItemType.Potion:
                case ItemType.RepairKit:
                case ItemType.Material:
                case ItemType.Teleport_Ticket:
                case ItemType.QuestItem:
                case ItemType.Food:
                    p.PutByte(0xB);
                    p.PutByte(4);
                    p.PutUShort(SlotID);
                    //p.PutByte((byte)this.InventoryEquipSlot);
                    if (Count <= 0)
                    {
                        p.PutUInt(0);
                        p.PutByte(0);
                        p.PutUShort(0);
                    }
                    else
                    {
                        p.PutUInt(ItemID);
                        p.PutByte(0);
                        p.PutUShort(Count);
                    }
                    break;
                case ItemType.Bagua:
                case ItemType.Weapon_Gem:
                    {
                        p.PutByte(0xB);
                        p.PutByte(5);
                        p.PutUShort(SlotID);
                        //p.PutByte((byte)this.InventoryEquipSlot);
                        if (Count <= 0)
                        {
                            p.PutUInt(0);
                            p.PutByte(0);
                            p.PutUShort(0);
                        }
                        else
                        {
                            p.PutUInt(ItemID);
                            p.PutByte(0);
                            p.PutByte(Synthesis);
                            p.PutByte((byte)Count);
                        }
                        break;
                    }
                case ItemType.Acc_Ring:
                case ItemType.Acc_Ear:
                case ItemType.Acc_Neckless:
                case ItemType.Eyeware:
                case ItemType.Hat:
                case ItemType.CostumeAccessory:
                    {
                        p.PutByte(0xB);
                        p.PutByte(6);
                        p.PutUShort(SlotID);
                       // p.PutByte((byte)this.InventoryEquipSlot);
                        if (Count <= 0)
                        {
                            p.PutUInt(0);
                            p.PutByte(0);
                            p.PutUShort(0);
                        }
                        else
                        {
                            p.PutUInt(ItemID);
                            p.PutByte(0);
                            p.PutByte(40);//durability
                            p.PutByte((byte)Count);
                        }
                    }
                    break;
                case ItemType.Weapon_AB:
                case ItemType.Weapon_DG:
                case ItemType.Weapon_GT:
                case ItemType.Weapon_ST:
                case ItemType.Weapon_SW:
                case ItemType.Weapon_TA:
                    if (Count <= 0)
                    {
                        p.PutByte(0xB);
                        p.PutByte(4);
                        p.PutUShort(SlotID);
                        //p.PutByte((byte)this.InventoryEquipSlot);
                        p.PutUInt(0);
                        p.PutByte(0);
                        p.PutUShort(0);
                    }
                    else
                    {
                        p.PutByte(0x28);
                        p.PutByte(2);
                        p.PutUShort(SlotID);
                        //p.PutByte((byte)this.InventoryEquipSlot);
                        p.PutUInt(ItemID);
                        p.PutByte(0);
                        p.PutByte((byte)BaseData.MaxDurability);//durability
                        p.PutUShort(Count);
                        p.Position += 26;
                        p.PutUShort(0);
                    }
                    break;
            }
        }
    }
}
