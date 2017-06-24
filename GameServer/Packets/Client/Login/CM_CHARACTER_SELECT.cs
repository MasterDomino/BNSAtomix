
using SmartEngine.Network;
using SagaBNS.Common.Packets;
using SagaBNS.GameServer.Network.Client;

namespace SagaBNS.GameServer.Packets.Client
{
    public class CM_CHARACTER_SELECT : Packet<GamePacketOpcode>
    {
        public CM_CHARACTER_SELECT()
        {
            ID = GamePacketOpcode.CM_CHARACTER_SELECT;
        }

        public override Packet<GamePacketOpcode> New()
        {
            return new CM_CHARACTER_SELECT();
        }

        public override void OnProcess(Session<GamePacketOpcode> client)
        {
            ((GameSession)client).CharacterSelect();
        }
    }
}
