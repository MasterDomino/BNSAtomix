using System.Text;
using SmartEngine.Network;
using SagaBNS.Common;
using SagaBNS.Common.Actors;
using SagaBNS.Common.Packets;

namespace SagaBNS.LobbyServer.Packets.Client
{
    public class SM_CHAR_CREATE : Packet<LobbyPacketOpcode>
    {
        private ushort marker;
        private ushort marker2;

        public SM_CHAR_CREATE()
        {
            ID = LobbyPacketOpcode.SM_CHAR_CREATE;
        }

        public ActorPC Character
        {
            set
            {
                PutBytes(Utils.slot2GuidBytes(value.SlotID));
                PutUShort(value.WorldID);
                PutShort((short)value.Name.Length);
                PutBytes(Encoding.Unicode.GetBytes(value.Name));
                marker = offset;
                PutShort(0);//length
                PutShort(0);//length
                PutUShort((ushort)value.Appearence1.Length);
                PutBytes(value.Appearence1);

                //Race ID
                PutUShort(0);
                PutByte((byte)value.Race);
                //Gender ID
                PutUShort(1);
                PutByte((byte)value.Gender);
                //Class ID
                PutUShort(2);
                PutByte((byte)value.Job);
                //Appearence ID
                PutUShort(3);
                PutUShort((ushort)value.Appearence2.Length);
                PutBytes(value.Appearence2);
                //Name ID
                PutUShort(4);
                PutBytes(Encoding.Unicode.GetBytes(value.Name));
                PutShort(0);//Terminator for String
                //Unknown ID
                PutUShort(5);
                PutUShort(0);
                //Map ID
                PutUShort(6);
                PutUInt(value.MapID);
                //X ID
                PutUShort(7);
                PutShort((short)value.X);
                //Y ID
                PutUShort(8);
                PutShort((short)value.Y);
                //Z ID
                PutUShort(9);
                PutShort((short)value.Z);
                //Level ID
                PutUShort(10);
                PutByte(value.Level);
                //Unknown ID
                PutUShort(11);
                PutUInt(0);
                //Health Points ID
                PutUShort(12);
                PutInt(value.MaxHP);
                //Unknown ID
                PutUShort(13);
                PutUShort(0);
                //Gold ID
                PutUShort(14);
                PutInt(value.Gold);
                //Weapon ID
                PutUShort(15);
                //if (value.Inventory.Equipments[Common.Inventory.InventoryEquipSlot.Weapon] != null)
                //    PutUInt(value.Inventory.Equipments[Common.Inventory.InventoryEquipSlot.Weapon].ItemID);
                //else
                    PutUInt(0);
                //Unknown ID
                PutUShort(16);
                PutUInt(0);
                //Costume ID
                PutUShort(17);
                //if (value.Inventory.Equipments[Common.Inventory.InventoryEquipSlot.Costume] != null)
                //    PutUInt(value.Inventory.Equipments[Common.Inventory.InventoryEquipSlot.Costume].ItemID);
                //else
                    PutUInt(0);
                //Unknown ID
                PutUShort(18);
                PutUInt(0);
                //Eye Accessory ID
                PutUShort(19);
                //if (value.Inventory.Equipments[Common.Inventory.InventoryEquipSlot.Eye] != null)
                //    PutUInt(value.Inventory.Equipments[Common.Inventory.InventoryEquipSlot.Eye].ItemID);
                //else
                    PutUInt(0);
                //Hat ID
                PutUShort(20);
                //if (value.Inventory.Equipments[Common.Inventory.InventoryEquipSlot.Hat] != null)
                //    PutUInt(value.Inventory.Equipments[Common.Inventory.InventoryEquipSlot.Hat].ItemID);
                //else
                    PutUInt(0);
                //Costume Accessory ID
                PutUShort(21);
                //if (value.Inventory.Equipments[Common.Inventory.InventoryEquipSlot.CostumeAccessory] != null)
                //    PutUInt(value.Inventory.Equipments[Common.Inventory.InventoryEquipSlot.CostumeAccessory].ItemID);
                //else
                    PutUInt(0);

                marker2 = offset;
                PutUShort((ushort)(marker2 - (marker + 2)), marker);
                PutUShort((ushort)(marker2 - (marker + 2)));
                offset = marker2;

                PutInt(0);
                PutInt(0);
                PutInt(0);
                PutInt(0);
            }
        }
    }
}
