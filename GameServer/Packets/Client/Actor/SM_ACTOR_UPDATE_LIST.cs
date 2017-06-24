using System;
using System.Collections.Generic;

using SmartEngine.Network;
using SagaBNS.Common.Actors;
using SagaBNS.Common.Skills;
using SagaBNS.Common.Packets;
using SagaBNS.Common.Packets.GameServer;
using SagaBNS.GameServer.Map;

namespace SagaBNS.GameServer.Packets.Client
{
    public class SM_ACTOR_UPDATE_LIST : Packet<GamePacketOpcode>
    {
        public SM_ACTOR_UPDATE_LIST()
        {
            ID = GamePacketOpcode.SM_ACTOR_UPDATE_LIST;
        }

        public List<UpdateEvent> Events
        {
            set
            {
                PutUShort((ushort)value.Count, 6);//Count
                foreach (UpdateEvent i in value)
                {
                    ushort lengthOffset = offset;
                    ushort endOffset = 0;
                    try
                    {
                        if (i.UpdateType != UpdateTypes.Debug)
                        {
                            PutByte(0);
                            PutByte((byte)i.UpdateType);
                        }
                        switch (i.UpdateType)
                        {
                            case UpdateTypes.Movement:
                                PutULong(i.Actor.ActorID);
                                PutShort((short)i.MoveArgument.X);
                                PutShort((short)i.MoveArgument.Y);
                                PutShort((short)i.MoveArgument.Z);
                                PutUShort(i.MoveArgument.Dir);
                                PutUShort(i.MoveArgument.Speed);
                                PutByte(0);
                                ushort movOffset = offset;
                                short counter = 0;
                                PutShort(1);
                                PutShort((short)i.MoveArgument.BNSMoveType);//run
                                sbyte[] diffs;
                                while (i.MoveArgument.PosDiffs.TryDequeue(out diffs))
                                {
                                    counter++;
                                    PutByte((byte)diffs[0]);
                                    PutByte((byte)diffs[1]);
                                    PutByte((byte)diffs[2]);
                                }
                                ushort endOff = offset;
                                offset = movOffset;
                                if (counter == 0)
                                {
                                    PutShort(1);
                                    PutShort((short)i.MoveArgument.BNSMoveType);
                                    PutByte(0);
                                    PutByte(0);
                                    PutByte(0);
                                }
                                else
                                {
                                    PutShort(counter);
                                    offset = endOff;
                                }
                                break;
                            case UpdateTypes.Skill:
                                {
                                    PutULong(i.Actor.ActorID);
                                    PutUInt(i.Skill.ID);
                                    PutByte(0);
                                    PutByte(i.SkillSession);
                                    PutUShort(i.Actor.Dir);
                                    PutByte((byte)i.SkillMode);
                                    switch (i.SkillMode)
                                    {
                                        case SkillMode.Cast:
                                            PutUShort((ushort)(i.Skill.BaseData.CastTime / 100));
                                            break;
                                        case SkillMode.CastActionDelay:
                                            PutUShort((ushort)((int)i.UserData / 100));
                                            break;
                                        case SkillMode.Activate:
                                            if (i.Skill.BaseData.Duration > 0)
                                            {
                                                PutUShort((ushort)(i.Skill.BaseData.Duration / 100));
                                            }
                                            else
                                            {
                                                int count = i.Skill.BaseData.ActivationTimes.Count;
                                                PutUShort((ushort)((i.UserData != null && count > 0 && count > ((int)i.UserData)) ? i.Skill.BaseData.ActivationTimes[((int)i.UserData)] : 4));
                                            }
                                            break;
                                        default:
                                            PutUShort(4);
                                            break;
                                    }
                                    switch (i.SkillCastMode)
                                    {
                                        case SkillCastMode.Single:
                                            {
                                                PutByte(10);
                                                PutByte(1);
                                                if (i.Target == null)
                                                {
                                                    PutULong(0);
                                                }
                                                else
                                                {
                                                    PutULong(i.Target.ActorID);
                                                }
                                            }
                                            break;
                                        case SkillCastMode.Coordinate:
                                            {
                                                PutByte(8);
                                                PutByte(2);
                                                PutShort(i.X);
                                                PutShort(i.Y);
                                                PutShort(i.Z);
                                            }
                                            break;
                                    }
                                }
                                break;
                            case UpdateTypes.Effect:
                                {
                                    PutULong(i.Actor.ActorID);
                                    if (i.Target == null)
                                    {
                                        PutULong(0);
                                    }
                                    else
                                    {
                                        PutULong(i.Target.ActorID);
                                    }

                                    PutUInt(i.Skill.ID);
                                    PutByte(0);
                                    PutByte(i.SkillSession);
                                    PutByte(4);
                                    PutByte((byte)i.SkillAttackResult);//1 miss 2 avoid 3 parry 4 totalParry 5 totalParry2 7 Critical
                                }
                                break;
                            case UpdateTypes.Actor:
                                if (i.AdditionID > 0)
                                {
                                    PutUInt(i.AdditionID);
                                }
                                else
                                {
                                    PutUInt(i.Skill == null ? 65004 : (i.Skill.ID < 1000000 ? (i.Skill.ID * 1000 + 11) : (i.Skill.ID * 10 + 1)));
                                }

                                PutUShort(i.AdditionSession);
                                PutULong(i.Actor.ActorID);
                                PutULong(i.Target.ActorID);
                                PutByte((byte)i.ExtraActivateMode);
                                PutByte(i.SkillSession);
                                PutShort((short)i.ActorUpdateParameters.Count);
                                ushort lengthOffset2 = offset;
                                PutShort(4);
                                foreach (ActorUpdateParameter j in i.ActorUpdateParameters)
                                {
                                    PutShort((short)j.Parameter);
                                    j.Write(this);
                                }
                                endOffset = offset;
                                offset = lengthOffset2;
                                PutUShort((ushort)(endOffset - lengthOffset2 - 2));
                                offset = endOffset;
                                if (i.UserData == null)
                                {
                                    PutByte(2);
                                    PutByte(1);
                                }
                                else
                                {
                                    byte[] data = (byte[])i.UserData;
                                    PutByte((byte)(data.Length + 1));
                                    PutBytes(data);
                                }
                                break;
                            case UpdateTypes.ActorExtension:
                                PutULong(i.Actor.ActorID);
                                if (i.ExtraActivateMode != UpdateEvent.ExtraUpdateModes.Cancel)
                                {
                                    PutByte(14);
                                    PutByte(1);
                                    PutUShort(i.AdditionSession);
                                    PutUInt(i.AdditionID);
                                    PutInt(i.RestTime);
                                    PutShort(i.AdditionCount);
                                }
                                else
                                {
                                    PutByte(4);
                                    PutByte(2);
                                    PutUShort(i.AdditionSession);
                                }
                                break;
                            case UpdateTypes.MapObjectOperate:
                                PutULong(i.Actor.ActorID);
                                PutULong(i.Target != null ? i.Target.ActorID : 0);
                                PutByte((byte)i.UserData);
                                break;
                            case UpdateTypes.MapObjectVisibilityChange:
                                {
                                    ActorMapObj obj = (ActorMapObj)i.Actor;
                                    PutULong(i.Actor.ActorID);
                                    if (obj.Special)
                                    {
                                        PutInt(obj.Available ? 4 : 3);//2 invisible 3 visible
                                    }
                                    else
                                    {
                                        PutInt(obj.Available ? 2 : 1);//4 invisible 5 visible
                                    }

                                    PutByte(0);
                                    PutByte(1);
                                    PutShort((obj.AvailableItems.Count > 0 || obj.Gold > 0) ? (short)0 : (short)1);
                                    PutByte(0);
                                }
                                break;
                            case UpdateTypes.NPCTalk:
                                PutULong(i.Actor.ActorID);
                                PutInt((int)i.UserData);
                                PutShort(0xC8);
                                break;
                            case UpdateTypes.ItemAppear:
                                PutULong(i.Actor.ActorID);
                                PutULong(i.Target.ActorID);
                                PutULong(((ActorItem)i.Target).CorpseID);
                                PutByte(1);
                                PutUInt(((ActorItem)i.Target).ObjectID);
                                PutByte(0);
                                PutShort(0);
                                break;
                            case UpdateTypes.ItemShow:
                                PutULong(i.Actor.ActorID);
                                PutULong(i.Target.ActorID);
                                PutByte(1);
                                PutUInt(((ActorItem)i.Target).ObjectID);
                                PutShort(i.X);
                                PutShort(i.Y);
                                PutShort(i.Z);
                                PutByte(0);
                                PutInt((int)i.UserData);
                                PutByte(1);
                                break;
                            case UpdateTypes.ItemHide:
                                PutULong(i.Actor.ActorID);
                                PutULong(i.Target.ActorID);
                                break;
                            case UpdateTypes.ItemPick:
                                PutULong(i.Actor.ActorID);
                                PutULong(i.Target.ActorID);
                                PutByte((byte)i.UserData);
                                break;
                            case UpdateTypes.ItemPickCorpse:
                                PutULong(i.Actor.ActorID);
                                PutULong(i.Target.ActorID);
                                PutByte((byte)i.UserData);
                                break;
                            case UpdateTypes.ItemDrop:
                                PutULong(i.Actor.ActorID);
                                PutULong(i.Target.ActorID);
                                PutShort(i.X);
                                PutShort(i.Y);
                                PutShort(i.Z);
                                PutByte((byte)i.UserData);
                                break;
                            case UpdateTypes.ItemDisappear:
                                PutULong(i.Target.ActorID);
                                PutULong(i.Actor.ActorID);
                                PutByte(1);
                                PutUInt(((ActorItem)i.Target).ObjectID);
                                PutByte(2);
                                break;
                            case UpdateTypes.ShowCorpse:
                                {
                                    ActorCorpse corpse = (ActorCorpse)i.Actor;
                                    PutULong(corpse.ActorID);
                                    PutULong(corpse.NPC.ActorID);
                                    PutUInt(corpse.NPC.NpcID);
                                    PutULong(corpse.Owner != null ? corpse.Owner.ActorID : 0);
                                    PutULong(0);
                                    PutShort((short)corpse.X);
                                    PutShort((short)corpse.Y);
                                    PutShort((short)corpse.Z);
                                    if (corpse.Gold > 0 || (corpse.AvailableItems.Count > 0))
                                    {
                                        PutUInt(corpse.TreasureType);//0x1100
                                    }
                                    else
                                    {
                                        PutUInt(0);
                                    }
                                }
                                break;
                            case UpdateTypes.DeleteCorpse:
                                {
                                    ActorCorpse corpse = (ActorCorpse)i.Actor;
                                    PutULong(corpse.ActorID);
                                    if (corpse.Gold > 0 || (corpse.AvailableItems.Count > 0))
                                    {
                                        if (corpse.QuestID > 0 || (!corpse.ShouldDisappear && corpse.NPC.BaseData.CorpseItemID > 0))
                                        {
                                            PutUShort(0x100);
                                        }
                                        else if (corpse.PickUp)
                                        {
                                            PutUShort(0x101);
                                        }
                                        else
                                        {
                                            PutUShort(0x102);
                                        }
                                    }
                                    else
                                    {
                                        //if (corpse.QuestID > 0)
                                        //     PutUShort(0); 
                                        //else 
                                        if (/*corpse.NPC.BaseData.CorpseItemID > 0 && */!corpse.ShouldDisappear)
                                        {
                                            PutUShort(0);
                                        }
                                        else
                                        {
                                            if (!corpse.PickUp)
                                            {
                                                PutUShort(2);
                                            }
                                            else
                                            {
                                                PutUShort(1);
                                            }
                                        }
                                    }
                                    PutULong(i.Target != null ? i.Target.ActorID : 0);
                                }
                                break;
                            case UpdateTypes.CorpseDoQuest:
                                {
                                    ActorCorpse corpse = (ActorCorpse)i.Actor;
                                    PutULong(i.Actor.ActorID);
                                    PutUShort(corpse.QuestID);
                                    PutByte(corpse.Step);
                                    PutByte((byte)i.UserData);
                                }
                                break;
                            case UpdateTypes.MapObjectDoQuest:
                                {
                                    PutULong(i.Actor.ActorID);
                                    PutUShort((ushort)i.UserData);
                                    PutUShort((ushort)i.UserData2);
                                }
                                break;
                            case UpdateTypes.PlayerRecover:
                                {
                                    PutULong(i.Actor.ActorID);
                                    PutByte((byte)i.ExtraActivateMode);
                                }
                                break;
                            case UpdateTypes.CorpseInteraction:
                                {
                                    PutULong(i.Actor.ActorID);
                                    PutULong(i.Target != null ? i.Target.ActorID : 0);
                                    PutByte((byte)i.UserData);
                                }
                                break;
                            case UpdateTypes.MapObjectInteraction:
                                {
                                    PutULong(i.Actor.ActorID);
                                    PutULong(i.Target != null ? i.Target.ActorID : 0);
                                    PutByte((byte)i.UserData);
                                }
                                break;
                            case UpdateTypes.NPCDash:
                                PutULong(i.Actor.ActorID);
                                PutByte(1);
                                PutShort((short)i.MoveArgument.X);
                                PutShort((short)i.MoveArgument.Y);
                                PutShort((short)i.MoveArgument.Z);
                                PutUShort(i.MoveArgument.Dir);
                                PutByte(3);
                                PutInt(i.MoveArgument.DashID);
                                PutShort(i.MoveArgument.DashUnknown);
                                break;
                            case UpdateTypes.DragonStream:
                                PutULong(i.Actor.ActorID);
                                PutULong(i.Target.ActorID);
                                PutByte((byte)i.AdditionCount);
                                PutShort(i.X);
                                PutShort(i.Y);
                                PutShort(i.Z);
                                PutUShort(i.Actor.Dir);
                                break;
                            case UpdateTypes.Debug:
                                PutBytes((byte[])i.UserData);
                                break;
                            case UpdateTypes.Repair:
                                PutULong(i.Actor.ActorID);
                                PutULong(i.Target.ActorID);
                                PutByte((byte)i.AdditionCount);
                                break;
                            case UpdateTypes.Teleport:
                                PutULong(i.Actor.ActorID);
                                PutByte((byte)i.AdditionCount);
                                break;
                        }
                        if (i.UpdateType != UpdateTypes.Debug)
                        {
                            endOffset = offset;
                            offset = lengthOffset;
                            PutByte((byte)(endOffset - lengthOffset));
                            offset = endOffset;
                        }
                    }
                    catch (Exception ex)
                    {
                        SmartEngine.Core.Logger.Log.Error(ex);
                        PutUShort((ushort)(GetUShort(14) - 1), 14);
                        offset = lengthOffset;
                    }
                }
                PutInt((int)Length - 6, 2);
            }
        }
    }
}
