using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

using SmartEngine.Network.Utils;
using SagaBNS.Common.Actors;
using SagaBNS.Common.Skills;
using SagaBNS.Common.Quests;
using SagaBNS.GameServer.Map;
using SagaBNS.GameServer.Skills;
using SagaBNS.GameServer.Manager;

namespace SagaBNS.GameServer.Quests
{
    public class QuestManager : Factory<QuestManager, QuestDetail>
    {
        private readonly Dictionary<ushort, Dictionary<uint, QuestDetail>> npcMapping = new Dictionary<ushort, Dictionary<uint, QuestDetail>>();
        private readonly Dictionary<ulong, Dictionary<uint, byte>> mapObjectMapping = new Dictionary<ulong, Dictionary<uint, byte>>();
        public QuestManager()
        {
            loadingTab = "Loading quest database";
            loadedTab = " quests loaded.";
            databaseName = "Quests";
            FactoryType = FactoryType.XML;
        }

        protected override uint GetKey(QuestDetail item)
        {
            return item.QuestID;
        }

        protected override void ParseCSV(QuestDetail item, string[] paras)
        {
            throw new NotImplementedException();
        }

        protected override void ParseXML(System.Xml.XmlElement root, System.Xml.XmlElement current, QuestDetail item)
        {
            switch (root.Name.ToLower())
            {
                case "quest":
                    {
                        switch (current.Name.ToLower())
                        {
                            case "id":
                                item.QuestID = uint.Parse(current.InnerText);
                                break;
                            case "step":
                                {
                                    QuestStep s = new QuestStep()
                                    {
                                        StepID = byte.Parse(current.Attributes["id"].Value),
                                        Finish = current.HasAttribute("finish") && bool.Parse(current.Attributes["finish"].Value)
                                    };
                                    item.Steps.Add(s.StepID, s);
                                }
                                break;
                            case "nextquest":
                                {
                                    Job job = Job.None;
                                    if (current.HasAttribute("job"))
                                    {
                                        job = (Job)Enum.Parse(typeof(Job), current.Attributes["job"].Value);
                                    }

                                    item.NextQuest[job] = uint.Parse(current.InnerText);
                                }
                                break;
                        }
                    }
                    break;
                case "step":
                    {
                        QuestStep step = item.Steps[byte.Parse(root.Attributes["id"].Value)];
                        switch (current.Name.ToLower())
                        {
                            case "target":
                                {
                                    QuestTarget target = new QuestTarget();
                                    string type = current.Attributes["type"].Value;
                                    switch (type.ToLower())
                                    {
                                        case "npc":
                                            target.TargetType = StepTargetType.NPC;
                                            break;
                                        case "mapobject":
                                            target.TargetType = StepTargetType.MapObject;
                                            break;
                                        case "map":
                                            target.TargetType = StepTargetType.Map;
                                            break;
                                        case "loot":
                                            target.TargetType = StepTargetType.Loot;
                                            break;
                                        default:
                                            target.TargetType = StepTargetType.None;
                                            break;
                                    }
                                    if (current.HasAttribute("flagidx"))
                                    {
                                        target.TargetFlagIndex = int.Parse(current.Attributes["flagidx"].Value);
                                    }

                                    if (current.HasAttribute("increment"))
                                    {
                                        target.TargetFlagIncrement = short.Parse(current.Attributes["increment"].Value);
                                    }

                                    if (current.HasAttribute("index"))
                                    {
                                        target.SpecifyIndex = int.Parse(current.Attributes["index"].Value);
                                    }

                                    if (current.HasAttribute("finishflagidx"))
                                    {
                                        target.TargetFinishFlagIndex = int.Parse(current.Attributes["finishflagidx"].Value);
                                    }

                                    if (current.HasAttribute("finishincrement"))
                                    {
                                        target.TargetFinishFlagIncrement = short.Parse(current.Attributes["finishincrement"].Value);
                                    }

                                    if (current.HasAttribute("count"))
                                    {
                                        target.TargetCount = short.Parse(current.Attributes["count"].Value);
                                    }

                                    if (current.HasAttribute("x"))
                                    {
                                        target.HasCoordinate = true;
                                        target.TargetX = short.Parse(current.Attributes["x"].Value);
                                    }
                                    if (current.HasAttribute("y"))
                                    {
                                        target.HasCoordinate = true;
                                        target.TargetY = short.Parse(current.Attributes["y"].Value);
                                    }
                                    if (current.HasAttribute("z"))
                                    {
                                        target.HasCoordinate = true;
                                        target.TargetZ = short.Parse(current.Attributes["z"].Value);
                                    }
                                    if (target.TargetType != StepTargetType.None)
                                    {
                                        foreach (string s in current.InnerText.Split(','))
                                        {
                                            target.TargetIDs.Add(uint.Parse(s));
                                        }
                                    }

                                    if (target.TargetType == StepTargetType.MapObject)
                                    {
                                        target.TargetMapID = uint.Parse(current.Attributes["map"].Value);
                                        foreach (uint id in target.TargetIDs)
                                        {
                                            ulong objID = (ulong)target.TargetMapID << 32 | id;
                                            Dictionary<uint, byte> tbl;
                                            if (mapObjectMapping.ContainsKey(objID))
                                            {
                                                tbl = mapObjectMapping[objID];
                                            }
                                            else
                                            {
                                                tbl = new Dictionary<uint, byte>();
                                                mapObjectMapping.Add(objID, tbl);
                                            }
                                            tbl.Add(item.QuestID, step.StepID);
                                        }
                                    }
                                    if (target.TargetType == StepTargetType.Map && target.HasCoordinate)
                                    {
                                        foreach (uint i in target.TargetIDs)
                                        {
                                            NPC.SpawnData spawn = new NPC.SpawnData()
                                            {
                                                IsQuest = true,
                                                MapID = i,
                                                X = target.TargetX,
                                                Y = target.TargetY,
                                                Z = target.TargetZ
                                            };
                                            NPC.NPCSpawnManager.Instance[i].Add(spawn);
                                        }
                                    }
                                    step.Targets.Add(target);
                                }
                                break;
                            case "nextstep":
                                step.NextStep = byte.Parse(current.InnerText);
                                break;
                            case "stepstatus":
                                step.StepStatus = byte.Parse(current.InnerText);
                                break;
                            case "giveitem":
                                {
                                    ushort count = 1;
                                    Job job = Job.None;
                                    if (current.HasAttribute("count"))
                                    {
                                        count = ushort.Parse(current.Attributes["count"].Value);
                                    }

                                    if (current.HasAttribute("job"))
                                    {
                                        job = (Job)Enum.Parse(typeof(Job), current.Attributes["job"].Value);
                                    }

                                    step.GiveItems[job].Add(uint.Parse(current.InnerText), count);
                                }
                                break;
                            case "rewarditemoption":
                                {
                                    int count = 1;
                                    Job job = Job.None;
                                    if (current.HasAttribute("count"))
                                    {
                                        count = int.Parse(current.Attributes["count"].Value);
                                    }

                                    if (current.HasAttribute("job"))
                                    {
                                        job = (Job)Enum.Parse(typeof(Job), current.Attributes["job"].Value);
                                    }

                                    step.RewardOptions[job][uint.Parse(current.InnerText)] = count;
                                }
                                break;
                            case "teleport":
                                step.TeleportMapID = uint.Parse(current.InnerText);
                                if (current.HasAttribute("x"))
                                {
                                    step.X = short.Parse(current.Attributes["x"].Value);
                                }

                                if (current.HasAttribute("y"))
                                {
                                    step.Y = short.Parse(current.Attributes["y"].Value);
                                }

                                if (current.HasAttribute("z"))
                                {
                                    step.Z = short.Parse(current.Attributes["z"].Value);
                                }

                                if (current.HasAttribute("u1"))
                                {
                                    step.TeleportU1 = short.Parse(current.Attributes["u1"].Value);
                                }

                                if (current.HasAttribute("u2"))
                                {
                                    step.TeleportU2 = short.Parse(current.Attributes["u2"].Value);
                                }

                                if (current.HasAttribute("cutscene"))
                                {
                                    step.TeleportCutscene = uint.Parse(current.Attributes["cutscene"].Value);
                                }

                                break;
                            case "cutscene":
                                step.CutScene = uint.Parse(current.InnerText);
                                break;
                            case "takeitem":
                                {
                                    ushort count = 1;
                                    Job job = Job.None;
                                    if (current.HasAttribute("count"))
                                    {
                                        count = ushort.Parse(current.Attributes["count"].Value);
                                    }

                                    if (current.HasAttribute("job"))
                                    {
                                        job = (Job)Enum.Parse(typeof(Job), current.Attributes["job"].Value);
                                    }

                                    step.TakeItems[job].Add(uint.Parse(current.InnerText), count);
                                }
                                break;
                            case "exp":
                                {
                                    step.Exp = uint.Parse(current.InnerText);
                                }
                                break;
                            case "learnskill":
                                {
                                    Job job = Job.None;
                                    if (current.HasAttribute("job"))
                                    {
                                        job = (Job)Enum.Parse(typeof(Job), current.Attributes["job"].Value);
                                    }

                                    step.LearnSkills[job].Add(uint.Parse(current.InnerText));
                                }
                                break;
                            case "gold":
                                {
                                    step.Gold = int.Parse(current.InnerText);
                                }
                                break;
                            case "spawn":
                                {
                                    NPC.SpawnData spawn = new NPC.SpawnData()
                                    {
                                        NpcID = ushort.Parse(current.Attributes["npcID"].Value),
                                        X = short.Parse(current.Attributes["x"].Value),
                                        Y = short.Parse(current.Attributes["y"].Value),
                                        Z = short.Parse(current.Attributes["z"].Value),
                                        Dir = ushort.Parse(current.Attributes["dir"].Value)
                                    };
                                    if (current.HasAttribute("motion"))
                                    {
                                        spawn.Motion = ushort.Parse(current.Attributes["motion"].Value);
                                    }

                                    if (current.HasAttribute("appeareffect"))
                                    {
                                        spawn.AppearEffect = ushort.Parse(current.Attributes["appeareffect"].Value);
                                    }

                                    step.Spawns.Add(spawn);
                                }
                                break;
                            case "flag1":
                                step.Flag1 = short.Parse(current.InnerText);
                                break;
                            case "flag2":
                                step.Flag2 = short.Parse(current.InnerText);
                                break;
                            case "flag3":
                                step.Flag3 = short.Parse(current.InnerText);
                                break;
                            case "holditem":
                                step.HoldItem = uint.Parse(current.InnerText);
                                break;
                            case "dropitem":
                                step.DropItem = uint.Parse(current.InnerText);
                                break;
                        }
                    }
                    break;
            }
        }

