
using SmartEngine.Network;
using SagaBNS.Common.Packets;

namespace SagaBNS.GameServer.Packets.Client
{
    public class SM_PARTY_MEMBER_ONLINE_STATUS : Packet<GamePacketOpcode>
    {
        public enum Statuses
        {
            Online = 1,
            Offline,
        }

        public SM_PARTY_MEMBER_ONLINE_STATUS()
        {
            ID = GamePacketOpcode.SM_PARTY_MEMBER_ONLINE_STATUS;
        }

        public ulong ActorID
        {
            set
            {
                PutULong(value, 2);
            }
        }

        public Statuses Status
        {
            set
            {
                PutByte((byte)value, 10);
            }
        }
    }
}
