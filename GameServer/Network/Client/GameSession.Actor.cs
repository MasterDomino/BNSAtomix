using System;
using System.Collections.Generic;
using System.Threading;
using SmartEngine.Core;
using SmartEngine.Network.Map;
using SmartEngine.Network;
using SmartEngine.Network.Tasks;
using SagaBNS.Common.Actors;
using SagaBNS.Common.Packets;
using SagaBNS.Common.Packets.GameServer;
using SagaBNS.GameServer.Map;
using SagaBNS.GameServer.Packets.Client;

namespace SagaBNS.GameServer.Network.Client
{
    public partial class GameSession : Session<GamePacketOpcode>
    {
        private ActorCorpse currentCorpse;
        private DateTime jumpStamp = DateTime.Now;
        public void OnActorMovement(CM_ACTOR_MOVEMENT p)
        {
            if (chara == null || chara.Status.Down)
            {
                return;
            }
            //Logger.Log.Info(p.ToString());
            MoveArgument arg = new MoveArgument()
            {
                BNSMoveType = p.MoveType
            };
            if (p.MoveType == SagaBNS.GameServer.Map.MoveType.Jump)
            {
                if (p.Unknown82 > 0)
                {
                    if (p.XDiff == 0 && p.YDiff == 0)
                    {
                        arg.BNSMoveType = SagaBNS.GameServer.Map.MoveType.Run;
                    }
                    else
                    {
                        arg.BNSMoveType = SagaBNS.GameServer.Map.MoveType.Falling;
                    }
                }
            }
            if (p.MoveType == SagaBNS.GameServer.Map.MoveType.DashJump)
            {
                if (p.Unknown82 > 0)
                {
                    arg.BNSMoveType = SagaBNS.GameServer.Map.MoveType.Falling;
                }
            }
            arg.X = p.X2;
            arg.Y = p.Y2;
            arg.Z = p.Z2;
            int xDiff = p.XDiff;
            int yDiff = p.YDiff;
            int zDiff = p.ZDiff;
            arg.PosDiffs.Enqueue(new sbyte[] { p.XDiff, p.YDiff, p.ZDiff });
            arg.Dir = p.Dir;
            arg.Speed = p.Speed;
            int distance = chara.DistanceToPoint(p.X2, p.Y2, p.Z2);
            //Logger.Log.Info(string.Format("Distance:{0} Diff: {1},{2},{3}", distance, p.XDiff, p.YDiff, p.ZDiff));
            if (account.GMLevel == 0 &&((distance > 40 && p.MoveType == SagaBNS.GameServer.Map.MoveType.Run) || (distance > 65 && p.MoveType == SagaBNS.GameServer.Map.MoveType.Dash)))
            {
                if (account.GMLevel > 0)
                {
                    Logger.Log.Debug("Skip movement");
                }

                arg.X = chara.X;
                arg.Y = chara.Y;
                arg.Z = chara.Z;
                arg.BNSMoveType = SagaBNS.GameServer.Map.MoveType.StepForward;
            }

            map.MoveActor(chara, arg, true);

            if (chara.Tasks.TryGetValue("SwordBlocking", out Task task))
            {
                Buff buf = (Buff)task;
                if ((buf.TotalLifeTime - buf.RestLifeTime) > 500)
                {
                    task.Deactivate();
                }
            }
            if (chara.Tasks.TryGetValue("FoodRecovery", out task))
            {
                task.Deactivate();
            }
            if (chara.Tasks.TryGetValue("Teleport", out task))
            {
                task.Deactivate();
            }
            if (chara.Party != null)
            {
                Party.PartyManager.Instance.PartyMemberPositionUpdate(chara.Party, chara);
            }
        }

        public void SaveSettings(CM_SAVE_UI_SETTING p)
        {
            chara.UISettings = p.Settings;
        }

