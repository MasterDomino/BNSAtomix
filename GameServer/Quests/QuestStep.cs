using System;
using System.Collections.Generic;

using SagaBNS.Common.Actors;

namespace SagaBNS.GameServer.Quests
{
    public enum StepTargetType
    {
        None,
        NPC,
        Loot,
        MapObject,
        Map,
    }

    public class QuestTarget
    {
        private readonly List<uint> targetIDs = new List<uint>();

        public StepTargetType TargetType { get; set; }
        public int TargetFlagIndex { get; set; }
        public short TargetFlagIncrement { get; set; }
        public int TargetFinishFlagIndex { get; set; }
        public short TargetFinishFlagIncrement { get; set; }
        public int TargetCount { get; set; }
        public uint TargetMapID { get; set; }
        public int SpecifyIndex { get; set; }
        public List<uint> TargetIDs { get { return targetIDs; } }
        public bool HasCoordinate { get; set; }
        public short TargetX { get; set; }
        public short TargetY { get; set; }
        public short TargetZ { get; set; }
        public QuestTarget()
        {
            TargetCount = int.MaxValue;
            SpecifyIndex = -1;
        }
    }

    public class QuestStep
    {
        private readonly Dictionary<Job, Dictionary<uint, ushort>> giveItems = new Dictionary<Job, Dictionary<uint, ushort>>();
        private readonly Dictionary<Job, Dictionary<uint, ushort>> takeItems = new Dictionary<Job, Dictionary<uint, ushort>>();
        private readonly Dictionary<Job, List<uint>> learnSkills = new Dictionary<Job, List<uint>>();
        private readonly Dictionary<Job, Dictionary<uint, int>> rewardOptions = new Dictionary<Job, Dictionary<uint, int>>();
        private readonly List<QuestTarget> targets = new List<QuestTarget>();
        private readonly List<NPC.SpawnData> spawns = new List<NPC.SpawnData>();
        public byte StepID { get; set; }
        public List<QuestTarget> Targets { get { return targets; } }
        public bool Finish { get; set; }
        public uint TeleportMapID { get; set; }
        public uint TeleportCutscene { get; set; }
        public int TeleportU1 { get; set; }
        public int TeleportU2 { get; set; }
        public byte NextStep { get; set; }
        public byte StepStatus { get; set; }
        public uint Exp { get; set; }
        public int Gold { get; set; }
        public uint CutScene { get; set; }
        public short Flag1 { get; set; }
        public short Flag2 { get; set; }
        public short Flag3 { get; set; }
        public short X { get; set; }
        public short Y { get; set; }
        public short Z { get; set; }
        public Dictionary<Job, Dictionary<uint, ushort>> GiveItems { get { return giveItems; } }
        public Dictionary<Job, Dictionary<uint, ushort>> TakeItems { get { return takeItems; } }
        public Dictionary<Job, List<uint>> LearnSkills { get { return learnSkills; } }
        public Dictionary<Job, Dictionary<uint, int>> RewardOptions { get { return rewardOptions; } }
        public List<NPC.SpawnData> Spawns { get { return spawns; } }
        public uint HoldItem { get; set; }
        public uint DropItem { get; set; }

        public QuestStep()
        {
            foreach (Job i in Enum.GetValues(typeof(Job)))
            {
                giveItems.Add(i, new Dictionary<uint, ushort>());
                takeItems.Add(i, new Dictionary<uint, ushort>());
                rewardOptions.Add(i, new Dictionary<uint, int>());
                learnSkills.Add(i, new List<uint>());
            }
        }
    }
}
