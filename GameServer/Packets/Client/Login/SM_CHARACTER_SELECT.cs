
using SmartEngine.Network;
using SagaBNS.Common.Packets;

namespace SagaBNS.GameServer.Packets.Client
{
    public class SM_CHARACTER_SELECT : Packet<GamePacketOpcode>
    {
        public SM_CHARACTER_SELECT()
        {
            ID = GamePacketOpcode.SM_CHARACTER_SELECT;
        }

        public byte[] Token
        {
            set
            {
                PutBytes(value);
            }
        }
    }
}
