
using SmartEngine.Network;

namespace SagaBNS.Common.Packets.CharacterServer
{
    public class CM_ACTOR_INFO_REQUEST : Packet<CharacterPacketOpcode>
    {
        public CM_ACTOR_INFO_REQUEST()
        {
            ID = CharacterPacketOpcode.CM_ACTOR_INFO_REQUEST;
        }

        public long SessionID
        {
            get
            {
                return GetLong(2);
            }
            set
            {
                PutLong(value, 2);
            }
        }

        public uint CharID
        {
            get
            {
                return GetUInt(10);
            }
            set
            {
                PutUInt(value, 10);
            }
        }
    }
}
