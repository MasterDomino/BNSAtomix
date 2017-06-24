﻿
using SmartEngine.Network;

namespace SagaBNS.Common.Packets.CharacterServer
{
    public class CM_CHAR_LIST_REQUEST : Packet<CharacterPacketOpcode>
    {
        public CM_CHAR_LIST_REQUEST()
        {
            ID = CharacterPacketOpcode.CM_CHAR_LIST_REQUEST;
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

        public uint AccountID
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
