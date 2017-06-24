
using SmartEngine.Network;
using SagaBNS.Common.Packets;

namespace SagaBNS.GameServer.Packets.Client
{
    public class SM_PARTY_INVITE_FAILED : Packet<GamePacketOpcode>
    {
        public enum Reasons
        {
            ALREADY_SENT,
        }

        public SM_PARTY_INVITE_FAILED()
        {
            ID = GamePacketOpcode.SM_PARTY_INVITE_FAILED;
        }

        public Reasons Reason
        {
            set
            {
                PutInt((int)value, 2);
            }
        }
    }
}