        private bool ProcessTarget(ActorPC pc, Quest quest, QuestStep s, QuestTarget t,int index)
        {
            bool finished = false;
            if (t.TargetFlagIncrement > 0)
            {
                quest.Step = (byte)(quest.NextStep + index);
                bool shoudUpdate = true;

                quest.Count[index]++;
                if (quest.Count[index] >= t.TargetCount)
                {
                    if (quest.Count[index] == t.TargetCount)
                    {
                        switch (t.TargetFinishFlagIndex)
                        {
                            case 1:
                                quest.Flag1 += t.TargetFinishFlagIncrement;
                                break;
                            case 2:
                                quest.Flag2 += t.TargetFinishFlagIncrement;
                                break;
                            case 3:
                                quest.Flag3 += t.TargetFinishFlagIncrement;
                                break;
                        }
                    }
                    else
                    {
                        quest.Count[index]--;
                        shoudUpdate = false;
                    }
                }
                if (shoudUpdate)
                {
                    switch (t.TargetFlagIndex)
                    {
                        case 1:
                            quest.Flag1 += t.TargetFlagIncrement;
                            if (quest.Flag1 == s.Flag1)
                            {
                                finished = true;
                            }

                            break;
                        case 2:
                            quest.Flag2 += t.TargetFlagIncrement;
                            if (quest.Flag2 == s.Flag2)
                            {
                                finished = true;
                            }

                            break;
                        case 3:
                            quest.Flag3 += t.TargetFlagIncrement;
                            if (quest.Flag3 == s.Flag3)
                            {
                                finished = true;
                            }

                            break;
                    }
                }
            }
            else
            {
                finished = true;
            }

            return finished;
        }

