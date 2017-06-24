
using SmartEngine.Network;
namespace PacketViewer
{
    public class Packet : Packet<int>
    {
        public string ServerIP { get; set; }
        public bool IsGameServer { get; set; }
        public bool IsLobbyGateway { get; set; }
    }
}
