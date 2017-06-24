
using SmartEngine.Network;
using SagaBNS.Common.Packets;

namespace SagaBNS.GameServer.Packets.Client
{
    public class SM_MAP_PORTAL_ACTIVATE : Packet<GamePacketOpcode>
    {
        public SM_MAP_PORTAL_ACTIVATE()
        {
            ID = GamePacketOpcode.SM_MAP_PORTAL_ACTIVATE;

            PutInt(0x0201);
            PutShort(0);
            PutInt(0);
            PutInt(0);
            PutInt(0);
            PutInt(0);
            PutInt(0);
        }

        public byte U1
        {
            set
            {
                PutByte(value, 3);
            }
        }

        public byte U3
        {
            set
            {
                PutByte(value, 2);
            }
        }

        public int DisappearEffect
        {
            set
            {
                PutInt(value, 4);
            }
        }

        public int U2
        {
            set
            {
                PutInt(value, 20);
            }
        }

        public uint CutScene
        {
            set
            {
                if (value > 0)
                {
                    PutUInt(value, 12);
                }
            }
        }
    }
}
