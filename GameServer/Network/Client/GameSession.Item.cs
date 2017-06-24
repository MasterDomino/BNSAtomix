using System;
using System.Collections.Generic;
using System.Threading;

using SmartEngine.Core;
using SmartEngine.Network;
using SagaBNS.Common.Actors;
using SagaBNS.Common.Packets;
using SagaBNS.Common.Skills;
using SagaBNS.Common.Inventory;
using SagaBNS.Common.Item;
using SagaBNS.GameServer.Skills;
using SagaBNS.GameServer.Skills.SkillHandlers.Common;
using SagaBNS.GameServer.Packets.Client;
using SagaBNS.GameServer.Network.CharacterServer;
using SagaBNS.GameServer.Item;

namespace SagaBNS.GameServer.Network.Client
{
    public partial class GameSession : Session<GamePacketOpcode>
    {
        private List<Common.Item.Item> duplicateEquipSlots = new List<Common.Item.Item>();
        public void SendItemUpdate(Item.ItemUpdateMethod reason, Common.Item.Item item)
        {
            SM_ITEM_INFO p = new SM_ITEM_INFO();
            List<Common.Item.Item> list = new List<Common.Item.Item>();
            list.Add(item);
            p.Reason = reason;
            p.Items = list;

            Network.SendPacket(p);
        }

        public void SendItemUpdate(Item.ItemUpdateMethod reason, List<Common.Item.Item> list)
        {
            SM_ITEM_INFO p = new SM_ITEM_INFO()
            {
                Reason = reason,
                Items = list
            };
            Logger.Log.Debug(p.DumpData());
            Network.SendPacket(p);
        }

        public void LoadInventory(CM_ITEM_LOAD_INVENTORY p)
        {
            SendItemList();
        }

        public void SendItemList()
        {
            List<Common.Item.Item> items = new List<Common.Item.Item>();
            foreach (Containers c in Enum.GetValues(typeof(Containers)))
            {
                foreach (Common.Item.Item i in chara.Inventory.Container[c])
                {
                    if (i != null)
                    {
                        items.Add(i);
                    }
                }
            }
            if (items.Count > 0)
            {
                SM_ITEM_INFO p = new SM_ITEM_INFO()
                {
                    Reason = ItemUpdateMethod.List,
                    Items = items
                };
                Network.SendPacket(p);
            }
        }

        public void OnItemDrop(CM_ITEM_DROP p)
        {
            RemoveItemSlot((byte)p.SlotID, p.Count);
        }

        public void OnHammerRepair(CM_HAMMER_REPAIR p)
        {
            if (map.Campfires[p.CampfireID] is ActorMapObj campfire)
            {
                Dictionary<Common.Item.Item, ushort> items = new Dictionary<Common.Item.Item, ushort>();
                Common.Item.Item add;
                ushort total = 0;
                foreach (KeyValuePair<ushort, ushort> i in p.ExchangeItems)
                {
                    add = Character.Inventory.Container[Containers.Inventory][i.Key];
                    if (add != null)
                    {
                        if (add.Count >= i.Value && add.BaseData.ItemType == ItemType.RepairKit)//TODO: Find which Repair Kits go with which items
                        {
                            items.Add(add, i.Value);
                            total += i.Value;
                        }
                        else
                        {
                            break;
                        }
                    }
                }
                if (total == 1)
                {
                    Repair repair = new Repair();
                    SkillArg arg = new SkillArg()
                    {
                        Caster = chara,
                        Target = campfire
                    };
                    repair.HandleSkillActivate(arg, items);
                }
            }
        }

