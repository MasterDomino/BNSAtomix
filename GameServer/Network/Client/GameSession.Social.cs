using System;
using System.Linq;
using SmartEngine.Network;
using SagaBNS.Common.Packets;
using SagaBNS.GameServer.Map;
using SagaBNS.GameServer.Packets.Client;
using SagaBNS.GameServer.Command;
using SagaBNS.GameServer.Manager;

namespace SagaBNS.GameServer.Network.Client
{
    public partial class GameSession : Session<GamePacketOpcode>
    {
        public void OnChat(CM_CHAT p)
        {
            string recipient = p.Recipient;
            string txt = p.Text;
            if (!Commands.Instance.ProcessCommand(this, txt))
            {
                ChatArgument arg = new ChatArgument()
                {
                    Sender = chara,
                    Type = p.Type,
                    Recipient = recipient,
                    Message = txt
                };
                switch (p.Type)
                {
                    case ChatType.Whisper:
                        GameSession temp = GameClientManager.Instance.FindClient(p.Recipient);
                        if (temp != null)
                        {
                            map.OnEvent(MapEvents.CHAT, arg, chara, null);
                            map.OnEvent(MapEvents.CHAT, arg, temp.chara, null);
                        }
                        else
                        {
                            SM_CHAT_RESPONSE r = new SM_CHAT_RESPONSE()
                            {
                                MessageId = 17
                            };
                            Network.SendPacket(r);
                        }
                        break;
                    case ChatType.UnknownParty:
                        break;
                    case ChatType.General:
                    default:
                        arg.Recipient = null;
                        map.SendEventToAllActorsWhoCanSeeActor(MapEvents.CHAT, arg, chara, true);
                        break;
                }
            }
        }

        public void OnHelpUnstuck(CM_HELP_UNSTUCK p)
        {
            var points = from point in Map.RespawnPoints
                         orderby Character.DistanceToPoint(point.X, point.Y, point.Z)
                         select point;
            RespawnPoint res = points.FirstOrDefault();
            Map.Map tMap = MapManager.Instance.GetMap(res.MapID, chara.CharID, chara.PartyID);
            if (map != null)
            {
                chara.Dir = res.Dir;
                chara.Status.ShouldLoadMap = true;
                map.SendActorToMap(chara, tMap, res.X, res.Y, res.Z);
            }
            else
            {
                SendServerMessage("No respawn point defined in this map!", SM_SERVER_MESSAGE.Positions.ChatWindow);
            }
        }

        public void SendChat(ChatArgument arg)
        {
                SM_CHAT p = new SM_CHAT();
                p.Recipient(arg.Recipient, arg.Sender.ActorID);
                p.PutMessage(arg.Sender.Name, (byte)arg.Type, arg.Message);
            Network.SendPacket(p);
        }

        public void SendServerMessage(string txt,SM_SERVER_MESSAGE.Positions position)
        {
            SM_SERVER_MESSAGE p = new SM_SERVER_MESSAGE()
            {
                MessagePosition = position,
                Message = txt
            };
            Network.SendPacket(p);
        }

        public void OnGetTime()
        {
            System.TimeSpan length = DateTime.Now - loginTime;
            String output = "You've been online for: ";
            if (length.Days > 0)
            {
                output += String.Format("{0} Days ", length.Days);
            }

            if (length.Hours > 0)
            {
                output += String.Format("{0} Hours ", length.Hours);
            }

            if (length.Minutes > 0)
            {
                output += String.Format("{0} Minutes ", length.Minutes);
            }

            if (length.Seconds > 0)
            {
                output += String.Format("{0} Seconds ", length.Seconds);
            }

            SM_SERVER_MESSAGE r = new SM_SERVER_MESSAGE()
            {
                MessagePosition = SM_SERVER_MESSAGE.Positions.ChatWindow,
                Message = output
            };
            Network.SendPacket(r);
        }
    }
}
