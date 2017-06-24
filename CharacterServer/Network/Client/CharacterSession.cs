
using SmartEngine.Network;
using SagaBNS.Common.Packets;

namespace SagaBNS.CharacterServer.Network.Client
{
    public partial class CharacterSession : Session<CharacterPacketOpcode>
    {
        public override void OnConnect()
        {

        }

        public override void OnDisconnect()
        {

        }

        public override string ToString()
        {
            return "Session " + curSession.ToString();
        }
    }
}
