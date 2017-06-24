using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Windows.Forms;
using SmartEngine.Network;
using SmartEngine.Network.Utils;
using System.Xml;
using SagaBNS.Common.Packets;

namespace PacketViewer
{
    public unsafe partial class PacketParser
    {
        public enum PacketVersion
        {
            CBT1,
            CBT2,
            CBT3,
            RetailV81,
        }

        private System.IO.FileStream fs;
        private string serverip;
        private string clientip;
        private System.IO.BinaryReader br;
        public List<Packet> Packets;
        public List<Packet> PacketsUnknown;
        public List<Packet> PacketsChat;
        public Dictionary<int, List<Packet>> PacketsByID;
        public Dictionary<int, List<Packet>> PacketsByIDAuction;
        public Dictionary<int, List<Packet>> PacketsByIDChat;
        private readonly Dictionary<PacketVersion, Dictionary<string, string>> keyDic = new Dictionary<PacketVersion, Dictionary<string, string>>();
        private readonly Dictionary<string, byte[]> buffers = new Dictionary<string, byte[]>();
        private ICryptoTransform dec;
        private readonly Rijndael aes = Rijndael.Create();
        private byte[] key1, key2, key3;
        private string exchangeVersion = string.Empty;

        public Encryption Crypt;

        public PacketVersion Version { get; set; }

        private Packet LastServer;

        private readonly Dictionary<int, Table.TableSchema> schemas = new Dictionary<int, Table.TableSchema>();

        public PacketParser()
        {
            Version = PacketVersion.CBT3;
            keyDic[PacketVersion.CBT1] = new Dictionary<string, string>();
            keyDic[PacketVersion.CBT2] = new Dictionary<string, string>();
            keyDic[PacketVersion.CBT3] = new Dictionary<string, string>();
        }

        private byte[] getBuffer(string ip)
        {
            if (buffers.TryGetValue(ip, out byte[] res))
            {
                return res;
            }
            else
            {
                return new byte[0];
            }
        }

        private void GetHeader(out long pos,out uint ori_Len, out ushort length,out string sip,out string cip,out ushort sPort,out ushort dPort)
        {
            fs.Position += 12;
            ori_Len = br.ReadUInt32();
            pos = br.BaseStream.Position;
            length = (ushort)(ori_Len - 54);
            fs.Position += 23;
            byte protocol = br.ReadByte();
            fs.Position += 2;
            sip = string.Format("{0:###}.{1:###}.{2:###}.{3:###}", br.ReadByte(), br.ReadByte(), br.ReadByte(), br.ReadByte());
            cip = string.Format("{0:###}.{1:###}.{2:###}.{3:###}", br.ReadByte(), br.ReadByte(), br.ReadByte(), br.ReadByte());
            sPort = (ushort)(br.ReadByte() << 8);
            sPort += br.ReadByte();
            dPort = (ushort)(br.ReadByte() << 8);
            dPort += br.ReadByte();
            if (protocol == 0x2f)
            {
                byte flag = (byte)(sPort & 0xFF);
                if (flag == 1)
                {
                    fs.Position += 21;
                    length -= 33;
                }
                else if (flag == 0x81)
                {
                    fs.Position += 25;
                    length -= 37;
                }
                else
                {

                }
                sip = string.Format("{0:###}.{1:###}.{2:###}.{3:###}", br.ReadByte(), br.ReadByte(), br.ReadByte(), br.ReadByte());
                cip = string.Format("{0:###}.{1:###}.{2:###}.{3:###}", br.ReadByte(), br.ReadByte(), br.ReadByte(), br.ReadByte());
                sPort = (ushort)(br.ReadByte() << 8);
                sPort += br.ReadByte();
                dPort = (ushort)(br.ReadByte() << 8);
                dPort += br.ReadByte();
            }
        }

