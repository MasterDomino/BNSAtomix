using System;

using SmartEngine.Network.Utils;
using SagaBNS.Common.Actors;

namespace SagaBNS.GameServer.NPC
{
    public class FactionRelationFactory : Factory<FactionRelationFactory, FactionRelation>
    {
        public FactionRelationFactory()
        {
            loadingTab = "Loading faction relationship database";
            loadedTab = " relationships loaded.";
            databaseName = "Faction Relationships";
            FactoryType = FactoryType.XML;
        }

        public FactionRelation this[Factions faction]
        {
            get
            {
                return base[(uint)faction];
            }
        }

        protected override uint GetKey(FactionRelation item)
        {
            return (uint)item.Faction;
        }

        protected override void ParseCSV(FactionRelation item, string[] paras)
        {
            throw new NotImplementedException();
        }

        protected override void ParseXML(System.Xml.XmlElement root, System.Xml.XmlElement current, FactionRelation item)
        {
            switch (root.Name.ToLower())
            {
                case "relation":
                    {
                        switch (current.Name.ToLower())
                        {
                            case "faction":
                                item.Faction = (Factions)Enum.Parse(typeof(Factions), current.InnerText);
                                break;
                            case "target":
                                {
                                    Factions target = (Factions)Enum.Parse(typeof(Factions), current.Attributes["faction"].Value);
                                    item[target] = (Relations)Enum.Parse(typeof(Relations), current.InnerText);
                                }
                                break;
                        }
                    }
                    break;
            }
        }
    }
}
