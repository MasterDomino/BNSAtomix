﻿
using SmartEngine.Network;

namespace SagaBNS.Common.Packets.CharacterServer
{
    public class CM_SKILL_GET : Packet<CharacterPacketOpcode>
    {
        public CM_SKILL_GET()
        {
            ID = CharacterPacketOpcode.CM_SKILL_GET;
        }

        public long SessionID
        {
            get
            {
                return GetLong(2);
            }
            set
            {
                PutLong(value, 2);
            }
        }

        public uint CharID
        {
            get
            {
                return GetUInt(10);
            }
            set
            {
                PutUInt(value, 10);
            }
        }
    }
}
