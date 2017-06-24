using System.Collections.Generic;

using SagaBNS.Common.Actors;
using SagaBNS.GameServer.Map;
using SmartEngine.Network.Map.PathFinding;

namespace SagaBNS.GameServer.NPC.AI.AICommands
{
    public class Move : IAICommand
    {
        private readonly AI ai;
        private readonly ActorNPC self;
        private readonly short x, y, z;
        private readonly Map.Map map;
        private List<PathNode> currentPath;
        private int curPathIdx;
        public Move(AI ai, ActorNPC self, short x, short y, short z)
        {
            this.ai = ai;
            this.self = self;
            this.x = x;
            this.y = y;
            this.z = z;
            map = Map.MapManager.Instance.GetMap(self.MapInstanceID);
            ai.Period = (int)(2000 / ((float)self.Speed / 200));
            if (self.BaseData.NoMove)
            {
                status = CommandStatus.Finished;
            }
        }

        #region IAICommand 成员
        private CommandStatus status;
        public CommandTypes Type
        {
            get { return CommandTypes.Move; }
        }

        public CommandStatus Status
        {
            get { return status; }
        }

        public ActorExt Target
        {
            get { return null; }
            set
            {
                if (value != null)
                {
                    status = CommandStatus.Finished;
                }
            }
        }

        private void ReCalculatePath()
        {
            currentPath = map.PathFinding.FindPath(self.X, self.Y, self.Z, x, y, z);
            curPathIdx = 0;
        }

        public void Update()
        {
            if (currentPath == null)
            {
                ReCalculatePath();
            }
            if (self.DistanceToPoint(x, y, z) > 30)
            {
                if (currentPath.Count > curPathIdx)
                {
                    PathNode node = currentPath[curPathIdx++];
                    MoveArgument arg = new MoveArgument()
                    {
                        X = node.X,
                        Y = node.Y,
                        Z = map.HeightMapBuilder.GetZ((short)node.X, (short)node.Y, (short)node.Z)
                    };
                    if (arg.Z == 0)
                    {
                        status = CommandStatus.Finished;
                        return;
                    }
                    arg.Dir = self.DirectionFromTarget(node.X, node.Y);
                    arg.Speed = (ushort)((500 * ((float)500 / self.Speed)) * 2);
                    arg.DashID = 0;
                    arg.DashUnknown = 0;
                    arg.BNSMoveType = MoveType.Walk;
                    map.MoveActor(self, arg);
                }
                else
                {
                    status = CommandStatus.Finished;
                }
            }
            else
            {
                status = CommandStatus.Finished;
            }
        }

        #endregion
    }
}