        public void OnItemBuy(CM_ITEM_BUY p)
        {
            if (map.GetActor(p.ActorID) is ActorNPC npc)
            {
                if (NPC.NPCStoreFactory.Instance.Stores.ContainsKey(npc.BaseData.StoreID))
                {
                    NPC.NPCStore store = NPC.NPCStoreFactory.Instance[npc.BaseData.StoreID];
                    foreach (KeyValuePair<byte, ushort> i in p.Items)
                    {
                        if (i.Key < store.Items.Length)
                        {
                            Common.Item.Item item = ItemFactory.Instance.CreateNewItem(store.Items[i.Key]);
                            if (item != null)
                            {
                                int price = (int)(item.BaseData.Price * store.BuyRate) * i.Value;
                                if (chara.Gold > price)
                                {
                                    Interlocked.Add(ref chara.Gold, -price);
                                    item.Count = i.Value;
                                    AddItem(item);
                                    SendPlayerGold();
                                }
                            }
                        }
                    }
                }
            }
        }

        public void OnItemBuy(CM_ITEM_EXCHANGE p)
        {
            if (map.GetActor(p.ActorID) is ActorNPC npc)
            {
                if (NPC.NPCStoreFactory.Instance.StoresByItem.ContainsKey(npc.BaseData.StoreByItemID))
                {
                    NPC.NPCStore store = NPC.NPCStoreFactory.Instance.StoresByItem[npc.BaseData.StoreByItemID];
                    if (store != null)
                    {
                        Common.Item.Item item = ItemFactory.Instance.CreateNewItem(store.Items[p.BuySlot]);
                        Dictionary<Common.Item.Item, ushort> items = new Dictionary<Common.Item.Item, ushort>();
                        ushort total = 0;
                        if (item != null)
                        {
                            Common.Item.Item add;
                            foreach (KeyValuePair<ushort, ushort> i in p.ExchangeItems)
                            {
                                add = Character.Inventory.Container[Containers.Inventory][i.Key];
                                if (add != null)
                                {
                                    if (add.Count >= i.Value && add.ItemID == store.Materials[p.BuySlot])
                                    {
                                        items.Add(add, i.Value);
                                        total += add.Count;
                                    }
                                    else
                                    {
                                        break;
                                    }
                                }
                            }

                            if (total >= p.BuyCount * store.MaterialCounts[p.BuySlot])
                            {
                                foreach (KeyValuePair<Common.Item.Item, ushort> i in items)
                                {
                                    RemoveItemSlot(i.Key.SlotID, i.Value);
                                }
                                AddItem(item);
                            }
                        }
                    }
                }
            }
        }

        public void OnItemBuyBack(CM_ITEM_BUYBACK p)
        {
            if (map.GetActor(p.ActorID) is ActorNPC npc)
            {
                if (NPC.NPCStoreFactory.Instance.Stores.ContainsKey(npc.BaseData.StoreID))
                {
                    NPC.NPCStore store = NPC.NPCStoreFactory.Instance[npc.BaseData.StoreID];
                    if (p.Slot < chara.Inventory.SoldItems.Count)
                    {
                        Common.Item.Item item = chara.Inventory.SoldItems[p.Slot];
                        if (item != null)
                        {
                            int price = (int)(item.BaseData.Price * store.BuyBackRate) * item.Count;
                            if (chara.Gold > price)
                            {
                                Interlocked.Add(ref chara.Gold, -price);
                                chara.Inventory.SoldItems.RemoveAt(p.Slot);
                                OperationResults res = chara.Inventory.AddItem(Containers.Inventory, item, out List<Common.Item.Item> affected);
                                if (affected.Count > 0)
                                {
                                    CharacterSession.Instance.SaveItem(affected);
                                }

                                if (res != OperationResults.FAILED)
                                {
                                    if (item.Count > 0)
                                    {
                                        affected.Add(item);
                                    }

                                    switch (res)
                                    {
                                        case OperationResults.NEW_INDEX:
                                            SendItemUpdate(ItemUpdateMethod.Add, affected);
                                            break;
                                        case OperationResults.STACK_UPDATE:
                                            SendItemUpdate(ItemUpdateMethod.Update, affected);
                                            break;
                                    }
                                    if (res == OperationResults.NEW_INDEX && item.Count > 0)
                                    {
                                        CharacterSession.Instance.SaveItem(item);
                                    }
                                    if (item.Count == 0)
                                    {
                                        List<Common.Item.Item> remove = new List<Common.Item.Item>();
                                        remove.Add(item);
                                        CharacterSession.Instance.DeleteItem(remove);
                                    }
                                }
                                SendPlayerGold();
                                SendItemBuyBackList();
                            }
                        }
                    }
                }
            }
        }

