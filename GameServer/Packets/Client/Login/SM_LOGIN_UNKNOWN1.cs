
using SmartEngine.Network;
using SagaBNS.Common.Packets;

namespace SagaBNS.GameServer.Packets.Client
{
    public class SM_LOGIN_UNKNOWN1 : Packet<GamePacketOpcode>
    {
        public SM_LOGIN_UNKNOWN1()
        {
            ID = GamePacketOpcode.SM_LOGIN_UNKNOWN1;
        }

        public long Unknown1
        {
            set
            {
                PutLong(value, 2);
            }
        }

        public long Unknown2
        {
            set
            {
                PutLong(value, 10);
            }
        }

        public long Unknown3
        {
            set
            {
                PutLong(value, 18);
            }
        }
    }
}
