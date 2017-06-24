using System.Collections.Generic;
using System.Linq;
using System.Numerics;

using SmartEngine.Network;
using SagaBNS.Common.Packets;

namespace SagaBNS.GameServer.Packets.Client
{
    public class SM_LOCATION_LOAD_TELEPORT : Packet<GamePacketOpcode>
    {
        public SM_LOCATION_LOAD_TELEPORT()
        {
            ID = GamePacketOpcode.SM_LOCATION_LOAD_TELEPORT;
        }

        public List<ushort> Locations
        {
            set
            {
                BigInteger val = new BigInteger();
                foreach (ushort i in value)
                {
                    val |= (BigInteger.One << (i - 1));
                }
                byte[] buf = val.ToByteArray();
                if (value.Count() == 0)
                {
                    PutUShort(0);
                }
                else
                {
                    PutUShort((ushort)buf.Length, 2);
                    PutBytes(buf);
                }
            }
        }
    }
}
