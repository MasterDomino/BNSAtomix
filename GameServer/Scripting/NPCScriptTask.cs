using SagaBNS.GameServer.Map;

namespace SagaBNS.GameServer.Scripting
{
    public abstract class NPCScriptTask
    {
        #region Properties

        public int Cycles { get; set; }

        public int EndCycles { get; set; }

        public NPCScriptHandler ScriptHandler { get; set; }

        protected Map.Map NPCMap
        {
            get
            {
                return MapManager.Instance.GetMap(ScriptHandler.NPC.MapInstanceID);
            }
        }

        #endregion

        #region Methods

        public abstract void DoUpdate();

        #endregion
    }
}