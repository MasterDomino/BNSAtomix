using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using SmartEngine.Core;
using SmartEngine.Network;
using SmartEngine.Network.Utils;
using SmartEngine.Network.Map;
using SagaBNS.Common.Actors;
using SagaBNS.Common.Packets;
using SagaBNS.GameServer.Map;
using SagaBNS.GameServer.Packets.Client;

namespace SagaBNS.GameServer.Network.Client
{
    public partial class GameSession : Session<GamePacketOpcode>
    {
        public void SendWarp()
        {
            SM_MAP_PORTAL_ACTIVATE p = new SM_MAP_PORTAL_ACTIVATE()
            {
                CutScene = chara.MapChangeCutScene
            };
            if (chara.Status.DisappearEffect == 539)
            {
                p.U3 = 0;
                p.U1 = 5;
                p.DisappearEffect = chara.Status.DisappearEffect;
            }
            else if (chara.Status.DisappearEffect != 0)
            {
                p.U3 = 0;
                p.DisappearEffect = chara.Status.DisappearEffect;
            }
            else if (chara.Status.ShouldRespawn)
            {
                p.U1 = 3;
                chara.Status.DisappearEffect = 535;
                p.DisappearEffect = 535;
            }
            else if (chara.Status.ShouldLoadMap)
            {
                p.U1 = 6;
                p.DisappearEffect = 0;
            }
            else
            {
                p.U1 = (byte)chara.MapChangeCutSceneU1;
                p.DisappearEffect = 0;
            }
            p.U2 = chara.MapChangeCutSceneU2;
            Network.SendPacket(p);
            chara.MapChangeCutScene = 0;
            chara.MapChangeCutSceneU1 = 0;
            chara.MapChangeCutSceneU2 = 0;
            SendChangeMap();
        }

        public void SendChangeMap()
        {
            map = MapManager.Instance.GetMap(chara.MapInstanceID);
            SM_MAP_CHANGE_MAP p = new SM_MAP_CHANGE_MAP()
            {
                InstanceID = map.InstanceID,
                MapID = map.ID,
                Time = 0x91D33B
            };
            System.Security.Cryptography.MD5 md5 = System.Security.Cryptography.MD5.Create();
            p.MapServerAESKey = md5.ComputeHash(Encoding.ASCII.GetBytes(Global.Random.Next().ToString()));
            p.X = (short)chara.X;
            p.Y = (short)chara.Y;
            p.Z = (short)chara.Z;
            p.Dir = chara.Dir;
            Network.SendPacket(p);
        }

        public void OnMapLoadFinished(CM_MAP_LOAD_FINISHED p)
        {
            SM_MAP_LOAD_REPLY p1 = new SM_MAP_LOAD_REPLY();
            Network.SendPacket(p1);
            chara.Invisible = false;
            map.SendVisibleActorsToActor(chara);
            map.OnActorVisibilityChange(chara);
            PC.Status.CalcStatus(chara);
            /*foreach (Common.Item.Item i in chara.Inventory.Equipments.Values)
            {
                if (i != null)
                    SendPlayerEquiptStats(i,Stats.None,true);
            }
            if (chara.Status.Dead)
            {
                Interlocked.Exchange(ref chara.HP, chara.MaxHP);
                Task removed;
                if (chara.Tasks.TryRemove("DieTask", out removed))
                    removed.Deactivate();
                chara.Status.Dead = false;
                chara.Status.Dying = false;
                chara.Status.Recovering = false;
                UpdateEvent evt = new UpdateEvent();
                evt.Actor = chara;
                evt.Target = chara;
                evt.UpdateType = UpdateTypes.Actor;
                //evt.AddActorPara(Common.Packets.GameServer.PacketParameter.Dead, 0);
                evt.AddActorPara(Common.Packets.GameServer.PacketParameter.HP, chara.HP);
                map.SendEventToAllActorsWhoCanSeeActor(MapEvents.EVENT_BROADCAST, evt, chara, true);
            }*/
            /*chara.Status.ShouldRespawn = false;
            chara.Status.ShouldLoadMap = false;
            chara.Status.DisappearEffect = 0;            
            SendPlayerMP();
            SendPlayerHP(true);
            //SendItemBuyBackList();
             */
            //TODO: Ruo start here
            if (chara.Quests.ContainsKey(250))
            {
                ProcessQuest(250, 1);
            }

            Quests.QuestManager.Instance.ProcessQuest(chara, chara.MapID);
            /*ChangeCombatStatus(false);
            if (chara.Offline && chara.Party != null)
            {
                Party.PartyManager.Instance.PartyMemberOfflineChange(chara.Party, chara, false);
            }
            if (!chara.Tasks.ContainsKey("HPRegeneration"))
            {
                Tasks.Player.HPRegenerationTask rege = new Tasks.Player.HPRegenerationTask(chara);
                rege.Activate();
                chara.Tasks[rege.Name] = rege;
            }
            if(chara .Job == Job.ForceMaster && !chara.Tasks.ContainsKey("MPRegeneration"))
            {
                Tasks.Player.MPRegenerationTask rege = new Tasks.Player.MPRegenerationTask(chara);
                rege.Activate();
                chara.Tasks[rege.Name] = rege;
            }*/
        }