        public void OnItemSell(CM_ITEM_SELL p)
        {
            Dictionary<ushort, ushort> items = p.Items;
            List<Common.Item.Item> shouldRemove = new List<Common.Item.Item>();
            foreach (ushort i in items.Keys)
            {
                Common.Item.Item review = chara.Inventory.Container[Containers.Inventory][i];
                if (review != null)
                {
                    if (review.Count >= items[i] && items[i] > 0 && review.Container == Containers.Inventory)
                    {
                        chara.Inventory.RemoveItemSlot(Containers.Inventory, i, items[i], out List<Common.Item.Item> updated, out List<Common.Item.Item> removed);
                        if (removed.Count > 0)
                        {
                            SendItemUpdate(ItemUpdateMethod.Sold, removed);
                        }

                        if (updated.Count > 0)
                        {
                            SendItemUpdate(ItemUpdateMethod.Sold, updated);
                        }

                        uint price = 0;
                        foreach (Common.Item.Item item in removed)
                        {
                            price = item.BaseData.Price;
                            item.SlotID = 255;
                        }
                        CharacterSession.Instance.SaveItem(removed);
                        foreach (Common.Item.Item item in updated)
                        {
                            Common.Item.Item newItem = ItemFactory.Instance.CreateNewItem(item.ItemID);
                            newItem.CharID = chara.CharID;
                            newItem.Count = items[i];
                            newItem.SlotID = 255;
                            CharacterSession.Instance.CreateItem(newItem);
                            removed.Add(newItem);
                            price = newItem.BaseData.Price;
                        }
                        price *= items[i];
                        Interlocked.Add(ref chara.Gold, (int)price);
                        SendPlayerGold();
                        foreach (Common.Item.Item item in removed)
                        {
                            item.Count = items[i];
                            lock (chara.Inventory.SoldItems)
                            {
                                while (chara.Inventory.SoldItems.Count >= 14)
                                {
                                    shouldRemove.Add(chara.Inventory.SoldItems[0]);
                                    chara.Inventory.SoldItems.RemoveAt(0);
                                }
                                chara.Inventory.SoldItems.Add(item);
                            }
                        }
                    }
                    else
                    {
                        SM_SERVER_MESSAGE r = new SM_SERVER_MESSAGE()
                        {
                            MessagePosition = SM_SERVER_MESSAGE.Positions.ChatWindow,
                            Message = "Bug fixed, better luck next time."
                        };
                        Network.SendPacket(r);
                    }
                }
            }
            if (shouldRemove.Count > 0)
            {
                CharacterSession.Instance.DeleteItem(shouldRemove);
            }

            SendItemBuyBackList();
        }

        public void OnItemMove(CM_ITEM_MOVE p)
        {
            ActorNPC npc = map.GetActor(p.ActorID) as ActorNPC;
            bool warehouse = false;

            if (p.SlotID > 79 || p.TargetSlot > 79)
            {
                if (npc != null)
                {
                    if (chara.DistanceToPoint(npc.X, npc.Y, npc.Z) > 100)
                    {
                        return;
                    }
                    else
                    {
                        warehouse = true;
                    }
                }
            }

            switch (chara.Inventory.MoveItem(Containers.Inventory,p.SlotID, p.Count, Containers.Inventory, p.TargetSlot, out List<Common.Item.Item> affected, out List<Common.Item.Item> deleted))
            {
                case OperationResults.OK:
                    SendItemUpdate(ItemUpdateMethod.Delete, affected);
                    CharacterSession.Instance.SaveItem(affected);
                    if (deleted.Count > 0)
                    {
                        SendItemUpdate(ItemUpdateMethod.Delete, deleted);
                        CharacterSession.Instance.DeleteItem(deleted);
                    }
                    break;
                case OperationResults.NEW_INDEX:
                    foreach (Common.Item.Item i in affected)
                    {
                        ushort newIdx = i.SlotID;
                        ushort newCount = i.Count;
                        i.SlotID = (byte)p.SlotID;
                        i.Count = 0;
                        SendItemUpdate(ItemUpdateMethod.Delete, i);
                        i.SlotID = newIdx;
                        i.Count = newCount;
                        SendItemUpdate(ItemUpdateMethod.Delete, i);
                        CharacterSession.Instance.SaveItem(affected);
                    }
                    break;
            }
        }

