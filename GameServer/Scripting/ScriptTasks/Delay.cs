namespace SagaBNS.GameServer.Scripting.ScriptTasks
{
    public class Delay : NPCScriptTask
    {
        #region Instantiation

        public Delay(int cycles)
        {
            Cycles = cycles;
        }

        #endregion

        #region Methods

        public override void DoUpdate()
        {
            // do nothing
        }

        #endregion
    }
}