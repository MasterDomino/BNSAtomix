
using SmartEngine.Network;
using SagaBNS.Common.Packets;

namespace SagaBNS.GameServer.Packets.Client
{
    public class SM_LOGIN_AUTH_RESULT : Packet<GamePacketOpcode>
    {
        public SM_LOGIN_AUTH_RESULT()
        {
            ID = GamePacketOpcode.SM_LOGIN_AUTH_RESULT;
            PutByte(0,2);//0 regular account 1 trial
            PutInt(0x2C812D);
            PutByte(0);
            PutInt(0x1214);
            PutShort(0);
        }
    }
}
