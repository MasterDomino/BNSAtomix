
using SmartEngine.Network.Map;

using SagaBNS.GameServer.Network.Client;

namespace SagaBNS.GameServer.Map
{
    public class ChatArgument : MapEventArgs
    {
        public Actor Sender { get; set; }
        public string Recipient { get; set; }
        public ChatType Type { get; set; }
        public string Message { get; set; }
    }
}
