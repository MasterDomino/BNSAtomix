using System.Collections.Concurrent;

using SmartEngine.Network.Map;

namespace SagaBNS.GameServer.Map
{
    public enum MoveType
    {
        Walk,
        Run,
        Jump = 3,
        Dash = 6,
        DashJump = 9,
        Falling = 14,
        Swimming = 15,
        Glide = 17,
        Dive = 26,
        WallRun = 29,
        StepForward,
        PushBack,
    }

    public class MoveArgument : MoveArg
    {
        public MoveArgument()
        {
            BNSMoveType = SagaBNS.GameServer.Map.MoveType.Run;
        }

        public MoveType BNSMoveType { get; set; }

        public int DashID { get; set; }
        public short DashUnknown { get; set; }
        public Actor PushBackSource { get; set; }
        public byte SkillSession { get; set; }
        public ConcurrentQueue<sbyte[]> PosDiffs = new ConcurrentQueue<sbyte[]>();
    }
}
