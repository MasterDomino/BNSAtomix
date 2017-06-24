using System.Text;

using SmartEngine.Network;
using SagaBNS.Common.Packets;

namespace SagaBNS.GameServer.Packets.Client
{
    public class SM_PARTY_MEMBER_LEAVE : Packet<GamePacketOpcode>
    {
        public SM_PARTY_MEMBER_LEAVE()
        {
            ID = GamePacketOpcode.SM_PARTY_MEMBER_LEAVE;
        }

        public string MemberName
        {
            set
            {
                PutShort((short)value.Length, 2);
                PutBytes(Encoding.Unicode.GetBytes(value));
                PutByte(0);
            }
        }
    }
}
