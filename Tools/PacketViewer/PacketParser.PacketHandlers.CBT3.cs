using System;
using System.Text;
using System.Windows.Forms;
using System.Linq;
using SagaBNS.Common.Actors;
using SagaBNS.Common.Packets;
using SagaBNS.Common.Packets.GameServer;
using SmartEngine.Network;
using SmartEngine.Network.Utils;

namespace PacketViewer
{
    public unsafe partial class PacketParser
    {
        private string ParseServerActorListCBT3(Packet p)
        {
            string parse = string.Empty;
            switch (p.ID)
            {
                case (int)GamePacketOpcode.SM_ACTOR_LIST:
                    {
                        parse += string.Format("MapInstanceID:{0}\r\n", p.GetUInt(6));
                        short count = p.GetShort(15);
                        parse += string.Format("Count:{0}\r\n", count);
                        try
                        {
                            for (int i = 0; i < count; i++)
                            {
                                ulong actorID = p.GetULong();
                                if (actorID > 0x4000000000000)
                                {
                                    p.GetBytes(5);
                                    ushort npcID = Global.LittleToBigEndian(p.GetUShort());
                                    p.Position += 93;
                                    parse += string.Format("NPC ActorID:0x{0:X} NPCID:{1}\r\n", actorID, npcID);
                                }
                                else
                                {
                                    Race race = (Race)p.GetByte();
                                    Gender gender = (Gender)p.GetByte();
                                    Job job = (Job)p.GetByte();
                                    p.Position += 92;
                                    string name = Encoding.Unicode.GetString(p.GetBytes((ushort)(p.GetUShort() * 2)));
                                    parse += string.Format("PC ActorID:0x{0:X} Race:{1} Gender:{2} Job:{3} Name:{4} Unknown:{5:X}\r\n", actorID, race, gender, job, name,Conversions.bytes2HexString(p.GetBytes(3)));
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show(ex.ToString(), "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                    break;
                case (int)GamePacketOpcode.SM_ACTOR_INFO_LIST:
                    {
                        int length = p.GetInt(2);
                        if (length == 0)
                        {
                            break;
                        }

                        int index = Packets.IndexOf(p) + 1;
                        while (p.Length + 6 < length)
                        {
                            Packet<int> tmp = Packets[index++];
                            p.PutBytes(tmp.GetBytes((ushort)(tmp.Length - 6), 6), (ushort)p.Length);
                        }

                        parse += string.Format("MapInstanceID:{0}\r\n", p.GetUInt(6));
                        short count = p.GetShort(14);
                        parse += string.Format("Count:{0}\r\n", count);
                        try
                        {
                            for (int i = 0; i < count; i++)
                            {
                                ulong actorID = p.GetULong();
                                uint mapID = p.GetUInt();
                                short x = p.GetShort();
                                short y = p.GetShort();
                                short z = p.GetShort();
                                ushort dir = p.GetUShort();
                                byte level = p.GetByte();
                                p.GetInt();
                                int hp = p.GetInt();
                                p.Position += 16;
                                short mp = p.GetShort();
                                ManaType mana = (ManaType)p.GetInt();
                                p.Position += 76;
                                byte friendly = p.GetByte();
                                int maxhp = p.GetInt();
                                short maxmp = p.GetShort();
                                p.Position += 35;
                                short toggle = p.GetShort();
                                if (toggle > 0)
                                {
                                    byte[] temp = p.GetBytes((ushort)(12 * toggle));
                                }
                                string type;

                                if (actorID > 0x4000000000000 && actorID < 0x5000000000000)
                                {
                                    type = "NPC";
                                }
                                else if (actorID > 0x2000000000000 && actorID < 0x3000000000000)
                                {
                                    type = "Summon";
                                }
                                else if (actorID > 0x1000000000000 && actorID < 0x2000000000000)
                                {
                                    type = "PC";
                                }
                                else
                                {
                                    type = "Unknown";
                                }

                                parse += string.Format(type + " ActorID:0x{0:X16} MapID:{1} Friendly:{12:X} Pos:{2},{3},{4} Dir:{5} Level:{6} HP:{7}/{8} MP:{9}/{10} ManaType:{11}\r\n",
                                    actorID, mapID, x, y, z, dir, level, hp, maxhp, mp, maxmp, mana, friendly);

                                /*
                                int mp = p.GetInt();
                                p.GetUShort();
                                ManaType mana = (ManaType)p.GetInt();
                                p.Position += 8;
                                ulong unknownActor = p.GetULong();
                                p.Position += 44;
                                byte friendly = p.GetByte();
                                int maxHP = p.GetInt();
                                ushort maxMP = p.GetUShort();
                                if (actorID > 0x4000000000000)
                                {
                                    p.GetBytes(9);
                                    int motion = p.GetInt();
                                    p.GetBytes(24);
                                    short count2 = p.GetShort();
                                    string unknownToken = string.Empty;
                                    for (int j = 0; j < count2; j++)
                                    {
                                        unknownToken += string.Format(" UnknownToken{0}:{1},{2},{3},{4},{5}", j + 1, p.GetByte(), p.GetByte(), p.GetInt(), p.GetInt(), p.GetShort());
                                    }
                                    parse += string.Format("NPC ActorID:0x{0:X} Friendly:{13} MapID:{1} Pos:{2},{3},{4} Dir:{5} Level:{6} HP:{7}/{8} MP: {9}/{10} ManaType:{11} Motion:{12} {14}",
                                        actorID, mapID, x, y, z, dir, level, hp, maxHP, mp, maxMP, mana, motion, friendly, unknownToken);
                                }
                                else
                                {
                                    p.GetBytes(37);
                                    short toggle = p.GetShort();
                                    if (toggle > 0)
                                    {
                                        p.GetBytes((ushort)(12 * toggle));
                                    }
                                    parse += string.Format("PC ActorID:0x{0:X} MapID:{1} Pos:{2},{3},{4} Dir:{5} Level:{6} HP:{7}/{8} MP: {9}/{10} ManaType:{11}",
                                        actorID, mapID, x, y, z, dir, level, hp, maxHP, mp, maxMP, mana);
                                }
                                parse += "\r\n";
                                */
                            }
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show(ex.ToString(), "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                        count = p.GetShort();
                        parse += string.Format("\r\nMap Obj Count:{0}\r\n", count);
                        try
                        {
                            for (int i = 0; i < count; i++)
                            {
                                ulong actorID = p.GetULong();
                                int stateId = p.GetInt();
                                string state;
                                switch (stateId)
                                {
                                    case 2:
                                    case 4:
                                        state = "Invisible";
                                        break;
                                    case 3:
                                    case 5:
                                        state = "Visible";
                                        break;
                                    default:
                                        state = "Unknown: " + stateId;
                                        break;
                                }
                                p.GetByte();
                                bool enabled = p.GetByte() == 1 ? true : false;
                                p.GetBytes(1);
                                short toggle = p.GetShort();
                                p.GetBytes((ushort)(toggle * 4));
                                parse += string.Format("Map Obj:0x{0:X16} is {1} and {2}\r\n", actorID, state, enabled ? "Enabled" : "Disabled");
                            }
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show(ex.ToString(), "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                        count = p.GetShort();
                        parse += string.Format("\r\nUnknown Count:{0}\r\n", count);
                        try
                        {
                            for (int i = 0; i < count; i++)
                            {
                                ulong actorID = p.GetULong();
                                p.GetBytes(5);
                                short x = p.GetShort();
                                short y = p.GetShort();
                                short z = p.GetShort();
                                p.GetBytes(5);
                                parse += string.Format("Unknown Obj:0x{0:X16} X:{1}, Y:{2}, Z:{3}\r\n", actorID, x, y, z);
                            }
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show(ex.ToString(), "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }

                        count = p.GetShort();
                        if (count > 0)
                        {
                            MessageBox.Show("Unknown Object used", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }

                        count = p.GetShort();
                        parse += string.Format("\r\nUnknown Count:{0}\r\n", count);
                        try
                        {
                            for (int i = 0; i < count; i++)
                            {
                                ulong actorID = p.GetULong();
                                p.GetBytes(22);
                                short x = p.GetShort();
                                short y = p.GetShort();
                                short z = p.GetShort();
                                p.GetShort();
                                ushort temp = (ushort)(p.GetUShort() * 4);
                                if (temp > 0)
                                {
                                    p.GetBytes(temp);
                                }

                                parse += string.Format("Unknown Obj:0x{0:X16} X:{1}, Y:{2}, Z:{3}\r\n", actorID, x, y, z);
                            }
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show(ex.ToString(), "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }

                        count = p.GetShort();
                        parse += string.Format("\r\nUnknown Count:{0}\r\n", count);
                        try
                        {
                            for (int i = 0; i < count; i++)
                            {
                                ulong actorID = p.GetULong();
                                ulong actorID2 = p.GetULong();
                                parse += string.Format("Unknown Obj:0x{0:X16} associated with 0x{1:X16}\r\n", actorID,actorID2);
                                p.GetBytes(7);
                            }
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show(ex.ToString(), "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }

                        count = p.GetShort();
                        parse += string.Format("\r\nCampfires Count:{0}\r\n", count);
                        try
                        {
                            for (int i = 0; i < count; i++)
                            {
                                ulong actorID = p.GetULong();
                                p.GetBytes(13);
                                short x = p.GetShort();
                                short y = p.GetShort();
                                short z = p.GetShort();
                                p.GetShort();
                                parse += string.Format("Campfires:0x{0:X16} X:{1}, Y:{2}, Z:{3}\r\n", actorID, x, y, z);
                            }
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show(ex.ToString(), "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }

                        if (length != p.Position - 6)
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
                    break;
            }
            return parse;
        }

        private string ParseServerActorAppearCBT3(Packet<int> p)
        {
            string parse = string.Empty;
            switch (p.ID)
            {
                case (int)GamePacketOpcode.SM_ACTOR_APPEAR_LIST:
                    {
                        parse += string.Format("MapInstanceID:{0}\r\n", p.GetUInt(6));
                        short count = p.GetShort(14);
                        parse += string.Format("Disappear Count:{0}\r\n", count);
                        for (int i = 0; i < count; i++)
                        {
                            ulong actorID = p.GetULong();
                            if (actorID > 0x4000000000000)
                            {
                                parse += string.Format("NPC ActorID:0x{0:X16}\r\n", actorID);
                            }
                            else
                            {
                                parse += string.Format("PC ActorID:0x{0:X16}\r\n", actorID);
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
                                p.GetBytes(93);
                                parse += string.Format("NPC ActorID:0x{0:X16} NPCID:{1}\r\n", actorID, npcID);
                            }
                            else
                            {
                                p.GetByte();
                                Race race = (Race)p.GetByte();
                                Gender gender = (Gender)p.GetByte();
                                Job job = (Job)p.GetByte();
                                p.GetBytes(92);
                                string name = Encoding.Unicode.GetString(p.GetBytes((ushort)((p.GetUShort()) * 2)));
                                parse += string.Format("PC ActorID:0x{0:X16} Race:{1} Gender:{2} Job:{3} Name:{4}\r\n", actorID, race, gender, job, name);
                                p.GetBytes(3);
                            }
                        }
                        count = p.GetShort();
                        for (int j = 0; j < count; j++)
                        {
                            ulong actorID = p.GetULong();
                            p.GetBytes(3);
                            string name = Encoding.Unicode.GetString(p.GetBytes((ushort)((p.GetUShort()) * 2)));
                            p.GetBytes(7);
                            parse += string.Format("Summon ActorID:0x{0:X16} Name:{1}", actorID, name);
                        }
                    }
                    break;
                case (int)GamePacketOpcode.SM_ACTOR_APPEAR_INFO_LIST:
                    {
                        parse += string.Format("MapInstanceID:{0}\r\n", p.GetUInt(6));
                        short count = p.GetShort(14);
                        parse += string.Format("Disappear Count:{0}\r\n", count);
                        for (int i = 0; i < count; i++)
                        {
                            parse += string.Format("ActorID:0x{0:X16} Option:{1}\r\n", p.GetULong(),p.GetByte());
                        }
                        count = p.GetShort();
                        parse += string.Format("Appear Count:{0}\r\n", count);
                        for (int i = 0; i < count; i++)
                        {
                            ulong actorID = p.GetULong();
                            p.Position++;
                            int appearEffect = p.GetInt();
                            p.Position += 4;
                            uint mapID = p.GetUInt();
                            short x = p.GetShort();
                            short y = p.GetShort();
                            short z = p.GetShort();
                            ushort dir = p.GetUShort();
                            byte level = p.GetByte();
                            p.GetInt();
                            int hp = p.GetInt();
                            p.Position += 16;
                            short mp = p.GetShort();
                            ManaType mana = (ManaType)p.GetInt();
                            p.GetBytes(76);
                            byte friendly = p.GetByte();
                            int maxHP = p.GetInt();
                            ushort maxMP = p.GetUShort();
                            p.GetBytes(35);
                            ushort count2 = p.GetUShort();
                            p.GetBytes((ushort)(count2 * 12));

                            string type;

                            if (actorID > 0x4000000000000 && actorID < 0x5000000000000)
                            {
                                type = "NPC";
                            }
                            else if (actorID > 0x2000000000000 && actorID < 0x3000000000000)
                            {
                                type = "Summon";
                            }
                            else if (actorID > 0x1000000000000 && actorID < 0x2000000000000)
                            {
                                type = "PC";
                            }
                            else
                            {
                                type = "Unknown";
                            }

                            parse += string.Format("{12} ActorID:0x{0:X16} MapID:{1} Friendly:{12:X} Pos:{2},{3},{4} Dir:{5} Level:{6} HP:{7}/{8} MP: {9}/{10} ManaType:{11} \r\n",
                                actorID, mapID, x, y, z, dir, level, hp, maxHP, mp, maxMP, mana, type, friendly);
                            /*
                            if (actorID > 0x4000000000000)
                            {
                                p.GetBytes(9);
                                int motion = p.GetInt();
                                p.GetBytes(24);
                                short count2 = p.GetShort();
                                string unknownToken = string.Empty;
                                for (int j = 0; j < count2; j++)
                                {
                                    unknownToken += string.Format(" UnknownToken{0}:{1},{2},{3},{4},{5}", j + 1, p.GetByte(), p.GetByte(), p.GetInt(), p.GetInt(), p.GetShort());
                                }
                                parse += string.Format("NPC ActorID:0x{0:X} AppearEffect:{13} MapID:{1} Pos:{2},{3},{4} Dir:{5} Level:{6} HP:{7}/{8} MP: {9}/{10} ManaType:{11} Motion:{12} {14}\r\n",
                                    actorID, mapID, x, y, z, dir, level, hp, maxHP, mp, maxMP, mana, motion, appearEffect, unknownToken);
                            }
                            else
                            {
                                p.GetBytes(37);
                                short count2 = p.GetShort();
                                string unknownToken = string.Empty;
                                for (int j = 0; j < count2; j++)
                                {
                                    unknownToken += string.Format(" UnknownToken{0}:{1},{2},{3},{4},{5}", j + 1, p.GetByte(), p.GetByte(), p.GetInt(), p.GetInt(), p.GetShort());
                                }
                                parse += string.Format("PC ActorID:0x{0:X} MapID:{1} Pos:{2},{3},{4} Dir:{5} Level:{6} HP:{7}/{8} MP: {9}/{10} ManaType:{11} {12}\r\n",
                                    actorID, mapID, x, y, z, dir, level, hp, maxHP, mp, maxMP, mana, unknownToken);
                            }*/
                        }
                        count = p.GetShort();
                        parse += string.Format("Unknown Object Count: {0}\r\n",count);
                        for (int i = 0; i < count; i++)
                        {
                            ulong actorID = p.GetULong();
                            ulong reference = p.GetULong();
                            string unk = Conversions.bytes2HexString(p.GetBytes(7));
                            parse += string.Format("ActorID:0x{0:X16} Referenced ActorID 0x{1:X16} Unknown:{2}\r\n",actorID,reference,unk);
                        }
                        if (count > 0)
                        {
                            MessageBox.Show("Unknown Type", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                    break;
            }
            return parse;
        }

        private string ParseServer06CBT3(Packet<int> p)
        {
            //return "";//4157
            string parse = string.Empty;
            uint length = p.GetUInt(2);
            ushort count = p.GetUShort();
            byte hidden = 0;
            parse += string.Format("Data Length: {0}\r\n", length);
            parse += string.Format("Number of Parameters: {0}\r\n", count);
            for (int i = 0; i < count; i++)
            {
                byte dataLen = p.GetByte();
                byte type = p.GetByte();
                if (Form1.updateSelected?.Contains(type) == false)
                {
                    p.Position += dataLen - 2;
                    hidden++;
                    continue;
                }
                switch (type)
                {
                    case 0x01:
                        {
                            ushort varNum = (ushort)(dataLen - 25);
                            ulong actorID = p.GetULong();
                            short x = p.GetShort();
                            short y = p.GetShort();
                            short z = p.GetShort();
                            ushort dir = p.GetUShort();
                            ushort speed = p.GetUShort();
                            byte unk1 = p.GetByte();
                            ushort unk2 = p.GetUShort();
                            ushort moveType = p.GetUShort();
                            string run;
                            if (moveType <= 1)
                            {
                                run = moveType == 1 ? "true" : "false";
                            }
                            else
                            {
                                run = moveType.ToString();
                            }

                            byte[] temp = p.GetBytes(varNum);
                            if (Form1.validActorIDs.Count > 0 && !Form1.validActorIDs.Contains(actorID))
                            {
                                hidden++;
                            }
                            else
                            {
                                parse += string.Format("Move 0x{0:X16},{1},{2},{3},{4},{5},{6},{7},{8},{9}\r\n", actorID, x, y, z, dir, speed, run, unk1, unk2, Conversions.bytes2HexString(temp));
                            }
                        }
                        break;
                    case 0x02:
                        {
                            ushort varNum = (ushort)(dataLen - 18);
                            ulong actorID = p.GetULong();
                            short x = p.GetShort();
                            short y = p.GetShort();
                            short z = p.GetShort();
                            ushort dir = p.GetUShort();
                            byte[] temp = p.GetBytes(varNum);
                            if (Form1.validActorIDs.Count > 0 && !Form1.validActorIDs.Contains(actorID))
                            {
                                hidden++;
                            }
                            else
                            {
                                parse += string.Format("0x02 0x{0:X16},{1},{2},{3},{4},{5}\r\n", actorID, x, y, z, dir, Conversions.bytes2HexString(temp));
                            }
                        }
                        break;
                    case 0x03:
                        {
                            ulong actorId1 = p.GetULong(); //Attacker
                            uint skillId = p.GetUInt(); //Skill being used
                            ushort unk1 = p.GetUShort();
                            ushort unk2 = p.GetUShort();
                            byte skillMode = p.GetByte();
                            ushort unk4 = p.GetUShort();
                            byte pLength = p.GetByte();
                            byte pType = p.GetByte();
                            ulong actorId2 = 0;
                            string temp;
                            switch (pType)
                            {
                                case 1:
                                    {
                                        actorId2 = p.GetULong(); //Being attacked
                                        temp = string.Format("0x{0:X16}", actorId2);
                                    }
                                    break;
                                case 2:
                                    {
                                        short x = p.GetShort();//x cord
                                        short y = p.GetShort();//y cord
                                        short z = p.GetShort();//z cord
                                        temp = string.Format("X:{0},Y:{1},Z:{2}", x, y, z);
                                    }
                                    break;
                                default:
                                    {
                                        temp = Conversions.bytes2HexString(p.GetBytes((ushort)(pLength - 2)));
                                    }
                                    break;
                            }
                            if (Form1.validActorIDs.Count > 0 && !(Form1.validActorIDs.Contains(actorId1) || actorId1 == 0) && !(Form1.validActorIDs.Contains(actorId2) || actorId2 == 0))
                            {
                                hidden++;
                            }
                            else if (Form1.foodSkill)
                            {
                                if (skillId > 7000000 && skillId < 8000000)
                                {
                                    parse += string.Format("Skill Caster:0x{0:X16},SkillID:{1},U1:{2},U2:{3},SkillMode:{4},{5},Length:{6},CastMode:{7},TargetContent:{8}\r\n",
                                        actorId1, skillId, unk1, unk2, skillMode, unk4, pLength, pType, temp);
                                }
                                else
                                {
                                    hidden++;
                                }
                            }
                            else
                            {
                                parse += string.Format("Skill Caster:0x{0:X16},SkillID:{1},U1:{2},U2:{3},SkillMode:{4},{5},Length:{6},CastMode:{7},TargetContent:{8}\r\n",
                                    actorId1, skillId, unk1, unk2, skillMode, unk4, pLength, pType, temp);
                            }
                        }
                        break;
                    case 0x04:
                        {
                            ulong actorID1 = p.GetULong();
                            ulong actorID2 = p.GetULong();
                            string temp = string.Format("Effect Caster:0x{0:X16},Effector:0x{1:X16},SkillID:{2},{3},{4},{5},{6}\r\n", actorID1, actorID2, p.GetUInt(), p.GetByte(), p.GetByte(), p.GetByte(), (SagaBNS.Common.Skills.SkillAttackResult)p.GetByte());
                            if (dataLen == 26)
                            {
                                if (Form1.validActorIDs.Count > 0 && !(Form1.validActorIDs.Contains(actorID1) || actorID1 == 0) && !(Form1.validActorIDs.Contains(actorID2) || actorID2 == 0))
                                {
                                    hidden++;
                                }
                                else
                                {
                                    parse += temp;
                                }
                            }
                            else
                            {
                                goto default;
                            }
                        }
                        break;
                    case 0x05:
                        {
                            Packet<SagaBNS.Common.Packets.GamePacketOpcode> pak = new Packet<SagaBNS.Common.Packets.GamePacketOpcode>();
                            pak.PutBytes(p.ToArray());
                            pak.Position = p.Position;
                            uint skillId = pak.GetUInt();
                            ushort unk1 = pak.GetUShort();
                            ulong actorId1 = pak.GetULong();
                            ulong actorId2 = pak.GetULong();
                            ushort unk2 = pak.GetUShort();
                            string temp = string.Empty;
                            ushort count1 = pak.GetUShort();
                            ushort dataLen2 = pak.GetUShort();
                            temp = string.Format(",{0},{1},", count1, dataLen2);
                            ushort dataAcc = 0;
                            SagaBNS.Common.Packets.GameServer.PacketParameter id = 0;
                            long offset = pak.Position;
                            ushort offset2 = (ushort)(dataAcc + 2);
                            for (int j = 0; j < count1; j++)
                            {
                                try
                                {
                                    dataAcc += 2;
                                    id = (SagaBNS.Common.Packets.GameServer.PacketParameter)pak.GetUShort();
                                    ActorUpdateParameter para = new ActorUpdateParameter(id);
                                    para.Read(pak);
                                    dataAcc += (ushort)id.GetLength();
                                    temp += string.Format("{0},{1}(0x{1:X}),", id, para.Value);
                                }
                                catch { }
                            }
                            int test = dataLen2 - dataAcc;
                            if (test != 0)
                            {
                                pak.Position = offset;
                                ushort id2 = pak.GetUShort();
                                ushort[] temp3 = new ushort[1] {id2};
                                temp = string.Format(",{2},trace into parameters(0x{0},{1}),",  Conversions.ushort2HexString(temp3), Conversions.bytes2HexString(pak.GetBytes((ushort)(dataLen2-offset2))),count1);
                            }
                            byte unk3 = pak.GetByte();
                            byte unk4 = pak.GetByte();
                            string temp2 = string.Empty;
                            if (unk3 > 2)
                            {
                                for (int j = 0; j < unk3 - 2; j++)
                                {
                                    temp2 += string.Format(",{0:X}", pak.GetByte());
                                }
                            }

                            if (temp2.StartsWith(","))
                            {
                                temp2 = temp2.Substring(1);
                            }

                            if (Form1.validActorIDs.Count > 0 && !(Form1.validActorIDs.Contains(actorId1) || actorId1 == 0) && !(Form1.validActorIDs.Contains(actorId2) || actorId2 == 0))
                            {
                                hidden++;
                            }
                            else
                            {
                                parse += string.Format("Actor SkillID:{0},{1},0x{2:X16},0x{3:X16},{4}{5}Rest:{6},Type:{7} Data:{8}\r\n",
                                skillId, unk1, actorId1, actorId2, unk2, temp, unk3, unk4, temp2);
                            }
                        }
                        p.Position += dataLen - 2;
                        break;
                    case 0x06:
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
                            if (Form1.validActorIDs.Count > 0 && !(Form1.validActorIDs.Contains(actorId1) || actorId1 == 0))
                            {
                                hidden++;
                            }
                            else
                            {
                                parse += string.Format("0x06 0x{0:X16},{1},{2},{3}\r\n", actorId1, pLength, pType, temp);
                            }
                        }
                        break;
                    case 0x07:
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
                            if (dataLen == 19)
                            {
                                parse += string.Format("0x08 0x{0:X16},0x{1:X16},{2}\r\n", p.GetULong(), p.GetULong(), p.GetByte());
                            }
                            else
                            {
                                parse += "Trace here ...";
                            }
                        }
                        break;
                    case 0x0A:
                        {
                            if (dataLen == 19)
                            {
                                parse += string.Format("MapObjectVisibilityChange 0x{0:X16},0x{1:X16},{2}\r\n", p.GetULong(), p.GetULong(), p.GetByte());
                            }
                            else
                            {
                                goto default;
                            }
                        }
                        break;
                    case 0x0C:
                        {
                            if (dataLen == 18)
                            {
                                parse += string.Format("0x0C 0x{0:X16},{1},{2},{3}\r\n", p.GetULong(), p.GetByte(), p.GetByte(), p.GetUInt(), p.GetUShort());
                            }
                            else
                            {
                                goto default;
                            }
                        }
                        break;
                    case 0x0D:
                        {
                            if (dataLen == 15)
                            {
                                parse += string.Format("0x0D 0x{0:X16},{1}\r\n", p.GetULong(), Conversions.bytes2HexString(p.GetBytes(5)));
                            }
                            else
                            {
                                goto default;
                            }
                        }
                        break;
                    //haven't added fixed length past this point could have unexpected results
                    case 0x0E:
                        {
                            ulong actorID = p.GetULong();
                            uint dialogID = p.GetUInt();
                            ushort unknown = p.GetUShort();
                            parse += string.Format("NPCChat 0x{0:X16},{1},{2}\r\n", actorID, dialogID, unknown);
                        }
                        break;
                    case 0x0F:
                        {
                            parse += string.Format("0x0F 0x{0:X16},{1}\r\n", p.GetULong(), p.GetUInt());
                        }
                        break;
                    case 0x11:
                        {
                            parse += string.Format("Interaction 0x{0:X16},0x{1:X16},{2}\r\n", p.GetULong(), p.GetULong(), Conversions.bytes2HexString(p.GetBytes(15)));
                        }
                        break;
                    case 0x12:
                        {
                            parse += string.Format("0x12 0x{0:X16},{1},{2},{3},{4},{5},{6},{7},{8}\r\n", p.GetULong(), p.GetUInt(), p.GetUShort(), p.GetUShort(), p.GetUShort(), p.GetByte(), p.GetUInt(),p.GetUShort(),p.GetUShort());
                        }
                        break;
                    case 0x13:
                        {
                            parse += string.Format("0x13 0x{0:X16},0x{1:X16},{2},{3},{4},{5},{6},{7}\r\n", p.GetULong(), p.GetULong(), p.GetInt(), p.GetShort(), p.GetShort(), p.GetShort(), p.GetUShort(),Conversions.bytes2HexString(p.GetBytes(4)));
                        }
                        break;
                    case 0x16:
                        {
                            parse += string.Format("0x16 0x{0:X16},0x{1:X16}\r\n", p.GetULong(), p.GetULong());
                        }
                        break;
                    case 0x17:
                        {
                            parse += string.Format("InteractionCancel 0x{0:X16},0x{1:X16},{2}\r\n", p.GetULong(), p.GetULong(), Conversions.bytes2HexString(p.GetBytes((ushort)(dataLen-18))));
                        }
                        break;
                    case 0x18:
                        {
                            parse += string.Format("0x18 0x{0:X16},0x{1:X16},{2}\r\n", p.GetULong(), p.GetULong(), Conversions.bytes2HexString(p.GetBytes((ushort)(dataLen-18))));
                        }
                        break;
                    case 0x19:
                        {
                            parse += string.Format("0x19 0x{0:X16},0x{1:X16},{2}\r\n", p.GetULong(), p.GetULong(), p.GetByte());
                        }
                        break;
                    case 0x1A:
                        {
                            parse += string.Format("0x1A 0x{0:X16},0x{1:X16},{2}\r\n", p.GetULong(), p.GetULong(), p.GetByte());
                        }
                        break;
                    case 0x1B:
                        {
                            parse += string.Format("0x1B 0x{0:X16},0x{1:X16},{2},{3},{4},{5}\r\n", p.GetULong(), p.GetULong(), p.GetShort(), p.GetShort(), p.GetShort(), p.GetByte());
                        }
                        break;
                    case 0x1C:
                        {
                            ushort varNum = (ushort)(dataLen - 38);
                            ulong actorId1 = p.GetULong();
                            ulong actorId2 = p.GetULong();
                            uint unk1 = p.GetUInt();
                            string unk2 = Conversions.bytes2HexString(p.GetBytes(10));
                            short x = p.GetShort();
                            short y = p.GetShort();
                            short z = p.GetShort();
                            parse += string.Format("0x1C 0x{0:X16},0x{1:X16},{2},0x{3:X10},{4},{5},{6},{7}\r\n",
                                actorId1, actorId2, unk1, unk2, x, y, z, Conversions.bytes2HexString(p.GetBytes(varNum)));
                        }
                        break;
                    case 0x1D:
                        {
                            parse += string.Format("0x1D 0x{0:X16}\r\n", p.GetULong());
                        }
                        break;
                    case 0x1E:
                        {
                            parse += string.Format("0x1E 0x{0:X16},{1},0x{2:X16}\r\n", p.GetULong(), p.GetUShort(), p.GetULong());
                        }
                        break;
                    case 0x1F:
                        {
                            parse += string.Format("0x1F 0x{0:X16},{1}\r\n", p.GetULong(), p.GetUInt());
                        }
                        break;
                    case 0x24:
                        {
                            parse += string.Format("0x24 0x{0:X16},0x{1:X16},{2}\r\n", p.GetULong(), p.GetULong(), p.GetByte());
                        }
                        break;
                    case 0x25:
                        {
                            parse += string.Format("0x25 0x{0:X16},{1}\r\n", p.GetULong(), p.GetByte());
                        }
                        break;
                    case 0x26:
                        {
                            parse += string.Format("0x26 0x{0:X16},0x{1:X16},{2}\r\n", p.GetULong(), p.GetULong(), p.GetByte());
                        }
                        break;
                    case 0x27:
                        {
                            parse += string.Format("0x27 0x{0:X16},0x{1:X16},{2},{3},{4}\r\n", p.GetULong(), p.GetULong(), p.GetShort(),p.GetInt(),p.GetByte());
                        }
                        break;
                    case 0x28:
                        {
                            parse += string.Format("0x28 0x{0:X16},{1}\r\n", p.GetULong(), p.GetByte());
                        }
                        break;
                    case 0x29:
                        {
                            ulong actorID = p.GetULong();
                            p.Position++;
                            short x = p.GetShort();
                            short y = p.GetShort();
                            short z = p.GetShort();
                            ushort dir = p.GetUShort();
                            p.Position += 1;
                            uint dashID = p.GetUInt();
                            ushort unknown = p.GetUShort();
                            parse += string.Format("Dash 0x{0:X16},{1},{2},{3},{4},{5},{6}\r\n", actorID, x, y, z, dir, dashID, unknown);
                        }
                        break;
                    case 0x2A:
                        {
                            ulong actorID = p.GetULong();
                            ulong MapObj = p.GetULong();
                            byte toggle = p.GetByte();
                            short x = p.GetShort();
                            short y = p.GetShort();
                            short z = p.GetShort();
                            ushort dir = p.GetUShort();
                            parse += string.Format("0x2A ActorId: 0x{0:X16} MapObj: 0x{1:X16} Toggle:{2} X:{3} Y:{4} Z:{5} Dir:{6}\r\n", actorID, MapObj, toggle, x, y, z, dir);
                        }
                        break;
                    case 0x2B:
                        {
                            parse += string.Format("0x2B 0x{0:X16},{1}\r\n", p.GetULong(), p.GetByte());
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
            if (numLines - 2 + hidden != count)
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

        private string ParseServer07CBT3(Packet<int> pak)
        {
            Packet<SagaBNS.Common.Packets.GamePacketOpcode> p = new Packet<SagaBNS.Common.Packets.GamePacketOpcode>();
            p.PutBytes(pak.ToArray());
            string parse = string.Empty;
            uint dataLen = p.GetUInt(2);
            byte unk1 = p.GetByte();
            ushort numSets = p.GetUShort();
            ushort numParams;
            ushort paramLen;
            ushort dataAcc;
            SagaBNS.Common.Packets.GameServer.PacketParameter id;
            string temp;
            for (int i = 0; i < numSets; i++)
            {
                numParams = p.GetUShort();
                paramLen = p.GetUShort();
                temp = string.Empty;
                dataAcc = 0;
                parse += string.Format("Set Number: {0}\r\n", i + 1);
                for (int j = 0; j < numParams; j++)
                {
                    try
                    {
                        dataAcc += 2;
                        id = (SagaBNS.Common.Packets.GameServer.PacketParameter)p.GetUShort();
                        ActorUpdateParameter para = new ActorUpdateParameter(id);
                        para.Read(p);
                        dataAcc += (ushort)id.GetLength();
                        temp += string.Format("Parameter: {0} = {1}\r\n", id, para.Value);
                    }
                    catch { j = 999; }
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

        private string ParseReceiveCBT3(Packet p)
        {
            string parse = string.Empty;
            if (false)//p.IsGameServer)
            {
                #region #GameServer
                switch (p.ID)
                {
                    case (int)GamePacketOpcode.SM_ACTOR_APPEAR_LIST:
                    case (int)GamePacketOpcode.SM_ACTOR_APPEAR_INFO_LIST:
                        {
                            parse = ParseServerActorAppearCBT3(p);
                        }
                        break;
                    case (int)GamePacketOpcode.SM_ACTOR_LIST:
                    case (int)GamePacketOpcode.SM_ACTOR_INFO_LIST:
                        {
                            parse = ParseServerActorListCBT3(p);
                        }
                        break;
                    case 0x05://remove objects
                        {
                            //Reply to Client 0x0091
                            uint dataLength = p.GetUInt(2);
                            ushort map = p.GetUShort();
                            uint unk1 = p.GetUInt(); //0x00
                            ushort unk2 = p.GetUShort();//16
                            ushort count = p.GetUShort();//NPC & Players
                            parse += string.Format("Map Id: {0}\r\n\r\nPlayers & NPC:\r\n", map);
                            for (int i = 0; i < count; i++)
                            {
                                parse += string.Format("{0}. 0x{1:X16}\r\n", i + 1, p.GetULong());
                            }
                            count = p.GetUShort();//Map Objects
                            parse += "\r\nMap Objects:\r\n";
                            for (int i = 0; i < count; i++)
                            {
                                parse += string.Format("{0}. 0x{1:X16}\r\n", i + 1, p.GetULong());
                            }
                            count = p.GetUShort();
                            parse += "\r\nUnknown Objects 1:\r\n";
                            for (int i = 0; i < count; i++)
                            {
                                parse += string.Format("{0}. 0x{1:X16}\r\n", i + 1, p.GetULong());
                            }
                            count = p.GetUShort();
                            parse += "\r\nUnknown Objects 2:\r\n";
                            for (int i = 0; i < count; i++)
                            {
                                parse += string.Format("{0}. 0x{1:X16}\r\n", i + 1, p.GetULong());
                            }

                            count = p.GetUShort();
                            parse += "\r\nUnknown Objects 3:\r\n";
                            for (int i = 0; i < count; i++)
                            {
                                parse += string.Format("{0}. 0x{1:X16}\r\n", i + 1, p.GetULong());
                            }
                            count = p.GetUShort();
                            parse += "\r\nCampfires:\r\n";
                            for (int i = 0; i < count; i++)
                            {
                                parse += string.Format("{0}. 0x{1:X16}\r\n", i + 1, p.GetULong());
                            }
                            if (dataLength != p.Position - 6)
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
                        break;
                    case (int)GamePacketOpcode.SM_ACTOR_UPDATE_LIST:
                        {
                            parse = ParseServer06CBT3(p);
                            break;
                        }
                    case (int)GamePacketOpcode.SM_PLAYER_UPDATE_LIST:
                        {
                            parse = ParseServer07CBT3(p);
                        }
                        break;
                    case (int)GamePacketOpcode.SM_LOGIN_INIT:
                        {
                            string temp1, temp2, temp3, temp4;
                            parse += string.Format("ActorID:0x{0:X}\r\n", p.GetULong(6));
                            parse += string.Format("Chat Server:{0}.{1}.{2}.{3}:{4}\r\n", p.GetByte(), p.GetByte(), p.GetByte(), p.GetByte(), p.GetUShort());
                            parse += string.Format("WorldID:{0}\r\n", p.GetUInt());
                            parse += string.Format("Race:{0} Gender:{1} Job:{2}\r\n", (Race)p.GetByte(), (Gender)p.GetByte(), (Job)p.GetByte());
                            parse += string.Format("Appearence:{0}\r\n", Conversions.bytes2HexString(p.GetBytes(92)));
                            parse += string.Format("Name:{0}\r\n", Encoding.Unicode.GetString(p.GetBytes((ushort)((p.GetUShort()) * 2))));
                            parse += string.Format("MapID:{0} X:{1} Y:{2} Z:{3} Dir:{4}\r\n", p.GetUInt(), p.GetShort(), p.GetShort(), p.GetShort(), p.GetUShort());
                            parse += string.Format("Level:{0} Exp:{1} Unknown:{2} HP:{3} Gold:{4}\r\n", p.GetByte(), p.GetInt(), p.GetInt(), p.GetInt(), p.GetInt());
                            parse += string.Format("Unknown1:{0}\r\n", Conversions.bytes2HexString(p.GetBytes(22)));
                            parse += string.Format("ManaType:{0}\r\n", (ManaType)p.GetInt());
                            parse += string.Format("Unknown2:{0}\r\n", Conversions.bytes2HexString(p.GetBytes(76)));
                            parse += string.Format("Inventory Slots:{0}\r\n", p.GetByte());
                            parse += string.Format("Bank Slots:{0}\r\n", p.GetByte());
                            parse += string.Format("Bank Costume Slots:{0}\r\n", p.GetByte());
                            parse += string.Format("Skilltree Points Available:{0} Used:{1}\r\n", p.GetByte(), p.GetByte());
                            parse += string.Format("U4:{0}\r\n", p.GetByte());
                            temp1 = Crafts(p.GetByte());
                            temp2 = Crafts(p.GetByte());
                            temp3 = Crafts(p.GetByte());
                            temp4 = Crafts(p.GetByte());
                            parse += string.Format("Craft Job:{0} Reputation:{1}\r\n", temp1, p.GetUShort());
                            parse += string.Format("Craft Job:{0} Reputation:{1}\r\n", temp2, p.GetUShort());
                            parse += string.Format("Craft Job:{0} Reputation:{1}\r\n", temp3, p.GetUShort());
                            parse += string.Format("Craft Job:{0} Reputation:{1}\r\n", temp4, p.GetUShort());
                            //parse += string.Format("Unknown5:{0}\r\n", Conversions.bytes2HexString(p.GetBytes(32)));
                            //parse += string.Format("Craft 3 Order:{0}\r\n", Global.LittleToBigEndian(p.GetULong()));
                            //parse += string.Format("Craft 4 Order:{0}\r\n", Global.LittleToBigEndian(p.GetULong()));
                            //parse += string.Format("Craft 1 Order:{0}\r\n", Global.LittleToBigEndian(p.GetULong()));
                            //parse += string.Format("Craft 2 Order:{0}\r\n", Global.LittleToBigEndian(p.GetULong()));
                            parse += string.Format("Surverys Completed:{0}\r\n", Conversions.bytes2HexString(p.GetBytes(p.GetUShort())));
                            parse += string.Format("Unk:{0}\r\n", Conversions.bytes2HexString(p.GetBytes(22)));
                            parse += string.Format("Max HP:{0} New Unk:{1} Max MP:{2}\r\n", p.GetInt(), p.GetInt(), p.GetShort());
                            parse += string.Format("Unknown5:{0}\r\n", Conversions.bytes2HexString(p.GetBytes(0x83)));
                            parse += string.Format("Weapon:{0} U6:{1} Costume:{2} U7:{3} Eyeware:{4} Hat:{5} Costume Accessory:{6} U8:{7}\r\n", p.GetUInt(), p.GetUInt(), p.GetUInt(), p.GetUInt(), p.GetUInt(), p.GetUInt(), p.GetUInt(), p.GetUInt());
                            parse += string.Format("New Unknown:{0}\r\n", p.GetUShort());
                            byte[] rijdealn = p.GetBytes(p.GetUShort());
                            ushort u6 = p.GetUShort();
                            if (u6 > 0)
                            {
                                parse += string.Format("Rijdaeln Key:{0} U9:{1} U10:{2} U11:{3}\r\n", Conversions.bytes2HexString(rijdealn), u6, p.GetInt(), p.GetByte());
                            }
                            else
                            {
                                parse += string.Format("Rijdaeln Key:{0} U9:{1}\r\n", Conversions.bytes2HexString(rijdealn), u6);
                            }

                            parse += string.Format("User Interface Info:{0}\r\n", Conversions.bytes2HexString(p.GetBytes(p.GetUShort())));
                            if (p.Position < p.Length)
                            {
                                parse += string.Format("U10:{0} U11:{1}", p.GetInt(), p.GetByte());
                            }
                        }
                        break;
                    case (int)GamePacketOpcode.SM_MAP_CHANGE_MAP:
                        {
                            parse = string.Format("MapId:{0}\r\nInstanceId:{1}", p.GetUInt(12), p.GetUInt(2));
                        }
                        break;
                    case (int)GamePacketOpcode.SM_SKILL_LOAD:
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
                    /*case (int)GamePacketOpcode.SM_LOGIN_AUCTION_START:
                        {
                            ushort unk1 = p.GetUShort(2);
                            parse += string.Format("LocationId:{0}\r\n", unk1);
                        }
                        break;*/
                    case (int)GamePacketOpcode.SM_ITEM_INFO:
                        {
                            uint length = p.GetUInt(2);
                            byte method = p.GetByte();
                            ushort count = p.GetUShort();
                            parse += string.Format("UpdateMethod:{0}\r\nCount:{1}\r\n\r\n", method, count);
                            for (int i = 0; i < count; i++)
                            {
                                byte len2 = p.GetByte();
                                byte itemType = p.GetByte();
                                string slotType = string.Empty;
                                switch (p.GetByte())
                                {
                                    case 1:
                                        slotType = "Equipted";
                                        break;
                                    case 2:
                                        slotType = "Inventory";
                                        break;
                                    case 3:
                                        slotType = "Wearhouse";
                                        break;
                                }
                                ushort slotId = p.GetUShort();
                                uint itemId = p.GetUInt();
                                string temp = string.Empty;
                                switch (itemType)
                                {
                                    case 2:
                                        {
                                            temp = string.Format("Durability:{0} Hidden:{1} Gem:{2} Gem:{3} Gem:{4} Gem:{5} Gem:{6}", p.GetByte(), p.GetByte() == 1 ? "True" : "False", p.GetUInt(), p.GetUInt(), p.GetUInt(), p.GetUInt(), p.GetUInt());
                                        }
                                        break;
                                    case 3:
                                    case 6:
                                        {
                                            temp = string.Format("Hidden:{0}", p.GetByte() == 1 ? "True" : "False");
                                        }
                                        break;
                                    case 4:
                                        {
                                            temp = string.Format("Item Count:{0} Hidden:{1}", p.GetUShort(), p.GetByte() == 1 ? "True" : "False");
                                        }
                                        break;
                                    case 5:
                                        {
                                            ushort value = p.GetUShort();
                                            temp = string.Format("Bonus Stat:{0} Hidden:{1}", value == 0 ? "None" : ((PacketParameter)value).ToString(), p.GetByte() == 1 ? "True" : "False");
                                        }
                                        break;
                                    default:
                                        p.Position += len2 - 9;
                                        break;
                                }
                                parse += string.Format("Item Type:{0} Inventory Location:{1} Slot Id:{2} Item Id:{3} {4}\r\n", itemType, slotType, slotId, itemId, temp);
                            }
                        }
                        break;
                    /*case (int)GamePacketOpcode.SM_SKILL_ADD:
                        {
                            uint skillId = p.GetUInt(2);
                            p.GetByte();
                            string skill = "SkillId Added: {0}\r\n";
                            parse += string.Format(skill, skillId);

                        }
                        break;*/
                    case (int)GamePacketOpcode.SM_QUEST_NEXT_QUEST:
                        {
                            parse += "QuestID:" + p.GetUShort();
                        }
                        break;
                    case (int)GamePacketOpcode.SM_QUEST_UPDATE:
                        {
                            parse += string.Format("QuestID:{0} Step:{1} NextStep:{3} StepStatus:{2},Flags:{4},{5},{6}", p.GetUShort(2), p.GetByte(), p.GetByte(), p.GetByte(), p.GetUShort(), p.GetUShort(), p.GetUShort());
                        }
                        break;
                    case (int)GamePacketOpcode.SM_QUEST_FINISH:
                        {
                            parse += string.Format("QuestID:{0} Step:{1} StepStatus:{2}", p.GetUShort(2), p.GetByte(), p.GetByte());
                        }
                        break;
                }

            #endregion
            }
            else if (p.IsLobbyGateway)
            {
                #region #LobbyServer
                switch (p.ID)
                {
                    case (int)LobbyPacketOpcode.SM_SERVER_LIST:
                        {
                            uint dataLength = p.GetUInt(2);
                            ushort count = p.GetUShort();
                            ulong allPlayerPossible = 0;
                            ulong allPlayerOnline = 0;
                            for (int i = 0; i < count; i++)
                            {
                                ushort id = p.GetUShort();
                                string name = Encoding.Unicode.GetString(p.GetBytes((ushort)(p.GetUShort() * 2)));
                                string unknownFlags1 = Conversions.bytes2HexString(p.GetBytes(2));
                                string ipPort = p.GetByte() + "." + p.GetByte() + "." + p.GetByte() + "." + p.GetByte() + ":" + p.GetUShort();
                                string unknownFlags2 = Conversions.bytes2HexString(p.GetBytes(11));
                                ushort maxPlayer = p.GetUShort();
                                allPlayerPossible += maxPlayer;
                                ushort currentPlayer = p.GetUShort();
                                allPlayerOnline += currentPlayer;
                                ushort maxWait = p.GetUShort();
                                ushort currentWait = p.GetUShort();
                                parse += string.Format("Server Name:{0} IP:{1} Players:{2}/{3} Wait:{4}/{5} Unknown1:{6:X} Unknown2:{7:X}\r\n",
                                    name, ipPort, currentPlayer, maxPlayer, currentWait, maxWait, unknownFlags1, unknownFlags2);
                            }
                            parse += string.Format("The servers support {0} players, there are currently {1} online.\r\n", allPlayerPossible, allPlayerOnline);
                        }
                        break;
                    case (int)LobbyPacketOpcode.SM_CHARACTER_LIST:
                            {
                                uint dataLength = p.GetUInt(2);
                                ushort count = p.GetUShort();
                                parse += string.Format("Data Length:{0}\r\nCharacter Count:{1}\r\n\r\n",dataLength,count);
                                for (int i = 0; i < count; i++)
                                {
                                    byte[] guid = p.GetBytes(16);
                                    ushort world = p.GetUShort();
                                    ushort nameLength = p.GetUShort();
                                    string name = Encoding.Unicode.GetString(p.GetBytes((ushort)(nameLength * 2)));
                                    ushort lengthCheckSum = p.GetUShort();
                                    ushort runningTotal = 0;
                                    p.GetUShort();//length checksum twice as ushort
                                    ushort appearence1Length = p.GetUShort();
                                    byte[] appearence1 = p.GetBytes(appearence1Length);
                                    runningTotal += (ushort)(4 + appearence1Length);
                                    string parameters = string.Empty;
                                    #region Parameters
                                    while (runningTotal != lengthCheckSum)
                                    {
                                        ushort id = p.GetUShort();
                                        runningTotal += 2;
                                        switch (id)
                                        {
                                            case 0:
                                                {
                                                    parameters += string.Format("Race:{0}\r\n", (Race)p.GetByte());
                                                    runningTotal += 1;
                                                }
                                                break;
                                            case 1:
                                                {
                                                    parameters += string.Format("Gender:{0}\r\n", (Gender)p.GetByte());
                                                    runningTotal += 1;
                                                }
                                                break;
                                            case 2:
                                                {
                                                    parameters += string.Format("Class:{0}\r\n", (Job)p.GetByte());
                                                    runningTotal += 1;
                                                }
                                                break;
                                            case 3:
                                                {
                                                    ushort appearence2Length = p.GetUShort();
                                                    parameters += string.Format("Appearence2:{0}\r\n", Conversions.bytes2HexString(p.GetBytes(appearence2Length)));
                                                    runningTotal += (ushort)(2 + appearence2Length);
                                                }
                                                break;
                                            case 4:
                                                {
                                                    parameters += string.Format("Name Null Terminated:{0}\r\n", Encoding.Unicode.GetString(p.GetBytes((ushort)((nameLength + 1) * 2))).Trim('\0'));
                                                    runningTotal += (ushort)((nameLength + 1) * 2);
                                                }
                                                break;
                                            case 5:
                                                {
                                                    parameters += string.Format("Unknown 0x05:{0}\r\n", p.GetUShort());
                                                    runningTotal += 2;
                                                }
                                                break;
                                            case 6:
                                                {
                                                    parameters += string.Format("Map ID:{0}\r\n", p.GetUInt());
                                                    runningTotal += 4;
                                                }
                                                break;
                                            case 7:
                                                {
                                                    parameters += string.Format("X:{0}\r\n", p.GetShort());
                                                    runningTotal += 2;
                                                }
                                                break;
                                            case 8:
                                                {
                                                    parameters += string.Format("Y:{0}\r\n", p.GetShort());
                                                    runningTotal += 2;
                                                }
                                                break;
                                            case 9:
                                                {
                                                    parameters += string.Format("Z:{0}\r\n", p.GetShort());
                                                    runningTotal += 2;
                                                }
                                                break;
                                            case 10:
                                                {
                                                    parameters += string.Format("Level:{0}\r\n", p.GetByte());
                                                    runningTotal += 1;
                                                }
                                                break;
                                            case 11:
                                                {
                                                    parameters += string.Format("Unknown 0x0B:{0}\r\n", p.GetUInt());
                                                    runningTotal += 4;
                                                }
                                                break;
                                            case 12:
                                                {
                                                    parameters += string.Format("HP:{0}\r\n", p.GetUInt());
                                                    runningTotal += 4;
                                                }
                                                break;
                                            case 13:
                                                {
                                                    parameters += string.Format("Unknown 0x0D:{0}\r\n", p.GetUShort());
                                                    runningTotal += 2;
                                                }
                                                break;
                                            case 14:
                                                {
                                                    parameters += string.Format("Gold:{0}\r\n", p.GetUInt());
                                                    runningTotal += 4;
                                                }
                                                break;
                                            case 15:
                                                {
                                                    parameters += string.Format("Weapon ID:{0}\r\n", p.GetUInt());
                                                    runningTotal += 4;
                                                }
                                                break;
                                            case 16:
                                                {
                                                    parameters += string.Format("Unknown 0x10:{0}\r\n", p.GetUInt());
                                                    runningTotal += 4;
                                                }
                                                break;
                                            case 17:
                                                {
                                                    parameters += string.Format("Costume ID:{0}\r\n", p.GetUInt());
                                                    runningTotal += 4;
                                                }
                                                break;
                                            case 18:
                                                {
                                                    parameters += string.Format("Unknown 0x12:{0}\r\n", p.GetUInt());
                                                    runningTotal += 4;
                                                }
                                                break;
                                            case 19:
                                                {
                                                    parameters += string.Format("Eye Accessory ID:{0}\r\n", p.GetUInt());
                                                    runningTotal += 4;
                                                }
                                                break;
                                            case 20:
                                                {
                                                    parameters += string.Format("Hat ID:{0}\r\n", p.GetUInt());
                                                    runningTotal += 4;
                                                }
                                                break;
                                            case 21:
                                                {
                                                    parameters += string.Format("Costume Accessory ID:{0}\r\n", p.GetUInt());
                                                    runningTotal += 4;
                                                }
                                                break;
                                        }
                                    }
                                    #endregion
                                    parse += string.Format("Character GUID:{0}\r\nWorld ID:{1}\r\nName:{2}\r\nAppearence1:{3}\r\n{4}Unknown Bytes:{5}\r\n\r\n",
                                        Conversions.bytes2HexString(guid),world,name,Conversions.bytes2HexString(appearence1),parameters,Conversions.bytes2HexString(p.GetBytes(13)));
                                }
                            }
                        break;
                }
                #endregion
            }
            return parse;
        }
    }
}