        public void ProcessQuest(ActorPC pc, ushort questID, byte step, Quest quest, ActorNPC npc, bool isLoot = false, bool isHunt = false, int selection = 0, bool party = false)
        {
            if (pc.Party != null && (isLoot || isHunt) && !party)
            {
                foreach (ActorPC i in pc.Party.Members)
                {
                    if (i != pc && !i.Offline && pc.MapInstanceID == i.MapInstanceID && pc.DistanceToActor(pc) < 200)
                    {
                        if (i.Quests.TryGetValue(questID, out Quest q))
                        {
                            ProcessQuest(i, questID, step, q, npc, isLoot, isHunt, selection, true);
                        }
                    }
                }
            }
            if (items.ContainsKey(questID))
            {
                QuestDetail detail = items[questID];
                if (pc.Quests.ContainsKey(questID) && detail.Steps.ContainsKey(pc.Quests[questID].NextStep))
                {
                    QuestStep s = detail.Steps[pc.Quests[questID].NextStep];
                    byte index = 0;
                    foreach (QuestTarget t in s.Targets)
                    {
                        if (((t.TargetType == StepTargetType.NPC && !isLoot) || (isLoot && t.TargetType == StepTargetType.Loot)) && t.TargetIDs.Contains(npc.NpcID))
                        {
                            if (s.DropItem > 0 && !isHunt && !isLoot)
                            {
                                if (pc.HoldingItem != null && s.DropItem == pc.HoldingItem.ObjectID)
                                {
                                    HoldItemCancel(pc, s.DropItem);
                                }
                                else
                                {
                                    continue;
                                }
                            }
                            if (quest == null)
                            {
                                quest = new Quest()
                                {
                                    QuestID = questID
                                };
                                if (pc.Quests.ContainsKey(questID))
                                {
                                    return;
                                }
                                else
                                {
                                    pc.Quests[questID] = quest;
                                }
                            }
                            bool finished = ProcessTarget(pc, quest, s, t, t.SpecifyIndex >= 0 ? t.SpecifyIndex : index);

                            if (finished)
                            {
                                ProcessQuestSub(pc, quest, s, detail, selection);
                            }
                            else
                            {
                                UpdateQuest(pc, quest);
                            }

                            QuestArgument arg = new QuestArgument()
                            {
                                Player = pc,
                                OriginNPC = npc.NpcID,
                                Quest = quest,
                                Step = quest.Step
                            };
                            pc.Client().Map.SendEventToAllActorsWhoCanSeeActor(MapEvents.QUEST_UPDATE, arg, pc, false);
                        }
                        index++;
                    }
                }
            }
        }

