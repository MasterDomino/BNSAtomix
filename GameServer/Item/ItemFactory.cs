using System;

using SmartEngine.Network.Utils;
using SagaBNS.Common.Item;
using SagaBNS.Common.Actors;

namespace SagaBNS.GameServer.Item
{
    public class ItemFactory : Factory<ItemFactory, ItemData>
    {
        public ItemFactory()
        {
            loadingTab = "Loading item template database";
            loadedTab = " item templates loaded.";
            databaseName = "item Templates";
            FactoryType = FactoryType.XML;
        }

        protected override uint GetKey(ItemData item)
        {
            return item.ItemID;
        }

        protected override void ParseCSV(ItemData item, string[] paras)
        {
            throw new NotImplementedException();
        }

        protected override void ParseXML(System.Xml.XmlElement root, System.Xml.XmlElement current, ItemData item)
        {
            int temp;
            switch (root.Name.ToLower())
            {
                case "item":
                    {
                        switch (current.Name.ToLower())
                        {
                            case "id":
                                item.ItemID = uint.Parse(current.InnerText);
                                break;
                            case "itemtype":
                                item.ItemType = (ItemType)int.Parse(current.InnerText);
                                break;
                            case "equipmentslot":
                                item.EquipSlot = (Common.Inventory.EquipSlot)int.Parse(current.InnerText);
                                break;
                            case "price":
                                item.Price = uint.Parse(current.InnerText);
                                break;
                            case "dispose":
                                item.CanDispose = current.InnerText == "1";
                                break;
                            case "sell":
                                item.CanSell = current.InnerText == "1";
                                break;
                            case "trade":
                                item.CanTrade = current.InnerText == "1";
                                break;
                            case "playerlevel":
                                item.RequiredLevel = byte.Parse(current.InnerText);
                                break;
                            case "itemlevel":
                                item.ItemLevel = short.Parse(current.InnerText);
                                break;
                            case "playerjob":
                                item.RequiredJob = (Common.Actors.Job)byte.Parse(current.InnerText);
                                break;
                            case "grade":
                                item.Grade = byte.Parse(current.InnerText);
                                break;
                            case "synthesisitembopae":
                                item.BaGuaPowderId = uint.Parse(current.InnerText);
                                break;
                            case "synthesiscountbopae":
                                item.BaGuaPowderCount = uint.Parse(current.InnerText);
                                break;
                            case "synthesisitemweapon":
                                item.BaGuaPowderIdWep = uint.Parse(current.InnerText);
                                break;
                            case "synthesisitemcountweapon":
                                item.BaGuaPowderCountWep = uint.Parse(current.InnerText);
                                break;
                            case "baopaisetnumber":
                                item.BaoPaiSetNumber = int.Parse(current.InnerText);
                                break;
                            case "castskill":
                                uint value = uint.Parse(current.InnerText);
                                if (value >= 7000000 && value < 7900000)
                                {
                                    item.CastSkill = value;
                                }

                                break;
                            case "durability":
                                item.MaxDurability = ushort.Parse(current.InnerText);
                                break;
                            case "stackcount":
                                item.MaxStackableCount = int.Parse(current.InnerText);
                                break;
                            case "faction":
                                item.Faction = byte.Parse(current.InnerText);
                                break;
                            case "attackpowerequipmin":
                                temp = int.Parse(current.InnerText);
                                if(temp > 0)
                                {
                                    item.PrimaryStats.Add(Stats.MinAtk, temp);
                                }

                                break;
                            case "attackpowerequipmax":
                                temp = int.Parse(current.InnerText);
                                if(temp > 0)
                                {
                                    item.PrimaryStats.Add(Stats.MaxAtk, temp);
                                }

                                break;
                            case "defendpowerequipvalue":
                                temp = int.Parse(current.InnerText);
                                if(temp > 0)
                                {
                                    item.PrimaryStats.Add(Stats.Defense, temp);
                                }

                                break;
                            case "defendresistpowerequipvalue":
                                temp = int.Parse(current.InnerText);
                                if(temp > 0)
                                {
                                    item.PrimaryStats.Add(Stats.Resist, temp);
                                }

                                break;
                            case "attackhitbasepercent":
                                temp = int.Parse(current.InnerText);
                                if(temp > 0)
                                {
                                    item.PrimaryStats.Add(Stats.HitP, temp);
                                }

                                break;
                            case "attackhitvalue":
                                temp = int.Parse(current.InnerText);
                                if(temp > 0)
                                {
                                    item.PrimaryStats.Add(Stats.HitB, temp);
                                }

                                break;
                            case "attackpiercevalue":
                                temp = int.Parse(current.InnerText);
                                if(temp > 0)
                                {
                                    item.PrimaryStats.Add(Stats.PrcB, temp);
                                }

                                break;
                            case "attackcriticalbasepercent":
                                temp = int.Parse(current.InnerText);
                                if(temp > 0)
                                {
                                    item.PrimaryStats.Add(Stats.CritP, temp);
                                }

                                break;
                            case "attackcriticalvalue":
                                temp = int.Parse(current.InnerText);
                                if(temp > 0)
                                {
                                    item.PrimaryStats.Add(Stats.CritB, temp);
                                }

                                break;
                            case "defendcriticalbasepercent":
                                temp = int.Parse(current.InnerText);
                                if(temp > 0)
                                {
                                    item.PrimaryStats.Add(Stats.DefCritP, temp);
                                }

                                break;
                            case "defendcriticalvalue":
                                temp = int.Parse(current.InnerText);
                                if(temp > 0)
                                {
                                    item.PrimaryStats.Add(Stats.DefCritB, temp);
                                }

                                break;
                            case "defenddodgebasepercent":
                                temp = int.Parse(current.InnerText);
                                if(temp > 0)
                                {
                                    item.PrimaryStats.Add(Stats.AvoidP, temp);
                                }

                                break;
                            case "defenddodgevalue":
                                temp = int.Parse(current.InnerText);
                                if(temp > 0)
                                {
                                    item.PrimaryStats.Add(Stats.AvoidB, temp);
                                }

                                break;
                            case "defendparrybasepercent":
                                temp = int.Parse(current.InnerText);
                                if(temp > 0)
                                {
                                    item.PrimaryStats.Add(Stats.ParryP, temp);
                                }

                                break;
                            case "defendparryvalue":
                                temp = int.Parse(current.InnerText);
                                if(temp > 0)
                                {
                                    item.PrimaryStats.Add(Stats.ParryB, temp);
                                }

                                break;
                            case "attackstiffdurationbasepercent":
                                temp = int.Parse(current.InnerText);
                                if(temp > 0)
                                {
                                    item.PrimaryStats.Add(Stats.AttackStiffDurationBasePercent, temp);
                                }

                                break;
                            case "attackstiffdurationvalue":
                                temp = int.Parse(current.InnerText);
                                if(temp > 0)
                                {
                                    item.PrimaryStats.Add(Stats.AttackStiffDurationValue, temp);
                                }

                                break;
                            case "defendstiffdurationbasepercent":
                                temp = int.Parse(current.InnerText);
                                if(temp > 0)
                                {
                                    item.PrimaryStats.Add(Stats.DefendStiffDurationBasePercent, temp);
                                }

                                break;
                            case "defendstiffdurationvalue":
                                temp = int.Parse(current.InnerText);
                                if(temp > 0)
                                {
                                    item.PrimaryStats.Add(Stats.DefendStiffDurationValue, temp);
                                }

                                break;
                            case "castdurationbasepercent":
                                temp = int.Parse(current.InnerText);
                                if(temp > 0)
                                {
                                    item.PrimaryStats.Add(Stats.CastTimeP, temp);
                                }

                                break;
                            case "castdurationvalue":
                                temp = int.Parse(current.InnerText);
                                if(temp > 0)
                                {
                                    item.PrimaryStats.Add(Stats.CastTimeB, temp);
                                }

                                break;
                            case "defendphysicaldamagereducepercent":
                                temp = int.Parse(current.InnerText);
                                if(temp > 0)
                                {
                                    item.PrimaryStats.Add(Stats.DefPhysDmgP, temp);
                                }

                                break;
                            case "defendforcedamagereducepercent":
                                temp = int.Parse(current.InnerText);
                                if(temp > 0)
                                {
                                    item.PrimaryStats.Add(Stats.DefForceDmgP, temp);
                                }

                                break;
                            case "attackdamagemodifypercent":
                                temp = int.Parse(current.InnerText);
                                if(temp > 0)
                                {
                                    item.PrimaryStats.Add(Stats.AttackDamageModifyPercent, temp);
                                }

                                break;
                            case "attackdamagemodifydiff":
                                temp = int.Parse(current.InnerText);
                                if(temp > 0)
                                {
                                    item.PrimaryStats.Add(Stats.AttackDamageModifyDiff, temp);
                                }

                                break;
                            case "defenddamagemodifypercent":
                                temp = int.Parse(current.InnerText);
                                if(temp > 0)
                                {
                                    item.PrimaryStats.Add(Stats.DefendDamageModifyPercent, temp);
                                }

                                break;
                            case "defenddamagemodifydiff":
                                temp = int.Parse(current.InnerText);
                                if(temp > 0)
                                {
                                    item.PrimaryStats.Add(Stats.DefendDamageModifyDiff, temp);
                                }

                                break;
                            case "maxhp":
                                temp = int.Parse(current.InnerText);
                                if(temp > 0)
                                {
                                    item.PrimaryStats.Add(Stats.MaxHp, temp);
                                }

                                break;
                            case "maxfp":
                                temp = int.Parse(current.InnerText);
                                if(temp > 0)
                                {
                                    item.PrimaryStats.Add(Stats.MaxFp, temp);
                                }

                                break;
                            case "hpregen":
                                temp = int.Parse(current.InnerText);
                                if(temp > 0)
                                {
                                    item.PrimaryStats.Add(Stats.HPRegen, temp);
                                }

                                break;
                            case "fpregen":
                                temp = int.Parse(current.InnerText);
                                if(temp > 0)
                                {
                                    item.PrimaryStats.Add(Stats.FPRegen, temp);
                                }

                                break;
                            case "fpregencombat":
                                temp = int.Parse(current.InnerText);
                                if(temp > 0)
                                {
                                    item.PrimaryStats.Add(Stats.FpRegenCombat, temp);
                                }

                                break;
                            case "bpattackpowerequipmin":
                                temp = int.Parse(current.InnerText);
                                if(temp > 0)
                                {
                                    item.SecondaryStats.Add(Stats.MinAtk, temp);
                                }

                                break;
                            case "bpattackpowerequipmax":
                                temp = int.Parse(current.InnerText);
                                if(temp > 0)
                                {
                                    item.SecondaryStats.Add(Stats.MaxAtk, temp);
                                }

                                break;
                            case "bpdefendpowerequipvalue":
                                temp = int.Parse(current.InnerText);
                                if(temp > 0)
                                {
                                    item.SecondaryStats.Add(Stats.Defense, temp);
                                }

                                break;
                            case "bpdefendresistpowerequipvalue":
                                temp = int.Parse(current.InnerText);
                                if(temp > 0)
                                {
                                    item.SecondaryStats.Add(Stats.Resist, temp);
                                }

                                break;
                            case "bpattackhitbasepercent":
                                temp = int.Parse(current.InnerText);
                                if(temp > 0)
                                {
                                    item.SecondaryStats.Add(Stats.HitP, temp);
                                }

                                break;
                            case "bpattackhitvalue":
                                temp = int.Parse(current.InnerText);
                                if(temp > 0)
                                {
                                    item.SecondaryStats.Add(Stats.HitB, temp);
                                }

                                break;
                            case "bpattackpiercevalue":
                                temp = int.Parse(current.InnerText);
                                if(temp > 0)
                                {
                                    item.SecondaryStats.Add(Stats.PrcB, temp);
                                }

                                break;
                            case "bpattackcriticalbasepercent":
                                temp = int.Parse(current.InnerText);
                                if(temp > 0)
                                {
                                    item.SecondaryStats.Add(Stats.CritP, temp);
                                }

                                break;
                            case "bpattackcriticalvalue":
                                temp = int.Parse(current.InnerText);
                                if(temp > 0)
                                {
                                    item.SecondaryStats.Add(Stats.CritB, temp);
                                }

                                break;
                            case "bpdefendcriticalbasepercent":
                                temp = int.Parse(current.InnerText);
                                if(temp > 0)
                                {
                                    item.SecondaryStats.Add(Stats.DefCritP, temp);
                                }

                                break;
                            case "bpdefendcriticalvalue":
                                temp = int.Parse(current.InnerText);
                                if(temp > 0)
                                {
                                    item.SecondaryStats.Add(Stats.DefCritB, temp);
                                }

                                break;
                            case "bpdefenddodgebasepercent":
                                temp = int.Parse(current.InnerText);
                                if(temp > 0)
                                {
                                    item.SecondaryStats.Add(Stats.AvoidP, temp);
                                }

                                break;
                            case "bpdefenddodgevalue":
                                temp = int.Parse(current.InnerText);
                                if(temp > 0)
                                {
                                    item.SecondaryStats.Add(Stats.AvoidB, temp);
                                }

                                break;
                            case "bpdefendparrybasepercent":
                                temp = int.Parse(current.InnerText);
                                if(temp > 0)
                                {
                                    item.SecondaryStats.Add(Stats.ParryP, temp);
                                }

                                break;
                            case "bpdefendparryvalue":
                                temp = int.Parse(current.InnerText);
                                if(temp > 0)
                                {
                                    item.SecondaryStats.Add(Stats.ParryB, temp);
                                }

                                break;
                            case "bpattackstiffdurationbasepercent":
                                temp = int.Parse(current.InnerText);
                                if(temp > 0)
                                {
                                    item.SecondaryStats.Add(Stats.AttackStiffDurationBasePercent, temp);
                                }

                                break;
                            case "bpattackstiffdurationvalue":
                                temp = int.Parse(current.InnerText);
                                if(temp > 0)
                                {
                                    item.SecondaryStats.Add(Stats.AttackStiffDurationValue, temp);
                                }

                                break;
                            case "bpdefendstiffdurationbasepercent":
                                temp = int.Parse(current.InnerText);
                                if(temp > 0)
                                {
                                    item.SecondaryStats.Add(Stats.DefendStiffDurationBasePercent, temp);
                                }

                                break;
                            case "bpdefendstiffdurationvalue":
                                temp = int.Parse(current.InnerText);
                                if(temp > 0)
                                {
                                    item.SecondaryStats.Add(Stats.DefendStiffDurationValue, temp);
                                }

                                break;
                            case "bpcastdurationbasepercent":
                                temp = int.Parse(current.InnerText);
                                if(temp > 0)
                                {
                                    item.SecondaryStats.Add(Stats.CastTimeP, temp);
                                }

                                break;
                            case "bpcastdurationvalue":
                                temp = int.Parse(current.InnerText);
                                if(temp > 0)
                                {
                                    item.SecondaryStats.Add(Stats.CastTimeB, temp);
                                }

                                break;
                            case "bpdefendphysicaldamagereducepercent":
                                temp = int.Parse(current.InnerText);
                                if(temp > 0)
                                {
                                    item.SecondaryStats.Add(Stats.DefPhysDmgP, temp);
                                }

                                break;
                            case "bpdefendforcedamagereducepercent":
                                temp = int.Parse(current.InnerText);
                                if(temp > 0)
                                {
                                    item.SecondaryStats.Add(Stats.DefForceDmgP, temp);
                                }

                                break;
                            case "bpattackdamagemodifypercent":
                                temp = int.Parse(current.InnerText);
                                if(temp > 0)
                                {
                                    item.SecondaryStats.Add(Stats.AttackDamageModifyPercent, temp);
                                }

                                break;
                            case "bpattackdamagemodifydiff":
                                temp = int.Parse(current.InnerText);
                                if(temp > 0)
                                {
                                    item.SecondaryStats.Add(Stats.AttackDamageModifyDiff, temp);
                                }

                                break;
                            case "bpdefenddamagemodifypercent":
                                temp = int.Parse(current.InnerText);
                                if(temp > 0)
                                {
                                    item.SecondaryStats.Add(Stats.DefendDamageModifyPercent, temp);
                                }

                                break;
                            case "bpdefenddamagemodifydiff":
                                temp = int.Parse(current.InnerText);
                                if(temp > 0)
                                {
                                    item.SecondaryStats.Add(Stats.DefendDamageModifyDiff, temp);
                                }

                                break;
                            case "bpmaxhp":
                                temp = int.Parse(current.InnerText);
                                if(temp > 0)
                                {
                                    item.SecondaryStats.Add(Stats.MaxHp, temp);
                                }

                                break;
                            case "bpmaxfp":
                                temp = int.Parse(current.InnerText);
                                if(temp > 0)
                                {
                                    item.SecondaryStats.Add(Stats.MaxFp, temp);
                                }

                                break;
                            case "bphpregen":
                                temp = int.Parse(current.InnerText);
                                if(temp > 0)
                                {
                                    item.SecondaryStats.Add(Stats.HPRegen, temp);
                                }

                                break;
                            case "bpfpregen":
                                temp = int.Parse(current.InnerText);
                                if(temp > 0)
                                {
                                    item.SecondaryStats.Add(Stats.FPRegen, temp);
                                }

                                break;
                            case "bpfpregencombat":
                                temp = int.Parse(current.InnerText);
                                if(temp > 0)
                                {
                                    item.SecondaryStats.Add(Stats.FpRegenCombat, temp);
                                }

                                break;
                        }
                    }
                    break;
            }
        }

        public Common.Item.Item CreateNewItem(uint itemID)
        {
            if (items.ContainsKey(itemID))
            {
                Common.Item.Item item = new Common.Item.Item(this[itemID])
                {
                    Count = 1
                };
                return item;
            }
            else
            {
                return null;
            }
        }
    }
}
