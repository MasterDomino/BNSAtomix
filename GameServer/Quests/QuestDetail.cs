using System;
using System.Collections.Generic;

using SagaBNS.Common.Actors;

namespace SagaBNS.GameServer.Quests
{
    public class QuestDetail
    {
        private readonly Dictionary<byte, QuestStep> steps = new Dictionary<byte, QuestStep>();
        private readonly Dictionary<Job, uint> nextquest = new Dictionary<Job, uint>();
        public uint QuestID { get; set; }
        public Dictionary<Job, uint> NextQuest { get { return nextquest; } }

        public Dictionary<byte, QuestStep> Steps { get { return steps; } }

        public QuestDetail()
        {
            foreach (Job i in Enum.GetValues(typeof(Job)))
            {
                nextquest.Add(i, 0);
            }
        }
    }
}
