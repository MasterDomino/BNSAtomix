using System.Collections.Generic;
using System.Linq;
using SmartEngine.Network;
using SagaBNS.Common.Actors;
using SagaBNS.Common.Quests;
using SagaBNS.Common.Packets;
using SagaBNS.Common.Packets.GameServer;
using SagaBNS.GameServer.Map;
using SagaBNS.GameServer.Packets.Client;
using SagaBNS.GameServer.Network.CharacterServer;
using SagaBNS.GameServer.Scripting;
using SagaBNS.GameServer.Quests;
using SmartEngine.Core;

namespace SagaBNS.GameServer.Network.Client
{
    public partial class GameSession : Session<GamePacketOpcode>
    {
        private ActorMapObj currentMapObj;
        public void OnQuestNPCOpen(CM_QUEST_NPC_OPEN p)
        {
            SM_QUEST_NPC_OPEN p1 = new SM_QUEST_NPC_OPEN()
            {
                ActorID = p.ActorID
            };
            Network.SendPacket(p1);
        }

        public void OnQuestLootQuestItem(CM_QUEST_LOOT_QUEST_ITEM p)
        {
            if (currentCorpse != null && currentCorpse.ActorID == p.ActorID)
            {
                currentCorpse.QuestID = p.QuestID;
                currentCorpse.Step = p.Step;
                if (chara.Quests.TryGetValue(currentCorpse.QuestID, out Quest q))
                {
                    QuestManager.Instance.ProcessQuest(chara, currentCorpse.QuestID, currentCorpse.Step, q, currentCorpse.NPC, true);
                }

                for (int i = 0; i < currentCorpse.NPC.BaseData.QuestIDs.Count; i++)
                {
                    if (currentCorpse.NPC.BaseData.QuestIDs[i] == p.QuestID)
                    {
                        UpdateEvent evt = new UpdateEvent()
                        {
                            Actor = currentCorpse,
                            UpdateType = UpdateTypes.CorpseDoQuest,
                            UserData = currentCorpse.NPC.BaseData.QuestSteps[i]
                        };
                        map.SendEventToAllActorsWhoCanSeeActor(MapEvents.EVENT_BROADCAST, evt, currentCorpse, false);
                    }
                }
            }
        }

        public void OnQuestAccept(CM_QUEST_ACCEPT p)
        {
            ushort qID = (ushort)p.QuestID;
            ActorNPC npc = map.GetActor(p.ActorID) as ActorNPC;
            if (account.GMLevel > 0)
            {
                Logger.Log.Warn(string.Format("Player:{0}({1}) requests to accept quest:{2} at NPC:{3}", chara.Name, account.UserName, p.QuestID, npc.NpcID));
            }

            if (!chara.Quests.ContainsKey(qID) && !chara.QuestsCompleted.Contains(qID))
            {
                SendNextQuest(qID);
            }
        }

        public void DropQuest(CM_QUEST_DROP p)
        {
            if (chara.Quests.ContainsKey(p.Quest))
            {
                chara.Quests.TryRemove(p.Quest, out Quest dummy);
                SM_QUEST_DROP r = new SM_QUEST_DROP()
                {
                    Quest = p.Quest
                };
                Network.SendPacket(r);
            }
        }