        public void ProcessQuest(ActorPC pc, ActorMapObj obj)
        {
            ulong objID = obj.ToULong();
            if (mapObjectMapping.ContainsKey(objID))
            {
                foreach (uint questID in mapObjectMapping[objID].Keys)
                {
                    if (items.ContainsKey(questID))
                    {
                        QuestDetail detail = items[questID];
                        byte step = mapObjectMapping[objID][questID];
                        Quest quest;
                        if (detail.Steps.ContainsKey(step))
                        {
                            QuestStep s = detail.Steps[step];
                            byte index = 0;
                            foreach (QuestTarget t in s.Targets)
                            {
                                if (t.TargetType == StepTargetType.MapObject && t.TargetIDs.Contains(obj.ObjectID) && t.TargetMapID == obj.MapID)
                                {
                                    if (s.DropItem > 0)
                                    {
                                        if (pc.HoldingItem != null && s.DropItem == pc.HoldingItem.ObjectID)
                                        {
                                            HoldItemCancel(pc, s.DropItem);
                                        }
                                        else
                                        {
                                            continue;
                                        }
                                    }

                                    if (pc.Quests.ContainsKey((ushort)questID))
                                    {
                                        quest = pc.Quests[(ushort)questID];
                                    }
                                    else
                                    {
                                        continue;
                                    }

                                    bool finished = ProcessTarget(pc, quest, s, t, t.SpecifyIndex >= 0 ? t.SpecifyIndex : index);
                                    if (finished)
                                    {
                                        ProcessQuestSub(pc, quest, s, detail, 0);
                                    }
                                    else
                                    {
                                        UpdateQuest(pc, quest);
                                    }

                                    QuestArgument arg = new QuestArgument()
                                    {
                                        Player = pc,
                                        Quest = quest,
                                        Step = quest.Step
                                    };
                                    pc.Client().Map.SendEventToAllActorsWhoCanSeeActor(MapEvents.QUEST_UPDATE, arg, pc, false);
                                }
                                index++;
                            }
                        }
                    }
                }
            }
        }

