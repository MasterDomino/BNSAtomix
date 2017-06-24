using System;
using System.Collections.Generic;
using System.Linq;
using SmartEngine.Network.Utils;

namespace SagaBNS.GameServer.BaGua
{
    public class BaGuaSet
    {
        public uint SetID { get; set; }
        public Dictionary<short,List<uint>> Bonus = new Dictionary<short,List<uint>>();
    }

    public class BaGuaManager : Factory<BaGuaManager, BaGuaSet>
    {
        public BaGuaManager()
        {
            loadingTab = "Loading BaGua set template database";
            loadedTab = " bagua set templates loaded.";
            databaseName = "BaGua Set Templates";
            FactoryType = FactoryType.XML;
        }

        protected override uint GetKey(BaGuaSet bagua)
        {
            return bagua.SetID;
        }

        protected override void ParseCSV(BaGuaSet bagua, string[] paras)
        {
            throw new NotImplementedException();
        }

        protected override void ParseXML(System.Xml.XmlElement root, System.Xml.XmlElement current, BaGuaSet bagua)
        {
            switch (root.Name.ToLower())
            {
                case "set":
                    {
                        switch (current.Name.ToLower())
                        {
                            case "id":
                                bagua.SetID = uint.Parse(current.InnerText);
                                break;
                            case "bonus1":
                                {
                                    List<uint> temp = new List<uint>();
                                    string[] entrys = current.InnerText.Split(',');
                                    for (int i = 0; i < entrys.Count(); i++)
                                    {
                                        temp.Add(uint.Parse(entrys[i]));
                                    }

                                    bagua.Bonus.Add(3, temp);
                                }
                                break;
                            case "bonus2":
                                {
                                    List<uint> temp = new List<uint>();
                                    string[] entrys = current.InnerText.Split(',');
                                    for (int i = 0; i < entrys.Count(); i++)
                                    {
                                        temp.Add(uint.Parse(entrys[i]));
                                    }

                                    bagua.Bonus.Add(5, temp);
                                }
                                break;
                            case "bonus3":
                                {
                                    List<uint> temp = new List<uint>();
                                    string[] entrys = current.InnerText.Split(',');
                                    for (int i = 0; i < entrys.Count(); i++)
                                    {
                                        temp.Add(uint.Parse(entrys[i]));
                                    }

                                    bagua.Bonus.Add(8, temp);
                                }
                                break;
                        }
                    }
                    break;
            }
        }
    }
}
