using System;
using System.Collections.Generic;

using SmartEngine.Network.Utils;

namespace SagaBNS.GameServer.NPC
{
    public class NPCStoreFactory : Factory<NPCStoreFactory, NPCStore>
    {
        private readonly Dictionary<uint, NPCStore> stores = new Dictionary<uint, NPCStore>();
        private readonly Dictionary<uint, NPCStore> storesByItem = new Dictionary<uint, NPCStore>();

        public Dictionary<uint, NPCStore> Stores { get { return stores; } }
        public Dictionary<uint, NPCStore> StoresByItem { get { return storesByItem; } }
        public NPCStoreFactory()
        {
            loadingTab = "Loading npc store database";
            loadedTab = " npc stores loaded.";
            databaseName = "NPC Stores";
            FactoryType = FactoryType.XML;
        }

        protected override uint GetKey(NPCStore item)
        {
            return item.ID;
        }

        protected override void ParseCSV(NPCStore item, string[] paras)
        {
            throw new NotImplementedException();
        }

        protected override void ParseXML(System.Xml.XmlElement root, System.Xml.XmlElement current, NPCStore item)
        {
            switch (root.Name.ToLower())
            {
                case "store":
                    {
                        switch (current.Name.ToLower())
                        {
                            case "id":
                                items = stores;
                                item.ID = uint.Parse(current.InnerText);
                                break;
                            case "buyrate":
                                item.BuyRate = (float)int.Parse(current.InnerText) / 100;
                                break;
                            case "sellrate":
                                item.SellRate = (float)int.Parse(current.InnerText) / 100;
                                break;
                            case "buybackrate":
                                item.BuyBackRate = (float)int.Parse(current.InnerText) / 100;
                                break;
                            case "items":
                                string[] token = current.InnerText.Split(',');
                                List<uint> ids = new List<uint>();
                                foreach (string i in token)
                                {
                                    uint id = uint.Parse(i);
                                    if (id != 0)
                                    {
                                        ids.Add(id);
                                    }
                                }
                                item.Items = ids.ToArray();
                                break;
                        }
                    }
                    break;
                case "storebyitem":
                    {
                        switch (current.Name.ToLower())
                        {
                            case "id":
                                items = storesByItem;
                                item.IsStoreByItem = true;
                                item.ID = uint.Parse(current.InnerText);
                                break;
                            case "items":
                                {
                                    string[] token = current.InnerText.Split(',');
                                    List<uint> ids = new List<uint>();
                                    foreach (string i in token)
                                    {
                                        uint id = uint.Parse(i);
                                        if (id != 0)
                                        {
                                            ids.Add(id);
                                        }
                                    }
                                    item.Items = ids.ToArray();
                                    break;
                                }
                            case "materials":
                                {
                                    string[] token = current.InnerText.Split(',');
                                    List<uint> ids = new List<uint>();
                                    foreach (string i in token)
                                    {
                                        uint id = uint.Parse(i);
                                        if (id != 0)
                                        {
                                            ids.Add(id);
                                        }
                                    }
                                    item.Materials = ids.ToArray();
                                    break;
                                }
                            case "materialcounts":
                                {
                                    byte[] token = Conversions.HexStr2Bytes(current.InnerText);
                                    List<ushort> ids = new List<ushort>();
                                    foreach (byte i in token)
                                    {
                                        if (i != 0)
                                        {
                                            ids.Add(i);
                                        }
                                    }
                                    item.MaterialCounts = ids.ToArray();
                                    break;
                                }
                        }
                    }
                    break;
            }
        }
    }
}
