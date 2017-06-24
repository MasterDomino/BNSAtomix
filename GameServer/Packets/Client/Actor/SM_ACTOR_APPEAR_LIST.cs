using System.Collections.Generic;
using System.Text;

using SmartEngine.Network;
using SmartEngine.Network.Map;
using SagaBNS.Common.Actors;
using SagaBNS.Common.Packets;

namespace SagaBNS.GameServer.Packets.Client
{
    public class SM_ACTOR_APPEAR_LIST : Packet<GamePacketOpcode>
    {
        private ushort offsetAppear;
        public SM_ACTOR_APPEAR_LIST()
        {
            ID = GamePacketOpcode.SM_ACTOR_APPEAR_LIST;
        }

        public uint MapInstanceID
        {
            set
            {
                PutUInt(value, 6);//instance id?
                PutShort(0x10, 12);//high 32bits?
            }
        }

        public List<Actor> DisappearActors
        {
            set
            {
                PutShort((short)value.Count, 14);
                foreach (Actor i in value)
                {
                    switch (i.ActorType)
                    {
                        case ActorType.NPC:
                            {
                                ActorNPC npc = (ActorNPC)i;
                                PutULong(npc.ActorID);
                                PutByte(3);
                                PutInt(npc.DisappearEffect);
                                PutInt(0);
                            }
                            break;
                        case ActorType.PC:
                            {
                                ActorPC pc = (ActorPC)i;
                                PutULong(pc.ActorID);
                                PutByte(0x42);
                                PutInt(pc.Status.DisappearEffect);
                                PutInt(0);
                            }
                            break;
                    }
                }
                offsetAppear = offset;
            }
        }

        public List<Actor> AppearActors
        {
            set
            {
                PutUShort((ushort)value.Count, offsetAppear);//Count
                foreach (Actor i in value)
                {
                    switch (i.ActorType)
                    {
                        case ActorType.NPC:
                            {
                                ActorNPC npc = (ActorNPC)i;
                                PutULong(npc.ActorID);
                                PutByte(3);
                                offset += 5;
                                PutUShort(Global.LittleToBigEndian(npc.NpcID));
                                offset += 93;
                            }
                            break;
                        case ActorType.PC:
                            {
                                ActorPC pc = (ActorPC)i;
                                PutULong(pc.ActorID);
                                PutByte(1);
                                PutByte((byte)pc.Race);
                                PutByte((byte)pc.Gender);
                                PutByte((byte)pc.Job);
                                PutBytes(pc.Appearence2);
                                PutUShort((ushort)pc.Name.Length);
                                PutBytes(Encoding.Unicode.GetBytes(pc.Name));
                                offset += 1;//Unknown
                                PutShort(16);//Unknown
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
