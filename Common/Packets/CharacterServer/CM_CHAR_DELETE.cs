
using SmartEngine.Network;

namespace SagaBNS.Common.Packets.CharacterServer
{
    public class CM_CHAR_DELETE : Packet<CharacterPacketOpcode>
    {
        public CM_CHAR_DELETE()
        {
            ID = CharacterPacketOpcode.CM_CHAR_DELETE;
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
            get { return GetUInt(10); }
            set { PutUInt(value, 10); }
        }
    }
}