        public void OnQuestUpdateRequest(CM_QUEST_UPDATE_REQUEST p)
        {
            if (p.NpcActorID != 0)
            {
                Quest q =null;
                if (chara.Quests.ContainsKey(p.QuestID))
                {
                    q = chara.Quests[p.QuestID];
                }

                ActorExt actor = map.GetActor(p.NpcActorID) as ActorExt;
                if (actor is ActorCorpse corpse)
                {
                    corpse.QuestID = p.QuestID;
                    corpse.Step = p.Step;
                    if (chara.Quests.TryGetValue(corpse.QuestID, out q))
                    {
                        QuestManager.Instance.ProcessQuest(chara, corpse.QuestID, corpse.Step, q, corpse.NPC, true);
                    }

                    for (int i = 0; i < corpse.NPC.BaseData.QuestIDs.Count; i++)
                    {
                        if (corpse.NPC.BaseData.QuestIDs[i] == p.QuestID)
                        {
                            UpdateEvent evt = new UpdateEvent()
                            {
                                Actor = corpse,
                                UpdateType = UpdateTypes.CorpseDoQuest,
                                UserData = currentCorpse.NPC.BaseData.QuestSteps[i]
                            };
                            map.SendEventToAllActorsWhoCanSeeActor(MapEvents.EVENT_BROADCAST, evt, corpse, false);
                        }
                    }
                }
                else
                {
                    if (actor is ActorNPC npc)
                    {
                        if (account.GMLevel > 0)
                        {
                            Logger.Log.Warn(string.Format("Player:{0}({1}) requests to update quest:{2} to step:{3} at NPC:{4}", chara.Name, account.UserName, p.QuestID, p.Step, npc.NpcID));
                        }

                        if (Scripting.ScriptManager.Instance.NpcScripts.ContainsKey(npc.NpcID))
                        {
                            ((Scripting.NPCScriptHandler)npc.EventHandler).OnQuest(chara, q.QuestID, q.NextStep > 0 ? q.NextStep : (byte)1, q);
                        }
                        QuestManager.Instance.ProcessQuest(chara, q.QuestID, q.NextStep > 0 ? q.NextStep : (byte)1, q, npc, false, false, p.RewardSelection);
                    }
                    else
                    {
                        if (account.GMLevel > 0)
                        {
                            Logger.Log.Warn(string.Format("Player:{0}({1}) requests to update quest:{2} to step:{3} at NPC:{4}", chara.Name, account.UserName, p.QuestID, p.Step, p.NpcActorID));
                        }

                        ProcessQuest(p.QuestID, p.Step);
                    }
                }
            }
            else
            {
                ProcessQuest(p.QuestID, p.Step);
            }
        }

        private void ProcessQuest(ushort questID, byte step)
        {
            chara.Quests.TryGetValue(questID, out Quest quest);
            QuestManager.Instance.ProcessQuest(chara, quest.QuestID, quest.NextStep > 0 ? quest.NextStep : (byte)1, quest);
        }

        public void SendQuestUpdate(Quest quest)
        {
            SM_QUEST_UPDATE p1 = new SM_QUEST_UPDATE()
            {
                QuestID = quest.QuestID,
                Step = quest.Step,
                StepStatus = quest.StepStatus,
                NextStep = quest.NextStep,
                Flag1 = quest.Flag1,
                Flag2 = quest.Flag2,
                Flag3 = quest.Flag3
            };
            Network.SendPacket(p1);
        }

        public void FinishQuest(Quest quest)
        {
            chara.Quests.TryRemove(quest.QuestID, out Quest dummy);
            chara.QuestsCompleted.Add(quest.QuestID);
            SM_QUEST_FINISH p1 = new SM_QUEST_FINISH()
            {
                QuestID = quest.QuestID,
                Step = quest.Step,
                StepStatus = 0
            };
            Network.SendPacket(p1);
        }

        public void SendNextQuest(ushort next)
        {
            Common.Quests.Quest q = new Quest()
            {
                QuestID = next,
                NextStep = 1
            };
            chara.Quests[next] = q;
            SM_QUEST_NEXT_QUEST p = new SM_QUEST_NEXT_QUEST()
            {
                QuestID = next
            };
            Network.SendPacket(p);
        }

        public void OnMapObjectOpen(CM_MAPOBJECT_OPEN p)
        {
            Logger.Log.Info("MapObj:" + (p.ActorID & 0xFFFFFFFF));
            if (map.MapObjects.ContainsKey(p.ActorID))
            {
                currentMapObj = map.MapObjects[p.ActorID];
                UpdateEvent evt = new UpdateEvent()
                {
                    Actor = chara,
                    UpdateType = UpdateTypes.MapObjectOperate,
                    Target = currentMapObj,
                    UserData = (byte)0
                };
                map.SendEventToAllActorsWhoCanSeeActor(MapEvents.EVENT_BROADCAST, evt, chara, true);

                SmartEngine.Network.Tasks.SimpleTask sp = new SmartEngine.Network.Tasks.SimpleTask("sp", 2000, (task) =>
                {
                    currentMapObj.Available = false;
                    evt = new UpdateEvent()
                    {
                        Actor = currentMapObj,
                        UpdateType = UpdateTypes.MapObjectVisibilityChange,
                        Target = chara
                    };
                    map.SendEventToAllActorsWhoCanSeeActor(MapEvents.EVENT_BROADCAST, evt, chara, true);

                    evt = new UpdateEvent()
                    {
                        Actor = chara,
                        UpdateType = UpdateTypes.MapObjectOperate,
                        Target = currentMapObj,
                        UserData = (byte)1
                    };
                    map.SendEventToAllActorsWhoCanSeeActor(MapEvents.EVENT_BROADCAST, evt, chara, true);
                });
                sp.Activate();
            }
        }