        public void ProcessQuest(ActorPC pc, ushort questID, byte step, Quest quest)
        {
            if (items.ContainsKey(questID))
            {
                QuestDetail detail = items[questID];
                if (detail.Steps.ContainsKey(step))
                {
                    QuestStep s = detail.Steps[step];
                    foreach (QuestTarget t in s.Targets)
                    {
                        if (t.TargetType == StepTargetType.None)
                        {
                            if (quest == null)
                            {
                                quest = new Quest()
                                {
                                    QuestID = questID
                                };
                                if (pc.Quests.ContainsKey(questID))
                                {
                                    return;
                                }
                                else
                                {
                                    pc.Quests[questID] = quest;
                                }
                            }
                            ProcessQuestSub(pc, quest, s, detail);
                            QuestArgument arg = new QuestArgument()
                            {
                                Player = pc,
                                Quest = quest,
                                Step = quest.Step
                            };
                            pc.Client().Map.SendEventToAllActorsWhoCanSeeActor(MapEvents.QUEST_UPDATE, arg, pc, false);
                        }
                    }
                }
            }
        }

        public void ProcessQuest(ActorPC pc, uint mapID, bool hasCoordinate = false)
        {
            foreach (ushort questID in pc.Quests.Keys)
            {
                Quest quest = pc.Quests[questID];
                if (items.ContainsKey(questID))
                {
                    QuestDetail detail = items[questID];
                    if (detail.Steps.ContainsKey(quest.NextStep))
                    {
                        QuestStep s = detail.Steps[quest.NextStep];
                        foreach (QuestTarget t in s.Targets)
                        {
                            if (t.TargetType == StepTargetType.Map && t.TargetIDs.Contains(mapID) && ((hasCoordinate && t.HasCoordinate) || (!hasCoordinate && !t.HasCoordinate)))
                            {
                                ProcessQuestSub(pc, quest, s, detail);
                                QuestArgument arg = new QuestArgument()
                                {
                                    Player = pc,
                                    Quest = quest,
                                    Step = quest.Step
                                };
                                pc.Client().Map.SendEventToAllActorsWhoCanSeeActor(MapEvents.QUEST_UPDATE, arg, pc, false);
                            }
                        }
                    }
                }
            }
        }

