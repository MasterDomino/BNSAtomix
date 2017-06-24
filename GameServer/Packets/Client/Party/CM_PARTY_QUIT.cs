
using SmartEngine.Network;
using SagaBNS.Common.Packets;
using SagaBNS.GameServer.Network.Client;

namespace SagaBNS.GameServer.Packets.Client
{
    public class CM_PARTY_QUIT : Packet<GamePacketOpcode>
    {
        public CM_PARTY_QUIT()
        {
            ID = GamePacketOpcode.CM_PARTY_QUIT;
        }

        public override Packet<GamePacketOpcode> New()
        {
            return new CM_PARTY_QUIT();
        }

        public override void OnProcess(Session<GamePacketOpcode> client)
        {
            ((GameSession)client).OnPartyQuit(this);
        }
    }
}
