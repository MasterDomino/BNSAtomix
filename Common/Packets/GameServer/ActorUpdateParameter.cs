
using SmartEngine.Network;

namespace SagaBNS.Common.Packets.GameServer
{
    public class ActorUpdateParameter
    {
        public PacketParameter Parameter { get; set; }

        public long Value
        {
            get
            {
                switch (length)
                {
                    case 1:
                        return byteVal;
                    case 2:
                        return shortVal;
                    case 4:
                        return intVal;
                    case 8:
                        return longVal;
                }
                return longVal;
            }
            set
            {
                switch (length)
                {
                    case 1:
                        byteVal = (byte)value;
                        break;
                    case 2:
                        shortVal = (short)value;
                        break;
                    case 4:
                        intVal = (int)value;
                        break;
                    case 8:
                        longVal = value;
                        break;
                }
            }
        }

        private readonly int length;
        private byte byteVal;
        private short shortVal;
        private int intVal;
        private long longVal;
        public ActorUpdateParameter(PacketParameter para)
        {
            Parameter = para;
            length = para.GetLength();
        }

        public void Read(Packet<GamePacketOpcode> p)
        {
            switch (length)
            {
                case 1:
                    byteVal = p.GetByte();
                    break;
                case 2:
                    shortVal = p.GetShort();
                    break;
                case 4:
                    intVal = p.GetInt();
                    break;
                case 8:
                    longVal = p.GetLong();
                    break;
            }
        }

        public void Write(Packet<GamePacketOpcode> p)
        {
            switch (length)
            {
                case 1:
                    p.PutByte(byteVal);
                    break;
                case 2:
                    p.PutShort(shortVal);
                    break;
                case 4:
                    p.PutInt(intVal);
                    break;
                case 8:
                    p.PutLong(longVal);
                    break;
            }
        }
    }
}
