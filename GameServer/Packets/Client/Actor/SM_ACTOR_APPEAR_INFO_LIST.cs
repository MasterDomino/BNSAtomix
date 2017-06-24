using System.Collections.Generic;

using SmartEngine.Network;
using SmartEngine.Network.Map;
using SagaBNS.Common.Actors;
using SagaBNS.Common.Packets;

namespace SagaBNS.GameServer.Packets.Client
{
    public class SM_ACTOR_APPEAR_INFO_LIST : Packet<GamePacketOpcode>
    {
        private uint instanceID;
        private ushort offsetAppear;
        private Factions playerfaction;

        public SM_ACTOR_APPEAR_INFO_LIST()
        {
            ID = GamePacketOpcode.SM_ACTOR_APPEAR_INFO_LIST;
        }

        public uint MapInstanceID
        {
            set
            {
                instanceID = value;
                PutUInt(value, 6);//instance id?
                PutShort(0x10, 12);//high 32bits?
            }
        }

        public Factions PlayerFaction
        {
            set
            {
                playerfaction = value;
            }
        }

        public List<Actor> DisappearActors
        {
            set
            {
                PutShort((short)value.Count, 14);
                foreach (Actor i in value)
                {
                    PutULong(i.ActorID);
                    PutByte(0);
                }
                offsetAppear = offset;
            }
        }

        public List<Actor> AppearActors
        {
            set
            {
                PutUShort((ushort)value.Count, offsetAppear);//Count
                foreach (Actor i in value)
                {
                    switch (i.ActorType)
                    {
                        case ActorType.PC:
                            {
                                ActorPC pc = (ActorPC)i;
                                PutULong(pc.ActorID);
                                offset += 9;
                                PutUInt(pc.MapID);
                                PutShort((short)pc.X);//x
                                PutShort((short)pc.Y);//y
                                PutShort((short)pc.Z);//z
                                PutUShort(pc.Dir);//dir
                                PutByte(pc.Level);//level
                                PutInt(0);
                                PutInt(pc.HP);//hp
                                offset += 16;
                                PutShort((short)pc.MP);//mp
                                PutInt((int)pc.ManaType);
                                offset += 77;
                                PutInt(pc.MaxHP);//max hp
                                PutUShort(pc.MaxMP);//max mp
                                PutUShort(62);
                                offset += 1;
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
                                //else
                                    PutUInt(0);//hat
                                //if (pc.Inventory.Equipments[Common.Inventory.InventoryEquipSlot.CostumeAccessory] != null)
                                //    PutUInt(pc.Inventory.Equipments[Common.Inventory.InventoryEquipSlot.CostumeAccessory].ItemID);
                                //else
                                    PutUInt(0);//costume accessory
                                PutUInt(0); //weapon gem 1
                                offset += 2;
                            }
                            break;
                        case ActorType.NPC:
                            {
                                ActorNPC npc = (ActorNPC)i;
                                PutULong(npc.ActorID); //actorID
                                PutByte(0);
                                PutInt(npc.AppearEffect);
                                PutInt(0);
                                PutUInt(npc.MapID);//mapid?
                                PutShort((short)npc.X);//x
                                PutShort((short)npc.Y);//y
                                PutShort((short)npc.Z);//z
                                PutUShort(npc.Dir);//dir
                                PutByte(npc.Level);//level
                                PutInt(0);
                                PutInt(npc.HP);//hp
                                offset += 16;
                                PutUShort((ushort)npc.MP);//mp
                                PutInt((int)npc.ManaType); // somthing to do with job
                                offset += 76;
                                //PutByte(NPC.FactionRelationFactory.Instance[playerfaction][npc.Faction] == Relations.Friendly ? (byte)0 : (byte)8);
                                PutByte(0xC0);//Faction
                                PutInt(npc.MaxHP);//max hp
                                PutUShort(npc.MaxMP);//max mp
                                PutUShort(62);
                                offset += 35;
                            }
                            break;
                    }
                }
                PutUShort(0);
                PutInt((int)Length - 6, 2);
            }
        }
    }
}
