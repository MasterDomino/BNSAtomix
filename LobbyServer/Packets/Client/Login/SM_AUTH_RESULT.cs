
using SmartEngine.Network;
using SmartEngine.Network.Utils;
using SagaBNS.Common.Packets;

namespace SagaBNS.LobbyServer.Packets.Client
{
    public class SM_AUTH_RESULT : Packet<LobbyPacketOpcode>
    {
        public SM_AUTH_RESULT()
        {
            ID = LobbyPacketOpcode.SM_AUTH_RESULT;
            Unknown1 = 11606;
            Unknown2 = 0;
            AES_KEY = Conversions.HexStr2Bytes("00000000000000000000000000000000");
        }

        public int Unknown1
        {
            set
            {
                PutInt(value, 2);
            }
        }

        public int Unknown2
        {
            set
            {
                PutInt(value, 6);
            }
        }

        public byte[] AES_KEY
        {
            set
            {
                PutShort(16, 10);
                PutBytes(value);
            }
        }
    }
}
