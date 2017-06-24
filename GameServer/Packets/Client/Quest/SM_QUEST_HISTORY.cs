using System.Collections.Generic;
using System.Numerics;

using SmartEngine.Network;
using SagaBNS.Common.Packets;

namespace SagaBNS.GameServer.Packets.Client
{
    public class SM_QUEST_HISTORY : Packet<GamePacketOpcode>
    {
        public SM_QUEST_HISTORY()
        {
            ID = GamePacketOpcode.SM_QUEST_HISTORY;
        }

        public List<ushort> QuestsCompelted
        {
            set
            {
                BigInteger val = new BigInteger();
                foreach (ushort i in value)
                {
                    val |= (BigInteger.One << (i - 1));
                }
                byte[] buf = val.ToByteArray();
                PutUShort((ushort)buf.Length, 2);
                PutBytes(buf);
                PutUShort(0);
                PutUShort(0);
            }
        }
    }
}
