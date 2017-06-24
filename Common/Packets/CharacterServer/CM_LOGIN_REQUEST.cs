
using SmartEngine.Network;
namespace SagaBNS.Common.Packets.CharacterServer
{
    public class CM_LOGIN_REQUEST : Packet<CharacterPacketOpcode>
    {
        public CM_LOGIN_REQUEST()
        {
            ID = CharacterPacketOpcode.CM_LOGIN_REQUEST;
        }

        public string Password
        {
            get
            {
                return GetString(2);
            }
            set
            {
                PutString(value, 2);
            }
        }
    }
}
