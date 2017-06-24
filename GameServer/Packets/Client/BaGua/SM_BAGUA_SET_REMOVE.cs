
using SmartEngine.Network;
using SagaBNS.Common.Packets;

namespace SagaBNS.GameServer.Packets.Client
{
    public class SM_BAGUA_SET_REMOVE : Packet<GamePacketOpcode>
    {
        public SM_BAGUA_SET_REMOVE()
        {
            ID = GamePacketOpcode.SM_BAGUA_SET_REMOVE;
        }
    }
}
