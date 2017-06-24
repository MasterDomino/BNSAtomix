
using SmartEngine.Network;
using SagaBNS.Common.Packets;

namespace SagaBNS.GameServer.Packets.Client
{
    public class SM_QUEST_CUTSCENE : Packet<GamePacketOpcode>
    {
        public SM_QUEST_CUTSCENE()
        {
            ID = GamePacketOpcode.SM_QUEST_CUTSCENE;
        }

        public uint CutsceneID
        {
            set
            {
                PutUInt(value, 2);
            }
        }
    }
}
