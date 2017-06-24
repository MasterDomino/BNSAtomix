using System.Collections.Generic;
using System.Text;

using SmartEngine.Network;
using SmartEngine.Network.Map;
using SagaBNS.Common.Actors;
using SagaBNS.Common.Packets;

namespace SagaBNS.GameServer.Packets.Client
{
    public class SM_ACTOR_LIST : Packet<GamePacketOpcode>
    {
        public SM_ACTOR_LIST()
        {
            ID = GamePacketOpcode.SM_ACTOR_LIST;
        }

        public uint MapInstanceID
        {
            set
            {
                PutUInt(value, 6);//instance id?
                PutInt(0x100000);//high 32bits?
            }
        }

        public List<Actor> Actors
        {
            set
            {
                PutUShort((ushort)value.Count,15);//Count
                foreach (Actor i in value)
                {
                    switch (i.ActorType)
                    {
                        case ActorType.NPC:
                            {
                                ActorNPC npc = (ActorNPC)i;
                                PutULong(npc.ActorID);
                                offset += 5;
                                PutUShort(Global.LittleToBigEndian(npc.NpcID));
                                offset += 93;
                            }
                            break;
                        case ActorType.PC:
                            {
                                ActorPC pc = (ActorPC)i;
                                PutULong(pc.ActorID);
                                PutByte((byte)pc.Race);
                                PutByte((byte)pc.Gender);
                                PutByte((byte)pc.Job);
                                PutBytes(pc.Appearence2);
                                PutUShort((ushort)pc.Name.Length);
                                PutBytes(Encoding.Unicode.GetBytes(pc.Name));
                                PutByte(0);
                                PutShort(pc.WorldID);
                            }
                            break;
                    }
                }
                PutShort(0);//Summon Count
                EnsureLength(offset);
                PutInt((int)Length - 6, 2);
            }
        }
    }
}