        public void OnMapObjectClose(CM_MAPOBJECT_CLOSE p)
        {
            if (currentMapObj != null)
            {
                SM_MAPOBJECT_CLOSE p1 = new SM_MAPOBJECT_CLOSE()
                {
                    ActorID = p.ActorID
                };
                Network.SendPacket(p1);

                UpdateEvent evt = new UpdateEvent()
                {
                    Actor = chara,
                    UpdateType = UpdateTypes.Actor,
                    Target = chara
                };
                evt.AddActorPara(PacketParameter.PickingObject, 0);
                evt.AddActorPara(PacketParameter.PickingStatus, 0);
                map.SendEventToAllActorsWhoCanSeeActor(MapEvents.EVENT_BROADCAST, evt, chara, true);

                evt = new UpdateEvent()
                {
                    Actor = chara,
                    UpdateType = UpdateTypes.MapObjectInteraction,
                    Target = currentMapObj,
                    UserData = (byte)1
                };
                map.SendEventToAllActorsWhoCanSeeActor(MapEvents.EVENT_BROADCAST, evt, chara, true);

                currentMapObj = null;
            }
        }

        public void OnMapObjectInventoryOpen(CM_MAPOBJECT_INVENTORY_OPEN p)
        {
            if (currentMapObj != null)
            {
                SM_MAPOBJECT_INVENTORY_OPEN p1 = new SM_MAPOBJECT_INVENTORY_OPEN()
                {
                    ActorID = p.ActorID
                };
                Network.SendPacket(p1);

                UpdateEvent evt = new UpdateEvent()
                {
                    Actor = chara,
                    UpdateType = UpdateTypes.Actor,
                    Target = chara
                };
                evt.AddActorPara(PacketParameter.PickingObject, (long)currentMapObj.ActorID);
                evt.AddActorPara(PacketParameter.PickingStatus, 2);
                map.SendEventToAllActorsWhoCanSeeActor(MapEvents.EVENT_BROADCAST, evt, chara, true);

                evt = new UpdateEvent()
                {
                    Actor = chara,
                    UpdateType = UpdateTypes.MapObjectInteraction,
                    Target = currentMapObj,
                    UserData = (byte)0
                };
                map.SendEventToAllActorsWhoCanSeeActor(MapEvents.EVENT_BROADCAST, evt, chara, true);
            }
        }

        public void OnMapObjectGetItem(CM_MAPOBJECT_GET_ITEM p)
        {
            if (currentMapObj != null)
            {
                DoOperateMapObject(currentMapObj);
                UpdateEvent evt = new UpdateEvent()
                {
                    Actor = chara,
                    UpdateType = UpdateTypes.MapObjectOperate,
                    Target = currentMapObj,
                    UserData = (byte)1
                };
                map.SendEventToAllActorsWhoCanSeeActor(MapEvents.EVENT_BROADCAST, evt, chara, true);
            }
        }

        public void OnQuestUpdateRequestMapObject(CM_QUEST_UPDATE_REQUEST_MAPOBJECT p)
        {
            if (currentMapObj != null && currentMapObj.ActorID == p.NpcActorID)
            {
                DoOperateMapObject(currentMapObj);

                UpdateEvent evt = new UpdateEvent()
                {
                    Actor = currentMapObj,
                    UpdateType = UpdateTypes.MapObjectDoQuest,
                    UserData = p.QuestID,
                    UserData2 = (ushort)(p.Unknown << 8 | p.Step)
                };
                map.SendEventToAllActorsWhoCanSeeActor(MapEvents.EVENT_BROADCAST, evt, currentMapObj, true);
            }
        }

