using SagaBNS.Common.Actors;
using SagaBNS.Common.Quests;
using SagaBNS.GameServer.ActorEventHandlers;
using SmartEngine.Core;
using SmartEngine.Network.Map;
using System.Collections.Concurrent;
using System.Linq;

namespace SagaBNS.GameServer.Scripting
{
    public abstract class NPCScriptHandler : NPCEventHandler
    {
        #region Members

        private ScriptTaskExecutor taskExe;

        private ConcurrentQueue<NPCScriptTask> tasks;

        #endregion

        //public ActorNPC NPC { get { return base.NPC; } set{base.NPC  }

        #region Properties

        public abstract ushort NpcID { get; }

        #endregion

        #region Methods

        public virtual void OnQuest(ActorPC pc, ushort questID, byte step, Quest quest)
        {
        }

        public virtual void OnReceiveNPCCommand(ActorNPC npc, string command)
        {
            // do nothing
        }

        protected void BeginTask()
        {
            if (tasks == null)
            {
                tasks = new ConcurrentQueue<NPCScriptTask>();
            }
        }

        protected Quest CreateNewQuest(ActorPC pc, ushort questID)
        {
            Quest q = new Quest()
            {
                QuestID = questID
            };
            if (pc.Quests.ContainsKey(questID))
            {
                return null;
            }
            else
            {
                pc.Quests[questID] = q;
                return q;
            }
        }

        protected void CutScene(ActorPC pc, uint cutscene)
        {
            ((PCEventHandler)pc.EventHandler).Client.SendQuestCutScene(cutscene);
        }

        protected void Dash(short x, short y, short z, ushort dir, int dashID, short dashUnknown)
        {
            tasks.Enqueue(new ScriptTasks.Move(x, y, z, dir, dashID, dashUnknown));
        }

        protected void Delay(int cycles)
        {
            tasks.Enqueue(new ScriptTasks.Delay(cycles));
        }

        protected void Disappear()
        {
            Disappear(0);
        }

        protected void Disappear(int disappearEffect)
        {
            NPC.DisappearEffect = disappearEffect;
            Map.Map map = Map.MapManager.Instance.GetMap(NPC.MapInstanceID);
            map.DeleteActor(NPC);
        }

        protected void Move(short x, short y, short z, ushort dir, ushort speed)
        {
            Move(x, y, z, dir, speed, true);
        }

        protected void Move(short x, short y, short z, ushort dir, ushort speed, bool run)
        {
            tasks.Enqueue(new ScriptTasks.Move(x, y, z, dir, speed, run ? Map.MoveType.Run : Map.MoveType.Walk));
        }

        protected void NextStep(Quest q, byte status, short flag1, short flag2, short flag3)
        {
            q.Step++;
            q.NextStep = (byte)(q.Step + 1);
            q.StepStatus = status;
            q.Flag1 = flag1;
            q.Flag2 = flag2;
            q.Flag3 = flag3;
        }

        protected void NPCChat(int dialogID, ushort dir)
        {
            tasks.Enqueue(new ScriptTasks.Chat(dialogID, dir));
        }

        protected void NPCChat(int dialogID, Actor faceTo)
        {
            tasks.Enqueue(new ScriptTasks.Chat(dialogID, NPC.DirectionFromTarget(faceTo)));
        }

        protected void ProcessQuest(ActorPC pc, byte step, Quest q)
        {
            Quests.QuestManager.Instance.ProcessQuest(pc, q.QuestID, step, q, NPC);
        }

        protected void SendNPCCommand(string command)
        {
            Map.Map map = Map.MapManager.Instance.GetMap(NPC.MapInstanceID);
            foreach (Actor i in map.Actors.Values.ToList())
            {
                if (i == NPC)
                {
                    continue;
                }

                if (i.EventHandler.GetType().IsSubclassOf(typeof(NPCScriptHandler)))
                {
                    ((NPCScriptHandler)i.EventHandler).OnReceiveNPCCommand(NPC, command);
                }
            }
        }

        protected void SpawnNPC(ushort npcID, short x, short y, short z, ushort dir, ushort motion)
        {
            Map.Map map = Map.MapManager.Instance.GetMap(NPC.MapInstanceID);
            Utils.SpawnNPC(map, npcID, x, y, z, dir, motion);
        }

        protected void SpawnNPC(ushort npcID, int appearEffect, short x, short y, short z, ushort dir, ushort motion)
        {
            Map.Map map = Map.MapManager.Instance.GetMap(NPC.MapInstanceID);
            Utils.SpawnNPC(map, npcID, appearEffect, x, y, z, dir, motion);
        }

        protected void SpawnNPCTask(ushort npcID, short x, short y, short z, ushort dir, ushort motion)
        {
            tasks.Enqueue(new ScriptTasks.SpawnNPC(npcID, 0, x, y, z, dir, motion));
        }

        protected void SpawnNPCTask(ushort npcID, int appearEffect, short x, short y, short z, ushort dir, ushort motion)
        {
            tasks.Enqueue(new ScriptTasks.SpawnNPC(npcID, appearEffect, x, y, z, dir, motion));
        }

        protected ScriptTaskExecutor StartTask()
        {
            return StartTask(false);
        }

        protected ScriptTaskExecutor StartTask(bool overwrite)
        {
            if (taskExe != null)
            {
                if (overwrite && taskExe.Activated)
                {
                    Logger.Log.Warn(string.Format("NPC:{0} already has task started", NPC.NpcID));
                    taskExe = null;
                }
                else
                {
                    taskExe.Append(tasks);
                }
            }
            if (taskExe == null)
            {
                taskExe = new ScriptTaskExecutor(this, tasks);
                taskExe.Activate();
            }
            else
            {
                if (!taskExe.Activated)
                {
                    taskExe.Reset();
                    taskExe.Activate();
                }
            }
            tasks = null;
            return taskExe;
        }

        protected void UpdateQuest(ActorPC pc, Quest quest)
        {
            ((PCEventHandler)pc.EventHandler).Client.SendQuestUpdate(quest);
        }

        #endregion
    }
}