        public void OnActorTurn(CM_ACTOR_TURN p)
        {
            UpdateEvent evt = new UpdateEvent()
            {
                Actor = chara,
                Target = chara,
                UpdateType = UpdateTypes.Actor
            };
            evt.AddActorPara(PacketParameter.Dir, p.Dir);
            map.SendEventToAllActorsWhoCanSeeActor(MapEvents.EVENT_BROADCAST, evt, chara, true);

            MoveArgument arg = new MoveArgument()
            {
                X = chara.X,
                Y = chara.Y,
                Z = chara.Z,
                Dir = p.Dir,
                BNSMoveType = SagaBNS.GameServer.Map.MoveType.Run,
                Speed = 0
            };
            map.MoveActor(chara, arg, true);

            if (chara.Party != null)
            {
                Party.PartyManager.Instance.PartyMemberDirUpdate(chara.Party, chara);
            }
        }

        public void OnActorCorpseOpen(CM_ACTOR_CORPSE_OPEN p)
        {
            ActorCorpse corspe = map.GetActor(p.ActorID) as ActorCorpse;
            if (corspe == null && map.MapObjects.ContainsKey(p.ActorID))
            {
                corspe = map.MapObjects[p.ActorID];
            }
            if (corspe != null)
            {
                if (corspe.CurrentPickingPlayer != 0 || currentCorpse != null || (corspe.Owner != chara && corspe.Owner != null && !Party.PartyManager.Instance.PartyCanLoot(chara,corspe.Owner as ActorPC)))
                {
                    SM_ACTOR_CORPSE_OPEN_FAILED p1 = new SM_ACTOR_CORPSE_OPEN_FAILED();
                    Network.SendPacket(p1);
                }
                else
                {
                    currentCorpse = corspe;
                    corspe.CurrentPickingPlayer = chara.ActorID;
                    SM_ACTOR_CORPSE_ITEM_LIST p1 = new SM_ACTOR_CORPSE_ITEM_LIST()
                    {
                        ActorID = corspe.ActorID,
                        Gold = corspe.Gold,
                        Items = corspe.AvailableItems
                    };
                    Network.SendPacket(p1);

                    UpdateEvent evt = new UpdateEvent()
                    {
                        Actor = chara,
                        Target = chara,
                        UpdateType = UpdateTypes.Actor
                    };
                    //evt.AddActorPara(PacketParameter.PickingUp, 1);
                    map.SendEventToAllActorsWhoCanSeeActor(MapEvents.EVENT_BROADCAST, evt, chara, true);

                    evt = new UpdateEvent()
                    {
                        Actor = chara,
                        Target = corspe,
                        UpdateType = UpdateTypes.CorpseInteraction,
                        UserData = (byte)0
                    };
                    map.SendEventToAllActorsWhoCanSeeActor(MapEvents.EVENT_BROADCAST, evt, chara, true);
                }
            }
            else
            {
                SM_ACTOR_CORPSE_OPEN_FAILED p1 = new SM_ACTOR_CORPSE_OPEN_FAILED();
                Network.SendPacket(p1);
            }
        }

        public void OnActorCorpseClose(CM_ACTOR_CORPSE_CLOSE p)
        {
            SendActorCorpseClose(p.ActorID);
        }

        public void OnActorItemPickUp(CM_ACTOR_ITEM_PICK_UP p)
        {
            if (map.GetActor(p.ActorID) is ActorItem item)
            {
                if (chara.DistanceToActor(item) < 50)
                {
                    if (item.Tasks.TryRemove("ItemDelete", out Task removed))
                    {
                        removed.Deactivate();
                    }

                    chara.HoldingItem = item;
                    SendHoldItem(item);

                    UpdateEvent evt = new UpdateEvent()
                    {
                        UpdateType = UpdateTypes.ItemHide,
                        Actor = chara,
                        Target = item
                    };
                    map.SendEventToAllActorsWhoCanSeeActor(MapEvents.EVENT_BROADCAST, evt, chara, true);

                    Tasks.Player.ActorItemPickDropTask task = new Tasks.Player.ActorItemPickDropTask(chara, item, Tasks.Player.ActorItemPickDropTask.ActionTypes.Pick);
                    chara.Tasks["ActorItemPickDropTask"] = task;
                    task.Activate();
                }
            }
        }

