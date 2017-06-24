
using SmartEngine.Network;
using SagaBNS.Common.Packets;
using SagaBNS.GameServer.Network.Client;

namespace SagaBNS.GameServer.Packets.Client
{
    public class CM_QUEST_DROP : Packet<GamePacketOpcode>
    {
        public CM_QUEST_DROP()
        {
            ID = GamePacketOpcode.CM_QUEST_DROP;
        }

        public ushort Quest
        {
            get
            {
                return GetUShort(2);
            }
        }

        public override Packet<GamePacketOpcode> New()
        {
            return new CM_QUEST_DROP();
        }

        public override void OnProcess(Session<GamePacketOpcode> client)
        {
            ((GameSession)client).DropQuest(this);
        }
    }
}