        public void Open(string path)
        {
            int count = 0;
            bool fixedCBT2 = false;
            if (fs != null)
            {
                fs.Close();
                fs = null;
            }
            aes.Mode = CipherMode.ECB;
            key1 = null;
            key2 = null;

            if (keyDic[Version].Count == 0)
            {
                System.IO.StreamReader sr = new System.IO.StreamReader("keys.txt");
                while (!sr.EndOfStream)
                {
                    string line = sr.ReadLine();
                    string[] token = line.Split(',');
                    switch (token[0])
                    {
                        case "1":
                            if (Version == PacketVersion.CBT1)
                            {
                                keyDic[PacketVersion.CBT1].Add(token[1].ToUpper(), token[2].ToUpper());
                            }

                            break;
                        case "2":
                            if (Version == PacketVersion.CBT2)
                            {
                                keyDic[PacketVersion.CBT2].Add(token[1].ToUpper(), token[2].ToUpper());
                            }

                            break;
                        case "3":
                            if (Version == PacketVersion.CBT3)
                            {
                                keyDic[PacketVersion.CBT3].Add(token[1].ToUpper(), token[2].ToUpper());
                            }

                            break;
                    }
                }
                sr.Close();

                InitTblSchema();
            }

            fs = new System.IO.FileStream(path, System.IO.FileMode.Open);
            br = new System.IO.BinaryReader(fs);
            Packets = new List<Packet>();
            PacketsUnknown = new List<Packet>();
            PacketsChat = new List<Packet>();
            PacketsByID = new Dictionary<int, List<Packet>>();
            PacketsByIDAuction = new Dictionary<int, List<Packet>>();
            PacketsByIDChat = new Dictionary<int, List<Packet>>();
            byte[] buf = new byte[0];
            serverip = null;
            fs.Position += 24;
            ushort len = 0;
            int lastSeq = 0;
            while (fs.Position < fs.Length - 1)
            {
                try
                {
                    byte[] tmp;
                    byte[] tmp2;
                    long posBack = fs.Position;
                    GetHeader(out long pos, out uint ori_Len, out ushort length, out string sip, out string cip, out ushort sPort, out ushort dPort);
                    int seq = br.ReadInt32();
                    posBack = fs.Position;
                    bool valid = true;
                    {
                        fs.Position += 4;
                        try
                        {
                            bool hasOption2 = br.ReadByte() == 0x80;
                            if (hasOption2)
                            {
                                fs.Position += 19;
                            }
                            else
                            {
                                fs.Position += 7;
                            }

                            if (length > 12)
                            {
                                br.ReadBytes(length - 12);
                                GetHeader(out long tmpl, out uint lenl, out ushort tmpush, out string tmps, out tmps, out ushort tmpsh, out ushort tmpsh2);
                                valid = seq != br.ReadInt32();
                            }
                        }
                        catch
                        {
                        }
                        fs.Position = posBack;
                    }
                    if (serverip == null)
                    {
                        if (dPort == 7518 || dPort == 10241 || dPort == 11010 || dPort == 10900 || dPort == 10100)
                        {
                            serverip = sip;
                            clientip = cip;
                        }
                        else if (sPort == 7518 || sPort == 10241 || sPort == 11010 || sPort == 10900 || dPort == 10100)
                        {
                            serverip = cip;
                            clientip = sip;
                        }
                    }
                    buf = getBuffer(sip);
                    fs.Position += 4;
                    bool hasOption = br.ReadByte() == 0x80;
                    if (hasOption)
                    {
                        length -= 12;
                        fs.Position += 19;
                    }
                    else
                    {
                        fs.Position += 7;
                    }
                    //len = br.ReadUInt16();
                    //fs.Position -= 2;

                    if (buf == null || (buf.Length + length < len) && (buf.Length > 5000))
                    {
                        buf = new byte[0];
                    }
                    tmp = new byte[length];
                    tmp = br.ReadBytes(length);
                    tmp2 = new byte[buf.Length + length];
                    Array.Copy(buf, 0, tmp2, 0, buf.Length);
                    Array.Copy(tmp, 0, tmp2, buf.Length, tmp.Length);
                    buf = tmp2;

                    if ((sPort == 7518 || dPort == 7518 || sPort == 10100 || dPort == 10100 || sPort == 10900 || dPort ==10900) && valid)
                    {
                        #region GameServer
                        if ((buf[0] == 0x80 && buf[1] == 0x40 && buf.Length == 0x202 && Version == PacketVersion.CBT1)
                            || (buf[0] == 0x40 && buf[1] == 0x40 && buf.Length == 258 && Version >= PacketVersion.CBT2))
                        {
                            exchangeVersion = Conversions.bytes2HexString(buf);
                            key1 = null;
                            buffers.Clear();
                        }
                        else
                        {
                            if (key1 == null && buf[0] == 0x10 && buf[1] == 0x50 && buf.Length == 66)
                            {
                                string exchange = Conversions.bytes2HexString(buf);
                                if (!keyDic[Version].ContainsKey(exchange))
                                {
                                    KeyInput input = new KeyInput()
                                    {
                                        ExchangeKey = exchange,
                                        ExchangeVersion = exchangeVersion,
                                        ServerHost = serverip == sip ? cip + ":" + dPort : sip + ":" + sPort
                                    };
                                    input.ShowDialog(Form1.ActiveForm);
                                    key1 = Conversions.HexStr2Bytes(input.Key);
                                }
                                else
                                {
                                    key1 = Conversions.HexStr2Bytes(keyDic[Version][exchange]);
                                }
                            }
                            else
                            {
                                Packet tmpP = new Packet();
                                tmpP.PutBytes(buf);
                                int index = 0;
                                while (index < buf.Length)
                                {
                                    ushort size = tmpP.GetUShort((ushort)index);
                                    int type = size >> 12;
                                    size = (ushort)((size & 0xfff) << 2);
                                    if (size % 16 != 0)
                                    {
                                        index = buf.Length;
                                        break;
                                    }
                                    if ((size + index) > buf.Length)
                                    {
                                        int rest = buf.Length - index;
                                        if (sip == serverip)
                                        {
                                            byte[] sbuffer = new byte[rest];
                                            Array.Copy(buf, index, sbuffer, 0, rest);
                                            buffers[sip] = sbuffer;
                                        }
                                        else
                                        {
                                            byte[] cbuffer = new byte[rest];
                                            Array.Copy(buf, index, cbuffer, 0, rest);
                                            buffers[sip] = cbuffer;
                                        }
                                        break;
                                    }
                                    byte[] tmp4 = tmpP.GetBytes(size, (ushort)(index + 2));
                                    index += (size + 2);
                                    byte[] key = null;
                                    switch (type)
                                    {
                                        case 8:
                                            key = key1;
                                            break;
                                        case 0xc:
                                            key = key2;
                                            break;
                                        case 0xe:
                                            key = key3;
                                            break;
                                        default:
                                            {
                                                //MessageBox.Show(String.Format("Unknown type: {0}\nPacket:\n{1}", type, LastServer.DumpData()));
                                                break;
                                            }
                                    }
                                    if (key != null)
                                    {
                                        dec = aes.CreateDecryptor(key, new byte[16]);
                                        byte[] tmp3 = new byte[tmp4.Length + 16];
                                        tmp4.CopyTo(tmp3, 0);
                                        dec.TransformBlock(tmp3, 0, tmp3.Length, tmp3, 0);
                                        Array.Copy(tmp3, 0, tmp4, 0, tmp4.Length);
                                    }
                                    //if(key!=null)

                                    Packet p1 = new Packet();
                                    p1.PutBytes(tmp4, 0);
                                    size = p1.GetUShort(0);
                                    size -= 2;
                                    byte[] buffer = p1.GetBytes(size, 2);
                                    p1 = new Packet();
                                    p1.PutBytes(buffer, 0);
                                    p1.ServerIP = serverip;
                                    if (p1.ID == 0x18 && Version == PacketVersion.CBT1)
                                    {
                                        key2 = p1.GetBytes(16, 19);
                                    }
                                    if (p1.ID == 0x1A && Version == PacketVersion.CBT2)
                                    {
                                        key2 = p1.GetBytes(16, 18);
                                    }
                                    if (p1.ID == 0x21 && Version == PacketVersion.CBT3)
                                    {
                                        key2 = p1.GetBytes(16, 24);
                                    }

                                    if (p1.ID == 0x04 && Version == PacketVersion.CBT3)
                                    {
                                        key3 = p1.GetBytes(16, 12);
                                    }

                                    if (p1.ID == 0x11 && Version == PacketVersion.CBT2)
                                    {
                                        if (p1.GetInt((ushort)(p1.Length - 4)) != 1)
                                        {
                                            fixedCBT2 = true;
                                        }
                                        else
                                        {
                                            fixedCBT2 = false;
                                        }
                                    }
                                    if (fixedCBT2 && p1.ID >= 0xF0)
                                    {
                                        p1.ID += 2;
                                    }

                                    if (sip.Substring(0, 8) == serverip.Substring(0, 8))
                                    {
                                        p1.HasWrapper = false;//use HasWrapper to indicate server packet
                                    }
                                    else
                                    {
                                        p1.HasWrapper = true;
                                        LastServer = p1;
                                    }

                                    if (p1.ID < 0x300 && p1.Length < 8000)
                                    {
                                        bool isAuction = sPort == 10900 || dPort == 10900;
                                        if (isAuction)
                                        {
                                            PacketsUnknown.Add(p1);
                                            p1.IsLobbyGateway = true;
                                        }
                                        else
                                        {
                                            Packets.Add(p1);
                                            p1.IsGameServer = true;
                                        }
                                        Dictionary<int, List<Packet>> list = isAuction ? PacketsByIDAuction : PacketsByID;
                                        if (!list.ContainsKey(p1.ID))
                                        {
                                            List<Packet> ps = new List<Packet>();
                                            list.Add(p1.ID, ps);
                                        }
                                        list[p1.ID].Add(p1);
                                    }
                                    else
                                    {
                                    }
                                    //if (sip == serverip && p1.SessionID != 0 && p1.ID == 0x0205)
                                    if ((count % 100) == 0)
                                    {
                                        try
                                        {
                                            System.Windows.Forms.Control.ControlCollection aa = Form1.ActiveForm.Controls;
                                            System.Windows.Forms.StatusStrip ss = (StatusStrip)aa["statusStrip1"];
                                            System.Windows.Forms.ToolStripProgressBar pb = (ToolStripProgressBar)ss.Items["pb"];
                                            pb.Value = (int)(((float)fs.Position / fs.Length) * 100);
                                            Application.DoEvents();
                                        }
                                        catch (Exception ex)
                                        { Application.DoEvents(); }
                                    }
                                    count++;
                                }
                                if (index == buf.Length)
                                {
                                    buffers.Remove(sip);
                                }
                            }
                        }
                        #endregion
                    }
                    else if ((sPort == 10241 || dPort == 10241) && valid)
                    {
                        #region ChatServer
                        Packet<int> tmpP = new Packet<int>();
                        tmpP.PutBytes(buf);

                        ushort size = tmpP.GetUShort(0);
                        byte[] tmp4 = tmpP.GetBytes((ushort)(size - 2), 2);

                        Packet p1 = new Packet();
                        p1.PutBytes(tmp4, 0);
                        p1.ServerIP = serverip;
                        if (sip.Substring(0, 8) == serverip.Substring(0, 8))
                        {
                            p1.HasWrapper = false;//use HasWrapper to indicate server packet
                        }
                        else
                        {
                            p1.HasWrapper = true;
                        }

                        PacketsChat.Add(p1);
                        if (!PacketsByIDChat.ContainsKey(p1.ID))
                        {
                            List<Packet> ps = new List<Packet>();
                            PacketsByIDChat.Add(p1.ID, ps);
                        }
                        PacketsByIDChat[p1.ID].Add(p1);
                        //if (sip == serverip && p1.SessionID != 0 && p1.ID == 0x0205)
                        if ((count % 100) == 0)
                        {
                            try
                            {
                                System.Windows.Forms.Control.ControlCollection aa = Form1.ActiveForm.Controls;
                                System.Windows.Forms.StatusStrip ss = (StatusStrip)aa["statusStrip1"];
                                System.Windows.Forms.ToolStripProgressBar pb = (ToolStripProgressBar)ss.Items["pb"];
                                pb.Value = (int)(((float)fs.Position / fs.Length) * 100);
                                Application.DoEvents();
                            }
                            catch (Exception ex)
                            { Application.DoEvents(); }
                        }
                        count++;
                        #endregion
                    }
                    fs.Position = pos + ori_Len;
                }
                catch (Exception ex)
                {
                    if (MessageBox.Show(ex.ToString(), "Error", MessageBoxButtons.AbortRetryIgnore, MessageBoxIcon.Error) == DialogResult.Abort)
                    {
                        fs.Close();
                        return;
                    }
                }
            }
            fs.Close();
        }

