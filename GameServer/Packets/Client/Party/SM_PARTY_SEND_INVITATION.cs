using System.Text;

using SmartEngine.Network;
using SagaBNS.Common.Packets;

namespace SagaBNS.GameServer.Packets.Client
{
    public class SM_PARTY_SEND_INVITATION : Packet<GamePacketOpcode>
    {
        public SM_PARTY_SEND_INVITATION()
        {
            ID = GamePacketOpcode.SM_PARTY_SEND_INVITATION;
        }

        public ulong PartyID
        {
            set
            {
                PutULong(value, 2);
            }
        }

        public ulong ActorID
        {
            set
            {
                PutULong(value, 10);
            }
        }

        public string Name
        {
            set
            {
                PutUShort((ushort)(value.Length), 18);
                PutBytes(Encoding.Unicode.GetBytes(value));
                PutUInt(0);
            }
        }
    }
}
