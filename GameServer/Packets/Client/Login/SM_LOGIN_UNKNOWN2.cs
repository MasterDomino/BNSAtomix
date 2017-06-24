
using SmartEngine.Network;
using SagaBNS.Common.Packets;

namespace SagaBNS.GameServer.Packets.Client
{
    public class SM_LOGIN_UNKNOWN2 : Packet<GamePacketOpcode>
    {
        public SM_LOGIN_UNKNOWN2()
        {
            ID = GamePacketOpcode.SM_LOGIN_UNKNOWN2;
        }

        public short Unknown1
        {
            set
            {
                PutShort(value, 2);
            }
        }
    }
}
