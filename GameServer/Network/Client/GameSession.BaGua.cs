using System;
using System.Collections.Generic;
using SmartEngine.Network;
using SagaBNS.Common.Actors;
using SagaBNS.Common.Item;
using SagaBNS.Common.Inventory;
using SagaBNS.Common.Packets;
using SagaBNS.GameServer.Network.CharacterServer;
using SagaBNS.GameServer.Packets.Client;
using SagaBNS.GameServer.Map;
using SagaBNS.GameServer.Item;

namespace SagaBNS.GameServer.Network.Client
{
    public partial class GameSession : Session<GamePacketOpcode>
    {
        public void DecomposeBaGua(CM_DECOMPOSE_BAGUA p)
        {
            Common.Item.Item item = chara.Inventory.Container[Containers.Inventory][p.ItemSlot];
            if (item != null)
            {
                if ((item.BaseData.ItemType == ItemType.Weapon_AB
                    || item.BaseData.ItemType == ItemType.Weapon_DG
                    || item.BaseData.ItemType == ItemType.Weapon_GT
                    || item.BaseData.ItemType == ItemType.Weapon_ST
                    || item.BaseData.ItemType == ItemType.Weapon_SW
                    || item.BaseData.ItemType == ItemType.Weapon_TA)
                    && item.BaseData.BaGuaPowderCountWep > 0)
                {
                    RemoveItemSlot((byte)p.ItemSlot, 1);
                    AddItem(item.BaseData.BaGuaPowderIdWep,(ushort)item.BaseData.BaGuaPowderCountWep);
                }
                else if (item.BaseData.ItemType == ItemType.Bagua && item.BaseData.BaGuaPowderCount > 0)
                {
                    RemoveItemSlot((byte)p.ItemSlot, 1);
                    AddItem(item.BaseData.BaGuaPowderId, (ushort)item.BaseData.BaGuaPowderCount);
                }
            }
        }

        public void ComposeBaGua(CM_COMPOSE_BAGUA p)
        {
            Common.Item.Item primary = chara.Inventory.Container[Containers.Inventory][p.PrimarySlot];
            Common.Item.Item secondary = chara.Inventory.Container[Containers.Inventory][p.SecondarySlot];
            Stats stat = Utils.FindStat(p.Stat);

            if (primary != null && secondary != null)
            {
                if (primary.BaseData.ItemType == ItemType.Bagua && secondary.BaseData.ItemType == ItemType.Bagua)
                {
                    if (primary.BaseData.SecondaryStats.ContainsKey(stat) && secondary.BaseData.PrimaryStats.ContainsKey(stat))
                    {
                        Dictionary<Common.Item.Item, ushort> update = new Dictionary<Common.Item.Item, ushort>();
                        Common.Item.Item add;
                        ushort total = 0;
                        bool pass;

                        foreach (KeyValuePair<ushort, ushort> i in p.ExchangeItems)
                        {
                            add = Character.Inventory.Container[Containers.Inventory][i.Key];
                            if (add != null)
                            {
                                if (add.Count >= i.Value && add.ItemID == primary.BaseData.BaGuaPowderId)
                                {
                                    update.Add(add, i.Value);
                                    total += i.Value;
                                }
                                else
                                {
                                    break;
                                }
                            }
                        }

                        if (total == primary.BaseData.BaGuaPowderCount)
                        {
                            pass = BaGuaSuccess(primary, secondary);
                            update.Add(secondary, secondary.Count);
                            RemoveItemsBySlot(update);
                            if (pass)
                            {
                                Common.Item.Item remove = new Common.Item.Item(primary.BaseData)
                                {
                                    Synthesis = primary.Synthesis
                                };
                                primary.Synthesis = p.Stat;
                                List<Common.Item.Item> items = new List<Common.Item.Item>();
                                items.Add(primary);
                                CharacterSession.Instance.SaveItem(primary);
                                SM_ITEM_INFO r = new SM_ITEM_INFO()
                                {
                                    Reason = Item.ItemUpdateMethod.Synthesis,
                                    Items = items
                                };
                                Network.SendPacket(r);
                                /*if (primary.InventoryEquipSlot != InventoryEquipSlot.None)
                                {
                                    PC.Status.EquipItem(chara, remove,false);
                                    PC.Status.EquipItem(chara, primary,true);
                                    PC.Status.CalcStatus(chara);
                                    SendPlayerEquiptStats(primary, Utils.FindStat(remove.Synthesis));
                                }*/
                            }
                            else
                            {
                                SM_COMPOSE_BAGUA_FAIL r = new SM_COMPOSE_BAGUA_FAIL()
                                {
                                    Code = 0x148
                                };
                                Network.SendPacket(r);
                            }
                        }
                    }
                }
            }
        }

