using System.Collections.Generic;

using SmartEngine.Network;
using SmartEngine.Network.Map;
using SagaBNS.Common.Actors;
using SagaBNS.Common.Packets;

namespace SagaBNS.GameServer.Packets.Client
{
    public class SM_ACTOR_INFO_LIST : Packet<GamePacketOpcode>
    {
        private ushort offsetAfterType;
        private uint instanceID;
        private readonly Factions playerFaction;
        public SM_ACTOR_INFO_LIST(Factions playerFaction)
        {
            ID = GamePacketOpcode.SM_ACTOR_INFO_LIST;
            this.playerFaction = playerFaction;
        }

        public uint MapInstanceID
        {
            set
            {
                instanceID = value;
                PutUInt(value, 6);//instance id?
                PutInt(0x100000);//high 32bits?
            }
        }

        public List<Actor> Actors
        {
            set
            {
                PutUShort((ushort)value.Count, 14);//Count
                foreach (Actor i in value)
                {
                    switch (i.ActorType)
                    {
                        case ActorType.PC:
                            {
                                ActorPC pc = (ActorPC)i;
                                PutULong(pc.ActorID);
                                PutUInt(pc.MapID);
                                PutShort((short)pc.X);//x
                                PutShort((short)pc.Y);//y
                                PutShort((short)pc.Z);//z
                                PutUShort(pc.Dir);//dir
                                PutByte(pc.Level);//level
                                PutInt(0);
                                PutInt(pc.HP);//hp
                                offset += 16;
                                PutShort((short)pc.MP);
                                PutInt((int)pc.ManaType);
                                offset += 77;
                                PutInt(pc.MaxHP);
                                PutUShort(pc.MaxMP);
                                PutByte(62);
                                offset += 2;

                                //if (pc.Inventory.Equipments[Common.Inventory.InventoryEquipSlot.Weapon] != null)
                                //    PutUInt(pc.Inventory.Equipments[Common.Inventory.InventoryEquipSlot.Weapon].ItemID);
                                //else
                                    PutUInt(0);//weapon
                                PutUInt(0);
                                //if (pc.Inventory.Equipments[Common.Inventory.InventoryEquipSlot.Costume] != null)
                                //    PutUInt(pc.Inventory.Equipments[Common.Inventory.InventoryEquipSlot.Costume].ItemID);
                                //else
                                    PutUInt(0);//cloth
                                PutUInt(0);
                                //if (pc.Inventory.Equipments[Common.Inventory.InventoryEquipSlot.Eye] != null)
                                //    PutUInt(pc.Inventory.Equipments[Common.Inventory.InventoryEquipSlot.Eye].ItemID);
                                //else
                                    PutUInt(0);//eyeware
                                //if (pc.Inventory.Equipments[Common.Inventory.InventoryEquipSlot.Hat] != null)
                                //    PutUInt(pc.Inventory.Equipments[Common.Inventory.InventoryEquipSlot.Hat].ItemID);
                               // else
                                    PutUInt(0);//hat
                                //if (pc.Inventory.Equipments[Common.Inventory.InventoryEquipSlot.CostumeAccessory] != null)
                                //    PutUInt(pc.Inventory.Equipments[Common.Inventory.InventoryEquipSlot.CostumeAccessory].ItemID);
                                //else
                                    PutUInt(0);//costume accessory
                                PutUInt(0);//weapon gem 1
                                offset += 2;
                            }
                            break;
                        case ActorType.NPC:
                            {
                                ActorNPC npc = (ActorNPC)i;
                                PutULong(npc.ActorID); //actorID
                                PutUInt(npc.MapID);//mapid?
                                PutShort((short)npc.X);//x
                                PutShort((short)npc.Y);//y
                                PutShort((short)npc.Z);//z
                                PutUShort(npc.Dir);//dir
                                PutByte(npc.Level);//level
                                PutInt(0);
                                PutInt(npc.HP);//hp
                                offset += 16;
                                PutShort((short)npc.MP);
                                PutInt((int)npc.ManaType);
                                offset += 76;
                                PutByte(0xC0);//Faction
                                PutInt(npc.MaxHP);
                                PutShort((short)npc.MaxMP);
                                PutByte(62);
                                offset += 36;
                                /*
                                PutUShort((ushort)npc.MP);//mp
                                PutByte(04);
                                PutByte(04); 
                                PutUShort(0);
                                PutInt((int)npc.ManaType); // somthing to do with job
                                offset += 60;
                                PutByte(NPC.FactionRelationFactory.Instance[playerFaction][npc.Faction] == Relations.Friendly ? (byte)0 : (byte)8);
                                PutInt(npc.MaxHP);//max hp
                                PutUShort(npc.MaxMP);//max mp
                                PutUShort(200);
                                PutUShort(62);
                                offset += 5;
                                PutInt(npc.StandartMotion);
                                offset += 26;
                                */
                            }
                            break;
                    }
                }
                offsetAfterType = offset;
            }
        }

        public Dictionary<ulong, ActorMapObj> MapObjects
        {
            set
            {
                PutUShort((ushort)value.Count, offsetAfterType);//another count
                foreach (ActorMapObj i in value.Values)
                {
                    PutULong(i.ActorID);
                    if (i.DragonStream)
                    {
                        PutInt(0);
                    }
                    else if (i.Special)
                    {
                        PutInt(i.Available ? 4 : 5);//2 invisible 3 visible
                    }
                    else
                    {
                        PutInt(i.Available ? 2 : 3);//4 invisible 5 visible
                    }

                    PutByte(0);
                    PutByte(i.Available ? (byte)1 : (byte)0);//1 disable
                    PutByte(1);
                    PutShort(0);
                }

                PutUShort(0);//Unknown Type
                PutUShort(0);//Unknown 0x09 Type
                PutUShort(0);//Unknown 0x0B Type
                PutUShort(0);//Unknown 0x0F Type

                offsetAfterType = offset;
            }
        }

        public Dictionary<ulong, ActorMapObj> Campfires
        {
            set
            {
                PutUShort((ushort)value.Count, offsetAfterType);//another count
                foreach (ActorMapObj i in value.Values)
                {
                    PutULong(i.ActorID);
                    PutUInt(i.SpecialMapID);
                    PutUInt(2);
                    PutUInt(0);
                    PutByte(0);
                    PutShort((short)i.X);
                    PutShort((short)i.Y);
                    PutShort((short)i.Z);
                    PutShort(0);
                }
                PutInt((int)Length - 6, 2);
            }
        }
    }
}
