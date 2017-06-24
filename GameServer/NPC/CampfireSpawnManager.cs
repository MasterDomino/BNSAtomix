using System;
using SmartEngine.Network.Utils;

namespace SagaBNS.GameServer.NPC
{
    public class CampfireSpawnManager : FactoryList<CampfireSpawnManager, SpawnData>
    {
        public CampfireSpawnManager()
        {
            loadingTab = "Loading campfire spawn database";
            loadedTab = " campfire spawns loaded.";
            databaseName = "Campfires";
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

        protected override void ParseXML(System.Xml.XmlElement root, System.Xml.XmlElement current, SpawnData item)
        {
            switch (root.Name.ToLower())
            {
                case "campfire":
                    {
                        switch (current.Name.ToLower())
                        {
                            case "npcid":
                                item.NpcID = ushort.Parse(current.InnerText);
                                item.IsCampfire = true;
                                break;
                            case "mapid":
                                item.MapID = uint.Parse(current.InnerText);
                                break;
                            case "type":
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
