namespace SmartEngine.Network.Map
{
    public enum MoveType
    {
        Start,
        End
    }

    public class MoveArg
    {
        public MoveType MoveType;
        public int X, Y, Z;
        public ushort Dir;
        public ushort Speed;
    }
}
