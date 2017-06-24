using System;
using System.Text;
using System.Windows.Forms;
using System.Linq;
using SagaBNS.Common.Actors;
using SagaBNS.Common.Packets.GameServer;
using SmartEngine.Network;
using SmartEngine.Network.Utils;
namespace PacketViewer
{
    public unsafe partial class PacketParser
    {
        private string ParseServer06CBT1(Packet<int> p)
        {
            string parse = string.Empty;
            uint length = p.GetUInt(2);
            uint time = p.GetUInt(6);
            ushort count = p.GetUShort(14);
            parse += string.Format("Data Length: {0}\r\n", length);
            parse += string.Format("Time: {0}\r\n", time);
            parse += string.Format("Number of Parameters: {0}\r\n", count);
            for (int i = 0; i < count; i++)
            {
                byte dataLen = p.GetByte();
                switch (p.GetByte())
                {
                    case 0x01:
                        {
                            ushort varNum = (ushort)(dataLen - 24);
                            ulong actorID = p.GetULong();
                            ushort x = p.GetUShort();
                            ushort y = p.GetUShort();
                            ushort z = p.GetUShort();
                            ushort dir = p.GetUShort();
                            ushort speed = p.GetUShort();
                            ushort unk1 = p.GetUShort();
                            bool run = p.GetUShort() == 1;
                            parse += string.Format("Move 0x{0:X16},{1},{2},{3},{4},{5},{6},{7},{8}\r\n", actorID, x, y, z, dir, speed, unk1, run ? "true" : "false", Conversions.bytes2HexString(p.GetBytes(varNum)));
                        }
                        break;
                    case 0x02:
                        {
                            ulong actorId1 = p.GetULong(); //Attacker
                            uint skillId = p.GetUInt(); //Skill being used
                            ushort unk1 = p.GetUShort();
                            ushort unk2 = p.GetUShort();
                            byte skillMode = p.GetByte();
                            ushort unk4 = p.GetUShort();
                            byte pLength = p.GetByte();
                            byte pType = p.GetByte();
                            string temp;
                            switch (pType)
                            {
                                case 1:
                                    {
                                        ulong actorId2 = p.GetULong(); //Being attacked
                                        temp = string.Format("0x{0:X16}", actorId2);
                                    }
                                    break;
                                case 2:
                                    {
                                        ushort x = p.GetUShort();//x cord
                                        ushort y = p.GetUShort();//y cord
                                        ushort z = p.GetUShort();//z cord
                                        temp = string.Format("X:{0},Y:{1},Z:{2}", x, y, z);
                                    }
                                    break;
                                default:
                                    {
                                        temp = Conversions.bytes2HexString(p.GetBytes((ushort)(pLength - 2)));
                                    }
                                    break;
                            }

                            parse += string.Format("Skill Caster:0x{0:X16},SkillID:{1},U1:{2},U2:{3},SkillMode:{4},{5},Length:{6},CastMode:{7},TargetContent:{8}\r\n",
                                actorId1, skillId, unk1, unk2, skillMode, unk4, pLength, pType, temp);
                        }
                        break;
                    case 0x03:
                        {
                            if (dataLen == 26)
                            {
                                parse += string.Format("Effect Caster:0x{0:X16},Effector:0x{1:X16},SkillID:{2},{3},{4}\r\n", p.GetULong(), p.GetULong(), p.GetUInt(), p.GetUShort(), p.GetUShort());
                            }
                            else
                            {
                                goto default;
                            }
                        }
                        break;
                    case 0x04:
                        {
                            uint skillId = p.GetUInt();
                            ushort unk1 = p.GetUShort();
                            ulong actorId1 = p.GetULong();
                            ulong actorId2 = p.GetULong();
                            ushort unk2 = p.GetUShort();
                            string temp = string.Empty;
                            ushort count1 = p.GetUShort();
                            ushort dataLen2 = p.GetUShort();
                            temp = string.Format(",{0},{1},", count1, dataLen2);
                            ushort dataAcc = 0;
                            ushort id;
                            ulong parameter = 0;
                            for (int j = 0; j < count1; j++)
                            {
                                dataAcc += 2;
                                id = p.GetUShort();
                                switch (id)
                                {
                                    case (int)PacketParameterCBT1.Unk16:
                                    case (int)PacketParameterCBT1.Unk1F:
                                    case (int)PacketParameterCBT1.Unk21:
                                    case (int)PacketParameterCBT1.Unk22:
                                    case (int)PacketParameterCBT1.Unk23:
                                    case (int)PacketParameterCBT1.Unk37:
                                    case (int)PacketParameterCBT1.CombatStatus:
                                    case (int)PacketParameterCBT1.Unk3B:
                                    case (int)PacketParameterCBT1.Unk3C:
                                    case (int)PacketParameterCBT1.Unk3D:
                                    case (int)PacketParameterCBT1.Unk3E:
                                    case (int)PacketParameterCBT1.Unk74:
                                    case (int)PacketParameterCBT1.Unk75:
                                    case (int)PacketParameterCBT1.Unk76:
                                    case (int)PacketParameterCBT1.Unk78:
                                    case (int)PacketParameterCBT1.Unk7A:
                                    case (int)PacketParameterCBT1.Unk80:
                                    case (int)PacketParameterCBT1.Unk82:
                                    case (int)PacketParameterCBT1.Unk83:
                                    case (int)PacketParameterCBT1.Unk85:
                                    case (int)PacketParameterCBT1.Unk88:
                                    case (int)PacketParameterCBT1.Unk89:
                                    case (int)PacketParameterCBT1.Unk90:
                                    case (int)PacketParameterCBT1.Unk92:
                                    case (int)PacketParameterCBT1.Unk95:
                                    case (int)PacketParameterCBT1.Unk97:
                                    case (int)PacketParameterCBT1.Unk98:
                                    case (int)PacketParameterCBT1.Unk9B:
                                    case (int)PacketParameterCBT1.Unk9C:
                                    case (int)PacketParameterCBT1.Unk9D:
                                    case (int)PacketParameterCBT1.UnkA0:
                                    case (int)PacketParameterCBT1.UnkA1:
                                    case (int)PacketParameterCBT1.UnkA2:
                                    case (int)PacketParameterCBT1.UnkA6:
                                    case (int)PacketParameterCBT1.UnkA7:
                                    case (int)PacketParameterCBT1.UnkA8:
                                    case (int)PacketParameterCBT1.UnkA9:
                                        parameter = p.GetByte();
                                        temp += string.Format("{0},{1},", ((SagaBNS.Common.Packets.GameServer.PacketParameterCBT1)id).ToString(), parameter);
                                        dataAcc += 1;
                                        break;
                                    case (int)PacketParameterCBT1.X:
                                    case (int)PacketParameterCBT1.Y:
                                    case (int)PacketParameterCBT1.Z:
                                    case (int)PacketParameterCBT1.Dir:
                                    case (int)PacketParameterCBT1.MP:
                                    case (int)PacketParameterCBT1.Unk1E:
                                    case (int)PacketParameterCBT1.Unk27:
                                    case (int)PacketParameterCBT1.Unk41:
                                    case (int)PacketParameterCBT1.Unk42:
                                    case (int)PacketParameterCBT1.Unk43:
                                    case (int)PacketParameterCBT1.Unk44:
                                    case (int)PacketParameterCBT1.Unk47:
                                    case (int)PacketParameterCBT1.Unk48:
                                    case (int)PacketParameterCBT1.Unk49:
                                    case (int)PacketParameterCBT1.Unk4A:
                                    case (int)PacketParameterCBT1.Unk4B:
                                    case (int)PacketParameterCBT1.Unk4C:
                                    case (int)PacketParameterCBT1.Unk4D:
                                    case (int)PacketParameterCBT1.Unk4E:
                                    case (int)PacketParameterCBT1.Unk4F:
                                    case (int)PacketParameterCBT1.Unk50:
                                    case (int)PacketParameterCBT1.Unk51:
                                    case (int)PacketParameterCBT1.Unk52:
                                    case (int)PacketParameterCBT1.Unk53:
                                    case (int)PacketParameterCBT1.Unk54:
                                    case (int)PacketParameterCBT1.Unk55:
                                    case (int)PacketParameterCBT1.Unk56:
                                    case (int)PacketParameterCBT1.Unk57:
                                    case (int)PacketParameterCBT1.Unk58:
                                    case (int)PacketParameterCBT1.Unk59:
                                    case (int)PacketParameterCBT1.Unk5A:
                                    case (int)PacketParameterCBT1.Unk5B:
                                    case (int)PacketParameterCBT1.Unk5C:
                                    case (int)PacketParameterCBT1.Unk5D:
                                    case (int)PacketParameterCBT1.Unk5E:
                                    case (int)PacketParameterCBT1.Unk5F:
                                    case (int)PacketParameterCBT1.Unk60:
                                    case (int)PacketParameterCBT1.Unk61:
                                    case (int)PacketParameterCBT1.Unk62:
                                    case (int)PacketParameterCBT1.Unk63:
                                    case (int)PacketParameterCBT1.Unk64:
                                    case (int)PacketParameterCBT1.Unk65:
                                    case (int)PacketParameterCBT1.Unk66:
                                    case (int)PacketParameterCBT1.Unk67:
                                    case (int)PacketParameterCBT1.Unk68:
                                    case (int)PacketParameterCBT1.Unk69:
                                    case (int)PacketParameterCBT1.Unk6A:
                                    case (int)PacketParameterCBT1.Unk6B:
                                    case (int)PacketParameterCBT1.Unk6C:
                                    case (int)PacketParameterCBT1.Unk6D:
                                    case (int)PacketParameterCBT1.Unk6E:
                                    case (int)PacketParameterCBT1.Unk6F:
                                    case (int)PacketParameterCBT1.Unk70:
                                    case (int)PacketParameterCBT1.Unk71:
                                    case (int)PacketParameterCBT1.Unk72:
                                        parameter = p.GetUShort();
                                        temp += string.Format("{0},{1},", ((SagaBNS.Common.Packets.GameServer.PacketParameterCBT1)id).ToString(), parameter);
                                        dataAcc += 2;
                                        break;
                                    case (int)PacketParameterCBT1.Unk17:
                                    case (int)PacketParameterCBT1.HP:
                                    case (int)PacketParameterCBT1.Unk1A:
                                    case (int)PacketParameterCBT1.Unk40:
                                    case (int)PacketParameterCBT1.Unk45:
                                    case (int)PacketParameterCBT1.Unk46:
                                    case (int)PacketParameterCBT1.Weapon:
                                    case (int)PacketParameterCBT1.Costume:
                                        parameter = p.GetUInt();
                                        temp += string.Format("{0},{1},", ((SagaBNS.Common.Packets.GameServer.PacketParameterCBT1)id).ToString(), parameter);
                                        dataAcc += 4;
                                        break;
                                    case (int)PacketParameterCBT1.Unk20:
                                    case (int)PacketParameterCBT1.FaceTo:
                                    case (int)PacketParameterCBT1.Unk29:
                                    case (int)PacketParameterCBT1.Unk2A:
                                    case (int)PacketParameterCBT1.Hold:
                                        parameter = p.GetULong();
                                        dataAcc += 8;
                                        temp += string.Format("{0},0x{1:X16},", ((SagaBNS.Common.Packets.GameServer.PacketParameterCBT1)id).ToString(), parameter);
                                        break;
                                }
                            }
                            int test = dataLen2 - dataAcc;
                            if (test != 0)
                            {
                                temp = ",trace into parameters...,";
                                p.Position += test;
                            }
                            byte unk3 = p.GetByte();
                            byte unk4 = p.GetByte();
                            string temp2 = string.Empty;
                            if (unk3 > 2)
                            {
                                for (int j = 0; j < unk3 - 2; j++)
                                {
                                    temp2 += string.Format(",{0}", p.GetByte());
                                }
                            }

                            parse += string.Format("Actor SkillID:{0},{1},0x{2:X16},0x{3:X16},{4}{5}{6},{7}{8}\r\n",
                                skillId, unk1, actorId1, actorId2, unk2, temp, unk3, unk4, temp2);
                        }
                        break;
                    case 0x05:
                        {
                            ulong actorId1 = p.GetULong();
                            byte pLength = p.GetByte();
                            byte pType = p.GetByte();
                            string temp = string.Empty;
                            switch (pType)
                            {
                                case 1:
                                    {
                                        ushort unk1 = p.GetUShort();
                                        uint unk2 = p.GetUInt();
                                        temp = string.Format("{0},{1},{2}", unk1, unk2, Conversions.bytes2HexString(p.GetBytes((ushort)(pLength - 8))));
                                    }
                                    break;
                                default:
                                    {
                                        temp = Conversions.bytes2HexString(p.GetBytes((ushort)(pLength - 2)));
                                    }
                                    break;
                            }
                            parse += string.Format("0x05 0x{0:X16},{1},{2},{3}\r\n", actorId1, pLength, pType, temp);
                        }
                        break;
                    case 0x06:
                        {
                            if (dataLen == 19)
                            {
                                parse += string.Format("MapObjectOperate 0x{0:X16},0x{1:X16},{2}\r\n", p.GetULong(), p.GetULong(), p.GetByte());
                            }
                            else
                            {
                                parse += "Trace here ...";
                            }
                        }
                        break;
                    case 0x08:
                        {
                            if (dataLen == 16)
                            {
                                parse += string.Format("MapObjectVisibilityChange 0x{0:X16},{1}\r\n", p.GetULong(), Conversions.bytes2HexString(p.GetBytes(6)));
                            }
                            else
                            {
                                goto default;
                            }
                        }
                        break;
                    case 0x0A:
                        {
                            if (dataLen == 16)
                            {
                                parse += string.Format("0x0A 0x{0:X16},{1},{2},{3}\r\n", p.GetULong(), p.GetByte(), p.GetByte(), p.GetUInt());
                            }
                            else
                            {
                                goto default;
                            }
                        }
                        break;
                    case 0x0B:
                        {
                            if (dataLen == 15)
                            {
                                parse += string.Format("0x0B 0x{0:X16},{1}\r\n", p.GetULong(), Conversions.bytes2HexString(p.GetBytes(5)));
                            }
                            else
                            {
                                goto default;
                            }
                        }
                        break;
                    //haven't added fixed length past this point could have unexpected results
                    case 0x0C:
                        {
                            ulong actorID = p.GetULong();
                            uint dialogID = p.GetUInt();
                            ushort unknown = p.GetUShort();
                            parse += string.Format("NPCChat 0x{0:X16},{1},{2}\r\n", actorID, dialogID, unknown);
                        }
                        break;
                    case 0x0D:
                        {
                            parse += string.Format("0x0D 0x{0:X16},{1}\r\n", p.GetULong(), p.GetUInt());
                        }
                        break;
                    case 0x0E:
                        {
                            parse += string.Format("Interaction 0x{0:X16},0x{1:X16},{2}\r\n", p.GetULong(), p.GetULong(), Conversions.bytes2HexString(p.GetBytes(16)));
                        }
                        break;
                    case 0x0F:
                        {
                            parse += string.Format("0x0F 0x{0:X16},{1},{2},{3},{4},{5},{6},{7}\r\n", p.GetULong(), Conversions.bytes2HexString(p.GetBytes(5)), p.GetUShort(), p.GetUShort(), p.GetUShort(), p.GetUShort(), p.GetUShort(), Conversions.bytes2HexString(p.GetBytes(5)));
                        }
                        break;
                    case 0x10:
                        {
                            parse += string.Format("0x10 0x{0:X16},0x{1:X16},{2},{3},{4},{5},{6}\r\n", p.GetULong(), p.GetULong(), Conversions.bytes2HexString(p.GetBytes(5)), p.GetUShort(), p.GetUShort(), p.GetUShort(), Conversions.bytes2HexString(p.GetBytes(6)));
                        }
                        break;
                    case 0x14:
                        {
                            parse += string.Format("0x14 0x{0:X16},0x{1:X16}\r\n", p.GetULong(), p.GetULong());
                        }
                        break;
                    case 0x15:
                        {
                            parse += string.Format("InteractionCancel 0x{0:X16},0x{1:X16},{2}\r\n", p.GetULong(), p.GetULong(), Conversions.bytes2HexString(p.GetBytes(6)));
                        }
                        break;
                    case 0x16:
                        {
                            parse += string.Format("0x16 0x{0:X16},0x{1:X16},{2}\r\n", p.GetULong(), p.GetULong(), p.GetByte());
                        }
                        break;
                    case 0x17:
                        {
                            parse += string.Format("0x17 0x{0:X16},0x{1:X16}\r\n", p.GetULong(), p.GetULong());
                        }
                        break;
                    case 0x18:
                        {
                            parse += string.Format("0x18 0x{0:X16},0x{1:X16},{2},{3},{4},{5}\r\n", p.GetULong(), p.GetULong(), p.GetUShort(), p.GetUShort(), p.GetUShort(), p.GetByte());
                        }
                        break;
                    case 0x19:
                        {
                            ushort varNum = (ushort)(dataLen - 44);
                            ulong actorId1 = p.GetULong();
                            ulong actorId2 = p.GetULong();
                            uint unk1 = p.GetUInt();
                            ulong actorId3 = p.GetULong();
                            ulong actorId4 = p.GetULong();
                            ushort x = p.GetUShort();
                            ushort y = p.GetUShort();
                            ushort z = p.GetUShort();
                            parse += string.Format("0x19 0x{0:X16},0x{1:X16},{2},0x{3:X16},0x{4:X16},{5},{6},{7},{8}\r\n",
                                actorId1, actorId2, unk1, actorId3, actorId4, x, y, z, Conversions.bytes2HexString(p.GetBytes(varNum)));
                        }
                        break;
                    case 0x1A:
                        {
                            parse += string.Format("0x1A 0x{0:X16}\r\n", p.GetULong());
                        }
                        break;
                    case 0x1B:
                        {
                            parse += string.Format("0x1B 0x{0:X16},{1},0x{2:X16}\r\n", p.GetULong(), p.GetUShort(), p.GetULong());
                        }
                        break;
                    case 0x20:
                        {
                            parse += string.Format("0x20 0x{0:X16},0x{1:X16},{2}\r\n", p.GetULong(), p.GetULong(), p.GetByte());
                        }
                        break;
                    case 0x21:
                        {
                            parse += string.Format("0x21 0x{0:X16},{1}\r\n", p.GetULong(), p.GetByte());
                        }
                        break;
                    case 0x25:
                        {
                            ulong actorID = p.GetULong();
                            p.Position++;
                            ushort x = p.GetUShort();
                            ushort y = p.GetUShort();
                            ushort z = p.GetUShort();
                            ushort dir = p.GetUShort();
                            p.Position += 1;
                            uint dashID = p.GetUInt();
                            ushort unknown = p.GetUShort();
                            parse += string.Format("Dash 0x{0:X16},{1},{2},{3},{4},{5},{6}\r\n", actorID, x, y, z, dir, dashID, unknown);
                        }
                        break;
                    default:
                        {
                            i = 999;
                        }
                        break;
                }
            }
            int numLines = parse.Count(f => f == '\r');
            if (numLines - 3 != count)
            {
                parse += "Missing lines!?\r\n";
            }

            string[] source = parse.Split(new char[] { ' ', ',', '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
            string[] find = { "missing", "trace" };
            for (int k = 0; k < find.Length; k++)
            {
                int numError = (from word in source where word.ToLowerInvariant() == find[k].ToLowerInvariant() select word).Count();
                if (numError != 0)
                {
                tryAgain:
                    switch (MessageBox.Show("Click No to continue.",
                                        "Catch for stopping script",
                                        MessageBoxButtons.YesNoCancel,
                                        MessageBoxIcon.Question))
                    {
                        case DialogResult.Yes:
                            goto tryAgain;

                        case DialogResult.No:
                            break;

                        case DialogResult.Cancel:
                            goto tryAgain;
                    }
                }
            }
            return parse;
        }

        private string ParseServer07CBT1(Packet<int> p)
        {
            string parse = string.Empty;
            uint dataLen = p.GetUInt(2);
            byte unk1 = p.GetByte();
            ushort numSets = p.GetUShort();
            ushort numParams;
            ushort paramLen;
            ushort dataAcc;
            ushort id;
            string temp;
            ulong parameter = 0;
            for (int i = 0; i < numSets; i++)
            {
                numParams = p.GetUShort();
                paramLen = p.GetUShort();
                temp = string.Empty;
                dataAcc = 0;
                parse += string.Format("Set Number: {0}\r\n", i + 1);
                for (int j = 0; j < numParams; j++)
                {
                    dataAcc += 2;
                    id = p.GetUShort();
                    switch (id)
                    {
                        case (int)PacketParameterCBT1.Unk16:
                        case (int)PacketParameterCBT1.Unk1F:
                        case (int)PacketParameterCBT1.Unk21:
                        case (int)PacketParameterCBT1.Unk22:
                        case (int)PacketParameterCBT1.Unk23:
                        case (int)PacketParameterCBT1.Unk37:
                        case (int)PacketParameterCBT1.CombatStatus:
                        case (int)PacketParameterCBT1.Unk3B:
                        case (int)PacketParameterCBT1.Unk3C:
                        case (int)PacketParameterCBT1.Unk3D:
                        case (int)PacketParameterCBT1.Unk3E:
                        case (int)PacketParameterCBT1.Unk74:
                        case (int)PacketParameterCBT1.Unk75:
                        case (int)PacketParameterCBT1.Unk76:
                        case (int)PacketParameterCBT1.Unk78:
                        case (int)PacketParameterCBT1.Unk7A:
                        case (int)PacketParameterCBT1.Unk80:
                        case (int)PacketParameterCBT1.Unk82:
                        case (int)PacketParameterCBT1.Unk83:
                        case (int)PacketParameterCBT1.Unk85:
                        case (int)PacketParameterCBT1.Unk88:
                        case (int)PacketParameterCBT1.Unk89:
                        case (int)PacketParameterCBT1.Unk90:
                        case (int)PacketParameterCBT1.Unk92:
                        case (int)PacketParameterCBT1.Unk95:
                        case (int)PacketParameterCBT1.Unk97:
                        case (int)PacketParameterCBT1.Unk98:
                        case (int)PacketParameterCBT1.Unk9B:
                        case (int)PacketParameterCBT1.Unk9C:
                        case (int)PacketParameterCBT1.Unk9D:
                        case (int)PacketParameterCBT1.UnkA0:
                        case (int)PacketParameterCBT1.UnkA1:
                        case (int)PacketParameterCBT1.UnkA2:
                        case (int)PacketParameterCBT1.UnkA6:
                        case (int)PacketParameterCBT1.UnkA7:
                        case (int)PacketParameterCBT1.UnkA8:
                        case (int)PacketParameterCBT1.UnkA9:
                            parameter = p.GetByte();
                            dataAcc += 1;
                            break;
                        case (int)PacketParameterCBT1.X:
                        case (int)PacketParameterCBT1.Y:
                        case (int)PacketParameterCBT1.Z:
                        case (int)PacketParameterCBT1.Dir:
                        case (int)PacketParameterCBT1.MP:
                        case (int)PacketParameterCBT1.Unk1E:
                        case (int)PacketParameterCBT1.Unk27:
                        case (int)PacketParameterCBT1.Unk41:
                        case (int)PacketParameterCBT1.Unk42:
                        case (int)PacketParameterCBT1.Unk43:
                        case (int)PacketParameterCBT1.Unk44:
                        case (int)PacketParameterCBT1.Unk47:
                        case (int)PacketParameterCBT1.Unk48:
                        case (int)PacketParameterCBT1.Unk49:
                        case (int)PacketParameterCBT1.Unk4A:
                        case (int)PacketParameterCBT1.Unk4B:
                        case (int)PacketParameterCBT1.Unk4C:
                        case (int)PacketParameterCBT1.Unk4D:
                        case (int)PacketParameterCBT1.Unk4E:
                        case (int)PacketParameterCBT1.Unk4F:
                        case (int)PacketParameterCBT1.Unk50:
                        case (int)PacketParameterCBT1.Unk51:
                        case (int)PacketParameterCBT1.Unk52:
                        case (int)PacketParameterCBT1.Unk53:
                        case (int)PacketParameterCBT1.Unk54:
                        case (int)PacketParameterCBT1.Unk55:
                        case (int)PacketParameterCBT1.Unk56:
                        case (int)PacketParameterCBT1.Unk57:
                        case (int)PacketParameterCBT1.Unk58:
                        case (int)PacketParameterCBT1.Unk59:
                        case (int)PacketParameterCBT1.Unk5A:
                        case (int)PacketParameterCBT1.Unk5B:
                        case (int)PacketParameterCBT1.Unk5C:
                        case (int)PacketParameterCBT1.Unk5D:
                        case (int)PacketParameterCBT1.Unk5E:
                        case (int)PacketParameterCBT1.Unk5F:
                        case (int)PacketParameterCBT1.Unk60:
                        case (int)PacketParameterCBT1.Unk61:
                        case (int)PacketParameterCBT1.Unk62:
                        case (int)PacketParameterCBT1.Unk63:
                        case (int)PacketParameterCBT1.Unk64:
                        case (int)PacketParameterCBT1.Unk65:
                        case (int)PacketParameterCBT1.Unk66:
                        case (int)PacketParameterCBT1.Unk67:
                        case (int)PacketParameterCBT1.Unk68:
                        case (int)PacketParameterCBT1.Unk69:
                        case (int)PacketParameterCBT1.Unk6A:
                        case (int)PacketParameterCBT1.Unk6B:
                        case (int)PacketParameterCBT1.Unk6C:
                        case (int)PacketParameterCBT1.Unk6D:
                        case (int)PacketParameterCBT1.Unk6E:
                        case (int)PacketParameterCBT1.Unk6F:
                        case (int)PacketParameterCBT1.Unk70:
                        case (int)PacketParameterCBT1.Unk71:
                        case (int)PacketParameterCBT1.Unk72:
                            parameter = p.GetUShort();
                            dataAcc += 2;
                            break;
                        case (int)PacketParameterCBT1.Unk17:
                        case (int)PacketParameterCBT1.HP:
                        case (int)PacketParameterCBT1.Unk1A:
                        case (int)PacketParameterCBT1.Unk40:
                        case (int)PacketParameterCBT1.Unk45:
                        case (int)PacketParameterCBT1.Unk46:
                        case (int)PacketParameterCBT1.Weapon:
                        case (int)PacketParameterCBT1.Costume:
                            parameter = p.GetUInt();
                            dataAcc += 4;
                            break;
                        case (int)PacketParameterCBT1.Unk20:
                        case (int)PacketParameterCBT1.FaceTo:
                        case (int)PacketParameterCBT1.Unk29:
                        case (int)PacketParameterCBT1.Unk2A:
                        case (int)PacketParameterCBT1.Hold:
                            parameter = p.GetULong();
                            dataAcc += 8;
                            break;
                        default:
                            j = 999;
                            break;
                    }
                    temp += string.Format("Parameter: {0} = {1}\r\n", ((SagaBNS.Common.Packets.GameServer.PacketParameterCBT1)id).ToString(), parameter);
                }
                int test = paramLen - dataAcc;
                if (test != 0)
                {
                    temp += "Trace into parameters...\r\n";
                    p.Position += test;
                }
                parse += temp + "\r\n";
            }
            string[] source = parse.Split(new char[] { ' ', ',', '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
            string find = "trace";
            int numError = (from word in source where word.ToLowerInvariant() == find.ToLowerInvariant() select word).Count();
            if (numError != 0)
            {
            tryAgain:
                switch (MessageBox.Show("Click No to continue.",
                                    "Catch for stopping script",
                                    MessageBoxButtons.YesNoCancel,
                                    MessageBoxIcon.Question))
                {
                    case DialogResult.Yes:
                        goto tryAgain;

                    case DialogResult.No:
                        break;

                    case DialogResult.Cancel:
                        goto tryAgain;
                }
            }
            return parse;
        }

        private string ParseServerActorAppearCBT1(Packet<int> p)
        {
            string parse = string.Empty;
            switch (p.ID)
            {
                case 0x01:
                    {
                        parse += string.Format("MapInstanceID:{0}\r\n", p.GetUInt(6));
                        short count = p.GetShort(14);
                        parse += string.Format("Disappear Count:{0}\r\n", count);
                        for (int i = 0; i < count; i++)
                        {
                            ulong actorID = p.GetULong();
                            if (actorID > 0x4000000000000)
                            {
                                parse += string.Format("NPC ActorID:0x{0:X}\r\n", actorID);
                            }
                            else
                            {
                                parse += string.Format("PC ActorID:0x{0:X}\r\n", actorID);
                            }
                            p.GetBytes(9);
                        }
                        count = p.GetShort();
                        parse += string.Format("Appear Count:{0}\r\n", count);
                        for (int i = 0; i < count; i++)
                        {
                            ulong actorID = p.GetULong();
                            if (actorID > 0x4000000000000)
                            {
                                p.GetBytes(6);
                                ushort npcID = Global.LittleToBigEndian(p.GetUShort());
                                p.GetBytes(11);
                                parse += string.Format("NPC ActorID:0x{0:X} NPCID:{1}\r\n", actorID, npcID);
                            }
                            else
                            {
                                p.GetByte();
                                Race race = (Race)p.GetByte();
                                Gender gender = (Gender)p.GetByte();
                                Job job = (Job)p.GetByte();
                                p.GetBytes(11);
                                string name = Encoding.Unicode.GetString(p.GetBytes((ushort)((p.GetUShort() + 1) * 2))).Trim('\0');
                                parse += string.Format("PC ActorID:0x{0:X} Race:{1} Gender:{2} Job:{3} Name:{4}\r\n", actorID, race, gender, job, name);
                            }
                        }
                    }
                    break;
                case 0x02:
                    {
                        parse += string.Format("MapInstanceID:{0}\r\n", p.GetUInt(6));
                        short count = p.GetShort(14);
                        for (int i = 0; i < count; i++)
                        {
                            p.GetBytes(9);
                        }
                        count = p.GetShort();
                        parse += string.Format("Appear Count:{0}\r\n", count);
                        for (int i = 0; i < count; i++)
                        {
                            ulong actorID = p.GetULong();
                            p.GetBytes(9);
                            uint mapID = p.GetUInt();
                            ushort x = p.GetUShort();
                            ushort y = p.GetUShort();
                            ushort z = p.GetUShort();
                            ushort dir = p.GetUShort();
                            byte level = p.GetByte();
                            p.GetInt();
                            int hp = p.GetInt();
                            int mp = p.GetInt();
                            p.GetUShort();
                            ManaType mana = (ManaType)p.GetInt();
                            p.GetBytes(61);
                            byte friendly = p.GetByte();
                            int maxHP = p.GetInt();
                            ushort maxMP = p.GetUShort();
                            if (actorID > 0x4000000000000)
                            {
                                p.GetBytes(9);
                                int motion = p.GetInt();
                                p.GetBytes(26);
                                parse += string.Format("NPC ActorID:0x{0:X} Friendly:{13} MapID:{1} Pos:{2},{3},{4} Dir:{5} Level:{6} HP:{7}/{8} MP: {9}/{10} ManaType:{11} Motion:{12}\r\n",
                                    actorID, mapID, x, y, z, dir, level, hp, maxHP, mp, maxMP, mana, motion, friendly);
                            }
                            else
                            {
                                p.GetBytes(39);
                                parse += string.Format("PC ActorID:0x{0:X} MapID:{1} Pos:{2},{3},{4} Dir:{5} Level:{6} HP:{7}/{8} MP: {9}/{10} ManaType:{11}\r\n",
                                    actorID, mapID, x, y, z, dir, level, hp, maxHP, mp, maxMP, mana);
                            }
                        }
                    }
                    break;
            }
            return parse;
        }

        private string ParseServerActorListCBT1(Packet<int> p)
        {
            string parse = string.Empty;
            switch (p.ID)
            {
                case 0x03:
                    {
                        parse += string.Format("MapInstanceID:{0}\r\n", p.GetUInt(6));
                        short count = p.GetShort(14);
                        parse += string.Format("Count:{0}\r\n", count);
                        for (int i = 0; i < count; i++)
                        {
                            ulong actorID = p.GetULong();
                            if (actorID > 0x4000000000000)
                            {
                                p.GetBytes(5);
                                ushort npcID = Global.LittleToBigEndian(p.GetUShort());
                                p.GetBytes(11);
                                parse += string.Format("NPC ActorID:0x{0:X} NPCID:{1}\r\n", actorID, npcID);
                            }
                            else
                            {
                                Race race = (Race)p.GetByte();
                                Gender gender = (Gender)p.GetByte();
                                Job job = (Job)p.GetByte();
                                p.GetBytes(11);
                                string name = Encoding.Unicode.GetString(p.GetBytes((ushort)((p.GetUShort() + 1) * 2))).Trim('\0');
                                parse += string.Format("PC ActorID:0x{0:X} Race:{1} Gender:{2} Job:{3} Name:{4}\r\n", actorID, race, gender, job, name);
                            }
                        }
                    }
                    break;
                case 0x04:
                    {
                        parse += string.Format("MapInstanceID:{0}\r\n", p.GetUInt(6));
                        short count = p.GetShort(14);
                        parse += string.Format("Count:{0}\r\n", count);
                        for (int i = 0; i < count; i++)
                        {
                            ulong actorID = p.GetULong();
                            uint mapID = p.GetUInt();
                            ushort x = p.GetUShort();
                            ushort y = p.GetUShort();
                            ushort z = p.GetUShort();
                            ushort dir = p.GetUShort();
                            byte level = p.GetByte();
                            p.GetInt();
                            int hp = p.GetInt();
                            int mp = p.GetInt();
                            p.GetUShort();
                            ManaType mana = (ManaType)p.GetInt();
                            p.GetBytes(61);
                            byte friendly = p.GetByte();
                            int maxHP = p.GetInt();
                            ushort maxMP = p.GetUShort();
                            if (actorID > 0x4000000000000)
                            {
                                p.GetBytes(9);
                                int motion = p.GetInt();
                                p.GetBytes(26);
                                parse += string.Format("NPC ActorID:0x{0:X} Friendly:{13} MapID:{1} Pos:{2},{3},{4} Dir:{5} Level:{6} HP:{7}/{8} MP: {9}/{10} ManaType:{11} Motion:{12}\r\n",
                                    actorID, mapID, x, y, z, dir, level, hp, maxHP, mp, maxMP, mana, motion, friendly);
                            }
                            else
                            {
                                p.GetBytes(39);
                                parse += string.Format("PC ActorID:0x{0:X} MapID:{1} Pos:{2},{3},{4} Dir:{5} Level:{6} HP:{7}/{8} MP: {9}/{10} ManaType:{11}\r\n",
                                    actorID, mapID, x, y, z, dir, level, hp, maxHP, mp, maxMP, mana);
                            }
                        }
                    }
                    break;
            }
            return parse;
        }

        private string ParseReceiveCBT1(Packet<int> p)
        {
            string parse = string.Empty;
            switch (p.ID)
            {
                case 0x01:
                case 0x02:
                    {
                        parse = ParseServerActorAppearCBT1(p);
                    }
                    break;
                case 0x03:
                case 0x04:
                    {
                        parse = ParseServerActorListCBT1(p);
                    }
                    break;
                case 0x05:
                    {
                        //Reply to Client 0x0091
                        uint dataLength = p.GetUInt(2);
                        ushort map = p.GetUShort();
                        uint unk1 = p.GetUInt(); //0x00
                        ushort unk2 = p.GetUShort();//16
                        ushort count1 = p.GetUShort();//NPC & Players
                        parse += string.Format("Map Id: {0}\r\n\r\nPlayers & NPC:\r\n", map);
                        for (int i = 0; i < count1; i++)
                        {
                            parse += string.Format("{0}. 0x{1:X16}\r\n", i + 1, p.GetULong());
                        }
                        ushort count2 = p.GetUShort();//Map Objects
                        parse += "\r\nMap Objects:\r\n";
                        for (int i = 0; i < count2; i++)
                        {
                            parse += string.Format("{0}. 0x{1:X16}\r\n", i + 1, p.GetULong());
                        }
                        ulong unk3 = p.GetULong();
                    }
                    break;
                case 0x6:
                    {
                        parse = ParseServer06CBT1(p);
                        break;
                    }
                case 0x07:
                    {
                        parse = ParseServer07CBT1(p);
                    }
                    break;
                case 0x20:
                    {
                        ushort count = p.GetUShort(14);
                        byte[] uni = p.GetBytes((ushort)(count * 2));
                        parse = System.Text.Encoding.Unicode.GetString(uni).Replace("\0", "");
                    }
                    break;
                case 0x29:
                    {
                        parse = string.Format("0x{0:X16}", p.GetULong(2));
                    }
                    break;
                case 0x4C:
                    {
                        ushort count = p.GetUShort(2);
                        parse += "Number of Skills: " + count + "\r\n";
                        for (int i = 0; i < count; i++)
                        {
                            uint skillId = p.GetUInt();
                            p.GetByte();
                            string skill = "SkillId #{0}: {1}\r\n";
                            parse += string.Format(skill, i + 1, skillId);
                        }
                    }
                    break;
                case 0xED:
                    {
                        ushort unk1 = p.GetUShort(2);
                        byte unk2 = p.GetByte();
                        uint skillId = p.GetUInt();
                        byte unk3 = p.GetByte();
                        parse += string.Format("{0},{1},{2},{3}\r\n", unk1, unk2, skillId, unk3);
                    }
                    break;
                case 0x011B:
                    {
                        uint length = p.GetUInt(2);
                        byte method = p.GetByte();
                        ushort count = p.GetUShort();
                        byte itemType = p.GetByte();
                        byte unk = p.GetByte();
                        byte slotId = p.GetByte();
                        byte equipSlot = p.GetByte();
                        uint itemId = p.GetUInt();
                        parse += string.Format("ItemType:{0:X} EquipSlot:{1} ItemId:{2}\r\n", itemType, equipSlot, itemId);
                    }
                    break;
                case 0x0158:
                    {
                        uint skillId = p.GetUInt(2);
                        p.GetByte();
                        string skill = "SkillId Added: {0}\r\n";
                        parse += string.Format(skill, skillId);
                    }
                    break;
            }
            return parse;
        }
    }
}