        public void SendItemBuyBackList()
        {
            SM_ITEM_BUYBACK_LIST p1 = new SM_ITEM_BUYBACK_LIST()
            {
                Items = chara.Inventory.SoldItems
            };
            Network.SendPacket(p1);
        }

        public void OnItemUse(CM_ITEM_USE p)
        {
            chara.Inventory.RemoveItemSlot(Containers.Inventory, (byte)p.SlotID, 1, out List<Common.Item.Item> updated, out List<Common.Item.Item> removed);
            Common.Item.Item item;
            if (updated.Count > 0)
            {
                SendItemUpdate(ItemUpdateMethod.Use, updated);
                CharacterSession.Instance.SaveItem(updated);
                item = updated[0];
            }
            else
            {
                SendItemUpdate(ItemUpdateMethod.Use, removed);
                CharacterSession.Instance.DeleteItem(removed);
                item = removed[0];
            }
            if (item?.BaseData.CastSkill > 0)
            {
                if (SkillFactory.Instance.Items.ContainsKey(item.BaseData.CastSkill) && item.BaseData.ItemType == ItemType.Food) //&& SkillFactory.Instance.CreateNewSkill(item.BaseData.CastSkill).BaseData.Duration != 0)
                {
                    SkillArg arg = new SkillArg()
                    {
                        Caster = chara,
                        Target = chara,
                        Skill = SkillFactory.Instance.CreateNewSkill(item.BaseData.CastSkill),
                        Dir = chara.Dir,
                        SkillSession = (byte)Global.Random.Next(0, 255)
                    };
                    SkillManager.Instance.SkillCast(arg);
                }
                else
                {
                    string str = "Item {0} tried to cast skill {1}.";
                    Logger.Log.Debug(string.Format(str,item.ItemID,item.BaseData.CastSkill));
                }
            }
        }

        public void OnItemUnequip(CM_ITEM_UNEQUIP p)
        {
            /*Common.Item.Item item = chara.Inventory.Equipments[p.SlotID.ToInventoryEquipSlot()];
            if (item != null)
            {
                OperationResults res = chara.Inventory.UnequipItem(item);
                if (res != OperationResults.FAILED)
                {
                    SendItemUpdate(ItemUpdateMethod.Move, item);
                    PC.Status.EquipItem(chara, item,false);
                    PC.Status.CalcStatus(chara);
                    SendPlayerEquiptStats(item,Stats.None,true);
                    UpdateEvent evt = new UpdateEvent();
                    evt.Actor = chara;
                    evt.Target = chara;
                    evt.UpdateType = UpdateTypes.Actor;
                    switch (item.BaseData.EquipSlot)
                    {
                        case EquipSlot.Weapon:
                            {
                                //evt.AddActorPara(PacketParameter.Weapon, 0);
                            }
                            break;
                        case EquipSlot.Costume:
                            {
                                //evt.AddActorPara(PacketParameter.Costume, 0);
                            }
                            break;
                        case EquipSlot.Eyes:
                            {
                                //evt.AddActorPara(PacketParameter.Eyewear, 0);
                            }
                            break;
                        case EquipSlot.Hat:
                            {
                                //evt.AddActorPara(PacketParameter.Hat, 0);
                            }
                            break;
                        case EquipSlot.CostumeAccessory:
                            {
                                //evt.AddActorPara(PacketParameter.CostumeAccessory, 0);
                            }
                            break;
                    }
                    map.SendEventToAllActorsWhoCanSeeActor(MapEvents.EVENT_BROADCAST, evt, chara, true);
                }
            }*/
        }