        public static PacketParser Instance
        {
            get
            {
                return Nested.instance;
            }
        }

        private class Nested
        {
            // Explicit static constructor to tell C# compiler
            // not to mark type as beforefieldinit
            static Nested()
            {
            }

            internal static readonly PacketParser instance = new PacketParser();
        }

        private void InitTblSchema()
        {
            XmlDocument xml = new XmlDocument();

            XmlElement root;
            XmlNodeList list;
            xml.Load("Packets.xml");
            root = xml["CBT"];
            list = root.ChildNodes;

            PacketVersion Version  = PacketVersion.RetailV81;
            foreach (object j in list)
            {
                XmlElement i;
                if (j.GetType() != typeof(XmlElement))
                {
                    continue;
                }

                i = (XmlElement)j;
                switch (i.Name.ToLower())
                {
                    case "version":
                        Version = GetVersionType(i.InnerText);
                        break;
                    case "packet":
                        Table.TableSchema schema = new Table.TableSchema()
                        {
                            Version = Version
                        };
                        foreach (object l in i.ChildNodes)
                        {
                            XmlElement k;
                            if (l.GetType() != typeof(XmlElement))
                            {
                                continue;
                            }

                            k = (XmlElement)l;
                            switch (k.Name.ToLower())
                            {
                                case "id":
                                    schema.ID = loadID(Version,k.InnerText);//Convert.ToInt32(k.InnerText, 16);
                                    break;
                                case "elementname":
                                    schema.ElementName = k.InnerText;
                                    break;
                                case "field":
                                    Table.Field field = load(k);
                                    schema.Fields.Add(field.Name, field);
                                    break;
                            }
                        }
                        schemas.Add(schema.ID, schema);
                        break;
                }
            }
            xml = null;
        }

