using SagaBNS.GameServer.Map;

namespace SagaBNS.GameServer.Scripting.ScriptTasks
{
    public class Move : NPCScriptTask
    {
        #region Members

        private readonly int dashID;
        private readonly short dashUnknown;
        private readonly ushort dir, speed;
        private readonly MoveType moveType;
        private readonly short x, y, z;
        private bool updated;

        #endregion

        #region Instantiation

        public Move(short x, short y, short z, ushort dir, ushort speed, MoveType moveType)
        {
            Cycles = 5;
            this.x = x;
            this.y = y;
            this.z = z;
            this.dir = dir;
            this.speed = speed;
            this.moveType = moveType;
        }

        public Move(short x, short y, short z, ushort dir, int dashID, short dashUnknown)
        {
            Cycles = 20;
            this.x = x;
            this.y = y;
            this.z = z;
            this.dir = dir;
            moveType = MoveType.Dash;
            this.dashID = dashID;
            this.dashUnknown = dashUnknown;
        }

        #endregion

        #region Methods

        public override void DoUpdate()
        {
            if (!updated)
            {
                MoveArgument arg = new MoveArgument()
                {
                    X = x,
                    Y = y,
                    Z = z,
                    Dir = dir,
                    Speed = speed,
                    DashID = dashID,
                    DashUnknown = dashUnknown,
                    BNSMoveType = moveType
                };
                NPCMap.MoveActor(ScriptHandler.NPC, arg, false);
                updated = true;
            }
        }

        #endregion
    }
}