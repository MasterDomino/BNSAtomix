
using SmartEngine.Network;
using SagaBNS.Common.Packets;

namespace SagaBNS.LobbyServer.Packets.Client
{
    public class SM_CHAR_DELETE : Packet<LobbyPacketOpcode>
    {
        public enum Reasons
        {
            Okay = 0,
        }

        public SM_CHAR_DELETE()
        {
            ID = LobbyPacketOpcode.SM_CHAR_DELETE;
        }

        public byte[] SlotGuid
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
                PutByte(0,18);
                PutInt((int)value);
            }
        }
    }
}
