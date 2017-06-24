using SagaBNS.GameServer.Map;

namespace SagaBNS.GameServer.Scripting.ScriptTasks
{
    public class Chat : NPCScriptTask
    {
        #region Members

        private readonly int dialogID;
        private readonly ushort dir;
        private bool update;

        #endregion

        #region Instantiation

        public Chat(int dialogID, ushort dir)
        {
            Cycles = 2;
            this.dialogID = dialogID;
            this.dir = dir;
        }

        #endregion

        #region Methods

        public override void DoUpdate()
        {
            if (!update)
            {
                UpdateEvent evt = new UpdateEvent()
                {
                    Actor = ScriptHandler.NPC,
                    UpdateType = UpdateTypes.NPCTalk,
                    UserData = dialogID
                };
                NPCMap.SendEventToAllActorsWhoCanSeeActor(MapEvents.EVENT_BROADCAST, evt, ScriptHandler.NPC, false);
                update = true;
            }
        }

        #endregion
    }
}