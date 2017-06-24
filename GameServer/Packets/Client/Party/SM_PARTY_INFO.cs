using System.Collections.Generic;
using System.Text;

using SmartEngine.Network;
using SagaBNS.Common.Actors;
using SagaBNS.Common.Packets;

namespace SagaBNS.GameServer.Packets.Client
{
    public class SM_PARTY_INFO : Packet<GamePacketOpcode>
    {
        public SM_PARTY_INFO()
        {
            ID = GamePacketOpcode.SM_PARTY_INFO;
        }

        public ulong PartyID
        {
            set
            {
                PutULong(value, 6);
            }
        }

        public List<ActorPC> Members
        {
            set
            {
                byte index = 0;
                PutShort((short)value.Count, 24);
                foreach (ActorPC i in value)
                {
                    PutByte(index++);
                    PutShort((short)i.Name.Length);
                    PutBytes(Encoding.Unicode.GetBytes(i.Name));
                    PutByte(i.Offline ? (byte)2 : (byte)1);
                }
                index = 0;
                PutShort((short)value.Count);
                foreach (ActorPC i in value)
                {
                    PutByte(index++);
                    PutULong(i.ActorID);
                    PutByte((byte)i.Race);
                    PutByte((byte)i.Job);
                    PutByte(2);
                    PutInt(i.HP);
                    PutShort(1);
                    PutShort(6);
                    PutByte(i.Level);
                    PutShort((short)i.X);
                    PutShort((short)i.Y);
                    PutShort((short)i.Z);
                    PutUInt(i.MapID);
                    PutInt(i.MaxHP);
                    PutShort(1);
                    PutShort(10);
                    PutInt(78);
                    PutInt(0);
                    PutByte(0);
                    PutByte(0);//channel
                }

                PutInt((int)(Length - 6), 2);
            }
        }
    }
}
