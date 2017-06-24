
using SmartEngine.Network;
using SagaBNS.Common.Packets;
using SagaBNS.GameServer.Network.Client;

namespace SagaBNS.GameServer.Packets.Client
{
    public class CM_ACTOR_MOVEMENT : Packet<GamePacketOpcode>
    {
        public CM_ACTOR_MOVEMENT()
        {
            ID = GamePacketOpcode.CM_ACTOR_MOVEMENT;
        }

        public short X
        {
            get
            {
                return GetShort(3);
            }
        }

        public short Y
        {
            get
            {
                return GetShort(5);
            }
        }

        public short Z
        {
            get
            {
                return GetShort(7);
            }
        }

        public short X2
        {
            get
            {
                return GetShort(9);
            }
        }

        public short Y2
        {
            get
            {
                return GetShort(11);
            }
        }

        public short Z2
        {
            get
            {
                return GetShort(13);
            }
        }

        public ushort Speed
        {
            get
            {
                return GetUShort(15);
            }
        }

        public ushort Dir
        {
            get
            {
                return GetUShort(17);
            }
        }

        public short Unknown
        {
            get
            {
                return GetShort(19);
            }
        }

        public byte Unknown2
        {
            get
            {
                return GetByte(21);
            }
        }

        public short Unknown3
        {
            get
            {
                return GetShort(22);
            }
        }

        public Map.MoveType MoveType
        {
            get
            {
                return (Map.MoveType)GetShort(24);
            }
        }

        public byte Unknown5
        {
            get
            {
                return GetByte(26);
            }
        }

        public sbyte XDiff
        {
            get
            {
                return (sbyte)GetByte(27);
            }
        }

        public sbyte YDiff
        {
            get
            {
                return (sbyte)GetByte(28);
            }
        }

        public sbyte ZDiff
        {
            get
            {
                return (sbyte)GetByte(29);
            }
        }

        public byte Unknown8
        {
            get
            {
                return GetByte(30);
            }
        }

        public byte Unknown82
        {
            get
            {
                return GetByte(31);
            }
        }

        public short Unknown9
        {
            get
            {
                return GetShort(31);
            }
        }

        public short Unknown10
        {
            get
            {
                return GetShort(33);
            }
        }

        public byte Unknown11
        {
            get
            {
                return GetByte(34);
            }
        }

        public short[] Offsets
        {
            get
            {
                short count = GetShort(30);
                short[] res = new short[count * 3];
                for (int i = 0; i < count * 3; i++)
                {
                    res[i] = GetShort();
                }

                return res;
            }
        }

        public short[] Offsets2
        {
            get
            {
                short count = GetShort(36);
                short[] res = new short[count * 3];
                for (int i = 0; i < count * 3; i++)
                {
                    res[i] = GetShort();
                }

                return res;
            }
        }

        public override Packet<GamePacketOpcode> New()
        {
            return new CM_ACTOR_MOVEMENT();
        }

        public override void OnProcess(Session<GamePacketOpcode> client)
        {
            ((GameSession)client).OnActorMovement(this);
        }

        public override string ToString()
        {
            if (true)
            {
                return string.Format("Pos:{0},{1},{2} Pos2:{3},{4},{5} Speed:{6} Dir:{7}\n          " +
                    "U1:{8} U2:{9} U3:{10} MoveType:{11} U5:{12} U6:{13},U62:{19} U7:{14}\n          " +
                    "U8:{15} U82:{20} U9:{16} U10:{17} U11:{18}",
                    X, Y, Z, X2, Y2, Z2, Speed, Dir, Unknown, Unknown2, Unknown3, MoveType, Unknown5, XDiff,
                    ZDiff, Unknown8, Unknown9, Unknown10, Unknown11, YDiff, Unknown82);
            }
            else
            {
                return string.Format("Pos:{0},{1},{2} Pos2:{3},{4},{5} Speed:{6} Dir:{7}\n          " +
                    "U1:{8} U2:{9} U3:{10} MoveType:{11} U5:{12} U6:{13},U62:{14} U7:{15}",
                    X, Y, Z, X2, Y2, Z2, Speed, Dir, Unknown, Unknown2, Unknown3, MoveType, Unknown5, XDiff, YDiff,
                    ZDiff);
            }
        }

        private string OffsetsToString(short[] pos)
        {
            string res="";
            for(int i=0;i<pos.Length /3;i++)
            {
                res +=string.Format("Pos{0}:{1},{2},{3} ",i,pos[i*3],pos[i*3+1],pos[i*3+2]);
            }
            return res;
        }
    }
}
