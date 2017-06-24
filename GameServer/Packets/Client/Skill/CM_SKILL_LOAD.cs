
using SmartEngine.Network;
using SagaBNS.Common.Packets;
using SagaBNS.GameServer.Network.Client;

namespace SagaBNS.GameServer.Packets.Client
{
    public class CM_SKILL_LOAD : Packet<GamePacketOpcode>
    {
        public CM_SKILL_LOAD()
        {
            ID = GamePacketOpcode.CM_SKILL_LOAD;
        }

        public override Packet<GamePacketOpcode> New()
        {
            return new CM_SKILL_LOAD();
        }

        public override void OnProcess(Session<GamePacketOpcode> client)
        {
            ((GameSession)client).SendSkillLoad();
        }
    }
}
