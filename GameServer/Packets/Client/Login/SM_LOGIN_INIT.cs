using System.Text;

using SmartEngine.Network;
using SagaBNS.Common.Actors;
using SagaBNS.Common.Packets;

namespace SagaBNS.GameServer.Packets.Client
{
    public class SM_LOGIN_INIT : Packet<GamePacketOpcode>
    {
        private readonly ushort offsetAfterName;
        public SM_LOGIN_INIT()
        {
            ID = GamePacketOpcode.SM_LOGIN_INIT;
        }

        public ActorPC Player
        {
            set
            {
                PutInt(value.WorldID, 2);
                PutULong(value.ActorID);
                PutBytes(Configuration.Instance.ChatHost);
                PutUShort((ushort)Configuration.Instance.ChatPort);
                PutInt(value.WorldID);
                PutByte((byte)value.Race);
                PutByte((byte)value.Gender);
                PutByte((byte)value.Job);
                PutBytes(value.Appearence2);
                PutUShort((ushort)(value.Name.Length));
                PutBytes(Encoding.Unicode.GetBytes(value.Name));
                PutUInt(value.MapID);
                PutShort((short)value.X);
                PutShort((short)value.Y);
                PutShort((short)value.Z);
                PutUShort(value.Dir);
                PutByte(value.Level);
                PutUInt(value.Exp);
                offset += 4;
                PutInt(value.HP);
                PutInt(value.Gold);
                offset += 22;
                PutInt((int)value.ManaType);
                offset += 68;
                PutInt(0);//no idea what you're setting here
                PutInt(0);//or here ... default is 0 for both =X
                PutByte(value.InventorySize);
                PutByte(8);//Most Likely Faction
                PutByte(value.SkillPoints);
                //PutByte(value.SkillPointsUsed);
                offset += 2;
                PutByte((byte)value.Craft3);
                PutByte((byte)value.Craft4);
                PutByte((byte)value.Craft1);
                PutByte((byte)value.Craft2);
                PutUShort(value.Craft3Rep);
                PutUShort(value.Craft4Rep);
                PutUShort(value.Craft1Rep);
                PutUShort(value.Craft2Rep);
                PutULong(0);//Craft Order 3
                PutULong(0);//Craft Order 4
                PutULong(0);//Craft Order 1
                PutULong(0);//Craft Order 2
                PutShort(0);// Has to do with surveys; length of bytes and will change with later levels
                //PutBytes(Conversions.HexStr2Bytes("AAAAA2AAA8AA02"));// Wether or not survey been done 0s defaults, used value from Serephy end of game
                offset += 22;
                PutInt(value.MaxHP);
                PutUShort(value.MaxMP);
                PutUShort(0x3E);
                offset += 2;
                PutUShort(2);//Recovery Rate
                offset += 8;
                PutUShort(0x10);//Accuracy Base
                offset += 4;
                PutUShort(0x4E2);//Critical Rate %
                PutUShort(4);//Critical Rate Base
                offset += 12;
                PutUShort(6);//Parry Base
                PutUShort(0x12C);
                offset += 6;
                PutUShort(4);//Min Dmg
                PutUShort(6);//Max Dmg
                offset += 4;
                PutUShort(7);//Defense
                offset += 2;
                PutUShort(2);
                offset += 30;
                PutUShort(0x3E8);
                PutUShort(0x3E8);
                offset += 11;

                /*
                PutInt(0);
                PutUShort(1);
                PutShort(0);
                PutShort(0);//Accuracy Percent
                PutUShort((ushort)value.Status.Hit);//Accuracy Base
                PutUShort((ushort)value.Status.Pierce);//Pierce Base
                PutShort(0);
                PutUShort(0x5DC);//Crit Percent, Fixed value of 1500 for now
                PutUShort((ushort)value.Status.Critical);//Crit Base
                PutShort(0);//Crit Def Percent
                PutShort(0);//Crit Def Base
                PutShort(0);
                PutShort(0);//Dodge Percent
                PutUShort((ushort)value.Status.Avoid);//Dodge Base
                PutShort(0);//Parry Percent
                PutUShort((ushort)value.Status.Parry);//Parry Base
                PutInt(300);
                PutInt(0);
                PutUShort((ushort)value.Status.AtkMinBase);//AttackMin
                PutUShort((ushort)value.Status.AtkMaxBase);//AttackMax
                PutUShort(0);//AttackMin2 (gets filled in by weapon)
                PutUShort(0);//AttackMax2 (gets filled in by weapon)
                PutUShort((ushort)value.Status.Defence);//Defense
                offset += 48;
 */
                //if (value.Inventory.Equipments[Common.Inventory.InventoryEquipSlot.Weapon] != null)
                //    PutUInt(value.Inventory.Equipments[Common.Inventory.InventoryEquipSlot.Weapon].ItemID);
                //else
                    PutUInt(0);
                PutUInt(0);
                //if (value.Inventory.Equipments[Common.Inventory.InventoryEquipSlot.Costume] != null)
                 //   PutUInt(value.Inventory.Equipments[Common.Inventory.InventoryEquipSlot.Costume].ItemID);
                //else
                    PutUInt(0);
                PutUInt(0);
                //if (value.Inventory.Equipments[Common.Inventory.InventoryEquipSlot.Eye] != null)
                //    PutUInt(value.Inventory.Equipments[Common.Inventory.InventoryEquipSlot.Eye].ItemID);
                //else
                    PutUInt(0);
                //if (value.Inventory.Equipments[Common.Inventory.InventoryEquipSlot.Hat] != null)
                //    PutUInt(value.Inventory.Equipments[Common.Inventory.InventoryEquipSlot.Hat].ItemID);
                //else
                    PutUInt(0);
                //if (value.Inventory.Equipments[Common.Inventory.InventoryEquipSlot.CostumeAccessory] != null)
               //     PutUInt(value.Inventory.Equipments[Common.Inventory.InventoryEquipSlot.CostumeAccessory].ItemID);
               // else
                    PutUInt(0);
                PutUInt(0);
                PutShort(16);
                System.Security.Cryptography.MD5 md5 = System.Security.Cryptography.MD5.Create();
                PutBytes(md5.ComputeHash(Encoding.ASCII.GetBytes(Global.Random.Next().ToString())));
                offset += 2;
                //PutInt(1);
                //PutByte(0);
                if (!string.IsNullOrEmpty(value.UISettings))
                {
                    PutString(value.UISettings);
                }
                else
                {
                    PutShort(0);
                }
                PutInt(1);
            }
        }
    }
}