        public void OnItemEquip(CM_ITEM_EQUIP p)
        {
            /*Common.Item.Item item = null;
            if(p.SlotID < chara.Inventory.ListSize)
                item = chara.Inventory.Container[p.SlotID];
            if (item != null)
            {
                Common.Item.Item oldItem;
                OperationResults res = chara.Inventory.EquipItem(item, out oldItem, p.EquipSlot);
                if (res != OperationResults.FAILED)
                {
                    if (oldItem != null)
                    {
                        PC.Status.EquipItem(chara, oldItem,false);
                        PC.Status.CalcStatus(chara);
                        SendPlayerEquiptStats(oldItem);
                        List<Common.Item.Item> list = new List<Common.Item.Item>();
                        list.Add(item);
                        list.Add(oldItem);
                        SendItemUpdate(ItemUpdateMethod.Move, list);
                    }
                    else
                        SendItemUpdate(ItemUpdateMethod.Move, item);
                    PC.Status.EquipItem(chara, item,true);
                    PC.Status.CalcStatus(chara);
                    SendPlayerEquiptStats(item,Stats.None,true);
                    UpdateEvent evt = new UpdateEvent();
                    evt.Actor = chara;
                    evt.Target = chara;
                    evt.UpdateType = UpdateTypes.Actor;
                    switch (item.BaseData.EquipSlot)
                    {
                        case EquipSlot.Weapon:
                            {
                                //evt.AddActorPara(PacketParameter.Weapon, item.ItemID);
                            }
                            break;
                        case EquipSlot.Costume:
                            {
                                //evt.AddActorPara(PacketParameter.Costume, item.ItemID);
                            }
                            break;
                        case EquipSlot.Eyes:
                            {
                                //evt.AddActorPara(PacketParameter.Eyewear, item.ItemID);
                            }
                            break;
                        case EquipSlot.Hat:
                            {
                                //evt.AddActorPara(PacketParameter.Hat, item.ItemID);
                            }
                            break;
                        case EquipSlot.CostumeAccessory:
                            {
                                //evt.AddActorPara(PacketParameter.CostumeAccessory, item.ItemID);
                            }
                            break;
                    }
                    map.SendEventToAllActorsWhoCanSeeActor(MapEvents.EVENT_BROADCAST, evt, chara, true);
                    
                }
            }*/
        }

        public void AddItem(Common.Item.Item item,Containers container= Containers.Inventory)
        {
            item.CharID = chara.CharID;
            OperationResults res = chara.Inventory.AddItem(container, item, out List<Common.Item.Item> affected);
            if (affected.Count > 0)
            {
                CharacterSession.Instance.SaveItem(affected);
            }

            if (res != OperationResults.FAILED)
            {
                if (item.Count > 0)
                {
                    affected.Add(item);
                }

                switch (res)
                {
                    case OperationResults.NEW_INDEX:
                        SendItemUpdate(ItemUpdateMethod.Add, affected);
                        break;
                    case OperationResults.STACK_UPDATE:
                        SendItemUpdate(ItemUpdateMethod.Update, affected);
                        break;
                }
                if (res == OperationResults.NEW_INDEX && item.Count > 0)
                {
                    CharacterSession.Instance.CreateItem(item);
                }
            }
        }

        public Common.Item.Item AddItem(uint itemID, ushort count = 1, Containers container= Containers.Inventory)
        {
            Common.Item.Item item = ItemFactory.Instance.CreateNewItem(itemID);
            item.CharID = chara.CharID;
            item.Container = container;
            item.Count = count;
            OperationResults res = chara.Inventory.AddItem(container, item, out List<Common.Item.Item> affected);
            if (affected.Count > 0)
            {
                CharacterSession.Instance.SaveItem(affected);
            }

            if (res != OperationResults.FAILED)
            {
                if (item.Count > 0)
                {
                    affected.Add(item);
                }

                switch (res)
                {
                    case OperationResults.NEW_INDEX:
                        SendItemUpdate(ItemUpdateMethod.Add, affected);
                        break;
                    case OperationResults.STACK_UPDATE:
                        SendItemUpdate(ItemUpdateMethod.Update, affected);
                        break;
                }
                if (res == OperationResults.NEW_INDEX && item.Count > 0)
                {
                    CharacterSession.Instance.CreateItem(item);
                }
            }
            return item;
        }

