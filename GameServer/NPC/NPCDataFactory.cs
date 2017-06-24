using System;

using SmartEngine.Network.Utils;
using SagaBNS.Common.Actors;

namespace SagaBNS.GameServer.NPC
{
    public class NPCDataFactory : Factory<NPCDataFactory, NPCData>
    {
        public NPCDataFactory()
        {
            loadingTab = "Loading npc template database";
            loadedTab = " npc templates loaded.";
            databaseName = "NPC Templates";
            FactoryType = FactoryType.XML;
        }

        protected override uint GetKey(NPCData item)
        {
            return item.NpcID;
        }

        protected override void ParseCSV(NPCData item, string[] paras)
        {
            throw new NotImplementedException();
        }

        protected override void ParseXML(System.Xml.XmlElement root, System.Xml.XmlElement current, NPCData item)
        {
            switch (root.Name.ToLower())
            {
                case "npc":
                    {
                        switch (current.Name.ToLower())
                        {
                            case "id":
                                item.NpcID = ushort.Parse(current.InnerText);
                                break;
                            case "name":
                                item.Name = current.InnerText;
                                break;
                            case "questid":
                                {
                                    foreach (string i in current.InnerText.Split(','))
                                    {
                                        if (i != "0")
                                        {
                                            item.QuestIDs.Add(ushort.Parse(i));
                                        }
                                    }
                                }
                                break;
                            case "queststep":
                                {
                                    string[] token = current.InnerText.Split(',');
                                    item.QuestSteps = new byte[token.Length];
                                    for (int i = 0; i < token.Length; i++)
                                    {
                                        item.QuestSteps[i] = byte.Parse(token[i]);
                                    }
                                }
                                break;
                            case "faction":
                                item.Faction = (Factions)Enum.Parse(typeof(Factions), current.InnerText);
                                break;
                            case "manatype":
                                item.ManaType = (ManaType)int.Parse(current.InnerText);
                                break;
                            case "level":
                                item.Level = byte.Parse(current.InnerText);
                                break;
                            case "storeid":
                                item.StoreID = uint.Parse(current.InnerText);
                                break;
                            case "storebyitem":
                                item.StoreByItemID = uint.Parse(current.InnerText);
                                break;
                            case "sightrange":
                                item.SightRange = ushort.Parse(current.InnerText);
                                break;
                            case "aggrorange":
                                item.AggroRange = ushort.Parse(current.InnerText);
                                break;
                            case "hp":
                                item.MaxHP = int.Parse(current.InnerText);
                                break;
                            case "mp":
                                item.MaxMP = ushort.Parse(current.InnerText);
                                break;
                            case "nopushback":
                                item.NoPushBack = current.InnerText == "1";
                                break;
                            case "nomove":
                                item.NoMove = current.InnerText == "1";
                                break;
                            case "atkmin":
                                item.AtkMin = int.Parse(current.InnerText);
                                break;
                            case "atkmax":
                                item.AtkMax = int.Parse(current.InnerText);
                                break;
                            case "combatthinkperiod":
                                item.CombatThinkPeriod = int.Parse(current.InnerText);
                                break;
                            case "skill":
                                {
                                    uint skillID = uint.Parse(current.InnerText);
                                    item.Skill[skillID] = Skills.SkillFactory.Instance.CreateNewSkill(skillID);
                                }
                                break;
                            case "spawnondeath":
                                {
                                    NPCDeathSpawn spawn = new NPCDeathSpawn()
                                    {
                                        NPCID = ushort.Parse(current.InnerText)
                                    };
                                    if (current.HasAttribute("appearEffect"))
                                    {
                                        spawn.AppearEffect = int.Parse(current.Attributes["appearEffect"].Value);
                                    }

                                    if (current.HasAttribute("motion"))
                                    {
                                        spawn.Motion = ushort.Parse(current.Attributes["motion"].Value);
                                    }

                                    if (current.HasAttribute("count"))
                                    {
                                        spawn.Count = int.Parse(current.Attributes["count"].Value);
                                    }

                                    item.DeathSpawns.Add(spawn);
                                }
                                break;
                            case "corpseitemid":
                                {
                                    item.CorpseItemID = uint.Parse(current.InnerText);
                                }
                                break;
                            case "itemdrop":
                                {
                                    uint itemID = uint.Parse(current.InnerText);
                                    int rate = 0;
                                    int count = 1;
                                    if (current.HasAttribute("rate"))
                                    {
                                        rate = int.Parse(current.Attributes["rate"].Value);
                                    }

                                    if (current.HasAttribute("count"))
                                    {
                                        count = int.Parse(current.Attributes["count"].Value);
                                    }

                                    item.Items[itemID] = rate;
                                    item.ItemCounts[itemID] = (ushort)count;
                                }
                                break;
                        }
                    }
                    break;
            }
        }
    }
}
