
using SmartEngine.Network;
using SagaBNS.Common.Packets;

namespace SagaBNS.GameServer.Packets.Client
{
    public class SM_MAP_CHANGE_MAP : Packet<GamePacketOpcode>
    {
        public SM_MAP_CHANGE_MAP()
        {
            ID = GamePacketOpcode.SM_MAP_CHANGE_MAP;

            PutShort(0, 6);
            PutShort(16);
            PutShort(0);
            PutInt(0);
            PutShort(17, 20);
            PutShort(16);
            PutUShort(2, 48);
            PutUShort(0);
        }

        public uint InstanceID
        {
            set
            {
                PutUInt(value, 2);
            }
        }

        public uint MapID
        {
            set
            {
                PutUInt(value, 12);
            }
        }

        public uint Time
        {
            set
            {
                PutUInt(value, 16);
            }
        }

        public byte[] MapServerAESKey
        {
            set
            {
                PutBytes(value, 24);
            }
        }

        public short X
        {
            set
            {
                PutShort(value, 40);
            }
        }

        public short Y
        {
            set
            {
                PutShort(value, 42);
            }
        }

        public short Z
        {
            set
            {
                PutShort(value, 44);
            }
        }

        public ushort Dir
        {
            set
            {
                PutUShort(value, 46);
            }
        }
    }
}
