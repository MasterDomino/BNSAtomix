
using SmartEngine.Network;
using SagaBNS.Common.Packets;
using SagaBNS.LobbyServer.Network.Client;

namespace SagaBNS.LobbyServer.Packets.Client
{
    public class CM_CHARACTER_LIST : Packet<LobbyPacketOpcode>
    {
        public CM_CHARACTER_LIST()
        {
            ID = LobbyPacketOpcode.CM_CHARACTER_LIST;
        }

        public override Packet<LobbyPacketOpcode> New()
        {
            return new CM_CHARACTER_LIST();
        }

        public override void OnProcess(Session<LobbyPacketOpcode> client)
        {
            ((LobbySession)client).OnCharacterList(this);
        }
    }
}