        public void ChangeBaGua(CM_BAGUA_SET_CHANGE p)
        {
            chara.SendRemove = false;
            while (chara.StillProcess)
            { }
            foreach (KeyValuePair<ushort, byte> i in p.EquipItems)
            {
                Common.Item.Item bagua = chara.Inventory.Container[Containers.Inventory][i.Key];

                if (bagua?.BaseData.ItemType == ItemType.Bagua)
                {
                    OperationResults res = chara.Inventory.EquipItem(bagua, out Common.Item.Item oldItem);
                    if (res != OperationResults.FAILED)
                    {
                        if (oldItem != null)
                        {
                            PC.Status.EquipItem(chara, oldItem,false);
                            List<Common.Item.Item> list = new List<Common.Item.Item>();
                            list.Add(bagua);
                            list.Add(oldItem);
                            SendItemUpdate(ItemUpdateMethod.Move, list);
                            PC.Status.EquipItem(chara, bagua,true);
                        }
                        else
                        {
                            SendItemUpdate(ItemUpdateMethod.Move, bagua);
                            PC.Status.EquipItem(chara, bagua,true);
                        }
                    }
                }
            }

            SM_BAGUA_SET_CHANGE r = new SM_BAGUA_SET_CHANGE();
            Network.SendPacket(r);
            PC.Status.CalcStatus(chara);
            SendPlayerStats();
            UpdateEvent evt = new UpdateEvent()
            {
                Actor = chara,
                Target = chara,
                UpdateType = UpdateTypes.Actor
            };
            evt.AddActorPara(Common.Packets.GameServer.PacketParameter.HP, chara.HP);
            //evt.AddActorPara(Common.Packets.GameServer.PacketParameter.MaxHP, chara.MaxHP);
            map.SendEventToAllActorsWhoCanSeeActor(MapEvents.EVENT_BROADCAST, evt, chara, true);
        }

        public void RemoveBaGua(CM_BAGUA_SET_REMOVE p)
        {
            chara.SendRemove = true;
            chara.StillProcess = true;

            foreach (byte i in p.RemoveItems)
            {
                /*InventoryEquipSlot slot = ((ushort)i).ToInventoryEquipSlot();
                Common.Item.Item item = chara.Inventory.Equipments[slot];
                if ((slot == InventoryEquipSlot.BaoPai1 || slot == InventoryEquipSlot.BaoPai2 || slot == InventoryEquipSlot.BaoPai3 || slot == InventoryEquipSlot.BaoPai4 ||
                    slot == InventoryEquipSlot.BaoPai5 || slot == InventoryEquipSlot.BaoPai6 || slot == InventoryEquipSlot.BaoPai7 || slot == InventoryEquipSlot.BaoPai8) &&
                    item != null)
                {
                    OperationResults res = chara.Inventory.UnequipItem(item);
                    if (res != OperationResults.FAILED)
                    {
                        SendItemUpdate(ItemUpdateMethod.Remove, item);
                        PC.Status.EquipItem(chara, item,false);
                    }
                }*/
            }
            if (chara.SendRemove)
            {
                SM_BAGUA_SET_REMOVE r = new SM_BAGUA_SET_REMOVE();
                Network.SendPacket(r);
                PC.Status.CalcStatus(chara);
                SendPlayerStats();
                UpdateEvent evt = new UpdateEvent()
                {
                    Actor = chara,
                    Target = chara,
                    UpdateType = UpdateTypes.Actor
                };
                evt.AddActorPara(Common.Packets.GameServer.PacketParameter.HP, chara.HP);
                //evt.AddActorPara(Common.Packets.GameServer.PacketParameter.MaxHP, chara.MaxHP);
                map.SendEventToAllActorsWhoCanSeeActor(MapEvents.EVENT_BROADCAST, evt, chara, true);
            }
            chara.StillProcess = false;
        }

        private bool BaGuaSuccess(Common.Item.Item p, Common.Item.Item s)
        {
            Random r = new Random();
            int percent = r.Next(1, 100);
            switch (Math.Abs(p.BaseData.ItemLevel - s.BaseData.ItemLevel))
            {
                case 0:
                case 1:
                case 2:
                case 3:
                case 4:
                    if (percent <= 90)
                    {
                        return true;
                    }

                    break;

                case 5:
                case 6:
                case 7:
                    if (percent <= 60)
                    {
                        return true;
                    }

                    break;

                case 8:
                    if (percent <= 30)
                    {
                        return true;
                    }

                    break;

                case 9:
                    if (percent <= 10)
                    {
                        return true;
                    }

                    break;

                default:
                    break;
            }
            return false;
        }
    }
}
