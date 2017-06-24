﻿using System.Collections.Generic;

using SmartEngine.Network;

namespace SagaBNS.Common.Packets.CharacterServer
{
    public class CM_ITEM_DELETE : Packet<CharacterPacketOpcode>
    {
        public CM_ITEM_DELETE()
        {
            ID = CharacterPacketOpcode.CM_ITEM_DELETE;
        }

        public List<uint> ItemIDs
        {
            get
            {
                List<uint> list = new List<uint>();
                ushort count = GetUShort(2);
                for (int i = 0; i < count; i++)
                {
                    list.Add(GetUInt());
                }
                return list;
            }
            set
            {
                PutUShort((ushort)value.Count, 2);
                foreach (uint i in value)
                {
                    PutUInt(i);
                }
            }
        }
    }
}