        public void OnMapEnterStream(CM_DRAGON_STREAM p)
        {
            if (map.MapObjects[p.StreamID] is ActorMapObj stream)
            {
                if (stream.DragonStream)
                {
                    bool Debugging = false;
                    if (stream.SpecialMapID != 0)
                    {
                        if (Debugging)
                        {
                            SM_SERVER_MESSAGE c = new SM_SERVER_MESSAGE()
                            {
                                MessagePosition = SM_SERVER_MESSAGE.Positions.ChatWindow,
                                Message = string.Format("Dragon Stream:0x{0:X16}", stream.ActorID)
                            };
                            Network.SendPacket(c);
                        }
                        else
                        {
                            MapTeleport(stream);
                        }
                    }
                    else
                    {
                        OnMapTeleport(stream, Debugging);
                    }
                }
            }
        }

        private void LockForTeleport(bool Lock)
        {
            SM_PLAYER_UPDATE_LIST l = new SM_PLAYER_UPDATE_LIST();
            UpdateEvent evt = new UpdateEvent();
            //evt.AddActorPara(Common.Packets.GameServer.PacketParameter.NoMove, Lock ? 1 : 0);
            //evt.AddActorPara(Common.Packets.GameServer.PacketParameter.Unk103, Lock ? 1 : 0);
            //evt.AddActorPara(Common.Packets.GameServer.PacketParameter.Unk108, Lock ? 1 : 0);
            //evt.AddActorPara(Common.Packets.GameServer.PacketParameter.Unk109, Lock ? 1 : 0);
            l.Parameters = evt;
            evt.ActorUpdateParameters.Clear();
            Network.SendPacket(l);
        }

        private void MapTeleport(ActorMapObj stream)
        {
            int disappearEffect = DisappearEffect(stream.SpecialMapID);
            if (disappearEffect != -1)
            {
                chara.Status.ShouldLoadMap = false;
                chara.Status.DisappearEffect = disappearEffect;
                Map.Map map = MapManager.Instance.GetMap(stream.SpecialMapID, chara.CharID, chara.PartyID);
                RespawnPoint res;
                res.X = (short)stream.X;
                res.Y = (short)stream.Y;
                res.Z = (short)stream.Z;
                Map.SendActorToMap(chara, map, res.X, res.Y, res.Z);
            }
        }

        private int DisappearEffect(uint map)
        {
            switch (map)
            {
                case 2440:
                    return 0x566;
                case 2704:
                    return 0x567;
                default:
                    return -1;
            }
        }

