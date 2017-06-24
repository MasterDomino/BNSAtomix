
using SmartEngine.Network;
using SagaBNS.Common.Packets;
using SagaBNS.GameServer.Network.Client;

namespace SagaBNS.GameServer.Packets.Client
{
    public class CM_QUEST_UPDATE_REQUEST_MAPOBJECT : Packet<GamePacketOpcode>
    {
        public CM_QUEST_UPDATE_REQUEST_MAPOBJECT()
        {
            ID = GamePacketOpcode.CM_QUEST_UPDATE_REQUEST_MAPOBJECT;
        }

        public ushort Count
        {
            get
            {
                return GetUShort(2);
            }
        }

        public ushort QuestID
        {
            get
            {
                return GetUShort(4);
            }
        }

        public byte Step
        {
            get
            {
                return GetByte(6);
            }
        }

        public byte Unknown
        {
            get
            {
                return GetByte(7);
            }
        }

        public ulong NpcActorID
        {
            get
            {
                return GetULong(9);
            }
        }

        public short Unknown3
        {
            get
            {
                return GetShort(18);
            }
        }

        public override Packet<GamePacketOpcode> New()
        {
            return new CM_QUEST_UPDATE_REQUEST_MAPOBJECT();
        }

        public override void OnProcess(Session<GamePacketOpcode> client)
        {
            ((GameSession)client).OnQuestUpdateRequestMapObject(this);
        }
    }
}
