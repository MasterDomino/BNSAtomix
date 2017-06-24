using System.Collections.Generic;

using SmartEngine.Network;
using SagaBNS.Common.Packets;
using SagaBNS.Common.Skills;

namespace SagaBNS.GameServer.Packets.Client
{
    public class SM_SKILL_LOAD : Packet<GamePacketOpcode>
    {
        public SM_SKILL_LOAD()
        {
            ID = GamePacketOpcode.SM_SKILL_LOAD;
        }

        public List<Skill> Skills
        {
            set
            {
                PutUShort((ushort)value.Count);
                foreach (Skill i in value)
                {
                    PutUInt(i.ID);
                    PutByte(0);
                }
            }
        }
    }
}