        private int loadID(PacketVersion version, string id)
        {
            int iD = 0;
            switch (version)
            {
                case PacketVersion.CBT3:
                    iD = (int)((GamePacketOpcodeCBT3)Enum.Parse(typeof(GamePacketOpcodeCBT3), id));
                    break;
            }
            return iD;
        }

        private Table.Field load(XmlElement k)
        {
            Table.Field field = new Table.Field()
            {
                Name = k.Attributes["name"].Value,
                Type = GetFieldType(k.Attributes["type"].Value)
            };
            if (k.HasAttribute("Position"))
            {
                field.Position = ushort.Parse(k.Attributes["Position"].Value);
            }

            if (k.HasAttribute("AddPosition"))
            {
                field.AddPosition = ushort.Parse(k.Attributes["AddPosition"].Value);
            }

            if (k.HasAttribute("AfterAddPosition"))
            {
                field.AfterAddPosition = ushort.Parse(k.Attributes["AfterAddPosition"].Value);
            }

            if (k.HasAttribute("actorIDMin"))
            {
                field.ActorIDMin = Convert.ToUInt64(k.Attributes["actorIDMin"].Value, 16);
            }

            if (k.HasAttribute("actorIDMax"))
            {
                field.ActorIDMax = Convert.ToUInt64(k.Attributes["actorIDMax"].Value, 16);
            }

            if (k.HasAttribute("RunMin"))
            {
                field.RunMin = Convert.ToUInt64(k.Attributes["RunMin"].Value, 10);
                field.RunMORM = true;
            }
            if (k.HasAttribute("RunMax"))
            {
                field.RunMax = Convert.ToUInt64(k.Attributes["RunMax"].Value, 10);
                field.RunMORM = true;
            }

            if (k.HasAttribute("length"))
            {
                field.Length = ushort.Parse(k.Attributes["length"].Value);
            }

            if (k.HasAttribute("discard"))
            {
                field.Discard = short.Parse(k.Attributes["discard"].Value);
            }

            if (k.HasAttribute("subtype"))
            {
                field.SubType = GetFieldType(k.Attributes["subtype"].Value);
            }
            if (k.HasAttribute("while"))
            {
                field.While = true;
            }

            if (k.HasAttribute("merge"))
            {
                field.Merge = true;
            }

            if (k.HasAttribute("switch"))
            {
                field.Switch = true;
            }

            if (k.HasAttribute("outText"))
            {
                field.OutText = k.Attributes["outText"].Value;
            }

            if (k.HasAttribute("tempText"))
            {
                field.OutTextToTemp = k.Attributes["tempText"].Value;
            }

            if (k.HasAttribute("inText"))
            {
                field.InText = k.Attributes["inText"].Value.Split(',');
            }

            if (k.HasAttribute("multi"))
            {
                XmlNodeList m = k.ChildNodes;
                field.List = new Dictionary<string, Table.Field>();
                foreach (object n in m)
                {
                    Table.Field field_t = load((XmlElement)n);
                    field.List.Add(field_t.Name, field_t);

                    field_t.Fu = field;
                }
            }
            else
            {
                field.text = k.InnerText;
            }
            return field;
        }