        private void ProcessQuestSub(ActorPC pc, Quest quest, QuestStep s, QuestDetail detail, int selection = 0)
        {
            if (pc.EventHandler == null)
            {
                return;
            }

            if (s.NextStep > 0 || s.Finish)
            {
                quest.Step = s.StepID;
                if (!s.Finish)
                {
                    quest.StepStatus = s.StepStatus;
                    quest.NextStep = s.NextStep;
                    quest.Flag1 = s.Flag1;
                    quest.Flag2 = s.Flag2;
                    quest.Flag3 = s.Flag3;
                    UpdateQuest(pc, quest);
                }
                else
                {
                    FinishQuest(pc, quest);
                }
            }
            for (int i = 0; i < quest.Count.Length; i++)
            {
                quest.Count[i] = 0;
            }
            if (s.HoldItem != 0)
            {
                HoldItem(pc, s.HoldItem);
            }

            foreach (uint i in s.TakeItems[pc.Job].Keys)
            {
                ushort count = s.TakeItems[pc.Job][i];
                pc.Client().RemoveItem(i, count);
            }
            foreach (uint i in s.TakeItems[Job.None].Keys)
            {
                ushort count = s.TakeItems[Job.None][i];
                pc.Client().RemoveItem(i, count);
            }
            foreach (uint i in s.GiveItems[pc.Job].Keys)
            {
                ((ActorEventHandlers.PCEventHandler)pc.EventHandler).Client.AddItem(i, s.GiveItems[pc.Job][i]);
            }
            foreach (uint i in s.GiveItems[Job.None].Keys)
            {
                ((ActorEventHandlers.PCEventHandler)pc.EventHandler).Client.AddItem(i, s.GiveItems[Job.None][i]);
            }
            foreach (uint i in s.LearnSkills[pc.Job])
            {
                SkillManager.Instance.PlayerAddSkill(pc, i, true);
            }
            foreach (uint i in s.LearnSkills[Job.None])
            {
                ((ActorEventHandlers.PCEventHandler)pc.EventHandler).Client.SendSkillAdd(i);
                pc.Skills[i] = new Skill(SkillFactory.Instance[i]);
            }
            Map.Map map = MapManager.Instance.GetMap(pc.MapInstanceID);
            foreach (NPC.SpawnData i in s.Spawns)
            {
                Scripting.Utils.SpawnNPC(map, i.NpcID, i.AppearEffect, i.X, i.Y, i.Z, i.Dir, i.Motion);
            }
            {
                Dictionary<uint, int> rewardOptions = new Dictionary<uint, int>();

                if (s.RewardOptions[pc.Job].Count > 0)
                {
                    rewardOptions = s.RewardOptions[pc.Job];
                }
                else
                {
                    rewardOptions = s.RewardOptions[Job.None];
                }

                if (rewardOptions.Count > 0)
                {
                    if (selection < rewardOptions.Count)
                    {
                        KeyValuePair<uint, int> pair = rewardOptions.ToList()[selection];
                        pc.Client().AddItem(pair.Key, (ushort)pair.Value);
                    }
                }
            }
            if (s.Exp > 0)
            {
                ExperienceManager.Instance.ApplyExp(pc, s.Exp);
            }
            if (s.Gold > 0)
            {
                Interlocked.Add(ref pc.Gold, s.Gold);
                pc.Client().SendPlayerGold();
            }
            if (s.CutScene > 0)
            {
                pc.Client().SendQuestCutScene(s.CutScene);
            }
            if (s.TeleportMapID > 0)
            {
                map = MapManager.Instance.GetMap(s.TeleportMapID, pc.CharID, pc.PartyID);
                pc.MapChangeCutScene = s.TeleportCutscene;
                pc.MapChangeCutSceneU1 = s.TeleportU1;
                pc.MapChangeCutSceneU2 = s.TeleportU2;
                if (s.X != 0 && s.Y != 0 && s.Z != 0)
                {
                    pc.Client().Map.SendActorToMap(pc, map, s.X, s.Y, s.Z);
                }
                else
                {
                    pc.Client().Map.SendActorToMap(pc, map, pc.X, pc.Y, pc.Z);
                }
            }
            if (s.Finish)
            {
                if (detail.NextQuest[pc.Job] != 0)
                {
                    NextQuest(pc, (ushort)detail.NextQuest[pc.Job]);
                }
                else if (detail.NextQuest[Job.None] != 0)
                {
                    NextQuest(pc, (ushort)detail.NextQuest[Job.None]);
                }
            }
        }

        public new void Reload()
        {
            mapObjectMapping.Clear();
            npcMapping.Clear();
            base.Reload();
        }

        protected void UpdateQuest(ActorPC pc, Quest quest)
        {
            Network.Client.GameSession client = pc.Client();
            client?.SendQuestUpdate(quest);
        }

        protected void FinishQuest(ActorPC pc, Quest quest)
        {
            Network.Client.GameSession client = pc.Client();
            client?.FinishQuest(quest);
        }

        protected void NextQuest(ActorPC pc, ushort next)
        {
            Network.Client.GameSession client = pc.Client();
            client?.SendNextQuest(next);
        }

        protected void HoldItem(ActorPC pc, uint item)
        {
            Network.Client.GameSession client = pc.Client();
            client?.SendHoldItem(item);
        }

        protected void HoldItemCancel(ActorPC pc, uint item)
        {
            Network.Client.GameSession client = pc.Client();
            client?.SendHoldItemCancel(item);
        }
    }
}
