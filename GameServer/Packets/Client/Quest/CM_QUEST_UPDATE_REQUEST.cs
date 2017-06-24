
using SmartEngine.Network;
using SagaBNS.Common.Packets;
using SagaBNS.GameServer.Network.Client;

namespace SagaBNS.GameServer.Packets.Client
{
    public class CM_QUEST_UPDATE_REQUEST : Packet<GamePacketOpcode>
    {
        public CM_QUEST_UPDATE_REQUEST()
        {
            ID = GamePacketOpcode.CM_QUEST_UPDATE_REQUEST;
        }

        public ushort QuestID
        {
            get
            {
                return GetUShort(2);
            }
        }

        public byte Step
        {
            get
            {
                return GetByte(4);
            }
        }

        public byte Unknown
        {
            get
            {
                return GetByte(5);
            }
        }

        public byte RewardSelection
        {
            get
            {
                return GetByte(6);
            }
        }

        public ulong NpcActorID
        {
            get
            {
                return GetULong(7);
            }
        }

        public short Unknown3
        {
            get
            {
                return GetShort(15);
            }
        }

        public override Packet<GamePacketOpcode> New()
        {
            return new CM_QUEST_UPDATE_REQUEST();
        }

        public override void OnProcess(Session<GamePacketOpcode> client)
        {
            ((GameSession)client).OnQuestUpdateRequest(this);
        }
    }
}