        private void OnMapTeleport(ActorMapObj stream, bool Debugging)
        {
            List<UpdateEvent> events = new List<UpdateEvent>();
            UpdateEvent toadd4, toadd3, toadd2, toadd;

            LockForTeleport(true);

            toadd = new UpdateEvent()
            {
                Actor = chara,
                Target = stream,
                UpdateType = UpdateTypes.DragonStream,
                AdditionCount = 0,
                X = 0,
                Y = 0,
                Z = 0
            };
            events.Add(toadd);

            toadd2 = new UpdateEvent()
            {
                Actor = chara,
                Target = stream,
                UpdateType = UpdateTypes.DragonStream,
                AdditionCount = 1,
                X = (short)stream.X,
                Y = (short)stream.Y,
                Z = (short)stream.Z
            };
            events.Add(toadd2);

            toadd3 = new UpdateEvent()
            {
                Actor = chara,
                Target = chara,
                UpdateType = UpdateTypes.Actor,
                AdditionID = 65002,
                AdditionSession = 0x7001,
                ExtraActivateMode = UpdateEvent.ExtraUpdateModes.Activate,
                SkillSession = 0xFF
            };
            //toadd3.AddActorPara(Common.Packets.GameServer.PacketParameter.UnkD4, 1);
            events.Add(toadd3);

            toadd4 = new UpdateEvent()
            {
                UpdateType = UpdateTypes.ActorExtension,
                Actor = chara,
                ExtraActivateMode = UpdateEvent.ExtraUpdateModes.Activate,
                AdditionID = 65002,
                AdditionSession = 0x7001,
                RestTime = 0x1388,
                AdditionCount = 1
            };
            events.Add(toadd4);

            SendActorUpdates(events);
            toadd.ActorUpdateParameters.Clear();
            toadd2.ActorUpdateParameters.Clear();
            toadd3.ActorUpdateParameters.Clear();
            toadd4.ActorUpdateParameters.Clear();
            events.Clear();
            toadd = null;
            toadd2 = null;

            chara.X = stream.X;
            chara.Y = stream.Y;
            chara.Z = stream.Z;

            LockForTeleport(false);

            toadd3.ExtraActivateMode = UpdateEvent.ExtraUpdateModes.Cancel;
            //toadd3.AddActorPara(Common.Packets.GameServer.PacketParameter.UnkD4, 0);
            toadd3.UserData = Conversions.HexStr2Bytes("090000");
            events.Add(toadd3);

            toadd4.ExtraActivateMode = UpdateEvent.ExtraUpdateModes.Cancel;
            events.Add(toadd4);

            SendActorUpdates(events);
            toadd3.ActorUpdateParameters.Clear();
            toadd4.ActorUpdateParameters.Clear();
            events.Clear();

            if (Debugging)
            {
                SM_SERVER_MESSAGE c = new SM_SERVER_MESSAGE()
                {
                    MessagePosition = SM_SERVER_MESSAGE.Positions.ChatWindow,
                    Message = string.Format("Dragon Stream:0x{0:X16}", stream.ActorID)
                };
                Network.SendPacket(c);
            }
        }

        public void OnMapPortalEnter(CM_MAP_PORTAL_ENTER p)
        {
            if (account.GMLevel > 0)
            {
                Logger.Log.Warn(string.Format("Player:{0}({1}) is trying to enter portal at Map:{6} {2},{3},{4} Dir:{5}", chara.Name, account.UserName, chara.X, chara.Y, chara.Z, chara.Dir, chara.MapID));
            }

            if (Portal.PortalDataManager.Instance.Items.ContainsKey(chara.MapID))
            {
                var query = from portal in Portal.PortalDataManager.Instance[chara.MapID]
                            where chara.DistanceToActor(portal) < 1000
                            orderby chara.DistanceToActor(portal)
                            select portal;
                bool warped = false;
                ActorPortal tmp = query.FirstOrDefault();
                if (tmp != null)
                {
                    foreach (PortalTrigger i in tmp.PortalTriggers)
                    {
                        if (i.Quest > 0)
                        {
                            if (chara.Quests.ContainsKey(i.Quest))
                            {
                                if (i.Step == chara.Quests[i.Quest].NextStep - 1 || i.Step == -1 || (i.Step == 0 && i.Step == chara.Quests[i.Quest].Step))
                                {
                                    int abs = Math.Abs(i.Dir - chara.Dir);
                                    if (abs <= 90 || abs > 270)
                                    {
                                        Map.Map map = MapManager.Instance.GetMap(i.MapTarget, chara.CharID, chara.PartyID);
                                        if (map != null)
                                        {
                                            warped = true;
                                            if (i.X == 0 && i.Y == 0 && i.Z == 0)
                                            {
                                                Map.SendActorToMap(chara, map, chara.X, chara.Y, chara.Z);
                                            }
                                            else
                                            {
                                                Map.SendActorToMap(chara, map, i.X, i.Y, i.Z);
                                            }
                                        }
                                        else
                                        {
                                            Logger.Log.Warn(string.Format("MapID:{0} isn't defined!", i.MapTarget));
                                        }

                                        break;
                                    }
                                }
                            }
                        }
                        else
                        {
                            Map.Map map = MapManager.Instance.GetMap(i.MapTarget, chara.CharID, chara.PartyID);
                            if (map != null)
                            {
                                warped = true;
                                if (i.X == 0 && i.Y == 0 && i.Z == 0)
                                {
                                    Map.SendActorToMap(chara, map, chara.X, chara.Y, chara.Z);
                                }
                                else
                                {
                                    Map.SendActorToMap(chara, map, i.X, i.Y, i.Z);
                                }
                            }
                        }
                    }
                }
                if (!warped)
                {
                    SendPortalNotWarp();
                }
            }
            else
            {
                SendPortalNotWarp();
            }
            //map.SendEventToAllActorsWhoCanSeeActor(SagaBNS.GameServer.Map.MapEvents.PORTAL_ENTER, null, chara, false);
        }

