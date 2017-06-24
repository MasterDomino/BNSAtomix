using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using System.Text;

using SmartEngine.Network.Utils;
using SmartEngine.Network.VirtualFileSystem;

namespace SagaBNS.GameServer.Map
{
    public class MapManager : Factory<MapManager, Map>
    {
        private readonly ConcurrentDictionary<uint, Map> maps = new ConcurrentDictionary<uint, Map>();
        private readonly ConcurrentDictionary<uint, List<Map>> instanceMapping = new ConcurrentDictionary<uint, List<Map>>();
        private readonly Dictionary<uint, List<RespawnPoint>> respawnPoints = new Dictionary<uint, List<RespawnPoint>>();

        public Dictionary<uint, List<RespawnPoint>> RespawnPoints { get { return respawnPoints; } }
        private static uint instanceID = 1;
        public MapManager()
        {
            loadingTab = "Loading map template database";
            loadedTab = " map templates loaded.";
            databaseName = "map Templates";
            FactoryType = FactoryType.XML;
        }

        protected override uint GetKey(Map item)
        {
            return item.ID;
        }

        protected override void ParseCSV(Map item, string[] paras)
        {
            throw new NotImplementedException();
        }

        public void Save(string path)
        {
            //foreach (uint mapID in items.Keys)
            {
                System.IO.StreamWriter sw = new System.IO.StreamWriter(path, false, Encoding.UTF8);
                sw.WriteLine("<?xml version=\"1.0\" encoding=\"utf-8\"?>");
                sw.WriteLine("<Maps>");
                sw.WriteLine("  <!--Instance Types:");
                sw.WriteLine("            0: No Instance");
                sw.WriteLine("            1: Single Instance");
                sw.WriteLine("            2: Party Instance-->");
                foreach (Map j in items.Values)
                {
                    sw.WriteLine("  <Map>");
                    sw.WriteLine("    <ID>" + j.ID + "</ID>");
                    sw.WriteLine("    <InstanceType>" + (int)j.InstanceType + "</InstanceType>");
                    sw.WriteLine("    <HeightMap>" + j.HeightMapBuilder.Name + "</HeightMap>");
                    if (respawnPoints.ContainsKey(j.ID))
                    {
                        foreach (RespawnPoint k in respawnPoints[j.ID])
                        {
                            sw.WriteLine(string.Format("    <Respawn map=\"{0}\" x=\"{1}\" y=\"{2}\" z=\"{3}\" dir=\"{4}\"" + (k.teleportId != 0 ? " teleportId=\"{5}\" />" :" />"),
                                k.MapID, k.X, k.Y, k.Z, k.Dir, k.teleportId));
                        }
                    }
                    sw.WriteLine("  </Map>");
                }
                sw.WriteLine("</Maps>");
                sw.Flush();
                sw.Close();
            }
        }

        protected override void ParseXML(System.Xml.XmlElement root, System.Xml.XmlElement current, Map item)
        {
            switch (root.Name.ToLower())
            {
                case "map":
                    {
                        switch (current.Name.ToLower())
                        {
                            case "id":
                                item.ID = uint.Parse(current.InnerText);
                                break;
                            case "instancetype":
                                item.InstanceType = (MapInstanceType)int.Parse(current.InnerText);
                                break;
                            case "respawn":
                                RespawnPoint point = new RespawnPoint()
                                {
                                    MapID = current.HasAttribute("map") ? uint.Parse(current.Attributes["map"].Value) : item.ID,
                                    X = short.Parse(current.Attributes["x"].Value),
                                    Y = short.Parse(current.Attributes["y"].Value),
                                    Z = short.Parse(current.Attributes["z"].Value),
                                    Dir = ushort.Parse(current.Attributes["dir"].Value)
                                };
                                if (current.HasAttribute("teleportId"))
                                {
                                    point.teleportId = ushort.Parse(current.Attributes["teleportId"].Value);
                                }

                                if (!respawnPoints.ContainsKey(item.ID))
                                {
                                    respawnPoints.Add(item.ID, new List<RespawnPoint>());
                                }

                                respawnPoints[item.ID].Add(point);
                                break;
                            case "heightmap":
                                HeightMapBuilder builder;
                                if (Map.heightmapBuilder.TryGetValue(current.InnerText, out builder))
                                {
                                    item.HeightMapBuilder = builder;
                                }
                                else
                                {
                                    if (VirtualFileSystemManager.Instance.FileSystem.Exists("DB/HeightMaps/" + current.InnerText + ".builder"))
                                    {
                                        System.IO.Stream st = VirtualFileSystemManager.Instance.FileSystem.OpenFile("DB/HeightMaps/" + current.InnerText + ".builder");
                                        item.HeightMapBuilder = HeightMapBuilder.FromStream(st);
                                        st.Close();
                                    }
                                    else
                                    {
                                        item.HeightMapBuilder = new HeightMapBuilder();
                                    }

                                    item.HeightMapBuilder.Name = current.InnerText;

                                    Map.heightmapBuilder[current.InnerText] = item.HeightMapBuilder;
                                }
                                break;
                        }
                    }
                    break;
            }
        }

