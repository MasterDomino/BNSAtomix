using System;
using System.Text;
using SmartEngine.Network.Utils;

namespace SagaBNS.GameServer.NPC
{
    public class NPCSpawnManager : FactoryList<NPCSpawnManager, SpawnData>
    {
        public NPCSpawnManager()
        {
            loadingTab = "Loading npc spawn database";
            loadedTab = " spawns loaded.";
            databaseName = "NPCSpawn";
            FactoryType = FactoryType.XML;
        }

        protected override uint GetKey(SpawnData item)
        {
            return item.MapID;
        }

        protected override void ParseCSV(SpawnData item, string[] paras)
        {
            throw new NotImplementedException();
        }

        public void Save(string folder)
        {
            foreach (uint mapID in items.Keys)
            {
                System.IO.StreamWriter sw = new System.IO.StreamWriter(folder + "\\" + mapID + ".xml", false, Encoding.UTF8);
                sw.WriteLine("<?xml version=\"1.0\" encoding=\"utf-8\"?>");
                sw.WriteLine("<Spawns>");
                foreach (SpawnData j in items[mapID])
                {
                    if (j.IsQuest)
                    {
                        continue;
                    }

                    if (!j.IsMapObject)
                    {
                        sw.WriteLine("  <Spawn>");
                        sw.WriteLine("    <NPCID>" + j.NpcID + "</NPCID>");
                        sw.WriteLine("    <MapID>" + j.MapID + "</MapID>");
                        sw.WriteLine("    <X>" + j.X + "</X>");
                        sw.WriteLine("    <Y>" + j.Y + "</Y>");
                        sw.WriteLine("    <Z>" + j.Z + "</Z>");
                        sw.WriteLine("    <Dir>" + j.Dir + "</Dir>");
                        sw.WriteLine("    <Motion>" + j.Motion + "</Motion>");
                        if (j.Range > 0)
                        {
                            sw.WriteLine("    <Range>" + j.Range + "</Range>");
                        }
                        if (j.MoveRange > 0)
                        {
                            sw.WriteLine("    <MoveRange>" + j.MoveRange + "</MoveRange>");
                        }
                        if (j.AppearEffect > 0)
                        {
                            sw.WriteLine("    <AppearEffect>" + j.AppearEffect + "</AppearEffect>");
                        }
                        if (j.Delay != 60000)
                        {
                            sw.WriteLine("    <Delay>" + j.Delay + "</Delay>");
                        }
                        if (j.Hidden)
                        {
                            sw.WriteLine("    <Hidden>1</Hidden>");
                        }

                        //sw.WriteLine("    <ManaType>" + j.ManaType + "</ManaType>");
                        sw.WriteLine("  </Spawn>");
                    }
                    else
                    {
                        sw.WriteLine("  <MapObject>");
                        sw.WriteLine("    <ID>" + j.NpcID + "</ID>");
                        sw.WriteLine("    <MapID>" + j.MapID + "</MapID>");
                        if (j.Special)
                        {
                            sw.WriteLine("    <Special>1</Special>");
                        }

                        if (j.DragonStream)
                        {
                            sw.WriteLine("    <DragonStream/>");
                        }

                        if (j.SpecialMapID != 0)
                        {
                            sw.WriteLine("    <SMapID>" + j.X + "</SMapID>");
                        }

                        if (j.X != 0)
                        {
                            sw.WriteLine("    <X>" + j.X + "</X>");
                        }

                        if (j.Y != 0)
                        {
                            sw.WriteLine("    <Y>" + j.Y + "</Y>");
                        }

                        if (j.Z != 0)
                        {
                            sw.WriteLine("    <Z>" + j.Z + "</Z>");
                        }

                        if (j.Delay != 60000)
                        {
                            sw.WriteLine("    <Delay>" + j.Delay + "</Delay>");
                        }
                        foreach(uint i in j.ItemIDs.Keys)
                        {
                            sw.WriteLine("    <Item count=\"" + j.ItemIDs[i] + "\">" + i + "</Item>");
                        }
                        if (j.MaxGold > 0)
                        {
                            sw.WriteLine(string.Format("    <Gold min=\"{0}\" max=\"{1}\"/>", j.MinGold, j.MaxGold));
                        }
                        sw.WriteLine("  </MapObject>");
                    }
                }
                sw.WriteLine("</Spawns>");
                sw.Flush();
                sw.Close();
            }
        }

        protected override void ParseXML(System.Xml.XmlElement root, System.Xml.XmlElement current, SpawnData item)
        {
            switch (root.Name.ToLower())
            {
                case "spawn":
                    {
                        switch (current.Name.ToLower())
                        {
                            case "npcid":
                                item.NpcID = ushort.Parse(current.InnerText);
                                break;
                            case "mapid":
                                item.MapID = uint.Parse(current.InnerText);
                                break;
                            case "x":
                                item.X = short.Parse(current.InnerText);
                                break;
                            case "y":
                                item.Y = short.Parse(current.InnerText);
                                break;
                            case "z":
                                item.Z = short.Parse(current.InnerText);
                                break;
                            case "dir":
                                item.Dir = ushort.Parse(current.InnerText);
                                break;
                            case "motion":
                                item.Motion = ushort.Parse(current.InnerText);
                                break;
                            case "manatype":
                                item.ManaType = int.Parse(current.InnerText);
                                break;
                            case "count":
                                item.Count = int.Parse(current.InnerText);
                                break;
                            case "range":
                                item.Range = int.Parse(current.InnerText);
                                break;
                            case "moverange":
                                item.MoveRange = int.Parse(current.InnerText);
                                break;
                            case "delay":
                                item.Delay = int.Parse(current.InnerText);
                                break;
                            case "appeareffect":
                                item.AppearEffect = int.Parse(current.InnerText);
                                break;
                            case "hidden":
                                item.Hidden = current.InnerText == "1";
                                break;
                        }
                    }
                    break;
                case "mapobject":
                    {
                        switch (current.Name.ToLower())
                        {
                            case "id":
                                item.NpcID = ushort.Parse(current.InnerText);
                                item.IsMapObject = true;
                                break;
                            case "mapid":
                                item.MapID = uint.Parse(current.InnerText);
                                break;
                            case "smapid":
                                item.SpecialMapID = uint.Parse(current.InnerText);
                                break;
                            case "x":
                                item.X = short.Parse(current.InnerText);
                                break;
                            case "y":
                                item.Y = short.Parse(current.InnerText);
                                break;
                            case "z":
                                item.Z = short.Parse(current.InnerText);
                                break;
                            case "special":
                                item.Special = current.InnerText == "1";
                                break;
                            case "dragonstream":
                                item.DragonStream = true;
                                break;
                            case "item":
                                {
                                    int count = 1;
                                    if (current.HasAttribute("count"))
                                    {
                                        count = int.Parse(current.Attributes["count"].Value);
                                    }

                                    item.ItemIDs[uint.Parse(current.InnerText)] = count;
                                }
                                break;
                            case "gold":
                                item.MinGold = int.Parse(current.Attributes["min"].Value);
                                item.MaxGold = int.Parse(current.Attributes["max"].Value);
                                break;
                            case "delay":
                                item.Delay = int.Parse(current.InnerText);
                                break;
                        }
                    }
                    break;
            }
        }

        public void SpawnAll(uint mapID, Map.Map map)
        {
            if (items.ContainsKey(mapID))
            {
                foreach (SpawnData i in items[mapID])
                {
                    i.DoSpawn(map);
                }
            }
        }
    }
}
