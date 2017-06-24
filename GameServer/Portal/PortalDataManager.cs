using System;
using SmartEngine.Network.Utils;
using SagaBNS.Common.Actors;
namespace SagaBNS.GameServer.Portal
{
    public class PortalDataManager : FactoryList<PortalDataManager, ActorPortal>
    {
        public PortalDataManager()
        {
            loadingTab = "Loading portal database";
            loadedTab = " portals loaded.";
            databaseName = "Portal";
            FactoryType = FactoryType.XML;
        }

        protected override uint GetKey(ActorPortal item)
        {
            return item.MapID;
        }

        protected override void ParseCSV(ActorPortal item, string[] paras)
        {
            throw new NotImplementedException();
        }

        protected override void ParseXML(System.Xml.XmlElement root, System.Xml.XmlElement current, ActorPortal item)
        {
            switch (root.Name.ToLower())
            {
                case "portal":
                    {
                        switch (current.Name.ToLower())
                        {
                            case "id":
                                item.ID = uint.Parse(current.InnerText);
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
                            case "teleport":
                                {
                                    PortalTrigger trigger = new PortalTrigger();
                                    if (current.HasAttribute("dir"))
                                    {
                                        trigger.Dir = int.Parse(current.Attributes["dir"].Value);
                                    }

                                    if (current.HasAttribute("quest"))
                                    {
                                        trigger.Quest = ushort.Parse(current.Attributes["quest"].Value);
                                    }

                                    if (current.HasAttribute("step"))
                                    {
                                        trigger.Step = int.Parse(current.Attributes["step"].Value);
                                    }

                                    if (current.HasAttribute("x"))
                                    {
                                        trigger.X = int.Parse(current.Attributes["x"].Value);
                                    }

                                    if (current.HasAttribute("y"))
                                    {
                                        trigger.Y = int.Parse(current.Attributes["y"].Value);
                                    }

                                    if (current.HasAttribute("z"))
                                    {
                                        trigger.Z = int.Parse(current.Attributes["z"].Value);
                                    }

                                    trigger.MapTarget = uint.Parse(current.InnerText);
                                    item.PortalTriggers.Add(trigger);
                                }
                                break;
                        }
                    }
                    break;
            }
        }

        public void SpawnAll(uint mapID, Map.Map map)
        {
            /*if (items.ContainsKey(mapID))
            {
                foreach (ActorPortal i in items[mapID])
                {
                    if (map != null)
                    {
                        i.EventHandler = new ActorEventHandlers.PortalEventHandler(i);
                        map.RegisterActor(i);
                        map.SendVisibleActorsToActor(i);
                    }
                }
            }*/
        }
    }
}