        private void DoOperateMapObject(ActorMapObj obj)
        {
            //if (obj.Available)
            {
                obj.Available = false;
                QuestManager.Instance.ProcessQuest(chara, obj);
                ulong scriptID = obj.ToULong();
                if (ScriptManager.Instance.MapObjectScripts.ContainsKey(scriptID))
                {
                    ScriptManager.Instance.MapObjectScripts[scriptID].OnOperate(chara, map);
                }/*
                List<Common.Item.Item> items = new List<Common.Item.Item>();
                foreach (uint i in obj.ItemIDs.Keys)
                {
                    Common.Item.Item item = Item.ItemFactory.Instance.CreateNewItem(i);
                    if (item == null)
                    {
                        Logger.Log.Warn("Cannot find item:" + i);
                        continue;
                    }
                    item.Count = (ushort)obj.ItemIDs[i];
                    items.Add(item);
                }
                obj.Items = items.ToArray();
                if (obj.MinGold > 0)
                    obj.Gold = Global.Random.Next(obj.MinGold, obj.MaxGold);

                if (obj.RespawnTime > 0)
                {
                    Tasks.Actor.MapObjRespawnTask task = new Tasks.Actor.MapObjRespawnTask(obj);
                    task.Activate();
                }

                UpdateEvent evt = new UpdateEvent();
                evt.Actor = obj;
                evt.UpdateType = UpdateTypes.MapObjectVisibilityChange;
                map.SendEventToAllActors(MapEvents.EVENT_BROADCAST, evt, obj, true);*/
            }
        }

        public void OnGotQuestInfo(List<Quest> quests, List<ushort> completed)
        {
            foreach (Quest i in quests)
            {
                chara.Quests[i.QuestID] = i;
            }
            foreach (ushort i in completed)
            {
                chara.QuestsCompleted.Add(i);
            }
            CharacterSession.Instance.GetLocations(chara.CharID, this);
            //((BNSGameNetwork<GamePacketOpcode>)Network).SendExchangePacket();
        }

        public void SendQuestList()
        {
            SM_QUEST_INFO p = new SM_QUEST_INFO()
            {
                Quests = chara.Quests.Values.ToList()
            };
            Network.SendPacket(p);
            {
                SM_QUEST_HISTORY p2 = new SM_QUEST_HISTORY()
                {
                    QuestsCompelted = chara.QuestsCompleted
                };
                Network.SendPacket(p2);
            }
        }

        public void SendHoldItem(ActorItem actor)
        {
            UpdateEvent evt = new UpdateEvent()
            {
                Actor = chara,
                Target = chara,
                UpdateType = UpdateTypes.Actor
            };
            //evt.AddActorPara(PacketParameter.Hold, (long)actor.ActorID);
            map.SendEventToAllActorsWhoCanSeeActor(MapEvents.EVENT_BROADCAST, evt, chara, true);

            chara.HoldingItem = actor;
        }

        public void SendHoldItem(uint item, ulong corpseID = 0)
        {
            ActorItem actor = new ActorItem(item)
            {
                CorpseID = corpseID,
                Creator = chara,
                X = chara.X,
                Y = chara.Y,
                Z = chara.Z,
                EventHandler = new ActorEventHandlers.DummyEventHandler()
            };
            map.RegisterActor(actor);
            SendHoldItem(actor);
            actor.Invisible = false;
            map.OnActorVisibilityChange(actor);
        }

        public void SendHoldItemCancel(uint item, bool deleteItem = true)
        {
            if (chara.HoldingItem.ObjectID == item)
            {
                UpdateEvent evt = new UpdateEvent()
                {
                    Actor = chara,
                    Target = chara,
                    UpdateType = UpdateTypes.Actor
                };
                //evt.AddActorPara(PacketParameter.Hold, 0);
                map.SendEventToAllActorsWhoCanSeeActor(MapEvents.EVENT_BROADCAST, evt, chara, true);

                if (deleteItem)
                {
                    map.DeleteActor(chara.HoldingItem);
                }
                chara.HoldingItem = null;
            }
        }

        public void SendQuestCutScene(uint cutscene)
        {
            SM_QUEST_CUTSCENE p = new SM_QUEST_CUTSCENE()
            {
                CutsceneID = cutscene
            };
            Network.SendPacket(p);
        }
    }
}