        public void OnActorItemDrop(CM_ACTOR_ITEM_DROP p)
        {
            if (chara.HoldingItem != null)
            {
                ActorItem item = chara.HoldingItem;
                Tasks.Actor.ItemDeleteTask deleteTask = new Tasks.Actor.ItemDeleteTask(item);
                deleteTask.Activate();
                SendHoldItemCancel(chara.HoldingItem.ObjectID, false);

                MoveArgument arg = new MoveArgument()
                {
                    X = p.X,
                    Y = p.Y,
                    Z = p.Z,
                    BNSMoveType = SagaBNS.GameServer.Map.MoveType.Run
                };
                map.MoveActor(item, arg);

                Tasks.Player.ActorItemPickDropTask task = new Tasks.Player.ActorItemPickDropTask(chara, item, Tasks.Player.ActorItemPickDropTask.ActionTypes.Drop);
                chara.Tasks["ActorItemPickDropTask"] = task;
                task.Activate();

                UpdateEvent evt = new UpdateEvent()
                {
                    UpdateType = UpdateTypes.ItemShow,
                    Actor = chara,
                    Target = item,
                    X = p.X,
                    Y = p.Y,
                    Z = p.Z,
                    UserData = 200
                };
                map.SendEventToAllActorsWhoCanSeeActor(MapEvents.EVENT_BROADCAST, evt, chara, true);
            }
        }

        public void SendActorCorpseClose(ulong actorID)
        {
            if (currentCorpse != null && currentCorpse.ActorID == actorID)
            {
                ActorCorpse corpse = currentCorpse;
                currentCorpse = null;
                corpse.CurrentPickingPlayer = 0;
                UpdateEvent evt = new UpdateEvent()
                {
                    Actor = chara,
                    Target = chara,
                    UpdateType = UpdateTypes.Actor
                };
                //evt.AddActorPara(PacketParameter.PickingUp, 0);
                map.SendEventToAllActorsWhoCanSeeActor(MapEvents.EVENT_BROADCAST, evt, chara, true);

                evt = new UpdateEvent()
                {
                    Actor = chara,
                    Target = corpse,
                    UpdateType = UpdateTypes.CorpseInteraction,
                    UserData = (byte)1
                };
                map.SendEventToAllActorsWhoCanSeeActor(MapEvents.EVENT_BROADCAST, evt, chara, true);

                if (corpse?.AvailableItems.Count == 0 && corpse.Gold == 0)
                {
                    if (corpse.ActorType == ActorType.MAPOBJECT)
                    {
                        evt = new UpdateEvent()
                        {
                            Actor = corpse,
                            UpdateType = UpdateTypes.MapObjectVisibilityChange
                        };
                        map.SendEventToAllActorsWhoCanSeeActor(MapEvents.EVENT_BROADCAST, evt, chara, true);
                    }
                    else
                    {
                        if (corpse.NPC.BaseData.CorpseItemID == 0 && corpse.NPC.BaseData.QuestIDs.Count == 0)
                        {
                            if (corpse.Tasks.TryRemove("CorpseDelete", out Task task))
                            {
                                task.Deactivate();
                            }

                            map.DeleteActor(corpse);
                        }
                        else
                        {
                            evt = new UpdateEvent()
                            {
                                Actor = corpse,
                                UpdateType = UpdateTypes.DeleteCorpse
                            };
                            map.SendEventToAllActorsWhoCanSeeActor(MapEvents.EVENT_BROADCAST, evt, chara, true);
                        }
                    }
                }
            }
            SM_ACTOR_CORPSE_CLOSE_RESULT p1 = new SM_ACTOR_CORPSE_CLOSE_RESULT()
            {
                ActorID = actorID
            };
            Network.SendPacket(p1);
        }

