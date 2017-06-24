
using SmartEngine.Network;
using SagaBNS.Common.Packets;
using SagaBNS.GameServer.Network.Client;

namespace SagaBNS.GameServer.Packets.Client
{
    public class CM_QUEST_ACCEPT : Packet<GamePacketOpcode>
    {
        public CM_QUEST_ACCEPT()
        {
            ID = GamePacketOpcode.CM_QUEST_ACCEPT;
        }

        public uint QuestID
        {
            get
            {
                return GetUInt(2);
            }
        }

        public ulong ActorID
        {
            get
            {
                return GetULong(6);
            }
        }

        public override Packet<GamePacketOpcode> New()
        {
            return new CM_QUEST_ACCEPT();
        }

        public override void OnProcess(Session<GamePacketOpcode> client)
        {
            ((GameSession)client).OnQuestAccept(this);
        }
    }
}
