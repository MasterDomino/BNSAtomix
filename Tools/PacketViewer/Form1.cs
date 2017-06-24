using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Security.Cryptography;
using System.Data;
using System.Text;
using System.Windows.Forms;
using System.Xml;
using System.Linq;
using System.Diagnostics;

using SmartEngine.Network;
using SmartEngine.Network.Utils;

namespace PacketViewer
{
    public partial class Form1 : Form
    {
        private List<Packet> currentList;
        public static Boolean hideMove;
        private int currentPos;
        public static bool xmlMode;
        public Form1()
        {
            InitializeComponent();
        }

        private void toolStripButton1_Click(object sender, EventArgs e)
        {
            OD.Filter = "*.pcap|*.pcap|*.dat|*.dat";
            listBox1.Items.Clear();
            OD.ShowDialog();
            if (OD.FileName == "")
            {
                return;
            }

            PacketParser.Instance.Open(OD.FileName);
            currentList = PacketParser.Instance.Packets;
            currentPos = 0;
            if (currentList?.Count > 0)
            {
                textBox1.Text = PacketParser.Instance.Parse(currentList[currentPos]);
                foreach (int i in PacketParser.Instance.PacketsByID.Keys)
                {
                    if (PacketParser.Instance.Version >= PacketParser.PacketVersion.CBT2)
                    {
                        listBox1.Items.Add(string.Format("0x{0:X4},{2},{1}", i, PacketParser.Instance.PacketsByID[i].Count, ((SagaBNS.Common.Packets.GamePacketOpcode)i)), true);
                    }
                    else
                    {
                        listBox1.Items.Add(string.Format("0x{0:X4},{2},{1}", i, PacketParser.Instance.PacketsByID[i].Count, ((SagaBNS.Common.Packets.GamePacketOpcodeCBT1)i)), true);
                    }
                }
            }
            tbNowPageIndex.Text = (currentPos + 1).ToString();
            label1.Text = "/" + currentList.Count.ToString();
            chkAll.Checked = true;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            List<Packet> tmp = new List<Packet>();
            if (radioButton1.Checked)
            {
                foreach (Packet p in PacketParser.Instance.PacketsByID[(ushort)Convert.ToInt16(listBox1.SelectedItem.ToString().Substring(2, 4), 16)])
                {
                    switch (!p.HasWrapper)
                    {
                        case true://client
                            if (checkBox1.Checked == true)
                            {
                                tmp.Add(p);
                            }

                            break;
                        case false://server
                            if (checkBox2.Checked == true)
                            {
                                tmp.Add(p);
                            }

                            break;
                    }
                }
            }
            if (radioButton3.Checked)
            {
                foreach (Packet p in PacketParser.Instance.PacketsByIDAuction[(ushort)Convert.ToInt16(listBox1.SelectedItem.ToString().Substring(2, 4), 16)])
                {
                    switch (!p.HasWrapper)
                    {
                        case true://client
                            if (checkBox1.Checked == true)
                            {
                                tmp.Add(p);
                            }

                            break;
                        case false://server
                            if (checkBox2.Checked == true)
                            {
                                tmp.Add(p);
                            }

                            break;
                    }
                }
            }
            currentList = tmp;
            currentPos = 0;
            textBox1.Text = PacketParser.Instance.Parse(currentList[currentPos]);
            tbNowPageIndex.Text = (currentPos + 1).ToString();
            label1.Text = "/" + currentList.Count.ToString();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            currentPos = PacketParser.Instance.Packets.IndexOf(currentList[currentPos]);
            currentList = PacketParser.Instance.Packets;
            textBox1.Text = PacketParser.Instance.Parse(currentList[currentPos]);
            tbNowPageIndex.Text = (currentPos + 1).ToString();
            label1.Text = "/" + currentList.Count.ToString();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            currentPos++;
            if (currentPos >= currentList.Count)
            {
                currentPos = currentList.Count - 1;
            }

            textBox1.Text = PacketParser.Instance.Parse(currentList[currentPos]);
            tbNowPageIndex.Text = (currentPos + 1).ToString();
            label1.Text = "/" + currentList.Count.ToString();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            currentPos += 10;
            if (currentPos >= currentList.Count)
            {
                currentPos = currentList.Count - 1;
            }

            textBox1.Text = PacketParser.Instance.Parse(currentList[currentPos]);
            tbNowPageIndex.Text = (currentPos + 1).ToString();
            label1.Text = "/" + currentList.Count.ToString();
        }

        private void button5_Click(object sender, EventArgs e)
        {
            currentPos--;
            if (currentPos < 0)
            {
                currentPos = 0;
            }

            textBox1.Text = PacketParser.Instance.Parse(currentList[currentPos]);
            tbNowPageIndex.Text = (currentPos + 1).ToString();
            label1.Text = "/" + currentList.Count.ToString();
        }

        private void button6_Click(object sender, EventArgs e)
        {
            currentPos -= 10;
            if (currentPos < 0)
            {
                currentPos = 0;
            }

            textBox1.Text = PacketParser.Instance.Parse(currentList[currentPos]);
            tbNowPageIndex.Text = (currentPos + 1).ToString();
            label1.Text = "/" + currentList.Count.ToString();
        }

        private void button7_Click(object sender, EventArgs e)
        {
            Dictionary<uint, Dictionary<ulong, npc>> npcs = new Dictionary<uint, Dictionary<ulong, npc>>();
            Dictionary<uint, Dictionary<ulong, npc>> mapObjects = new Dictionary<uint, Dictionary<ulong, npc>>();
            Dictionary<uint, uint> mapMapping = new Dictionary<uint, uint>();
            BuildNPCData(true, true, null, true);

            label2.Text = "Statistics: HasData";
        }