        private Table.FieldType GetFieldType(string name)
        {
            switch (name.ToLower())
            {
                case "long":
                    return Table.FieldType.Long;
                case "int":
                    return Table.FieldType.Integer;
                case "string":
                    return Table.FieldType.String;
                case "sizedstring":
                    return Table.FieldType.SizedString;
                case "array":
                    return Table.FieldType.Array;
                case "short":
                    return Table.FieldType.Short;
                case "byte":
                    return Table.FieldType.Byte;
                case "null":
                    return Table.FieldType.Null;
                case "uint":
                    return Table.FieldType.Uint;
                case "ulong":
                    return Table.FieldType.Ulong;
                case "ushort":
                    return Table.FieldType.Ushort;
                case "gushort":
                    return Table.FieldType.GUshort;
                case "bytes":
                    return Table.FieldType.Bytes;
                case "packet":
                    return Table.FieldType.Packet;
                case "surplus":
                    return Table.FieldType.Surplus;
                default:
                    throw new NotSupportedException();
            }
        }

        private PacketVersion GetVersionType(string version)
        {
            switch (version.ToLower())
            {
                case "cbt1":
                    return PacketVersion.CBT1;
                case "cbt2":
                    return PacketVersion.CBT2;
                case "cbt3":
                    return PacketVersion.CBT3;
                case "retailv81":
                    return PacketVersion.RetailV81;
                default:
                    throw new NotSupportedException();
            }
        }
    }
}