        public void OnMapPortalSomeRequest(CM_MAP_PORTAL_SOME_REQUEST p)
        {
            SM_MAP_PORTAL_SOME_REPLY p1 = new SM_MAP_PORTAL_SOME_REPLY();
            Network.SendPacket(p1);
        }

        public void SendPortalNotWarp()
        {
            SM_MAP_PORTAL_CANCEL p = new SM_MAP_PORTAL_CANCEL();
            Network.SendPacket(p);
        }

        public void SendActorList(List<Actor> actors)
        {
            if (actors.Count < 30)
            {
                SM_ACTOR_LIST p = new SM_ACTOR_LIST()
                {
                    MapInstanceID = map.InstanceID,
                    Actors = actors
                };
                Network.SendPacket(p);

                SM_ACTOR_INFO_LIST p1 = new SM_ACTOR_INFO_LIST(chara.Faction)
                {
                    MapInstanceID = map.InstanceID,
                    Actors = actors,
                    MapObjects = map.MapObjects,
                    Campfires = map.Campfires
                };
                Network.SendPacket(p1);
            }
            else
            {
                SM_ACTOR_LIST p = new SM_ACTOR_LIST()
                {
                    MapInstanceID = map.InstanceID,
                    Actors = actors.Take(30).ToList()
                };
                Network.SendPacket(p);

                SM_ACTOR_INFO_LIST p1 = new SM_ACTOR_INFO_LIST(chara.Faction)
                {
                    MapInstanceID = map.InstanceID,
                    Actors = actors.Take(30).ToList(),
                    MapObjects = map.MapObjects,
                    Campfires = map.Campfires
                };
                Network.SendPacket(p1);
                for (int i = 1; i < actors.Count / 30; i++)
                {
                    SendActorAppear(actors.Skip(i * 30).Take(30).ToList(), new List<Actor>());
                }
                if ((actors.Count % 30) > 0)
                {
                    SendActorAppear(actors.Skip((actors.Count / 30) * 30).Take(actors.Count % 30).ToList(), new List<Actor>());
                }
            }
        }

        public void OnUnknown(CM_UNKNOWN p)
        {
            Packet<GamePacketOpcode> p1 = new Packet<GamePacketOpcode>();
            switch (p.ID)
            {
                case GamePacketOpcode.CM_UNKNOWN:
                    p1.PutUShort(0xF6,0);
                    p1.PutUShort(0x14);
                    break;
                case (GamePacketOpcode)0x18F:
                    p1.PutUShort(0x190, 0);
                    p1.PutUInt(0);
                    break;
                case (GamePacketOpcode)0x198:
                    p1.PutUShort(0x199, 0);
                    p1.PutUInt(2);
                    p1.PutUShort(0);
                    break;
                case (GamePacketOpcode)0x4C:
                    p1.PutUShort(0x4D, 0);
                    p1.PutUShort(0);
                    break;
                case  (GamePacketOpcode)0xEE:
                    p1.PutUShort(0xEF, 0);
                    p1.PutUInt(4);
                    p1.PutUInt(0);
                    break;
                default:
                    return;
            }
            Logger.Log.Warn(p1.DumpData());
            Network.SendPacket(p1);
        }
    }
}
