
using SmartEngine.Network;
using SagaBNS.Common.Packets;

namespace SagaBNS.GameServer.Packets.Client
{
    public class SM_MAP_PORTAL_SOME_REPLY : Packet<GamePacketOpcode>
    {
        public SM_MAP_PORTAL_SOME_REPLY()
        {
            ID = GamePacketOpcode.SM_MAP_PORTAL_SOME_REPLY;

            PutUShort(0x202);
        }
    }
}
