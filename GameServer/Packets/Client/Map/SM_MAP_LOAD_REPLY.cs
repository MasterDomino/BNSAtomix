
using SmartEngine.Network;
using SagaBNS.Common.Packets;

namespace SagaBNS.GameServer.Packets.Client
{
    public class SM_MAP_LOAD_REPLY : Packet<GamePacketOpcode>
    {
        public SM_MAP_LOAD_REPLY()
        {
            ID = GamePacketOpcode.SM_MAP_LOAD_REPLY;

            PutInt(0);
            PutInt(0);
        }
    }
}