        public int RemoveItemSlot(ushort slot, ushort count,Containers container= Containers.Inventory)
        {
            chara.Inventory.RemoveItemSlot(container,slot, count, out List<Common.Item.Item> updated, out List<Common.Item.Item> removed);
            SendItemUpdate(ItemUpdateMethod.Update, updated);
            CharacterSession.Instance.SaveItem(updated);
            SendItemUpdate(ItemUpdateMethod.Update, removed);
            CharacterSession.Instance.DeleteItem(removed);
            return removed.Count + updated.Count;
        }

        public void RemoveItemsBySlot(Dictionary<Common.Item.Item, ushort> items, Containers container = Containers.Inventory)
        {
            List<Common.Item.Item> total = new List<Common.Item.Item>();
            foreach (KeyValuePair<Common.Item.Item, ushort> i in items)
            {
                chara.Inventory.RemoveItemSlot(container, i.Key.SlotID, i.Value, out List<Common.Item.Item> updated, out List<Common.Item.Item> removed);
                total.AddRange(updated.ToArray());
                total.AddRange(removed.ToArray());
                CharacterSession.Instance.SaveItem(updated);
                CharacterSession.Instance.DeleteItem(removed);
            }
            SendItemUpdate(ItemUpdateMethod.Use, total);
        }

        public void RemoveItem(uint itemID, ushort count, Containers container = Containers.Inventory)
        {
            chara.Inventory.RemoveItem(container,itemID, count, out List<Common.Item.Item> updated, out List<Common.Item.Item> removed);
            SendItemUpdate(ItemUpdateMethod.Update, updated);
            CharacterSession.Instance.SaveItem(updated);
            SendItemUpdate(ItemUpdateMethod.Update, removed);
            CharacterSession.Instance.DeleteItem(removed);
        }

        public void OnGotInventoryItem(Common.Item.Item item, bool end)
        {
            if (item != null && chara != null)
            {
                Common.Item.Item newItem = ItemFactory.Instance.CreateNewItem(item.ItemID);
                if (newItem == null || newItem.BaseData == null)
                {
                    Logger.Log.Warn("Item or BaseData == null for ID:" + item.ID + ",ItemID:" + item.ItemID);
                }
                else
                {
                    newItem.CharID = item.CharID;
                    newItem.ID = item.ID;
                    newItem.SlotID = item.SlotID;
                    //newItem.InventoryEquipSlot = item.InventoryEquipSlot;
                    newItem.Count = item.Count;
                    newItem.Synthesis = item.Synthesis;
                    if (newItem.SlotID == 255)
                    {
                        lock (chara.Inventory.SoldItems)
                        {
                            chara.Inventory.SoldItems.Add(newItem);
                        }
                    }
                    else
                    {
                        /*chara.Inventory.Container[newItem.SlotID] = newItem;
                        if (item.InventoryEquipSlot != InventoryEquipSlot.None)
                        {
                            if (chara.Inventory.Equipments[newItem.InventoryEquipSlot] != null)
                            {
                                newItem.InventoryEquipSlot = InventoryEquipSlot.None;
                                duplicateEquipSlots.Add(newItem);
                            }
                            else
                            {
                                chara.Inventory.Equipments[newItem.InventoryEquipSlot] = newItem;
                                PC.Status.EquipItem(chara, newItem,true);
                            }
                        }*/
                    }
                }
            }
            if (end)
            {
                CharacterSession.Instance.SaveItem(duplicateEquipSlots);
                duplicateEquipSlots = chara.Inventory.CheckDuplicateSlot();
                CharacterSession.Instance.DeleteItem(duplicateEquipSlots);
                duplicateEquipSlots = null;
                CharacterSession.Instance.GetQuests(chara.CharID, this);
            }
        }
    }
}
