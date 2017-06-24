
using SmartEngine.Network;
using SagaBNS.Common.Packets;

namespace SagaBNS.GameServer.Packets.Client
{
    public class SM_BAGUA_SET_CHANGE : Packet<GamePacketOpcode>
    {
        public SM_BAGUA_SET_CHANGE()
        {
            ID = GamePacketOpcode.SM_BAGUA_SET_CHANGE;
        }
    }
}
