
using SmartEngine.Network;
using SagaBNS.Common.Packets;

namespace SagaBNS.LobbyServer.Packets.Client
{
    public class SM_REQUEST_LOGIN : Packet<LobbyPacketOpcode>
    {
        public SM_REQUEST_LOGIN()
        {
            ID = LobbyPacketOpcode.SM_REQUEST_LOGIN;
        }

        public ulong CharID
        {
            set
            {
                PutULong(value, 2);
            }
        }
    }
}