        public void InitStandardMaps()
        {
            foreach (Map i in items.Values)
            {
                if (i.InstanceType == MapInstanceType.NoInstance)
                {
                    CreateMapInstance(i.ID, 0);
                }
            }
        }

        public uint CreateMapInstance(uint mapID, uint charID)
        {
            if (items.ContainsKey(mapID))
            {
                Map map = new Map(mapID);
                lock (this)
                {
                    while (maps.ContainsKey(instanceID))
                    {
                        instanceID++;
                        if (instanceID >= 20000)
                        {
                            instanceID = 1;
                        }
                    }
                }
                map.InstanceID = instanceID;
                map.CreatorCharID = charID;
                map.InstanceType = items[mapID].InstanceType;
                map.HeightMapBuilder = items[mapID].HeightMapBuilder;
                maps[map.InstanceID] = map;
                if (!instanceMapping.ContainsKey(mapID))
                {
                    instanceMapping[mapID] = new List<Map>();
                }
                instanceMapping[mapID].Add(map);

                NPC.NPCSpawnManager.Instance.SpawnAll(mapID, map);
                NPC.CampfireSpawnManager.Instance.SpawnAll(mapID, map);
                Portal.PortalDataManager.Instance.SpawnAll(mapID, map);
                return map.InstanceID;
            }
            return 0;
        }

        public void DeleteMapInstance(Map map)
        {
            if (map.InstanceType != MapInstanceType.NoInstance)
            {
                maps.TryRemove(map.InstanceID, out map);
                instanceMapping[map.ID].Remove(map);
                map.Destroy();
            }
        }

        public new void Reload()
        {
            respawnPoints.Clear();
            base.Reload();
        }

        public Map GetMap(uint instanceID)
        {
            if (maps.TryGetValue(instanceID, out Map res))
            {
                return res;
            }
            else
            {
                return null;
            }
        }

        public Map GetMap(uint mapID, uint charID, ulong partyID)
        {
            if (items.ContainsKey(mapID))
            {
                if (items[mapID].InstanceType == MapInstanceType.NoInstance)
                {
                    return instanceMapping[mapID][0];
                }
                else
                {
                    if (items[mapID].InstanceType == MapInstanceType.Single)
                    {
                        if (instanceMapping.ContainsKey(mapID))
                        {
                            var list = from map in instanceMapping[mapID]
                                       where map != null && map.CreatorCharID == charID
                                       select map;
                            Map res = list.FirstOrDefault();
                            if (res != null)
                            {
                                return res;
                            }
                            else
                            {
                                return GetMap(CreateMapInstance(mapID, charID));
                            }
                        }
                        else
                        {
                            return GetMap(CreateMapInstance(mapID, charID));
                        }
                    }
                    else if (items[mapID].InstanceType == MapInstanceType.Party)
                    {
                        uint partyCreatorID = partyID != 0 ? partyID.PartyID2CreatorID() : charID;
                        if (instanceMapping.ContainsKey(mapID))
                        {
                            var list = from map in instanceMapping[mapID]
                                       where map.CreatorCharID == partyCreatorID
                                       select map;
                            Map res = list.FirstOrDefault();
                            if (res != null)
                            {
                                return res;
                            }
                            else
                            {
                                return GetMap(CreateMapInstance(mapID, partyCreatorID));
                            }
                        }
                        else
                        {
                            return GetMap(CreateMapInstance(mapID, partyCreatorID));
                        }
                    }
                    else
                    {
                        return null;
                    }
                }
            }
            else
            {
                return null;
            }
        }
    }
}
