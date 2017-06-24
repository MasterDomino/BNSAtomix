using System.Collections.Generic;
using System.Text;
using SmartEngine.Network;
using SagaBNS.Common;
using SagaBNS.Common.Actors;
using SagaBNS.Common.Packets;

namespace SagaBNS.LobbyServer.Packets.Client
{
    public class SM_CHARACTER_LIST : Packet<LobbyPacketOpcode>
    {
        private ushort marker;
        private ushort marker2;

        public SM_CHARACTER_LIST()
        {
            ID = LobbyPacketOpcode.SM_CHARACTER_LIST;
        }

        public List<ActorPC> Characters
        {
            set
            {
                PutUShort((ushort)value.Count, 6);
                foreach (ActorPC i in value)
                {
                    PutBytes(Utils.slot2GuidBytes(i.SlotID));
                    PutUShort(i.WorldID);
                    PutShort((short)i.Name.Length);
                    PutBytes(Encoding.Unicode.GetBytes(i.Name));
                    marker = offset;
                    PutShort(0);//length
                    PutShort(0);//length
                    PutUShort((ushort)i.Appearence1.Length);
                    PutBytes(i.Appearence1);

                    //Race ID
                    PutUShort(0);
                    PutByte((byte)i.Race);
                    //Gender ID
                    PutUShort(1);
                    PutByte((byte)i.Gender);
                    //Class ID
                    PutUShort(2);
                    PutByte((byte)i.Job);
                    //Appearence ID
                    PutUShort(3);
                    PutUShort((ushort)i.Appearence2.Length);
                    PutBytes(i.Appearence2);
                    //Name ID
                    PutUShort(4);
                    PutBytes(Encoding.Unicode.GetBytes(i.Name));
                    PutShort(0);//Terminator for String
                    //Unknown ID
                    PutUShort(5);
                    PutUShort(0);
                    //Map ID
                    PutUShort(6);
                    PutUInt(i.MapID);
                    //X ID
                    PutUShort(7);
                    PutShort((short)i.X);
                    //Y ID
                    PutUShort(8);
                    PutShort((short)i.Y);
                    //Z ID
                    PutUShort(9);
                    PutShort((short)i.Z);
                    //Level ID
                    PutUShort(10);
                    PutByte(i.Level);
                    //Unknown ID
                    PutUShort(11);
                    PutUInt(0);
                    //Health Points ID
                    PutUShort(12);
                    PutInt(i.MaxHP);
                    //Unknown ID
                    PutUShort(13);
                    PutUShort(0);
                    //Gold ID
                    PutUShort(14);
                    PutInt(i.Gold);
                    //Weapon ID
                    PutUShort(15);
                    //     if (i.Inventory.Equipments[Common.Inventory.InventoryEquipSlot.Weapon] != null)
                    //        PutUInt(i.Inventory.Equipments[Common.Inventory.InventoryEquipSlot.Weapon].ItemID);
                    //     else
                    PutUInt(0);
                    //Unknown ID
                    PutUShort(16);
                    PutUInt(0);
                    //Costume ID
                    PutUShort(17);
                    //     if (i.Inventory.Equipments[Common.Inventory.InventoryEquipSlot.Costume] != null)
                    //         PutUInt(i.Inventory.Equipments[Common.Inventory.InventoryEquipSlot.Costume].ItemID);
                    //     else
                    PutUInt(0);
                    //Unknown ID
                    PutUShort(18);
                    PutUInt(0);
                    //Eye Accessory ID
                    PutUShort(19);
                    //       if (i.Inventory.Equipments[Common.Inventory.InventoryEquipSlot.Eye] != null)
                    //           PutUInt(i.Inventory.Equipments[Common.Inventory.InventoryEquipSlot.Eye].ItemID);
                    //       else
                    PutUInt(0);
                    //Hat ID
                    PutUShort(20);
                    //         if (i.Inventory.Equipments[Common.Inventory.InventoryEquipSlot.Hat] != null)
                    //             PutUInt(i.Inventory.Equipments[Common.Inventory.InventoryEquipSlot.Hat].ItemID);
                    //        else
                    PutUInt(0);
                    //Costume Accessory ID
                    PutUShort(21);
                    //         if (i.Inventory.Equipments[Common.Inventory.InventoryEquipSlot.CostumeAccessory] != null)
                    //            PutUInt(i.Inventory.Equipments[Common.Inventory.InventoryEquipSlot.CostumeAccessory].ItemID);
                    //        else
                    PutUInt(0);

                    marker2 = offset;
                    PutUShort((ushort)(marker2 - (marker + 2)), marker);
                    PutUShort((ushort)(marker2 - (marker + 2)));
                    offset = marker2;

                    PutInt(0);
                    PutInt(0);
                    PutInt(0);
                    PutByte(0);
                }
                PutUInt((uint)offset - 6, 2);
            }
        }
    }
}