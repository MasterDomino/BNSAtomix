using SmartEngine.Network;
using SagaBNS.Common.Actors;
using SagaBNS.Common.Packets;
using SagaBNS.Common.Packets.GameServer;
using SagaBNS.GameServer.Packets.Client;
using SagaBNS.GameServer.Map;

namespace SagaBNS.GameServer.Network.Client
{
    public partial class GameSession : Session<GamePacketOpcode>
    {
        public void OnAddCraft(CM_ADD_CRAFT p)
        {
            SM_ADD_CRAFT r = new SM_ADD_CRAFT();
            SM_PLAYER_UPDATE_LIST r2 = new SM_PLAYER_UPDATE_LIST();
            Network.SendPacket(r);
            UpdateEvent Event = new UpdateEvent();
            switch ((Crafts)p.Craft)
            {
                case Crafts.Logger:
                case Crafts.Horticulture:
                case Crafts.Harvester:
                case Crafts.Herbalist:
                case Crafts.Miner:
                case Crafts.StoneCutter:
                case Crafts.Fisher:
                    {
                        if (Character.Craft1 == Crafts.None)
                        {
                            Character.Craft1 = (Crafts)p.Craft;
                            Event.AddActorPara(PacketParameter.Craft1, p.Craft);
                            r2.Parameters = Event;
                            Network.SendPacket(r2);
                        }
                        else if (Character.Craft2 == Crafts.None)
                        {
                            Character.Craft2 = (Crafts)p.Craft;
                            Event.AddActorPara(PacketParameter.Craft2, p.Craft);
                            r2.Parameters = Event;
                            Network.SendPacket(r2);
                        }
                    }
                    break;

                case Crafts.Cooking:
                case Crafts.Potion:
                case Crafts.WeaponSmith:
                case Crafts.ForceSmith:
                case Crafts.BoPae:
                case Crafts.Jewel:
                case Crafts.Pottery:
                    {
                        if (Character.Craft3 == Crafts.None)
                        {
                            Character.Craft3 = (Crafts)p.Craft;
                            Event.AddActorPara(PacketParameter.Craft3, p.Craft);
                            r2.Parameters = Event;
                            Network.SendPacket(r2);
                        }
                        else if (Character.Craft4 == Crafts.None)
                        {
                            Character.Craft4 = (Crafts)p.Craft;
                            Event.AddActorPara(PacketParameter.Craft4, p.Craft);
                            r2.Parameters = Event;
                            Network.SendPacket(r2);
                        }
                    }
                    break;

                default:
                    {
                        SM_SERVER_MESSAGE r3 = new SM_SERVER_MESSAGE()
                        {
                            MessagePosition = SM_SERVER_MESSAGE.Positions.ChatWindow,
                            Message = "Unknown Craft please report to LokiReborn."
                        };
                        Network.SendPacket(r3);
                    }
                    break;
            }
        }

        public void OnDeleteCraft(CM_DELETE_CRAFT p)
        {
            SM_PLAYER_UPDATE_LIST r = new SM_PLAYER_UPDATE_LIST();
            UpdateEvent Event = new UpdateEvent();
            if (Character.Craft1 == (Crafts)p.Craft)
            {
                Character.Craft1 = Crafts.None;
                Event.AddActorPara(PacketParameter.Craft1, 0);
                r.Parameters = Event;
                Network.SendPacket(r);
            }
            else if (Character.Craft2 == (Crafts)p.Craft)
            {
                Character.Craft2 = Crafts.None;
                Event.AddActorPara(PacketParameter.Craft2, 0);
                r.Parameters = Event;
                Network.SendPacket(r);
            }
            else if (Character.Craft3 == (Crafts)p.Craft)
            {
                Character.Craft3 = Crafts.None;
                Event.AddActorPara(PacketParameter.Craft3, 0);
                r.Parameters = Event;
                Network.SendPacket(r);
            }
            else if (Character.Craft4 == (Crafts)p.Craft)
            {
                Character.Craft4 = Crafts.None;
                Event.AddActorPara(PacketParameter.Craft4, 0);
                r.Parameters = Event;
                Network.SendPacket(r);
            }
        }
    }
}
