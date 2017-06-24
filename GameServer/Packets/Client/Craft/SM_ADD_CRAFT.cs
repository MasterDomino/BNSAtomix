
using SmartEngine.Network;
using SagaBNS.Common.Packets;

namespace SagaBNS.GameServer.Packets.Client
{
    public class SM_ADD_CRAFT : Packet<GamePacketOpcode>
    {
        public SM_ADD_CRAFT()
        {
            ID = GamePacketOpcode.SM_ADD_CRAFT;
        }
    }
}
