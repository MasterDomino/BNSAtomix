using System.Collections.Generic;
using System.Linq;

using SagaBNS.Common.Actors;
using SmartEngine.Core.Math;
using SagaBNS.GameServer.Map;
using SmartEngine.Network.Map.PathFinding;

namespace SagaBNS.GameServer.NPC.AI.AICommands
{
    public class Chase : IAICommand
    {
        private readonly AI ai;
        private readonly ActorNPC self;
        private ActorExt target;
        private short x, y, z;
        private readonly Map.Map map;
        private List<PathNode> currentPath;
        private int curPathIdx;
        public Chase(AI ai,ActorNPC self, ActorExt target)
        {
            this.ai = ai;
            this.self = self;
            this.target = target;
            map = Map.MapManager.Instance.GetMap(self.MapInstanceID);
            ai.Period = (int)(1000 / ((float)self.Speed / 100));
            ai.Period = SmartEngine.Network.Global.Random.Next(ai.Period - ai.Period / 20, ai.Period + ai.Period / 20);
            if (self.BaseData.NoMove)
            {
                status = CommandStatus.Finished;
            }
        }

        #region IAICommand 成员
        private CommandStatus status;
        public CommandTypes Type
        {
            get { return CommandTypes.Chase; }
        }

        public CommandStatus Status
        {
            get { return status; }
        }

        public ActorExt Target
        {
            get
            {
                return target;
            }
            set
            {
                if (target != value)
                {
                    target = value;
                    if (value == null)
                    {
                        status = CommandStatus.Finished;
                    }
                    else
                    {
                        ReCalculatePath();
                    }
                }
            }
        }

        private void ReCalculatePath()
        {
            if (map == null || map.HeightMapBuilder == null || target ==null)
            {
                return;
            }

            map.HeightMapBuilder.NormalizeCoordinates(target.X, target.Y, target.Z, out int tmpX, out int tmpY, out int tmpZ);
            bool found = false;
            List<Vec3i> points = new List<Vec3i>();
            for (int i = tmpX - 1; i <= tmpX + 1 && !found; i++)
            {
                for (int j = tmpY - 1; j <= tmpY + 1 && ! found; j++)
                {
                    if (map.HeightMapBuilder.IsWalkable(i, j, tmpZ, i, j, tmpZ))
                    {
                        map.HeightMapBuilder.RealCoordinatesFromNormalized(i, j, tmpZ, out int tmpX2, out int tmpY2, out int tmpZ2);
                        points.Add(new Vec3i(tmpX2, tmpY2, tmpZ2));
                    }
                }
            }
            points = points.OrderBy((point) =>
            {
                return self.DistanceToPoint(point.X, point.Y, point.Z);
            }).ToList();
            if (points.Count > 0)
            {
                found = true;
                x = (short)points[0].X;
                y = (short)points[0].Y;
                z = (short)points[0].Z;
            }
            if (found)
            {
                currentPath = map.PathFinding.FindPath(self.X, self.Y, self.Z, x, y, z);
                x = (short)target.X;
                y = (short)target.Y;
                z = (short)target.Z;
            }
            else
            {
                currentPath = map.PathFinding.FindPath(self.X, self.Y, self.Z, target.X, target.Y, target.Z);
            }

            curPathIdx = 0;
        }

        public void Update()
        {
            if (currentPath == null)
            {
                ReCalculatePath();
            }
            if (self.DistanceToActor(target) > ai.GetCurrentCastRange(target) * 0.75f)
            {
                if (target.DistanceToPoint(x, y, z) > 100)
                {
                    ReCalculatePath();
                }
                if (currentPath.Count > curPathIdx)
                {
                    PathNode node = currentPath[curPathIdx++];
                    if (curPathIdx % 2 == 0)
                    {
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
                        arg.Speed = (ushort)(500 * ((float)500 / self.Speed));
                        arg.DashID = 0;
                        arg.DashUnknown = 0;
                        arg.BNSMoveType = MoveType.Run;
                        map.MoveActor(self, arg);
                    }
                }
                else
                {
                    if (curPathIdx % 2 == 1)
                    {
                        PathNode node = currentPath[curPathIdx - 1];
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
                        arg.Speed = self.Speed;
                        arg.DashID = 0;
                        arg.DashUnknown = 0;
                        arg.BNSMoveType = MoveType.Run;
                        map.MoveActor(self, arg);
                    }
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
