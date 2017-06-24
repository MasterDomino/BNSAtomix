
using SmartEngine.Network;
using SagaBNS.Common.Packets;

namespace SagaBNS.LobbyServer.Packets.Client
{
    public class SM_CHAR_CREATE_FAILED : Packet<LobbyPacketOpcode>
    {
        public enum Reasons
        {
            Failed = 111,
            InvalidName = 113,
            NameAlreadyExists
        }

        public SM_CHAR_CREATE_FAILED()
        {
            ID = LobbyPacketOpcode.SM_CHAR_CREATE_FAILED;
        }

        public byte[] SlotID
        {
            set
            {
                PutBytes(value, 2);
            }
        }

        public Reasons Reason
        {
            set
            {
                PutShort((short)value, 18);
            }
        }
    }
}
