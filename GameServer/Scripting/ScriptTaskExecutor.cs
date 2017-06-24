using SmartEngine.Network.Tasks;
using System.Collections.Concurrent;

namespace SagaBNS.GameServer.Scripting
{
    public class ScriptTaskExecutor : Task
    {
        #region Members

        private readonly NPCScriptHandler npc;
        private readonly ConcurrentQueue<NPCScriptTask> tasks;
        private NPCScriptTask current;

        private int cycle;

        #endregion

        #region Instantiation

        public ScriptTaskExecutor(NPCScriptHandler npc, ConcurrentQueue<NPCScriptTask> tasks) : base(0, 100, "ScriptTaskExecutor")
        {
            period = 100;
            this.tasks = tasks;
            this.npc = npc;
            tasks.TryDequeue(out current);
            current.EndCycles = cycle + current.Cycles;
            current.ScriptHandler = npc;
        }

        #endregion

        #region Methods

        public void Append(ConcurrentQueue<NPCScriptTask> tasks)
        {
            if (tasks != null)
            {
                while (tasks.TryDequeue(out NPCScriptTask task))
                {
                    this.tasks.Enqueue(task);
                }
            }
        }

        public override void CallBack()
        {
            if (current == null || tasks == null)
            {
                Deactivate();
            }

            current.DoUpdate();
            cycle++;
            if (current.EndCycles <= cycle)
            {
                if (tasks.TryDequeue(out current))
                {
                    current.EndCycles = cycle + current.Cycles;
                    current.ScriptHandler = npc;
                }
                else
                {
                    Deactivate();
                }
            }
        }

        public void Reset()
        {
            current = null;
            cycle = 0;
            if (tasks != null)
            {
                if (tasks.TryDequeue(out current))
                {
                    current.EndCycles = cycle + current.Cycles;
                    current.ScriptHandler = npc;
                }
                else
                {
                    Deactivate();
                }
            }
        }

        #endregion
    }
}