namespace SagaBNS.GameServer.Scripting.ScriptTasks
{
    public class SpawnNPC : NPCScriptTask
    {
        #region Members

        private readonly int appearEffect;
        private readonly ushort npcID, dir, motion;
        private readonly short x, y, z;
        private bool update;

        #endregion

        #region Instantiation

        public SpawnNPC(ushort npcID, int appearEffect, short x, short y, short z, ushort dir, ushort motion)
        {
            Cycles = 2;
            this.npcID = npcID;
            this.appearEffect = appearEffect;
            this.x = x;
            this.y = y;
            this.z = z;
            this.dir = dir;
            this.motion = motion;
        }

        #endregion

        #region Methods

        public override void DoUpdate()
        {
            if (!update)
            {
                Utils.SpawnNPC(NPCMap, npcID, appearEffect, x, y, z, dir, motion);
                update = true;
            }
        }

        #endregion
    }
}