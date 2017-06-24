using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using SmartEngine.Network;
using SmartEngine.Network.Utils;
namespace PacketViewer
{
    public unsafe partial class PacketParser
    {
        public string Parse(Packet p)
        {
            string parse = string.Empty;
            string name = string.Empty;
            if (p.IsGameServer)
            {
                name = Version >= PacketVersion.CBT2 ? ((SagaBNS.Common.Packets.GamePacketOpcode)p.ID).ToString() :
                    ((SagaBNS.Common.Packets.GamePacketOpcodeCBT1)p.ID).ToString();
            }
            else if (p.IsLobbyGateway)
            {
                name = ((SagaBNS.Common.Packets.LobbyPacketOpcode)p.ID).ToString();
            }
            else
            {
                name = $"0x{p.ID:X4}";
            }

            switch (p.HasWrapper)
            {
                case true:
                    string res="";
                    switch (Version)
                    {
                        case PacketVersion.CBT1:
                            res = ParseReceiveCBT1(p);
                            break;
                        case PacketVersion.CBT2:
                            res = ParseReceiveCBT2(p);
                            break;
                        case PacketVersion.CBT3:
                            if (p.IsGameServer && Form1.xmlMode && schemas.ContainsKey(p.ID))
                            {
                                res = ParseLoad(p);
                            }
                            else
                            {
                                res = ParseReceiveCBT3(p);
                            }

                            break;
                    }
                    parse += res;
                    break;
                case false:
                    {
                        switch (p.ID)
                        {
                            case 0x2C:
                                {
                                    parse += string.Format("Pos1:{0},{1},{2} Pos2:{3},{4},{5}", p.GetUShort(3), p.GetUShort(), p.GetUShort(), p.GetUShort(), p.GetUShort(), p.GetUShort());
                                }
                                break;
                            case 0x27:
                                {
                                    parse += string.Format("0x{0:X16}", p.GetULong(2));
                                }
                                break;
                            case 0x013C:
                                {
                                    parse += string.Format("FromSlot:{0} ToSlot:{1}", p.GetUShort(2), p.GetUShort(6));
                                }
                                break;
                            case 0x56:
                                {
                                    parse += string.Format("0x{0:X16}", p.GetULong(2));
                                }
                                break;
                            case 0x98:
                                {
                                    ushort dataLen = p.GetUShort(2);
                                    parse += string.Format("{0},{1}", dataLen, Conversions.bytes2HexString(p.GetBytes(dataLen)));
                                }
                                break;
                            case 0xEC:
                                {
                                    byte unk1 = p.GetByte(2);
                                    uint skillId = p.GetUInt();
                                    byte unk2 = p.GetByte();
                                    ulong actorId = p.GetULong();
                                    ushort dir = p.GetUShort();
                                    parse += string.Format("{0},{1},{2},0x{3:X16},{4}\r\n", unk1, skillId, unk2, actorId, dir);
                                }
                                break;
                            case 0x91:
                                {
                                    parse += "**Request Server 0x05**";
                                }
                                break;
                            case 0x108:
                                {
                                    ushort skillID = p.GetUShort(3);
                                    if (skillID == 12237)
                                    {
                                        MessageBox.Show("fa");
                                    }
                                    parse += "SkillID:" + skillID;
                                }
                                break;
                            case 0x146:
                                {
                                    ushort invSlot = p.GetUShort(2);
                                    ushort equipSlot = p.GetUShort();
                                    parse += "Inventory Slot:" + invSlot + "\r\n";
                                    parse += "Equipment Slot:" + equipSlot + "\r\n";
                                }
                                break;
                        }
                        break;
                    }
            }
            string tmp = "Sender:{0}\r\nOpcode:0x{1:X4}\r\nName:{2}\r\n\r\n{5}\r\n\r\nLength:{3}\r\nData:\r\n{4}";
            string tmp2 = p.DumpData();
            tmp = string.Format(tmp, !p.HasWrapper ? "Client" : "Server", p.ID, name, p.Length, tmp2, parse);
            return tmp;
        }