        private void button10_Click(object sender, EventArgs e)
        {
            SD.Filter = "*.txt|*.txt";
            if (SD.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                bool outputCombat = MessageBox.Show("Output Combat Information?", "Question", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == System.Windows.Forms.DialogResult.Yes;
                System.IO.StreamWriter sw = new System.IO.StreamWriter(SD.FileName, false, Encoding.Unicode);
                BuildNPCData(true, true, sw, true, outputCombat);
                sw.Flush();
                sw.Close();
            }
        }

        private void button11_Click(object sender, EventArgs e)
        {
            label2.Text = "Statistic: New";
        }

        private void button12_Click(object sender, EventArgs e)
        {
            BuildNPCData(true, true);

            if (!System.IO.Directory.Exists("Spawns"))
            {
                System.IO.Directory.CreateDirectory("Spawns");
            }

            List<string> already = new List<string>();
            foreach (uint i in statistic.spawns.Keys)
            {
                try
                {
                    System.IO.StreamWriter sw = new System.IO.StreamWriter("Spawns\\" + i + ".xml", false, Encoding.UTF8);
                    sw.WriteLine("<?xml version=\"1.0\" encoding=\"utf-8\"?>");
                    sw.WriteLine("<Spawns>");
                    foreach (npc j in statistic.spawns[i].OrderBy((n) => n.npcID))
                    {
                        if (j.npcID == 0 || j.x == 0)
                        {
                            continue;
                        }

                        string hash = string.Format("{0}|{1}|{2}|{3}|", j.npcID, j.x / 50, j.y / 50, j.z / 50);
                        if (already.Contains(hash))
                        {
                            continue;
                        }

                        already.Add(hash);
                        sw.WriteLine("  <Spawn>");
                        sw.WriteLine("    <NPCID>" + j.npcID + "</NPCID>");
                        sw.WriteLine("    <MapID>" + j.mapID + "</MapID>");
                        sw.WriteLine("    <X>" + j.x + "</X>");
                        sw.WriteLine("    <Y>" + j.y + "</Y>");
                        sw.WriteLine("    <Z>" + j.z + "</Z>");
                        sw.WriteLine("    <Dir>" + j.dir + "</Dir>");
                        sw.WriteLine("    <Motion>" + j.motion + "</Motion>");
                        //sw.WriteLine("    <ManaType>" + j.ManaType + "</ManaType>");
                        sw.WriteLine("  </Spawn>");
                    }
                    if (statistic.mapObjects.ContainsKey(i))
                    {
                        foreach (npc j in statistic.mapObjects[i].Values)
                        {
                            sw.WriteLine("  <MapObject>");
                            sw.WriteLine("    <ID>" + j.actorID + "</ID>");
                            sw.WriteLine("    <MapID>" + j.mapID + "</MapID>");
                            sw.WriteLine("  </MapObject>");
                        }
                    }
                    sw.WriteLine("</Spawns>");
                    sw.Flush();
                    sw.Close();
                }
                catch
                {

                }
            }
        }

        private void button13_Click(object sender, EventArgs e)
        {
            OD.Filter = "*.xml|*.xml";
            MessageBox.Show("Please select npc_templates.xml", "Please select", MessageBoxButtons.OK, MessageBoxIcon.Information);

            if (OD.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                XmlDocument xml = new XmlDocument();
                xml.Load(OD.FileName);
                XmlElement root = xml["npc"];
                XmlNodeList list = root.ChildNodes;
                Dictionary<string, XmlElement> Npcs = new Dictionary<string, XmlElement>();
                List<string> already = new List<string>();
                List<XmlElement> alreadyNode = new List<XmlElement>();
                foreach (object j in list)
                {
                    XmlElement i;
                    if (j.GetType() != typeof(XmlElement))
                    {
                        continue;
                    }

                    i = (XmlElement)j;
                    if (i.Name.ToLower() == "npc")
                    {
                        XmlNodeList children = i.ChildNodes;
                        string npcid = string.Empty;
                        foreach (object l in children)
                        {
                            XmlElement k = l as XmlElement;
                            if (k == null)
                            {
                                continue;
                            }

                            if (k.Name.ToLower() == "id")
                            {
                                npcid = k.InnerText;
                            }
                        }
                        Npcs[npcid] = i;
                    }
                }
                foreach (XmlElement i in alreadyNode)
                {
                    root.RemoveChild(i);
                }

                MessageBox.Show("Please select NPCData.xml", "Please select", MessageBoxButtons.OK, MessageBoxIcon.Information);

                if (OD.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    XmlDocument xml2 = new XmlDocument();
                    xml2.Load(OD.FileName);
                    XmlElement root2 = xml2["NPCData"];
                    foreach (object j in root2.ChildNodes)
                    {
                        XmlElement i;
                        if (j.GetType() != typeof(XmlElement))
                        {
                            continue;
                        }

                        i = (XmlElement)j;
                        if (i.Name.ToLower() == "npc")
                        {
                            XmlNodeList children = i.ChildNodes;
                            string name = string.Empty;
                            List<XmlElement> skills = new List<XmlElement>(), items = new List<XmlElement>();
                            foreach (object l in children)
                            {
                                XmlElement k = l as XmlElement;
                                if (k == null)
                                {
                                    continue;
                                }

                                switch (k.Name.ToLower())
                                {
                                    case "npcid":
                                        name = k.InnerText;
                                        break;
                                    case "skill":
                                        {
                                            XmlElement skill = xml.CreateElement("Skill");
                                            skill.InnerText = k.InnerText;
                                            skill.SetAttribute("rate", k.GetAttribute("rate"));
                                            skills.Add(skill);
                                        }
                                        break;
                                    case "itemdrop":
                                        {
                                            XmlElement item = xml.CreateElement("ItemDrop");
                                            item.InnerText = k.InnerText;
                                            item.SetAttribute("rate", k.GetAttribute("rate"));
                                            items.Add(item);
                                        }
                                        break;
                                }
                            }
                            if (Npcs.ContainsKey(name))
                            {
                                XmlNodeList reskill = Npcs[name].GetElementsByTagName("Skill");
                                XmlNodeList resitem = Npcs[name].GetElementsByTagName("ItemDrop");

                                while (reskill.Count > 0)
                                {
                                    Npcs[name].RemoveChild(reskill[0]);
                                }

                                while (resitem.Count > 0)
                                {
                                    Npcs[name].RemoveChild(resitem[0]);
                                }

                                foreach (XmlElement l in skills)
                                {
                                    Npcs[name].AppendChild(l);
                                }

                                foreach (XmlElement l in items)
                                {
                                    Npcs[name].AppendChild(l);
                                }
                            }
                        }
                    }
                    string path = System.IO.Path.GetDirectoryName(OD.FileName) + "\\skill_templates.xml";
                    xml.Save(path);
                }
            }
        }

        private void button17_Click(object sender, EventArgs e)
        {
            if (!System.IO.Directory.Exists("NPC"))
            {
                System.IO.Directory.CreateDirectory("NPC");
            }

            System.IO.StreamWriter sw = new System.IO.StreamWriter("NPC\\NPCData.xml", false, Encoding.UTF8);
            sw.WriteLine("<?xml version=\"1.0\" encoding=\"utf-8\"?>");
            sw.WriteLine("<NPCData>");
            foreach (ushort i in statistic.npcs.Keys)
            {
                sw.WriteLine("  <NPC>");
                sw.WriteLine("    <NPCID>" + i + "</NPCID>");
                int count = 0;
                foreach (int j in statistic.npcs[i].skills.Values)
                {
                    count += j;
                }
                foreach (uint j in statistic.npcs[i].skills.Keys)
                {
                    int rate = (int)((float)statistic.npcs[i].skills[j] * 100 / count);
                    if (rate == 0)
                    {
                        rate = 1;
                    }

                    sw.WriteLine(string.Format("    <Skill rate=\"{1}\">{0}</Skill>", j, rate));
                }
                foreach (uint j in statistic.npcs[i].items.Keys)
                {
                    int rate = (int)((float)statistic.npcs[i].items[j] * 100 / statistic.npcs[i].corpseOpenCount);
                    if (rate == 0)
                    {
                        rate = 1;
                    }

                    sw.WriteLine(string.Format("    <ItemDrop rate=\"{1}\">{0}</ItemDrop>", j, rate));
                }
                if (statistic.npcs[i].minGold > 0 || statistic.npcs[i].maxGold > 0)
                {
                    sw.WriteLine(string.Format("    <GoldDrop min=\"{0}\" max=\"{1}\"/>", statistic.npcs[i].minGold, statistic.npcs[i].maxGold));
                }
                sw.WriteLine("  </NPC>");
            }

            sw.WriteLine("</NPCData>");
            sw.Flush();
            sw.Close();
        }

        public class npc
        {
            public ulong actorID;
            public uint mapID;
            public ushort npcID;
            public ushort dir, motion;
            public int ManaType;
            public short x, y, z;

            public Dictionary<uint, int> skills = new Dictionary<uint, int>();
            public Dictionary<uint, int> items = new Dictionary<uint, int>();
            public int corpseOpenCount, minGold, maxGold;
        }

        public class Statistics
        {
            public Dictionary<ushort, npc> npcs = new Dictionary<ushort, npc>();
            public Dictionary<uint, List<npc>> spawns = new Dictionary<uint, List<npc>>();
            public Dictionary<uint, Dictionary<ulong, npc>> mapObjects = new Dictionary<uint, Dictionary<ulong, npc>>();
        }

        private readonly Statistics statistic = new Statistics();
        private void BuildNPCData(bool initial, bool later, System.IO.StreamWriter sw = null, bool makeStatistic = false, bool outputCombat = true)
        {
            switch (PacketParser.Instance.Version)
            {
                case PacketParser.PacketVersion.CBT2:
                    BuildNPCDataSubCBT2(initial, later, sw, makeStatistic, outputCombat);
                    break;
                case PacketParser.PacketVersion.CBT3:
                    BuildNPCDataSubRetail(initial, later, sw, makeStatistic, outputCombat);
                    break;
            }
        }

        private void BuildNPCDataSubRetail(bool initial, bool later, System.IO.StreamWriter sw = null, bool makeStatistic = false, bool outputCombat = true)
        {
            Dictionary<uint, Dictionary<string, int>> skill_list = new Dictionary<uint, Dictionary<string, int>>();
            //Dictionary<ulong, ushort> npcIDs = new Dictionary<ulong, ushort>();
            Dictionary<ulong, npc> npcs = new Dictionary<ulong, npc>();
            Dictionary<ulong, npc> corpses = new Dictionary<ulong, npc>();
            Packet tmpIDs = null, tmpActors = null;
            List<ulong> relatedActors = new List<ulong>();
            uint currentMapID = 0, currentInstanceID;
            ulong myActorID = 0;
            int counter = 0;
            foreach (Packet ids in PacketParser.Instance.Packets)
            {
                int perc = (int)((float)(counter++) * 100 / PacketParser.Instance.Packets.Count);
                if (pb.Value != perc)
                {
                    pb.Value = perc;
                    Application.DoEvents();
                }
                switch (ids.ID)
                {
                    case 0x18:
                        {
                            myActorID = ids.GetULong(6);
                        }
                        break;
                    case 0x21:
                        {
                            currentInstanceID = ids.GetUInt(2);
                            currentMapID = ids.GetUInt(12);
                            if (!statistic.spawns.ContainsKey(currentMapID))
                            {
                                statistic.spawns.Add(currentMapID, new List<npc>());
                            }

                            if (!statistic.mapObjects.ContainsKey(currentMapID))
                            {
                                statistic.mapObjects.Add(currentMapID, new Dictionary<ulong, npc>());
                            }

                            sw?.WriteLine("ChangeMap Map:{0} {1},{2},{3} Dir:{4}", currentMapID, ids.GetShort(40), ids.GetShort(), ids.GetShort(), ids.GetUShort());
                        }
                        break;
                    case 3:
                        #region 3
                        {
                            if (initial)
                            {
                                try
                                {
                                    Packet<int> cur = ids;
                                    if (tmpIDs != null)
                                    {
                                        cur = tmpIDs;
                                        tmpIDs = null;
                                        cur.Position = cur.Length;
                                        cur.PutBytes(ids.GetBytes((ushort)(ids.Length - 6), 6));
                                    }
                                    uint length = cur.GetUInt(2);
                                    if (length == uint.MaxValue)
                                    {
                                        continue;
                                    }

                                    if (length > (cur.Length - 6))
                                    {
                                        tmpIDs = new Packet();
                                        tmpIDs.PutBytes(cur.ToArray(), 0);
                                        continue;
                                    }
                                    uint mapID = cur.GetUInt();
                                    //if (!npcs.ContainsKey(mapID))
                                    //    npcs.Add(mapID, new Dictionary<ulong, npc>());
                                    ushort count = cur.GetUShort(15);
                                    for (int i = 0; i < count; i++)
                                    {
                                        ulong id = cur.GetULong();
                                        if ((id & 0x4000000000000) != 0)
                                        {
                                            npc npc = new Form1.npc()
                                            {
                                                actorID = id
                                            };
                                            cur.Position += 5;
                                            npc.npcID = (ushort)(cur.GetByte() << 8 | cur.GetByte());
                                            npcs[id] = npc;
                                            cur.Position += 93;
                                        }
                                        else
                                        {
                                            cur.Position += 95;
                                            ushort size = cur.GetUShort();
                                            cur.Position += ((size) * 2) + 3;
                                        }
                                    }
                                }
                                catch
                                {
                                }
                            }
                        }
                        #endregion
                        break;
                    case 4:
                        #region 4
                        {
                            if (initial)
                            {
                                try
                                {
                                    Packet cur = ids;
                                    if (PacketParser.Instance.Packets.IndexOf(cur) == 65126)
                                    {
                                    }
                                    if (tmpActors != null)
                                    {
                                        cur = tmpActors;
                                        tmpActors = null;
                                        cur.Position = cur.Length;
                                        cur.PutBytes(ids.GetBytes((ushort)(ids.Length - 6), 6));
                                    }
                                    uint length = cur.GetUInt(2);
                                    if (length > (cur.Length - 6) && length != 0xFFFFFFFF)
                                    {
                                        tmpActors = new Packet();
                                        tmpActors.PutBytes(cur.ToArray(), 0);
                                        continue;
                                    }
                                    uint mapID = cur.GetUInt();

                                    ushort count = cur.GetUShort(14);
                                    for (int i = 0; i < count; i++)
                                    {
                                        ulong id = cur.GetULong();
                                        if (npcs.ContainsKey(id))
                                        {
                                            npc npc = npcs[id];
                                            npc.mapID = cur.GetUInt();
                                            statistic.spawns[currentMapID].Add(npc);
                                            npc.x = cur.GetShort();
                                            npc.y = cur.GetShort();
                                            npc.z = cur.GetShort();
                                            npc.dir = cur.GetUShort();
                                            cur.Position += 27;
                                            npc.ManaType = cur.GetInt();
                                            cur.Position += 90;
                                            npc.motion = cur.GetUShort();
                                            cur.Position += 26;
                                            ushort unknown = cur.GetUShort();
                                            for (int j = 0; j < unknown; j++)
                                            {
                                                cur.Position += 12;
                                            }
                                        }
                                        else
                                        {
                                            cur.Position += 161;
                                            ushort unknown = cur.GetUShort();
                                            for (int j = 0; j < unknown; j++)
                                            {
                                                cur.Position += 12;
                                            }
                                        }
                                    }
                                    count = cur.GetUShort();
                                    for (int i = 0; i < count; i++)
                                    {
                                        npc mapObj = new npc()
                                        {
                                            actorID = cur.GetUInt()
                                        };
                                        if (mapObj.actorID > 0x1000)
                                        {
                                        }
                                        mapObj.mapID = currentMapID;
                                        statistic.mapObjects[currentMapID][mapObj.actorID] = mapObj;
                                        if (PacketParser.Instance.Version == PacketParser.PacketVersion.CBT1)
                                        {
                                            cur.Position += 10;
                                        }
                                        else
                                        {
                                            cur.Position += 13;
                                        }
                                    }
                                }
                                catch
                                {
                                }
                            }
                        }
                        #endregion
                        break;
                    case 1:
                        #region 1
                        {
                            if (later)
                            {
                                try
                                {
                                    Packet<int> cur = ids;
                                    if (tmpIDs != null)
                                    {
                                        cur = tmpIDs;
                                        tmpIDs = null;
                                        cur.Position = cur.Length;
                                        cur.PutBytes(ids.GetBytes((ushort)(ids.Length - 6), 6));
                                    }
                                    uint length = cur.GetUInt(2);
                                    if (length > (cur.Length - 6))
                                    {
                                        tmpIDs = new Packet();
                                        tmpIDs.PutBytes(cur.ToArray(), 0);
                                        continue;
                                    }
                                    uint mapID = cur.GetUInt();
                                    ushort count = cur.GetUShort(14);
                                    for (int i = 0; i < count; i++)
                                    {
                                        ulong id = cur.GetULong();
                                        if (relatedActors.Contains(id))
                                        {
                                            relatedActors.Remove(id);
                                        }

                                        cur.Position += 9;
                                    }
                                    count = cur.GetUShort();
                                    for (int i = 0; i < count; i++)
                                    {
                                        ulong id = cur.GetULong();
                                        if ((id & 0x4000000000000) != 0)
                                        {
                                            npc npc = new Form1.npc()
                                            {
                                                actorID = id
                                            };
                                            cur.Position += 6;
                                            npc.npcID = (ushort)(cur.GetByte() << 8 | cur.GetByte());
                                            npcs[id] = npc;
                                            cur.Position += 93;
                                        }
                                        else
                                        {
                                            cur.Position += 96;

                                            ushort size = cur.GetUShort();
                                            cur.Position += ((size + 1) * 2) + 3;
                                        }
                                    }
                                }
                                catch { }
                            }
                        }
                        #endregion
                        break;
                    case 2:
                        #region 2
                        {
                            if (later)
                            {
                                try
                                {
                                    Packet<int> cur = ids;
                                    if (tmpActors != null)
                                    {
                                        cur = tmpActors;
                                        tmpActors = null;
                                        cur.Position = cur.Length;
                                        cur.PutBytes(ids.GetBytes((ushort)(ids.Length - 6), 6));
                                    }
                                    uint length = cur.GetUInt(2);
                                    if (length > (cur.Length - 6))
                                    {
                                        tmpActors = new Packet();
                                        tmpActors.PutBytes(cur.ToArray(), 0);
                                        continue;
                                    }
                                    uint mapID = cur.GetUInt();
                                    ushort count = cur.GetUShort(14);
                                    for (int i = 0; i < count; i++)
                                    {
                                        cur.Position += 9;
                                    }
                                    count = cur.GetUShort();
                                    for (int i = 0; i < count; i++)
                                    {
                                        ulong id = cur.GetULong();
                                        if (npcs.ContainsKey(id))
                                        {
                                            npc npc = npcs[id];
                                            if (npc.x != 0 && npc.y != 0)
                                            {
                                                npc = new Form1.npc();
                                            }

                                            cur.Position += 9;
                                            npc.mapID = cur.GetUInt();
                                            statistic.spawns[currentMapID].Add(npc);
                                            npc.x = cur.GetShort();
                                            npc.y = cur.GetShort();
                                            npc.z = cur.GetShort();
                                            npc.dir = cur.GetUShort();
                                            cur.Position += 27;
                                            npc.ManaType = cur.GetInt();
                                            cur.Position += 90;
                                            npc.motion = cur.GetUShort();
                                            cur.Position += 26;
                                            ushort unknown = cur.GetUShort();
                                            if (unknown != 0)
                                            {
                                                cur.Position += 12;
                                            }
                                        }
                                        else
                                        {
                                            cur.Position += 184;
                                            ushort unknown = cur.GetUShort();
                                            if (unknown != 0)
                                            {
                                                cur.Position += 12;
                                            }
                                        }
                                    }
                                }
                                catch
                                {
                                }
                            }
                        }
                        #endregion
                        break;
                    /* case 6:
                         #region 6
                         {
                             if (!makeStatistic)
                                 continue;
                             try
                             {
                                 uint timeStamp = ids.GetUInt(6);
                                 ushort count = ids.GetUShort(14);
                                 for (int j = 0; j < count; j++)
                                 {

                                     long pos = ids.Position;
                                     byte len = ids.GetByte();
                                     byte type = ids.GetByte();
                                     switch (type)
                                     {
                                         case 2:
                                             #region 2
                                             {
                                                 ulong actorID = ids.GetULong();
                                                 uint instanceID = (uint)((actorID >> 32) & 0xFFFF);

                                                 uint skillID = ids.GetUInt();
                                                 byte castMode = ids.GetByte();
                                                 byte session = ids.GetByte();
                                                 ids.Position += 2;
                                                 byte mode = ids.GetByte();
                                                 if (mode == 4 || mode == 1 || mode == 0)
                                                 {
                                                     if ((actorID & 0x4000000000000) != 0)
                                                     {
                                                         if (npcs.ContainsKey(actorID))
                                                         {
                                                             npc npc = npcs[actorID];
                                                             if (!statistic.npcs.ContainsKey(npc.npcID))
                                                                 statistic.npcs.Add(npc.npcID, new npc());
                                                             if (!statistic.npcs[npc.npcID].skills.ContainsKey(skillID))
                                                                 statistic.npcs[npc.npcID].skills[skillID] = 1;
                                                             else
                                                                 statistic.npcs[npc.npcID].skills[skillID]++;
                                                         }
                                                     }
                                                     if (sw != null && outputCombat)
                                                     {
                                                         int delay = ids.GetShort() * 100;
                                                         ids.Position++;
                                                         byte type2 = ids.GetByte();
                                                         string modeString = string.Empty;
                                                         switch (mode)
                                                         {
                                                             case 0:
                                                                 modeString = "Casting";
                                                                 break;
                                                             case 1:
                                                                 modeString = "Delay";
                                                                 break;
                                                             case 4:
                                                                 modeString = "Activate";
                                                                 break;
                                                         }
                                                         if (type2 == 1)
                                                         {
                                                             ulong targetID = ids.GetULong();
                                                             if (actorID == myActorID && !relatedActors.Contains(targetID))
                                                                 relatedActors.Add(targetID);
                                                             if (targetID == myActorID && !relatedActors.Contains(actorID))
                                                                 relatedActors.Add(actorID);
                                                             if (actorID == myActorID)
                                                             {
                                                                 string targetString = string.Empty;
                                                                 if (targetID == myActorID || relatedActors.Contains(targetID))
                                                                 {
                                                                     if (targetID == myActorID)
                                                                         targetString = "Player";
                                                                     else
                                                                     {
                                                                         if (npcs.ContainsKey(targetID))
                                                                             targetString = "NPC(" + npcs[targetID].npcID + ")";
                                                                         else
                                                                             targetString = string.Format("Unknown(0x{0:X})", targetID);
                                                                     }
                                                                     sw.WriteLine(string.Format("SkillCastInfo Caster:Player SkillID:{0} Mode:{1} Delay:{2} Session:{3} Target:{4}", skillID, modeString, delay, session, targetString));
                                                                 }
                                                             }
                                                             if (targetID == myActorID)
                                                             {
                                                                 string targetString = string.Empty;
                                                                 if (actorID == myActorID || relatedActors.Contains(actorID))
                                                                 {
                                                                     if (actorID == myActorID)
                                                                         targetString = "Player";
                                                                     else
                                                                     {
                                                                         if (npcs.ContainsKey(actorID))
                                                                             targetString = "NPC(" + npcs[actorID].npcID + ")";
                                                                         else
                                                                             targetString = string.Format("Unknown(0x{0:X})", actorID);
                                                                     }
                                                                     sw.WriteLine(string.Format("SkillCastInfo Caster:{4} SkillID:{0} Mode:{1} Delay:{2} Session:{3} Target:Player", skillID, modeString, delay, session, targetString));
                                                                 }
                                                             }
                                                         }
                                                         else
                                                         {
                                                             if (actorID == myActorID || relatedActors.Contains(actorID))
                                                             {
                                                                 short x = ids.GetShort();
                                                                 short y = ids.GetShort();
                                                                 short z = ids.GetShort();
                                                                 string targetString = string.Empty;
                                                                 if (actorID == myActorID)
                                                                     targetString = "Player";
                                                                 else
                                                                 {
                                                                     if (npcs.ContainsKey(actorID))
                                                                         targetString = "NPC(" + npcs[actorID].npcID + ")";
                                                                     else
                                                                         targetString = string.Format("Unknown(0x{0:X})", actorID);
                                                                 }
                                                                 sw.WriteLine(string.Format("SkillCastInfo Caster:{3} SkillID:{0} Mode:{1} Delay:{2} Session:{7} X:{4} Y:{5} Z:{6}", skillID, modeString, delay, targetString, x, y, z, session));
                                                             }
                                                         }

                                                         if (!skill_list.ContainsKey(skillID))
                                                         {
                                                             skill_list[skillID] = new Dictionary<string, int>();
                                                         }
                                                         skill_list[skillID][modeString] = delay;

                                                     }
                                                     //if(
                                                 }
                                             }
                                             #endregion
                                             break;
                                         case 4:
                                             {
                                                 #region 4 Actor
                                                 if (sw != null)
                                                 {
                                                     Packet<SagaBNS.Common.Packets.GamePacketOpcode> pak = new Packet<SagaBNS.Common.Packets.GamePacketOpcode>();
                                                     pak.PutBytes(ids.ToArray());
                                                     pak.Position = ids.Position;
                                                     uint skillId = pak.GetUInt();
                                                     ushort additionSession = pak.GetUShort();
                                                     ulong actorId1 = pak.GetULong();
                                                     ulong actorId2 = pak.GetULong();
                                                     byte extraMode = pak.GetByte();
                                                     byte session = pak.GetByte();
                                                     string temp = string.Empty;
                                                     ushort count1 = pak.GetUShort();
                                                     ushort dataLen2 = pak.GetUShort();
                                                     ushort dataAcc = 0;
                                                     SagaBNS.Common.Packets.GameServer.PacketParameter id = 0;
                                                     if (actorId1 == myActorID || relatedActors.Contains(actorId1) ||
                                                         actorId2 == myActorID || relatedActors.Contains(actorId2))
                                                     {
                                                         temp = string.Empty;
                                                         for (int k = 0; k < count1; k++)
                                                         {
                                                             try
                                                             {
                                                                 dataAcc += 2;
                                                                 id = (SagaBNS.Common.Packets.GameServer.PacketParameter)pak.GetUShort();
                                                                 SagaBNS.Common.Packets.GameServer.ActorUpdateParameter para = new SagaBNS.Common.Packets.GameServer.ActorUpdateParameter(id);
                                                                 para.Read(pak);
                                                                 dataAcc += (ushort)SagaBNS.Common.Packets.GameServer.Parameters.GetLength(id);
                                                                 switch(id)
                                                                 {
                                                                     case SagaBNS.Common.Packets.GameServer.PacketParameter.Hold:
                                                                         if (actorId1 == myActorID)
                                                                         {
                                                                             if (para.Value == 0)
                                                                                 sw.WriteLine("DropItem");
                                                                         }
                                                                         break;
                                                                     case SagaBNS.Common.Packets.GameServer.PacketParameter.Dead:
                                                                         {
                                                                             if (actorId1 == myActorID)
                                                                             {
                                                                                 string targetStr = string.Empty;
                                                                                 if (npcs.ContainsKey(actorId2))
                                                                                     targetStr = string.Format("NPC({0})", npcs[actorId2].npcID);
                                                                                 else
                                                                                     targetStr = string.Format("Unknown(0x{0:X})", actorId2);
                                                                                 sw.WriteLine("Player Killed:{0} with skill session:{1} addition:{2} additionSession:{3}", targetStr, session, skillId, additionSession);
                                                                             }
                                                                             if (actorId2 == myActorID)
                                                                             {
                                                                                 string targetStr = string.Empty;
                                                                                 if (npcs.ContainsKey(actorId1))
                                                                                     targetStr = string.Format("NPC({0})", npcs[actorId1].npcID);
                                                                                 else
                                                                                     targetStr = string.Format("Unknown(0x{0:X})", actorId1);
                                                                                 sw.WriteLine("{0} Killed:Player with skill session:{1} addition:{2} additionSession:{3}", targetStr, session,skillId, additionSession);
                                                                             }
                                                                         }
                                                                         break;
                                                                     case SagaBNS.Common.Packets.GameServer.PacketParameter.Level:
                                                                         {
                                                                             if (actorId1 == myActorID)
                                                                             {
                                                                                 sw.WriteLine("Player LevelUP Level:{0}", para.Value);
                                                                             }
                                                                         }
                                                                         break;
                                                                     case SagaBNS.Common.Packets.GameServer.PacketParameter.MaxHP:
                                                                         {
                                                                             if (actorId1 == myActorID)
                                                                             {
                                                                                 sw.WriteLine("Player MaxHP Change, MaxHP:{0}", para.Value);
                                                                             }
                                                                         }
                                                                         break;
                                                                 }
                                                                 temp += string.Format("{0},{1},", id, para.Value);
                                                             }
                                                             catch { }
                                                         }
                                                         int test = dataLen2 - dataAcc;
                                                         if (test != 0)
                                                         {
                                                             temp = string.Format(",trace into parameters(0x{0},{1}),", id, Conversions.bytes2HexString(pak.GetBytes((ushort)test)));
                                                         }
                                                         byte rest = pak.GetByte();
                                                         byte restType = pak.GetByte();
                                                         switch (restType)
                                                         {
                                                             case 2:
                                                                 temp += " made Damage:" + pak.GetInt();
                                                                 break;
                                                         }
                                                         if (session > 0 && outputCombat)
                                                         {
                                                             if (actorId1 == myActorID)
                                                             {
                                                                 string targetStr = string.Empty;
                                                                 if (actorId2 == myActorID)
                                                                     targetStr = "Player";
                                                                 else if (npcs.ContainsKey(actorId2))
                                                                     targetStr = string.Format("NPC({0})", npcs[actorId2].npcID);
                                                                 else
                                                                     targetStr = string.Format("Unknown(0x{0:X})", actorId2);
                                                                 sw.WriteLine("Player change Parameter for:{0} with skill session:{1} addition:{2} additionSession:{3} Parameters:{4}", targetStr, session,skillId, additionSession, temp);
                                                             }
                                                             else if (actorId2 == myActorID)
                                                             {
                                                                 string targetStr = string.Empty;
                                                                 if (actorId1 == myActorID)
                                                                     targetStr = "Player";
                                                                 else if (npcs.ContainsKey(actorId1))
                                                                     targetStr = string.Format("NPC({0})", npcs[actorId1].npcID);
                                                                 else
                                                                     targetStr = string.Format("Unknown(0x{0:X})", actorId1);
                                                                 sw.WriteLine("{0} change Parameter for:Player with skill session:{1} addition:{2} additionSession:{3} Parameters:{4}", targetStr, session, skillId, additionSession, temp);
                                                             }
                                                         }
                                                     }
                                                 }
                                                 #endregion
                                             }
                                             break;
                                         case 5:
                                             {
                                                 #region 5
                                                 ulong actorId1 = ids.GetULong();
                                                 byte pLength = ids.GetByte();
                                                 byte pType = ids.GetByte();
                                                 if (sw != null && outputCombat)
                                                 {
                                                     if (actorId1 == myActorID || relatedActors.Contains(actorId1))
                                                     {
                                                         switch (pType)
                                                         {
                                                             case 1:
                                                                 ushort additionSession = ids.GetUShort();
                                                                 uint additionID = ids.GetUInt();
                                                                 int duration = ids.GetInt();
                                                                 string targetStr="";
                                                                 if (actorId1 == myActorID)
                                                                 {
                                                                     targetStr = "Player";
                                                                 }
                                                                 else if (npcs.ContainsKey(actorId1))
                                                                     targetStr = string.Format("NPC({0})", npcs[actorId1].npcID);
                                                                 else
                                                                     targetStr = string.Format("Unknown(0x{0:X})", actorId1);
                                                                 sw.WriteLine(string.Format("Apply Addition to {0} AdditionID:{1} AdditionSession:{2} Duration:{3}", targetStr, additionID, additionSession, duration));
                                                                 break;
                                                         }
                                                     }
                                                 }
                                                 #endregion
                                             }
                                             break;
                                         case 0xF:
                                             #region Show ActorItem
                                             {
                                                 if (sw!=null && ids.GetULong() == myActorID)
                                                 {
                                                     ids.Position += 17;
                                                     uint itemID = ids.GetUInt();
                                                     sw.WriteLine(string.Format("HoldItem ItemID:{0}", itemID));
                                                 }
                                             }
 #endregion
                                             break;
                                         case 0x1A:
                                             #region 1A
                                             {
                                                 ulong actorID = ids.GetULong();
                                                 ids.Position += 8;
                                                 ushort npcID = ids.GetUShort();
                                                 npc n = new npc();
                                                 n.actorID = actorID;
                                                 n.npcID = npcID;
                                                 corpses[actorID] = n;
                                             }
                                             #endregion
                                             break;
                                     }
                                     ids.Position = pos + len;
                                 }
                             }
                             catch
                             {
                             }
                         }
                         #endregion
                         break;
                    case 0x1E:
                        {
                            if (sw != null)
                                sw.WriteLine(string.Format("PortalActivate CutSceneID:{0} U1:{1} U2:{2}", ids.GetUInt(12), ids.GetInt(3), ids.GetInt(20)));
                        }
                        break;
                    case 0x21:
                        {
                            if (sw != null)
                                sw.WriteLine(string.Format("PlayCutScene CutSceneID:{0}", ids.GetUInt(2)));
                        }
                        break;
                    case 0xFE:
                        {
                            #region FE
                            if (sw != null)
                            {
                                ulong id = ids.GetULong(2);
                                if (npcs.ContainsKey(id))
                                {
                                    sw.WriteLine(string.Format("TalkToNPC NPCID:{0}", npcs[id].npcID));
                                }
                            }
                            #endregion
                        }
                        break;
                    case 0x108:
                        {
                            #region 108
                            if (sw != null && outputCombat)
                            {
                                ulong id = ids.GetULong(8);
                                if (!relatedActors.Contains(id))
                                    relatedActors.Add(id);
                                if (npcs.ContainsKey(id))
                                {
                                    sw.WriteLine(string.Format("CastSkill SkillID:{0} TargetNPC:{1} Dir:{2}", ids.GetUInt(3), npcs[id].npcID, ids.GetUShort(16)));
                                }
                            }
                            #endregion
                        }
                        break;
                    case 0x11D:
                        {
                            #region 11D
                            if (sw != null)
                            {
                                uint id = ids.GetUInt(2);
                                if (statistic.mapObjects[currentMapID].ContainsKey(id))
                                {
                                    sw.WriteLine(string.Format("Open MapObj MapObjID:{0}", id));
                                }
                            }
                            #endregion
                        }
                        break;
                    case 0x127:
                        {
                            #region 127
                            ulong actorID = ids.GetULong(2);
                            if (corpses.ContainsKey(actorID))
                            {
                                ushort npcID = corpses[actorID].npcID;
                                if (!statistic.npcs.ContainsKey(npcID))
                                    statistic.npcs.Add(npcID, new npc());
                                statistic.npcs[npcID].corpseOpenCount++;
                                int gold = ids.GetInt();
                                if (statistic.npcs[npcID].minGold > gold)
                                    statistic.npcs[npcID].minGold = gold;
                                if (statistic.npcs[npcID].maxGold < gold)
                                    statistic.npcs[npcID].maxGold = gold;
                                short count = ids.GetShort();
                                for (int i = 0; i < count; i++)
                                {
                                    uint itemID = ids.GetUInt();
                                    ids.Position++;
                                    byte count2 = ids.GetByte();
                                    if (!statistic.npcs[npcID].items.ContainsKey(itemID))
                                        statistic.npcs[npcID].items[itemID] = count2;
                                    else
                                        statistic.npcs[npcID].items[itemID] += count2;
                                }
                            }
                            #endregion
                        }
                        break;
                    case 0x138:
                        {
                            #region 138
                            if (sw != null)
                            {
                                byte method = ids.GetByte(6);
                                if (method == 1)
                                {
                                    ushort count = ids.GetUShort();
                                    for (int i = 0; i < count; i++)
                                    {
                                        long pos = ids.Position;
                                        byte len = ids.GetByte();
                                        ids.Position += 3;
                                        uint itemID = ids.GetUInt();
                                        ids.Position = pos + len;
                                        sw.WriteLine(string.Format("GetItem ItemID:{0}", itemID));
                                    }
                                }
                            }
                            #endregion
                        }
                        break;
                    case 0x172:
                        {
                            if (sw != null)
                                sw.WriteLine(string.Format("Loot Quest Item QuestID:{0} Step:{1}", ids.GetUShort(4), ids.GetByte()));
                        }
                        break;
                    case 0x176:
                        {
                            if (sw != null)
                                sw.WriteLine(string.Format("NextQuest QuestID:{0}", ids.GetUShort(2)));
                        }
                        break;
                    case 0x177:
                        {
                            if (sw != null)
                                sw.WriteLine(string.Format("QuestUpdate QuestID:{0} Step:{1} NextStep:{3} StepStatus:{2},Flags:{4},{5},{6}", ids.GetUShort(2), ids.GetByte(), ids.GetByte(), ids.GetByte(), ids.GetUShort(), ids.GetUShort(), ids.GetUShort()));
                        }
                        break;
                    case 0x178:
                        {
                            if (sw != null)
                                sw.WriteLine(string.Format("QuestFinished QuestID:{0} Step:{1} StepStatus:{2}", ids.GetUShort(2), ids.GetByte(), ids.GetByte()));
                        }
                        break;
                    case 0x17F:
                        {
                            if (sw != null)
                                sw.WriteLine(string.Format("AddSkill SkillID:{0}", ids.GetUInt(2)));
                        }
                        break;
                     */
                }
            }

            #region TT
            /*
            OD.Filter = "*.xml|*.xml";
            if (OD.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                XmlDocument xml2 = new XmlDocument();
                xml2.Load(OD.FileName);
                XmlElement root2 = xml2["skill"];
                XmlNodeList list2 = root2.ChildNodes;
                foreach (object j in list2)
                {
                    XmlElement i;
                    if (j.GetType() != typeof(XmlElement)) continue;
                    i = (XmlElement)j;
                    if (i.Name.ToLower() == "skill")
                    {
                        XmlNodeList children = i.ChildNodes;
                        string name = string.Empty;
                        List<XmlElement> skills = new List<XmlElement>(), items = new List<XmlElement>();
                        foreach (object l in children)
                        {
                            XmlElement k = l as XmlElement;
                            if (k == null)
                                continue;
                            if (k.Name.ToLower() == "id")
                            {
                                    name = k.InnerText;
                                    continue;
                            }

                        }
                        if (skill_list.ContainsKey(uint.Parse(name)))
                        {
                            foreach (KeyValuePair<string, int> z in skill_list[uint.Parse(name)])
                            {
                                switch (z.Key)
                                {
                                    case "Casting":
                                        if (i.GetElementsByTagName("CastTime").Count == 0)
                                        {
                                            XmlElement item = xml2.CreateElement("CastTime");
                                            item.InnerText = z.Value.ToString();
                                            i.AppendChild(item);
                                        }
                                        break;
                                    case "Delay":
                                        if (i.GetElementsByTagName("ActionTime").Count == 0 && i.GetElementsByTagName("ShouldApproach").Count == 0)
                                        {
                                            XmlElement item = xml2.CreateElement("ActionTime");
                                            item.InnerText = z.Value.ToString();
                                            i.AppendChild(item);
                                        }
                                        break;
                                    case "Activate":
                                        if (i.GetElementsByTagName("ActivationTime").Count == 0)
                                        {
                                            XmlElement item = xml2.CreateElement("ActivationTime");
                                            item.InnerText = z.Value.ToString();
                                            i.AppendChild(item);
                                        }
                                        break;
                                }
                            }
                        }
                    }
                }
                string path = System.IO.Path.GetDirectoryName(OD.FileName) + "\\skill_templates.xml";
                xml2.Save(path);
            }//*/
            #endregion
        }

        private void BuildNPCDataSubCBT2(bool initial, bool later, System.IO.StreamWriter sw =null, bool makeStatistic = false,bool outputCombat=true)
        {
            Dictionary<uint, Dictionary<string, int>> skill_list = new Dictionary<uint, Dictionary<string, int>>();
            //Dictionary<ulong, ushort> npcIDs = new Dictionary<ulong, ushort>();
            Dictionary<ulong, npc> npcs = new Dictionary<ulong, npc>();
            Dictionary<ulong, npc> corpses = new Dictionary<ulong, npc>();
            Packet tmpIDs = null, tmpActors = null;
            List<ulong> relatedActors = new List<ulong>();
            uint currentMapID = 0, currentInstanceID;
            ulong myActorID = 0;
            int counter = 0;
            foreach (Packet ids in PacketParser.Instance.Packets)
            {
                int perc = (int)((float)(counter++) * 100 / PacketParser.Instance.Packets.Count);
                if (pb.Value != perc)
                {
                    pb.Value = perc;
                    Application.DoEvents();
                }
                switch (ids.ID)
                {
                    case 0x11:
                        {
                            myActorID = ids.GetULong(6);
                        }
                        break;
                    case 0x1A:
                        {
                            if (PacketParser.Instance.Version == PacketParser.PacketVersion.CBT2)
                            {
                                currentInstanceID = ids.GetUInt(2);
                                currentMapID = ids.GetUInt(11);
                                if (!statistic.spawns.ContainsKey(currentMapID))
                                {
                                    statistic.spawns.Add(currentMapID, new List<npc>());
                                }

                                if (!statistic.mapObjects.ContainsKey(currentMapID))
                                {
                                    statistic.mapObjects.Add(currentMapID, new Dictionary<ulong, npc>());
                                }

                                sw?.WriteLine("ChangeMap Map:{0} {1},{2},{3} Dir:{4}", currentMapID, ids.GetShort(34), ids.GetShort(), ids.GetShort(), ids.GetUShort());
                            }
                        }
                        break;
                    case 0x18:
                        {
                            if (PacketParser.Instance.Version == PacketParser.PacketVersion.CBT1)
                            {
                                currentInstanceID = ids.GetUInt(2);
                                currentMapID = ids.GetUInt(12);
                                if (!statistic.spawns.ContainsKey(currentMapID))
                                {
                                    statistic.spawns.Add(currentMapID, new List<npc>());
                                }

                                if (!statistic.mapObjects.ContainsKey(currentMapID))
                                {
                                    statistic.mapObjects.Add(currentMapID, new Dictionary<ulong, npc>());
                                }
                            }
                        }
                        break;
                    case 3:
                        #region 3
                        {
                            if (initial)
                            {
                                try
                                {
                                    Packet<int> cur = ids;
                                    if (tmpIDs != null)
                                    {
                                        cur = tmpIDs;
                                        tmpIDs = null;
                                        cur.Position = cur.Length;
                                        cur.PutBytes(ids.GetBytes((ushort)(ids.Length - 6), 6));
                                    }
                                    uint length = cur.GetUInt(2);
                                    if (length == uint.MaxValue)
                                    {
                                        continue;
                                    }

                                    if (length > (cur.Length - 6))
                                    {
                                        tmpIDs = new Packet();
                                        tmpIDs.PutBytes(cur.ToArray(), 0);
                                        continue;
                                    }
                                    uint mapID = cur.GetUInt();
                                    //if (!npcs.ContainsKey(mapID))
                                    //    npcs.Add(mapID, new Dictionary<ulong, npc>());
                                    ushort count = cur.GetUShort(14);
                                    for (int i = 0; i < count; i++)
                                    {
                                        ulong id = cur.GetULong();
                                        if ((id & 0x4000000000000) != 0)
                                        {
                                            npc npc = new Form1.npc()
                                            {
                                                actorID = id
                                            };
                                            cur.Position += 5;
                                            npc.npcID = (ushort)(cur.GetByte() << 8 | cur.GetByte());
                                            npcs[id] = npc;
                                            if (PacketParser.Instance.Version == PacketParser.PacketVersion.CBT1)
                                            {
                                                cur.Position += 11;
                                            }
                                            else
                                            {
                                                cur.Position += 92;
                                            }
                                        }
                                        else
                                        {
                                            if (PacketParser.Instance.Version == PacketParser.PacketVersion.CBT1)
                                            {
                                                cur.Position += 14;
                                            }
                                            else
                                            {
                                                cur.Position += 95;
                                            }

                                            ushort size = cur.GetUShort();
                                            cur.Position += ((size + 1) * 2);
                                        }
                                    }
                                }
                                catch
                                {
                                }
                            }
                        }
                        #endregion
                        break;
                    case 4:
                        #region 4
                        {
                            if (initial)
                            {
                                try
                                {
                                    Packet cur = ids;
                                    if (PacketParser.Instance.Packets.IndexOf(cur) == 65126)
                                    {
                                    }
                                    if (tmpActors != null)
                                    {
                                        cur = tmpActors;
                                        tmpActors = null;
                                        cur.Position = cur.Length;
                                        cur.PutBytes(ids.GetBytes((ushort)(ids.Length - 6), 6));
                                    }
                                    uint length = cur.GetUInt(2);
                                    if (length > (cur.Length - 6) && length != 0xFFFFFFFF)
                                    {
                                        tmpActors = new Packet();
                                        tmpActors.PutBytes(cur.ToArray(), 0);
                                        continue;
                                    }
                                    uint mapID = cur.GetUInt();

                                    ushort count = cur.GetUShort(14);
                                    for (int i = 0; i < count; i++)
                                    {
                                        ulong id = cur.GetULong();
                                        if (npcs.ContainsKey(id))
                                        {
                                            npc npc = npcs[id];
                                            npc.mapID = cur.GetUInt();
                                            statistic.spawns[currentMapID].Add(npc);
                                            npc.x = cur.GetShort();
                                            npc.y = cur.GetShort();
                                            npc.z = cur.GetShort();
                                            npc.dir = cur.GetUShort();
                                            cur.Position += 15;
                                            npc.ManaType = cur.GetInt();
                                            if (PacketParser.Instance.Version == PacketParser.PacketVersion.CBT1)
                                            {
                                                cur.Position += 77;
                                            }
                                            else
                                            {
                                                cur.Position += 76;
                                            }

                                            npc.motion = cur.GetUShort();
                                            cur.Position += 26;
                                            ushort unknown = cur.GetUShort();
                                            for (int j = 0; j < unknown; j++)
                                            {
                                                cur.Position += 12;
                                            }
                                        }
                                        else
                                        {
                                            if (PacketParser.Instance.Version == PacketParser.PacketVersion.CBT1)
                                            {
                                                cur.Position += 136;
                                            }
                                            else
                                            {
                                                cur.Position += 135;
                                            }

                                            ushort unknown = cur.GetUShort();
                                            for (int j = 0; j < unknown; j++)
                                            {
                                                cur.Position += 12;
                                            }
                                        }
                                    }
                                    count = cur.GetUShort();
                                    for (int i = 0; i < count; i++)
                                    {
                                        npc mapObj = new npc()
                                        {
                                            actorID = cur.GetUInt()
                                        };
                                        if (mapObj.actorID > 0x1000)
                                        {
                                        }
                                        mapObj.mapID = currentMapID;
                                        statistic.mapObjects[currentMapID][mapObj.actorID] = mapObj;
                                        if (PacketParser.Instance.Version == PacketParser.PacketVersion.CBT1)
                                        {
                                            cur.Position += 10;
                                        }
                                        else
                                        {
                                            cur.Position += 13;
                                        }
                                    }
                                }
                                catch
                                {
                                }
                            }
                        }
                        #endregion
                        break;
                    case 1:
                        #region 1
                        {
                            if (later)
                            {
                                try
                                {
                                    Packet<int> cur = ids;
                                    if (tmpIDs != null)
                                    {
                                        cur = tmpIDs;
                                        tmpIDs = null;
                                        cur.Position = cur.Length;
                                        cur.PutBytes(ids.GetBytes((ushort)(ids.Length - 6), 6));
                                    }
                                    uint length = cur.GetUInt(2);
                                    if (length > (cur.Length - 6))
                                    {
                                        tmpIDs = new Packet();
                                        tmpIDs.PutBytes(cur.ToArray(), 0);
                                        continue;
                                    }
                                    uint mapID = cur.GetUInt();
                                    ushort count = cur.GetUShort(14);
                                    for (int i = 0; i < count; i++)
                                    {
                                        ulong id = cur.GetULong();
                                        if (relatedActors.Contains(id))
                                        {
                                            relatedActors.Remove(id);
                                        }

                                        cur.Position += 9;
                                    }
                                    count = cur.GetUShort();
                                    for (int i = 0; i < count; i++)
                                    {
                                        ulong id = cur.GetULong();
                                        if ((id & 0x4000000000000) != 0)
                                        {
                                            npc npc = new Form1.npc()
                                            {
                                                actorID = id
                                            };
                                            cur.Position += 6;
                                            npc.npcID = (ushort)(cur.GetByte() << 8 | cur.GetByte());
                                            npcs[id] = npc;
                                            if (PacketParser.Instance.Version == PacketParser.PacketVersion.CBT1)
                                            {
                                                cur.Position += 11;
                                            }
                                            else
                                            {
                                                cur.Position += 92;
                                            }
                                        }
                                        else
                                        {
                                            if (PacketParser.Instance.Version == PacketParser.PacketVersion.CBT1)
                                            {
                                                cur.Position += 15;
                                            }
                                            else
                                            {
                                                cur.Position += 96;
                                            }

                                            ushort size = cur.GetUShort();
                                            cur.Position += ((size + 1) * 2);
                                        }
                                    }
                                }
                                catch { }
                            }
                        }
                        #endregion
                        break;
                    case 2:
                        #region 2
                        {
                            if (later)
                            {
                                try
                                {
                                    Packet<int> cur = ids;
                                    if (tmpActors != null)
                                    {
                                        cur = tmpActors;
                                        tmpActors = null;
                                        cur.Position = cur.Length;
                                        cur.PutBytes(ids.GetBytes((ushort)(ids.Length - 6), 6));
                                    }
                                    uint length = cur.GetUInt(2);
                                    if (length > (cur.Length - 6))
                                    {
                                        tmpActors = new Packet();
                                        tmpActors.PutBytes(cur.ToArray(), 0);
                                        continue;
                                    }
                                    uint mapID = cur.GetUInt();
                                    ushort count = cur.GetUShort(14);
                                    for (int i = 0; i < count; i++)
                                    {
                                        cur.Position += 9;
                                    }
                                    count = cur.GetUShort();
                                    for (int i = 0; i < count; i++)
                                    {
                                        ulong id = cur.GetULong();
                                        if (npcs.ContainsKey(id))
                                        {
                                            npc npc = npcs[id];
                                            if (npc.x != 0 && npc.y != 0)
                                            {
                                                npc = new Form1.npc();
                                            }

                                            cur.Position += 9;
                                            npc.mapID = cur.GetUInt();
                                            statistic.spawns[currentMapID].Add(npc);
                                            npc.x = cur.GetShort();
                                            npc.y = cur.GetShort();
                                            npc.z = cur.GetShort();
                                            npc.dir = cur.GetUShort();
                                            cur.Position += 15;
                                            npc.ManaType = cur.GetInt();
                                            if (PacketParser.Instance.Version == PacketParser.PacketVersion.CBT1)
                                            {
                                                cur.Position += 77;
                                            }
                                            else
                                            {
                                                cur.Position += 76;
                                            }

                                            npc.motion = cur.GetUShort();
                                            cur.Position += 26;
                                            ushort unknown = cur.GetUShort();
                                            if (unknown != 0)
                                            {
                                                cur.Position += 12;
                                            }
                                        }
                                        else
                                        {
                                            if (PacketParser.Instance.Version == PacketParser.PacketVersion.CBT1)
                                            {
                                                cur.Position += 145;
                                            }
                                            else
                                            {
                                                cur.Position += 144;
                                            }

                                            ushort unknown = cur.GetUShort();
                                            if (unknown != 0)
                                            {
                                                cur.Position += 12;
                                            }
                                        }
                                    }
                                }
                                catch
                                {
                                }
                            }
                        }
                        #endregion
                        break;
                   /* case 6:
                        #region 6
                        {
                            if (!makeStatistic)
                                continue;
                            try
                            {
                                uint timeStamp = ids.GetUInt(6);
                                ushort count = ids.GetUShort(14);
                                for (int j = 0; j < count; j++)
                                {

                                    long pos = ids.Position;
                                    byte len = ids.GetByte();
                                    byte type = ids.GetByte();
                                    switch (type)
                                    {
                                        case 2:
                                            #region 2
                                            {
                                                ulong actorID = ids.GetULong();
                                                uint instanceID = (uint)((actorID >> 32) & 0xFFFF);

                                                uint skillID = ids.GetUInt();
                                                byte castMode = ids.GetByte();
                                                byte session = ids.GetByte();
                                                ids.Position += 2;
                                                byte mode = ids.GetByte();
                                                if (mode == 4 || mode == 1 || mode == 0)
                                                {
                                                    if ((actorID & 0x4000000000000) != 0)
                                                    {
                                                        if (npcs.ContainsKey(actorID))
                                                        {
                                                            npc npc = npcs[actorID];
                                                            if (!statistic.npcs.ContainsKey(npc.npcID))
                                                                statistic.npcs.Add(npc.npcID, new npc());
                                                            if (!statistic.npcs[npc.npcID].skills.ContainsKey(skillID))
                                                                statistic.npcs[npc.npcID].skills[skillID] = 1;
                                                            else
                                                                statistic.npcs[npc.npcID].skills[skillID]++;
                                                        }
                                                    }
                                                    if (sw != null && outputCombat)
                                                    {
                                                        int delay = ids.GetShort() * 100;
                                                        ids.Position++;
                                                        byte type2 = ids.GetByte();
                                                        string modeString = string.Empty;
                                                        switch (mode)
                                                        {
                                                            case 0:
                                                                modeString = "Casting";
                                                                break;
                                                            case 1:
                                                                modeString = "Delay";
                                                                break;
                                                            case 4:
                                                                modeString = "Activate";
                                                                break;
                                                        }
                                                        if (type2 == 1)
                                                        {
                                                            ulong targetID = ids.GetULong();
                                                            if (actorID == myActorID && !relatedActors.Contains(targetID))
                                                                relatedActors.Add(targetID);
                                                            if (targetID == myActorID && !relatedActors.Contains(actorID))
                                                                relatedActors.Add(actorID);
                                                            if (actorID == myActorID)
                                                            {
                                                                string targetString = string.Empty;
                                                                if (targetID == myActorID || relatedActors.Contains(targetID))
                                                                {
                                                                    if (targetID == myActorID)
                                                                        targetString = "Player";
                                                                    else
                                                                    {
                                                                        if (npcs.ContainsKey(targetID))
                                                                            targetString = "NPC(" + npcs[targetID].npcID + ")";
                                                                        else
                                                                            targetString = string.Format("Unknown(0x{0:X})", targetID);
                                                                    }
                                                                    sw.WriteLine(string.Format("SkillCastInfo Caster:Player SkillID:{0} Mode:{1} Delay:{2} Session:{3} Target:{4}", skillID, modeString, delay, session, targetString));
                                                                }
                                                            }
                                                            if (targetID == myActorID)
                                                            {
                                                                string targetString = string.Empty;
                                                                if (actorID == myActorID || relatedActors.Contains(actorID))
                                                                {
                                                                    if (actorID == myActorID)
                                                                        targetString = "Player";
                                                                    else
                                                                    {
                                                                        if (npcs.ContainsKey(actorID))
                                                                            targetString = "NPC(" + npcs[actorID].npcID + ")";
                                                                        else
                                                                            targetString = string.Format("Unknown(0x{0:X})", actorID);
                                                                    }
                                                                    sw.WriteLine(string.Format("SkillCastInfo Caster:{4} SkillID:{0} Mode:{1} Delay:{2} Session:{3} Target:Player", skillID, modeString, delay, session, targetString));
                                                                }
                                                            }
                                                        }
                                                        else
                                                        {
                                                            if (actorID == myActorID || relatedActors.Contains(actorID))
                                                            {
                                                                short x = ids.GetShort();
                                                                short y = ids.GetShort();
                                                                short z = ids.GetShort();
                                                                string targetString = string.Empty;
                                                                if (actorID == myActorID)
                                                                    targetString = "Player";
                                                                else
                                                                {
                                                                    if (npcs.ContainsKey(actorID))
                                                                        targetString = "NPC(" + npcs[actorID].npcID + ")";
                                                                    else
                                                                        targetString = string.Format("Unknown(0x{0:X})", actorID);
                                                                }
                                                                sw.WriteLine(string.Format("SkillCastInfo Caster:{3} SkillID:{0} Mode:{1} Delay:{2} Session:{7} X:{4} Y:{5} Z:{6}", skillID, modeString, delay, targetString, x, y, z, session));
                                                            }
                                                        }

                                                        if (!skill_list.ContainsKey(skillID))
                                                        {
                                                            skill_list[skillID] = new Dictionary<string, int>();
                                                        }
                                                        skill_list[skillID][modeString] = delay;

                                                    }
                                                    //if(
                                                }
                                            }
                                            #endregion
                                            break;
                                        case 4:
                                            {
                                                #region 4 Actor
                                                if (sw != null)
                                                {
                                                    Packet<SagaBNS.Common.Packets.GamePacketOpcode> pak = new Packet<SagaBNS.Common.Packets.GamePacketOpcode>();
                                                    pak.PutBytes(ids.ToArray());
                                                    pak.Position = ids.Position;
                                                    uint skillId = pak.GetUInt();
                                                    ushort additionSession = pak.GetUShort();
                                                    ulong actorId1 = pak.GetULong();
                                                    ulong actorId2 = pak.GetULong();
                                                    byte extraMode = pak.GetByte();
                                                    byte session = pak.GetByte();
                                                    string temp = string.Empty;
                                                    ushort count1 = pak.GetUShort();
                                                    ushort dataLen2 = pak.GetUShort();
                                                    ushort dataAcc = 0;
                                                    SagaBNS.Common.Packets.GameServer.PacketParameter id = 0;
                                                    if (actorId1 == myActorID || relatedActors.Contains(actorId1) ||
                                                        actorId2 == myActorID || relatedActors.Contains(actorId2))
                                                    {
                                                        temp = string.Empty;
                                                        for (int k = 0; k < count1; k++)
                                                        {
                                                            try
                                                            {
                                                                dataAcc += 2;
                                                                id = (SagaBNS.Common.Packets.GameServer.PacketParameter)pak.GetUShort();
                                                                SagaBNS.Common.Packets.GameServer.ActorUpdateParameter para = new SagaBNS.Common.Packets.GameServer.ActorUpdateParameter(id);
                                                                para.Read(pak);
                                                                dataAcc += (ushort)SagaBNS.Common.Packets.GameServer.Parameters.GetLength(id);
                                                                switch(id)
                                                                {
                                                                    case SagaBNS.Common.Packets.GameServer.PacketParameter.Hold:
                                                                        if (actorId1 == myActorID)
                                                                        {
                                                                            if (para.Value == 0)
                                                                                sw.WriteLine("DropItem");
                                                                        }
                                                                        break;
                                                                    case SagaBNS.Common.Packets.GameServer.PacketParameter.Dead:
                                                                        {
                                                                            if (actorId1 == myActorID)
                                                                            {
                                                                                string targetStr = string.Empty;
                                                                                if (npcs.ContainsKey(actorId2))
                                                                                    targetStr = string.Format("NPC({0})", npcs[actorId2].npcID);
                                                                                else
                                                                                    targetStr = string.Format("Unknown(0x{0:X})", actorId2);
                                                                                sw.WriteLine("Player Killed:{0} with skill session:{1} addition:{2} additionSession:{3}", targetStr, session, skillId, additionSession);
                                                                            }
                                                                            if (actorId2 == myActorID)
                                                                            {
                                                                                string targetStr = string.Empty;
                                                                                if (npcs.ContainsKey(actorId1))
                                                                                    targetStr = string.Format("NPC({0})", npcs[actorId1].npcID);
                                                                                else
                                                                                    targetStr = string.Format("Unknown(0x{0:X})", actorId1);
                                                                                sw.WriteLine("{0} Killed:Player with skill session:{1} addition:{2} additionSession:{3}", targetStr, session,skillId, additionSession);
                                                                            }
                                                                        }
                                                                        break;
                                                                    case SagaBNS.Common.Packets.GameServer.PacketParameter.Level:
                                                                        {
                                                                            if (actorId1 == myActorID)
                                                                            {
                                                                                sw.WriteLine("Player LevelUP Level:{0}", para.Value);
                                                                            }
                                                                        }
                                                                        break;
                                                                    case SagaBNS.Common.Packets.GameServer.PacketParameter.MaxHP:
                                                                        {
                                                                            if (actorId1 == myActorID)
                                                                            {
                                                                                sw.WriteLine("Player MaxHP Change, MaxHP:{0}", para.Value);
                                                                            }
                                                                        }
                                                                        break;
                                                                }
                                                                temp += string.Format("{0},{1},", id, para.Value);
                                                            }
                                                            catch { }
                                                        }
                                                        int test = dataLen2 - dataAcc;
                                                        if (test != 0)
                                                        {
                                                            temp = string.Format(",trace into parameters(0x{0},{1}),", id, Conversions.bytes2HexString(pak.GetBytes((ushort)test)));
                                                        }
                                                        byte rest = pak.GetByte();
                                                        byte restType = pak.GetByte();
                                                        switch (restType)
                                                        {
                                                            case 2:
                                                                temp += " made Damage:" + pak.GetInt();
                                                                break;
                                                        }
                                                        if (session > 0 && outputCombat)
                                                        {
                                                            if (actorId1 == myActorID)
                                                            {
                                                                string targetStr = string.Empty;
                                                                if (actorId2 == myActorID)
                                                                    targetStr = "Player";
                                                                else if (npcs.ContainsKey(actorId2))
                                                                    targetStr = string.Format("NPC({0})", npcs[actorId2].npcID);
                                                                else
                                                                    targetStr = string.Format("Unknown(0x{0:X})", actorId2);
                                                                sw.WriteLine("Player change Parameter for:{0} with skill session:{1} addition:{2} additionSession:{3} Parameters:{4}", targetStr, session,skillId, additionSession, temp);
                                                            }
                                                            else if (actorId2 == myActorID)
                                                            {
                                                                string targetStr = string.Empty;
                                                                if (actorId1 == myActorID)
                                                                    targetStr = "Player";
                                                                else if (npcs.ContainsKey(actorId1))
                                                                    targetStr = string.Format("NPC({0})", npcs[actorId1].npcID);
                                                                else
                                                                    targetStr = string.Format("Unknown(0x{0:X})", actorId1);
                                                                sw.WriteLine("{0} change Parameter for:Player with skill session:{1} addition:{2} additionSession:{3} Parameters:{4}", targetStr, session, skillId, additionSession, temp);
                                                            }
                                                        }
                                                    }
                                                }
                                                #endregion
                                            }
                                            break;
                                        case 5:
                                            {
                                                #region 5
                                                ulong actorId1 = ids.GetULong();
                                                byte pLength = ids.GetByte();
                                                byte pType = ids.GetByte();
                                                if (sw != null && outputCombat)
                                                {
                                                    if (actorId1 == myActorID || relatedActors.Contains(actorId1))
                                                    {
                                                        switch (pType)
                                                        {
                                                            case 1:
                                                                ushort additionSession = ids.GetUShort();
                                                                uint additionID = ids.GetUInt();
                                                                int duration = ids.GetInt();
                                                                string targetStr="";
                                                                if (actorId1 == myActorID)
                                                                {
                                                                    targetStr = "Player";
                                                                }
                                                                else if (npcs.ContainsKey(actorId1))
                                                                    targetStr = string.Format("NPC({0})", npcs[actorId1].npcID);
                                                                else
                                                                    targetStr = string.Format("Unknown(0x{0:X})", actorId1);
                                                                sw.WriteLine(string.Format("Apply Addition to {0} AdditionID:{1} AdditionSession:{2} Duration:{3}", targetStr, additionID, additionSession, duration));
                                                                break;
                                                        }
                                                    }
                                                }
                                                #endregion
                                            }
                                            break;
                                        case 0xF:
                                            #region Show ActorItem
                                            {
                                                if (sw!=null && ids.GetULong() == myActorID)
                                                {
                                                    ids.Position += 17;
                                                    uint itemID = ids.GetUInt();
                                                    sw.WriteLine(string.Format("HoldItem ItemID:{0}", itemID));
                                                }
                                            }
#endregion
                                            break;
                                        case 0x1A:
                                            #region 1A
                                            {
                                                ulong actorID = ids.GetULong();
                                                ids.Position += 8;
                                                ushort npcID = ids.GetUShort();
                                                npc n = new npc();
                                                n.actorID = actorID;
                                                n.npcID = npcID;
                                                corpses[actorID] = n;
                                            }
                                            #endregion
                                            break;
                                    }
                                    ids.Position = pos + len;
                                }
                            }
                            catch
                            {
                            }
                        }
                        #endregion
                        break;
                        */
                    case 0x1E:
                        {
                            sw?.WriteLine(string.Format("PortalActivate CutSceneID:{0} U1:{1} U2:{2}", ids.GetUInt(12), ids.GetInt(3), ids.GetInt(20)));
                        }
                        break;
                    case 0x21:
                        {
                            sw?.WriteLine(string.Format("PlayCutScene CutSceneID:{0}", ids.GetUInt(2)));
                        }
                        break;
                    case 0xFE:
                        {
                            #region FE
                            if (sw != null)
                            {
                                ulong id = ids.GetULong(2);
                                if (npcs.ContainsKey(id))
                                {
                                    sw.WriteLine(string.Format("TalkToNPC NPCID:{0}", npcs[id].npcID));
                                }
                            }
                            #endregion
                        }
                        break;
                    case 0x108:
                        {
                            #region 108
                            if (sw != null && outputCombat)
                            {
                                ulong id = ids.GetULong(8);
                                if (!relatedActors.Contains(id))
                                {
                                    relatedActors.Add(id);
                                }

                                if (npcs.ContainsKey(id))
                                {
                                    sw.WriteLine(string.Format("CastSkill SkillID:{0} TargetNPC:{1} Dir:{2}", ids.GetUInt(3), npcs[id].npcID, ids.GetUShort(16)));
                                }
                            }
                            #endregion
                        }
                        break;
                    case 0x11D:
                        {
                            #region 11D
                            if (sw != null)
                            {
                                uint id = ids.GetUInt(2);
                                if (statistic.mapObjects[currentMapID].ContainsKey(id))
                                {
                                    sw.WriteLine(string.Format("Open MapObj MapObjID:{0}", id));
                                }
                            }
                            #endregion
                        }
                        break;
                    case 0x127:
                        {
                            #region 127
                            ulong actorID = ids.GetULong(2);
                            if (corpses.ContainsKey(actorID))
                            {
                                ushort npcID = corpses[actorID].npcID;
                                if (!statistic.npcs.ContainsKey(npcID))
                                {
                                    statistic.npcs.Add(npcID, new npc());
                                }

                                statistic.npcs[npcID].corpseOpenCount++;
                                int gold = ids.GetInt();
                                if (statistic.npcs[npcID].minGold > gold)
                                {
                                    statistic.npcs[npcID].minGold = gold;
                                }

                                if (statistic.npcs[npcID].maxGold < gold)
                                {
                                    statistic.npcs[npcID].maxGold = gold;
                                }

                                short count = ids.GetShort();
                                for (int i = 0; i < count; i++)
                                {
                                    uint itemID = ids.GetUInt();
                                    ids.Position++;
                                    byte count2 = ids.GetByte();
                                    if (!statistic.npcs[npcID].items.ContainsKey(itemID))
                                    {
                                        statistic.npcs[npcID].items[itemID] = count2;
                                    }
                                    else
                                    {
                                        statistic.npcs[npcID].items[itemID] += count2;
                                    }
                                }
                            }
                            #endregion
                        }
                        break;
                    case 0x138:
                        {
                            #region 138
                            if (sw != null)
                            {
                                byte method = ids.GetByte(6);
                                if (method == 1)
                                {
                                    ushort count = ids.GetUShort();
                                    for (int i = 0; i < count; i++)
                                    {
                                        long pos = ids.Position;
                                        byte len = ids.GetByte();
                                        ids.Position += 3;
                                        uint itemID = ids.GetUInt();
                                        ids.Position = pos + len;
                                        sw.WriteLine(string.Format("GetItem ItemID:{0}", itemID));
                                    }
                                }
                            }
                            #endregion
                        }
                        break;
                    case 0x172:
                        {
                            sw?.WriteLine(string.Format("Loot Quest Item QuestID:{0} Step:{1}", ids.GetUShort(4), ids.GetByte()));
                        }
                        break;
                    case 0x176:
                        {
                            sw?.WriteLine(string.Format("NextQuest QuestID:{0}", ids.GetUShort(2)));
                        }
                        break;
                    case 0x177:
                        {
                            sw?.WriteLine(string.Format("QuestUpdate QuestID:{0} Step:{1} NextStep:{3} StepStatus:{2},Flags:{4},{5},{6}", ids.GetUShort(2), ids.GetByte(), ids.GetByte(), ids.GetByte(), ids.GetUShort(), ids.GetUShort(), ids.GetUShort()));
                        }
                        break;
                    case 0x178:
                        {
                            sw?.WriteLine(string.Format("QuestFinished QuestID:{0} Step:{1} StepStatus:{2}", ids.GetUShort(2), ids.GetByte(), ids.GetByte()));
                        }
                        break;
                    case 0x17F:
                        {
                            sw?.WriteLine(string.Format("AddSkill SkillID:{0}", ids.GetUInt(2)));
                        }
                        break;
                }
            }

            #region TT
            /*
            OD.Filter = "*.xml|*.xml";
            if (OD.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                XmlDocument xml2 = new XmlDocument();
                xml2.Load(OD.FileName);
                XmlElement root2 = xml2["skill"];
                XmlNodeList list2 = root2.ChildNodes;
                foreach (object j in list2)
                {
                    XmlElement i;
                    if (j.GetType() != typeof(XmlElement)) continue;
                    i = (XmlElement)j;
                    if (i.Name.ToLower() == "skill")
                    {
                        XmlNodeList children = i.ChildNodes;
                        string name = string.Empty;
                        List<XmlElement> skills = new List<XmlElement>(), items = new List<XmlElement>();
                        foreach (object l in children)
                        {
                            XmlElement k = l as XmlElement;
                            if (k == null)
                                continue;
                            if (k.Name.ToLower() == "id")
                            {
                                    name = k.InnerText;
                                    continue;
                            }

                        }
                        if (skill_list.ContainsKey(uint.Parse(name)))
                        {
                            foreach (KeyValuePair<string, int> z in skill_list[uint.Parse(name)])
                            {
                                switch (z.Key)
                                {
                                    case "Casting":
                                        if (i.GetElementsByTagName("CastTime").Count == 0)
                                        {
                                            XmlElement item = xml2.CreateElement("CastTime");
                                            item.InnerText = z.Value.ToString();
                                            i.AppendChild(item);
                                        }
                                        break;
                                    case "Delay":
                                        if (i.GetElementsByTagName("ActionTime").Count == 0 && i.GetElementsByTagName("ShouldApproach").Count == 0)
                                        {
                                            XmlElement item = xml2.CreateElement("ActionTime");
                                            item.InnerText = z.Value.ToString();
                                            i.AppendChild(item);
                                        }
                                        break;
                                    case "Activate":
                                        if (i.GetElementsByTagName("ActivationTime").Count == 0)
                                        {
                                            XmlElement item = xml2.CreateElement("ActivationTime");
                                            item.InnerText = z.Value.ToString();
                                            i.AppendChild(item);
                                        }
                                        break;
                                }
                            }
                        }
                    }
                }
                string path = System.IO.Path.GetDirectoryName(OD.FileName) + "\\skill_templates.xml";
                xml2.Save(path);
            }//*/
            #endregion
        }

        private void textBox1_MouseUp(object sender, MouseEventArgs e)
        {
            try
            {
                string tmp = textBox1.SelectedText.Replace(" ", "").Replace(",", "");
                tmp = tmp.Replace("\r\n", "");
                lb_Selected.Text = (tmp.Length / 2).ToString();
                if (tmp.Length <= 8)
                {
                    int count = 8 - tmp.Length;
                    for (int i = 0; i < count; i++)
                    {
                        tmp += "0";
                    }

                    lb_Int.Text = BitConverter.ToInt32(Conversions.HexStr2Bytes(tmp), 0).ToString();
                    lb_Float.Text = BitConverter.ToSingle(Conversions.HexStr2Bytes(tmp), 0).ToString();
                }
                byte[] buf = Conversions.HexStr2Bytes(tmp);
                lb_Unicode.Text = System.Text.Encoding.Unicode.GetString(buf).Replace("\0", "");
                lb_Ansi.Text = System.Text.Encoding.Default.GetString(buf).Replace("\0", ""); ;
            }
            catch
            {
            }
        }

        private void tbNowPageIndex_TextChanged(object sender, EventArgs e)
        {
            int TargetPos = currentPos;
            if (int.TryParse(tbNowPageIndex.Text, out TargetPos) && currentList.Count >0)
            {
                TargetPos -= 1;
                currentPos = TargetPos;
                if (currentPos < 0)
                {
                    currentPos = 0;
                }

                if (currentPos >= currentList.Count)
                {
                    currentPos = currentList.Count - 1;
                }

                textBox1.Text = PacketParser.Instance.Parse(currentList[currentPos]);
                tbNowPageIndex.Text = (currentPos + 1).ToString();
                label1.Text = "/" + currentList.Count.ToString();
            }
        }

        public static List<int> updateSelected;
        public static List<ulong> validActorIDs = new List<ulong>();
        public static bool foodSkill;

        private void btnFilter_Click(object sender, EventArgs e)
        {
            Stopwatch watch = new Stopwatch();
            watch.Start();
            if (currentList != null)
            {
                var NowPacket = currentList.Count > (uint)currentPos ? currentList[currentPos] : new Packet();
                if (radioButton1.Checked)
                {
                    var lstSelected = (from string s in listBox1.CheckedItems.AsParallel()
                                       select Convert.ToInt32(s.Split(',')[0].Substring(2), 16)).ToList();
                    updateSelected = (from string s in lbUpdateID.CheckedItems.AsParallel()
                                      select Convert.ToInt32((s.Split(',')[0].Substring(2)), 16)).ToList();
                    validActorIDs = new List<ulong>();
                    foodSkill = cbFoodSkill.Checked;
                    foreach (string i in tbActorID.Text.Split(','))
                    {
                        if (string.IsNullOrEmpty(i))
                        {
                            continue;
                        }

                        validActorIDs.Add((ulong)Convert.ToInt64(i.Replace("0x", ""), 16));
                    }
                    System.Collections.Generic.IEnumerable<Packet> lstFiltered;
                    if (cbDebug.Checked)
                    {
                        lstFiltered = from p in PacketParser.Instance.Packets
                                      where lstSelected.Contains(p.ID)
                                      select p;
                    }
                    else
                    {
                        lstFiltered = from p in PacketParser.Instance.Packets
                                      where lstSelected.Contains(p.ID)
                                      && (p.ID != 6 || checkValidUpdateCount(p, updateSelected) > 0)
                                      select p;
                    }
                    /*var updatePackets = (from p in lstFiltered.AsParallel()
                                        where p.ID == 6 && checkValidUpdateCount(p, updateSelected) == 0
                                        select p).ToList();
                    lstFiltered = from p in lstFiltered.AsParallel()
                                  where !updatePackets.Contains(p)
                                  select p;*/
                    currentList = lstFiltered.ToList();
                }
                else if (radioButton2.Checked)
                {
                    var lstSelected = (from string s in listBox1.CheckedItems.AsParallel()
                                       select Convert.ToInt32(s.Split(',')[0].Substring(2), 16)).ToList();
                    var lstFiltered = from p in PacketParser.Instance.PacketsChat
                                      where lstSelected.Contains(p.ID)
                                      select p;
                    currentList = lstFiltered.ToList();
                }
                else if (radioButton3.Checked)
                {
                    var lstSelected = (from string s in listBox1.CheckedItems.AsParallel()
                                       select Convert.ToInt32(s.Split(',')[0].Substring(2), 16)).ToList();
                    var lstFiltered = from p in PacketParser.Instance.PacketsUnknown
                                      where lstSelected.Contains(p.ID)
                                        && (p.ID != 6 || checkValidUpdateCount(p, updateSelected) > 0)
                                      select p;
                    currentList = lstFiltered.ToList();
                }
                if (currentList.Count > 0)
                {
                    currentPos = NowPacket != null ? currentList.IndexOf(NowPacket) : 0;
                    if (currentPos > 0)
                    {
                        textBox1.Text = PacketParser.Instance.Parse(currentList[currentPos]);
                    }

                    tbNowPageIndex.Text = (currentPos + 1).ToString();
                }
                else
                {
                    tbNowPageIndex.Text = "0";
                }
                label1.Text = "/" + currentList.Count.ToString();
            }
            watch.Stop();
            MessageBox.Show(string.Format("Total query time:{0}ms", watch.ElapsedMilliseconds), "Finished", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private int checkValidUpdateCount(Packet<int> p, List<int> selected)
        {
            ushort count = PacketParser.Instance.Version <= PacketParser.PacketVersion.CBT2 ? p.GetUShort(14) : p.GetUShort(6);
            int valid = 0;

            try
            {
                for (int i = 0; i < count; i++)
                {
                    byte dataLen = p.GetByte();
                    byte type = p.GetByte();
                    long offset = p.Position;
                    ulong actorID = ulong.MaxValue, actorID2 = ulong.MaxValue;
                    uint skillID = uint.MaxValue;
                    if (selected.Contains(type))
                    {
                        switch (type)
                        {
                            case 2:
                                actorID = p.GetULong();
                                skillID = p.GetUInt();
                                break;
                            /*case 1:
                            case 3:
                            case 5:
                            case 8:
                            case 0xA:
                            case 0xB:
                            case 0xC:
                            case 0xD:
                            case 0x10:
                            case 0x18:
                            case 0x1B:
                            case 0x1D:
                            case 0x23:
                            case 0x27:
                            case 0x28:
                                actorID = p.GetULong();
                                break;
                            case 4:
                                p.Position += 6;
                                actorID = p.GetULong();
                                actorID2 = p.GetULong();
                                break;
                            case 6:
                            case 0xF:
                            case 0x11:
                            case 0x15:
                            case 0x16:
                            case 0x17:
                            case 0x19:
                            case 0x1A:
                            case 0x22:
                                actorID = p.GetULong();
                                actorID2 = p.GetULong();
                                break;
                            case 0x1C:
                                actorID = p.GetULong();
                                p.Position += 2;
                                actorID2 = p.GetULong();
                                break;*/

                        }
                        if (validActorIDs.Count == 0 || validActorIDs.Contains(actorID)
                             || validActorIDs.Contains(actorID2))
                        {
                            if (type == 2)
                            {
                                if (foodSkill)
                                {
                                    if (skillID > 7000000 && skillID < 8000000)
                                    {
                                        valid++;
                                    }
                                }
                                else
                                {
                                    valid++;
                                }
                            }
                            else
                            {
                                valid++;
                            }
                        }
                    }
                    p.Position = offset + dataLen - 2;
                }
            }
            catch
            {
            }
            return valid;
        }

        private void chkAll_CheckedChanged(object sender, EventArgs e)
        {
            for (int i = 0; i < listBox1.Items.Count; i++)
            {
                listBox1.SetItemChecked(i, chkAll.Checked);
            }
        }

        private void toolStripComboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            switch (toolStripComboBox1.SelectedIndex)
            {
                case 0:
                    PacketParser.Instance.Version = PacketParser.PacketVersion.CBT1;
                    break;
                case 1:
                    PacketParser.Instance.Version = PacketParser.PacketVersion.CBT2;
                    break;
                case 2:
                    PacketParser.Instance.Version = PacketParser.PacketVersion.CBT3;
                    break;
            }
        }

        private void cbUpdateAll_CheckedChanged(object sender, EventArgs e)
        {
            for (int i = 0; i < lbUpdateID.Items.Count; i++)
            {
                lbUpdateID.SetItemChecked(i, cbUpdateAll.Checked);
            }
        }

        #region ToHide       

        private void toolStripComboBox1_Click(object sender, EventArgs e)
        {

        }

        private void groupBox1_Enter(object sender, EventArgs e)
        {

        }

        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void lbUpdateID_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void button8_Click(object sender, EventArgs e)
        {

        }

        private void button9_Click(object sender, EventArgs e)
        {

        }

        private void button14_Click(object sender, EventArgs e)
        {

        }

        private void button15_Click(object sender, EventArgs e)
        {

        }

        private void button16_Click(object sender, EventArgs e)
        {

        }

        private void button18_Click(object sender, EventArgs e)
        {

        }

        private void button19_Click(object sender, EventArgs e)
        {

        }

        private void SD_FileOk(object sender, CancelEventArgs e)
        {

        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }
        #endregion

        private void radioButton1_CheckedChanged(object sender, EventArgs e)
        {
            if (radioButton1.Checked)
            {
                btnFilter_Click(null, null);
                listBox1.Items.Clear();
                foreach (int i in PacketParser.Instance.PacketsByID.Keys)
                {
                    if (PacketParser.Instance.Version >= PacketParser.PacketVersion.CBT2)
                    {
                        listBox1.Items.Add(string.Format("0x{0:X4},{2},{1}", i, PacketParser.Instance.PacketsByID[i].Count, ((SagaBNS.Common.Packets.GamePacketOpcode)i)), true);
                    }
                    else
                    {
                        listBox1.Items.Add(string.Format("0x{0:X4},{2},{1}", i, PacketParser.Instance.PacketsByID[i].Count, ((SagaBNS.Common.Packets.GamePacketOpcodeCBT1)i)), true);
                    }
                }
            }
        }

        private void radioButton2_CheckedChanged(object sender, EventArgs e)
        {
            if (radioButton2.Checked)
            {
                currentList = PacketParser.Instance.PacketsChat;
                if (currentList.Count > 0)
                {
                    currentPos = 0;
                    if (currentPos > 0)
                    {
                        textBox1.Text = PacketParser.Instance.Parse(currentList[currentPos]);
                    }

                    tbNowPageIndex.Text = (currentPos + 1).ToString();
                }
                else
                {
                    tbNowPageIndex.Text = "0";
                }
                label1.Text = "/" + currentList.Count.ToString();
                listBox1.Items.Clear();
                foreach (int i in PacketParser.Instance.PacketsByIDChat.Keys)
                {
                    listBox1.Items.Add(string.Format("0x{0:X4},{1}", i, PacketParser.Instance.PacketsByIDChat[i].Count), true);
                }
            }
        }

        private void radioButton3_CheckedChanged(object sender, EventArgs e)
        {
            if (radioButton3.Checked)
            {
                listBox1.Items.Clear();
                foreach (int i in PacketParser.Instance.PacketsByIDAuction.Keys)
                {
                    if (PacketParser.Instance.Version >= PacketParser.PacketVersion.CBT2)
                    {
                        listBox1.Items.Add(string.Format("0x{0:X4},{2},{1}", i, PacketParser.Instance.PacketsByIDAuction[i].Count, ((SagaBNS.Common.Packets.LobbyPacketOpcode)i)), true);
                    }
                }
                btnFilter_Click(null, null);
            }
        }

        private void button2_Click_1(object sender, EventArgs e)
        {
            Rijndael aes = Rijndael.Create();
            aes.Mode = CipherMode.ECB;
            byte[] Key = Conversions.HexStr2Bytes(tb_aeskey.Text);
            ICryptoTransform enc = aes.CreateEncryptor(Key, new byte[16]);
            byte[] src = Conversions.HexStr2Bytes(tb_aesInput.Text.Replace(" ", ""));
            enc.TransformBlock(src, 0, src.Length, src, 0);
            tb_aesOutput.Text=Conversions.bytes2HexString(src);
        }

        private void toolStripComboBox1_BackColorChanged(object sender, EventArgs e)
        {

        }

        private void mode_CheckedChanged(object sender, EventArgs e)
        {
            if (mode.Checked)
            {
                xmlMode = true;
            }
            else
            {
                xmlMode = false;
            }
        }

        private void button20_Click(object sender, EventArgs e)
        {
            Rijndael aes = Rijndael.Create();
            aes.Mode = CipherMode.ECB;
            byte[] Key = Conversions.HexStr2Bytes(tb_aeskey.Text);
            ICryptoTransform enc = aes.CreateEncryptor(Key, new byte[16]);

            byte[] src = Conversions.HexStr2Bytes(tb_aesInput.Text.Replace(" ", ""));
            ICryptoTransform dec = aes.CreateDecryptor(Key, new byte[16]);
            byte[] tmp3 = new byte[src.Length + 16];
            src.CopyTo(tmp3, 0);
            dec.TransformBlock(tmp3, 0, tmp3.Length, tmp3, 0);
            tb_aesOutput.Text = Conversions.bytes2HexString(tmp3);
        }
    }
}