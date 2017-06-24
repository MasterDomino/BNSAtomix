
using SmartEngine.Network;
using SagaBNS.Common.Packets;

namespace SagaBNS.GameServer.Packets.Client
{
    public class SM_ACTOR_CORPSE_LOOT_RESULT : Packet<GamePacketOpcode>
    {
        public SM_ACTOR_CORPSE_LOOT_RESULT()
        {
            ID = GamePacketOpcode.SM_ACTOR_CORPSE_LOOT_RESULT;
        }

        public ulong ActorID
        {
            set
            {
                PutULong(value, 2);
            }
        }

        public byte[] Indices
        {
            set
            {
                PutShort((short)value.Length, 10);
                PutBytes(value);
                foreach (byte i in value)
                {
                    PutByte(i == 255 ? (byte)0 : (byte)1);
                }
            }
        }
    }
}