        public void OnActorCorpsePickUp(CM_ACTOR_CORPSE_PICK_UP p)
        {
            if (map.GetActor(p.ActorID) is ActorCorpse corpse)
            {
                if (!corpse.PickUp && corpse.NPC.BaseData.CorpseItemID > 0)
                {
                    corpse.PickUp = true;
                    corpse.ShouldDisappear = true;

                    SendHoldItem(corpse.NPC.BaseData.CorpseItemID, corpse.ActorID);

                    UpdateEvent evt = new UpdateEvent()
                    {
                        Actor = corpse,
                        Target = chara,
                        UpdateType = UpdateTypes.DeleteCorpse
                    };
                    map.SendEventToAllActorsWhoCanSeeActor(MapEvents.EVENT_BROADCAST, evt, corpse, false);

                    Tasks.Player.ActorItemPickDropTask task = new Tasks.Player.ActorItemPickDropTask(chara, corpse, Tasks.Player.ActorItemPickDropTask.ActionTypes.PickCorpse);
                    chara.Tasks["ActorItemPickDropTask"] = task;
                    task.Activate();
                }
            }
        }

        public void OnActorCorpseLoot(CM_ACTOR_CORPSE_LOOT p)
        {
            if (currentCorpse != null && currentCorpse.ActorID == p.ActorID)
            {
                foreach (byte i in p.Indices)
                {
                    if (i == 255)
                    {
                        Interlocked.Add(ref chara.Gold, currentCorpse.Gold);
                        currentCorpse.Gold = 0;
                        SendPlayerGold();
                    }
                    else if (currentCorpse.Items.Length > i)
                    {
                        if (chara.Inventory.FindFreeIndex(Common.Item.Containers.Inventory) >= 0)
                        {
                            if (currentCorpse.Items[i] != null)
                            {
                                AddItem(currentCorpse.Items[i]);
                                currentCorpse.Items[i] = null;
                            }
                        }
                        else
                        {
                        }
                    }
                }
                SM_ACTOR_CORPSE_LOOT_RESULT p2 = new SM_ACTOR_CORPSE_LOOT_RESULT()
                {
                    ActorID = p.ActorID,
                    Indices = p.Indices
                };
                Network.SendPacket(p2);
            }
        }

        public void SendActorUpdates(List<UpdateEvent> events)
        {
            SM_ACTOR_UPDATE_LIST p = new SM_ACTOR_UPDATE_LIST()
            {
                Events = events
            };
            Network.SendPacket(p);
            /*foreach (UpdateEvent i in events)
            {
                i.Actor = null;
                i.Target = null;
                i.UserData = null;
                i.UserData2 = null;                
            }*/
            events.Clear();
        }

        public void SendActorAppear(List<Actor> appearActors, List<Actor> disappearActors)
        {
            SM_ACTOR_APPEAR_LIST p = new SM_ACTOR_APPEAR_LIST()
            {
                MapInstanceID = map.InstanceID,
                DisappearActors = disappearActors,
                AppearActors = appearActors
            };
            Network.SendPacket(p);

            SM_ACTOR_APPEAR_INFO_LIST p2 = new SM_ACTOR_APPEAR_INFO_LIST()
            {
                PlayerFaction = chara.Faction,
                MapInstanceID = map.InstanceID,
                DisappearActors = disappearActors,
                AppearActors = appearActors
            };
            Network.SendPacket(p2);
        }

        public void SendActorAggro(ulong actorID)
        {
            SM_ACTOR_AGGRO p = new SM_ACTOR_AGGRO()
            {
                ActorID = actorID
            };
            Network.SendPacket(p);
        }
    }
}
