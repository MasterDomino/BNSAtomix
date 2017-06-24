using System;
using System.Collections.Generic;
using System.Threading;

using SmartEngine.Core;
using SmartEngine.Network.Utils;

using SagaBNS.Common.Actors;

namespace SagaBNS.GameServer.Manager
{
    public class Level
    {
        public uint Lv;
        public uint EXP;
        public Dictionary<Job, JobStatus> JobStatus = new Dictionary<Job, JobStatus>();
        public Level()
        {
            foreach (Job i in Enum.GetValues(typeof(Job)))
            {
                JobStatus.Add(i, new JobStatus());
            }
        }
    }

    public class JobStatus
    {
        public int HP;
        public ushort CriticalBase, DodgeBase, ParryBase, AttackMin, AttackMax, Defense;
        public List<uint> Skills = new List<uint>();
    }

    public class ExperienceManager : Factory<ExperienceManager, Level>
    {
        private const uint MAX_LEVEL = 50;
        public ExperienceManager()
        {
            loadingTab = "Loading level database";
            loadedTab = " levels loaded.";
            databaseName = "Levels";
            FactoryType = FactoryType.XML;
        }

        protected override uint GetKey(Level item)
        {
            return item.Lv;
        }

        protected override void ParseCSV(Level item, string[] paras)
        {
            throw new NotImplementedException();
        }

        protected override void ParseXML(System.Xml.XmlElement root, System.Xml.XmlElement current, Level item)
        {
            switch (root.Name.ToLower())
            {
                case "level":
                    {
                        switch (current.Name.ToLower())
                        {
                            case "id":
                                item.Lv = uint.Parse(current.InnerText);
                                break;
                            case "exp":
                                item.EXP = uint.Parse(current.InnerText);
                                break;
                        }
                    }
                    break;
                case "jobstatus":
                    {
                        Job job = (Job)Enum.Parse(typeof(Job), root.Attributes["job"].Value);
                        switch (current.Name.ToLower())
                        {
                            case "hp":
                                item.JobStatus[job].HP = int.Parse(current.InnerText);
                                break;
                            case "criticalbase":
                                item.JobStatus[job].CriticalBase = ushort.Parse(current.InnerText);
                                break;
                            case "dodgebase":
                                item.JobStatus[job].DodgeBase = ushort.Parse(current.InnerText);
                                break;
                            case "parrybase":
                                item.JobStatus[job].ParryBase = ushort.Parse(current.InnerText);
                                break;
                            case "attackmin":
                                item.JobStatus[job].AttackMin = ushort.Parse(current.InnerText);
                                break;
                            case "attackmax":
                                item.JobStatus[job].AttackMax = ushort.Parse(current.InnerText);
                                break;
                            case "defense":
                                item.JobStatus[job].Defense = ushort.Parse(current.InnerText);
                                break;
                            case "skill":
                                item.JobStatus[job].Skills.Add(uint.Parse(current.InnerText));
                                break;
                        }
                    }
                    break;
            }
        }

        public void ApplyExp(ActorPC targetPC, uint exp)
        {
            float percentage = 1f;
            //percentage = percentage * Configuration.Instance.CalcQuestRateForPC(targetPC);

            targetPC.Exp += (uint)(exp * percentage);
            CheckExp(targetPC);
        }

        public void CheckExp(ActorPC pc)
        {
            uint lvlDelta = 0;
            lvlDelta = GetLevelDelta(pc.Level, pc.Exp, true);
            if (lvlDelta > 0)
            {
                SendLevelUp(pc, lvlDelta);
            }
            else
            {
                Network.Client.GameSession client = pc.Client();
                client?.SendPlayerEXP();
            }
        }

        public void SendMissingSkills(ActorPC pc)
        {
            for (int i = 1; i <= pc.Level; i++)
            {
                foreach (uint j in items[(uint)i].JobStatus[pc.Job].Skills)
                {
                    if (!pc.Skills.ContainsKey(j))
                    {
                        Skills.SkillManager.Instance.PlayerAddSkill(pc, j, true);
                    }
                }
            }
        }

        private void SendLevelUp(ActorPC pc, uint numLevels)
        {
            for (int i = pc.Level + 1; i <= pc.Level + numLevels; i++)
            {
                foreach (uint j in items[(uint)i].JobStatus[pc.Job].Skills)
                {
                    Skills.SkillManager.Instance.PlayerAddSkill(pc, j, true);
                }
            }
            pc.Level += (byte)numLevels;
            PC.Status.CalcStatus(pc);
            Interlocked.Exchange(ref pc.HP, pc.MaxHP);
            pc.Client().SendPlayerStats();
            pc.Client().SendPlayerLevelUp();
            Logger.Log.Info(pc.Name + " gained " + numLevels + "x levels");
        }

        private uint GetLevelDelta(uint level, uint exp, bool allowMultilevel)
        {
            uint currentMax;
            if (level <= MAX_LEVEL)
            {
                currentMax = MAX_LEVEL - level;    // Calculate maximum allowed levels to gain from current level
            }
            else
            {
                currentMax = 0;
            }

            uint delta;
            for (delta = 0;

                (allowMultilevel ? true : delta < 2) &&					// Multilevel constraint
                delta < currentMax &&								// Max level constraint
                exp >= GetExpForLevel(level + delta + 1);			// Walk the passed levels (note that GetExpForLevel() returns 0 if level is greater than max level, so it's vital that the max levels are synced with the exp chart)

                delta++)
            {
                ;                                               // Increase level delta
            }

            return delta;
        }

        public uint GetExpForLevel(uint level)
        {
            if (items.TryGetValue(level, out Level levelData))
            {
                return levelData.EXP;
            }
            else
            {
                return uint.MaxValue;
            }
        }
    }
}