        public string ParseLoad(Packet p)
        {
            whil = string.Empty;
            foreach (Table.Field k in schemas[p.ID].Fields.Values)
            {
                try
                {
                    //for (int j = 0; k.WhileLength >= j; j++)
                    {
                        ParseLoadT(p, k);
                        if (k.Merge)
                        {
                            int index = Packets.IndexOf(p) + 1;
                            while (p.Length + 6 < long.Parse(k.Return))
                            {
                                Packet<int> tmp = Packets[index++];
                                p.PutBytes(tmp.GetBytes((ushort)(tmp.Length - 6), 6), (ushort)p.Length);
                            }
                        }
                        if (ulong.TryParse(k.Return, out ulong r) && k.RunMORM)
                        {
                            if (r >= k.RunMin && r >= k.RunMax)
                            {
                                return "";
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.ToString(), "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            return whil;
        }

        private string whil = string.Empty;

        public void ParseLoadT(Packet p, Table.Field k)
        {
            Packet temPacket = null;
            {
                if (k.AddPosition != 0)
                {
                    p.Position += k.AddPosition;
                }

                if (k.Position != 0)
                {
                    p.Position = k.Position;
                }

                switch (k.Type)
                {
                    #region #dataLoad
                    case Table.FieldType.Integer:
                        k.Return = p.GetInt().ToString();
                        break;
                    case Table.FieldType.Uint:
                        k.Return = p.GetUInt().ToString();
                        break;
                    case Table.FieldType.Short:
                        k.Return = p.GetShort().ToString();
                        break;
                    case Table.FieldType.Ushort:
                        k.Return = p.GetUShort().ToString();
                        break;
                    case Table.FieldType.GUshort:
                        k.Return = Global.LittleToBigEndian(p.GetUShort()).ToString();
                        break;
                    case Table.FieldType.Ulong:
                        k.Return = p.GetULong().ToString();
                        break;
                    case Table.FieldType.Long:
                        k.Return = p.GetLong().ToString();
                        break;
                    case Table.FieldType.Byte:
                        k.Return = p.GetByte().ToString();
                        break;
                    case Table.FieldType.Bytes:
                        k.Return = Conversions.bytes2HexString(p.GetBytes(k.Length));
                        break;
                    case Table.FieldType.String:
                        k.Return = Encoding.Unicode.GetString(p.GetBytes((ushort)(p.GetUShort() * 2)));
                        break;
                    case Table.FieldType.Packet:
                        temPacket = new Packet();
                        temPacket.PutBytes(p.GetBytes((ushort)(p.GetByte() - 1)), 2);
                        break;
                    case Table.FieldType.Surplus:
                        k.Return = Conversions.bytes2HexString(p.GetBytes((ushort)(p.Length - p.Position)));
                        break;
                    case Table.FieldType.Null:
                        break;
                    default:
                        MessageBox.Show("null",
                                            "null",
                                            MessageBoxButtons.OK,
                                            MessageBoxIcon.Question);
                                            break;
                        #endregion
                }
                if (k.AfterAddPosition != 0)
                {
                    p.Position += k.AfterAddPosition;
                }

                if (k.Discard != 0)
                {
                    p.Position += k.Discard * short.Parse(k.Return);
                }

                if (k.List != null)
                {
                    if (k.While)
                    {
                        for (int j = 0; ushort.Parse(k.Return) > j; j++)
                        {
                            foreach (KeyValuePair<string, Table.Field> m in k.List)
                            {
                                ulong i = 0;
                                //if (m.Value.Name == "actorID")
                                {
                                    ParseLoadT(p, m.Value);
                                    if (m.Value.Return == "0" || m.Value.Return == null)//(temPacket == null)
                                    {
                                        break;
                                    }

                                    i = Convert.ToUInt64(m.Value.Return,16);
                                    foreach (KeyValuePair<string, Table.Field> l in m.Value.List)
                                    {
                                        if (l.Value.ActorIDMin <= i && l.Value.ActorIDMax >= i)
                                        {
                                            ParseLoadT(p, l.Value);
                                        }
                                    }
                                }
                            }
                        }
                    }

                    if (k.Name == "pc" || (k.Fu?.Switch == true))
                    {
                        foreach (KeyValuePair<string, Table.Field> l in k.List)
                        {
                            ParseLoadT(p, l.Value);
                        }
                    }
                    if (k.Switch)
                    {
                        ParseLoadT(p, k.List[k.Return]);
                    }
                    if (temPacket != null && temPacket.Length != 0)
                    {
                        foreach (KeyValuePair<string, Table.Field> l in k.List)
                        {
                            ParseLoadT(temPacket, l.Value);
                        }
                    }
                }
            }
            string tem = k.ToString();
            if (tem != string.Empty)
            {
                whil += tem + "\r\n";//string.Format("{0}:{1}\r\n",k.Name, k.ToString());
            }
        }
    }
}
