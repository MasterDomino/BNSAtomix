using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace BruteForce
{
    public partial class Form2 : Form
    {
        private readonly int[,] countersA = new int[3, 100];
        private readonly int[,] countersB = new int[3, 100];

        private readonly int[] recurssionA = new int[] { -1, -1, -1 };
        private readonly int[] recurssionB = new int[] { -1, -1, -1 };
        public Form2()
        {
            InitializeComponent();
            this.SetStyle(
            ControlStyles.UserPaint
            | ControlStyles.AllPaintingInWmPaint
            | ControlStyles.OptimizedDoubleBuffer, true);
        }

        private void Form2_Load(object sender, EventArgs e)
        {

        }

        private void test()
        {

        }

        private byte[] bruteForce(byte[] src, byte[] dst)
        {
            //init array
            System.IO.FileStream fs1 = new System.IO.FileStream("tmp.dat", System.IO.FileMode.Create);
            System.IO.FileStream fs2 = new System.IO.FileStream("tmp2.dat", System.IO.FileMode.Create);
            System.IO.BinaryWriter bw1 = new System.IO.BinaryWriter(fs1);
            System.IO.BinaryReader br1 = new System.IO.BinaryReader(fs1);
            System.IO.BinaryWriter bw2 = new System.IO.BinaryWriter(fs2);
            System.IO.BinaryReader br2 = new System.IO.BinaryReader(fs2);

            bw1.Write(256);
            for (int i = 0; i < 256; i++)
            {
                byte[] tmp = new byte[256];
                tmp[1] = (byte)i;
                bw1.Write(tmp);
            }
            for (int i = 0; i < src.Length; i++)
            {
                bw1.BaseStream.Position = 0;
                int count = br1.ReadInt32();
                bw2.BaseStream.Position = 4;
                int newCount = 0;
                for (int j = 0; j < count; j++)
                {
                    byte[] tmp = br1.ReadBytes(256);
                    byte[] buf = new byte[256];
                    tmp.CopyTo(buf, 0);
                    int[] mapping = new int[256];
                    for (int k = 0; k < 256; k++)
                    {
                        mapping[k] = k;
                    }
                    List<byte> already = new List<byte>();
                    List<int> alreadyIndex = new List<int>();
                    int sum = 0;
                    bool invalid = false;
                    for (int k = 0; k < (i + 1); k++)
                    {
                        byte a = (byte)((k + 1) & 0xff);
                        enumerateA(a, already, alreadyIndex, mapping, tmp, buf, src, dst, k, sum, bw2, 0, false);
                        return null;
                        if (buf[a] == 0 && already.Contains(buf[a]))
                        {
                            invalid = true;
                            break;
                        }
                        if (!already.Contains(buf[a]))
                        {
                            already.Add(buf[a]);
                        }

                        if (!alreadyIndex.Contains(a))
                        {
                            alreadyIndex.Add(a);
                        }

                        byte b = (byte)(sum + buf[a]);
                        if ((buf[b] == 0) && already.Contains(buf[b]))
                        {
                            enumerateB(a, b, already, alreadyIndex, mapping, tmp, buf, src, dst, k, sum, bw2,0);
                            invalid = true;
                            break;
                        }
                        if (!already.Contains(buf[b]))
                        {
                            already.Add(buf[b]);
                        }

                        if (!alreadyIndex.Contains(b))
                        {
                            alreadyIndex.Add(b);
                        }

                        return null;
                        sum = b;
                        byte v12 = buf[a];
                        buf[a] = buf[b];
                        buf[b] = v12;
                        mapping[a] = b;
                        mapping[b] = a;

                        byte c = (byte)(buf[b] + buf[a]);
                        byte val = (byte)(dst[k] ^ src[k]);

                        if ((buf[c] == 0 || buf[c] == val) && !alreadyIndex.Contains(mapping[c]))
                        {
                            if (buf[c] != tmp[mapping[c]])
                            {
                            }
                            buf[c] = val;
                            tmp[mapping[c]] = val;
                        }
                        else
                        {
                            invalid = true;
                            break;//invalid
                        }
                        if (!already.Contains(buf[c]))
                        {
                            already.Add(buf[c]);
                        }

                        if (!alreadyIndex.Contains(mapping[c]))
                        {
                            alreadyIndex.Add(mapping[c]);
                        }
                    }
                    if (!invalid)
                    {
                        bw2.Write(tmp);
                        newCount++;
                    }
                }

                if (newCount > 0)
                {
                    bw2.BaseStream.Position = 0;
                    bw2.Write(newCount);
                    System.IO.BinaryReader brt = br1;
                    System.IO.BinaryWriter bwt = bw1;
                    br1 = br2;
                    bw1 = bw2;
                    br2 = brt;
                    bw2 = bwt;
                    bw2.BaseStream.SetLength(0);
                }
            }

            return null;
        }

        private void enumerateA(byte a, List<byte> already, List<int> alreadyIndex, int[] mapping, byte[] ori, byte[] buf, byte[] src, byte[] dst, int k, int sum, System.IO.BinaryWriter bw, int thread, bool recurssion)
        {
            if (already.Count >= 256)
            {
                return;
            }

            recurssionA[thread]++;
            if (alreadyIndex.Contains(mapping[a]))
            {
                byte b = (byte)(sum + buf[a]);
                enumerateB(a, b, already, alreadyIndex, mapping, ori, buf, src, dst, k, sum, bw, thread);
            }
            else
            {
                int counter = 0;
                if (recurssionA[thread] == 0)
                {
                    System.Threading.Thread a1 = new System.Threading.Thread(() =>
                    {
                        List<byte> already2 = new List<byte>();
                        List<int> alreadyIndex2 = new List<int>();
                        int[] mapping2 = new int[256];
                        for (int i = 0; i < 256; i++)
                        {
                            mapping2[i] = i;
                        }

                        byte[] ori2 = new byte[256];
                        ori.CopyTo(ori2, 0);
                        byte[] buf2 = new byte[256];
                        buf.CopyTo(buf2, 0);
                        byte a_2 = 1;
                        for (int l = 0; l < 85; l++)
                        {
                            if (recurssionA[0] < 100)
                            {
                                countersA[0, recurssionA[0]] = l;
                            }

                            if (!already2.Contains((byte)l))
                            {
                                byte[] tmpNew = new byte[256];
                                ori2.CopyTo(tmpNew, 0);
                                if (buf2[a_2] != ori[mapping2[a_2]])
                                {
                                }//check if valid
                                tmpNew[mapping2[a_2]] = (byte)l;
                                byte[] bufN = new byte[256];
                                buf2.CopyTo(bufN, 0);
                                bufN[a_2] = (byte)l;
                                List<byte> alreadyN = new List<byte>();
                                alreadyN.AddRange(already2);
                                List<int> alreadyIndexN = new List<int>();
                                alreadyIndexN.AddRange(alreadyIndex2);
                                int[] mappingN = new int[256];
                                mapping2.CopyTo(mappingN, 0);
                                alreadyN.Add((byte)l);
                                alreadyIndexN.Add(mapping2[a_2]);
                                if (alreadyN.Count == 256)
                                {
                                    textBox3.Text += Convert.ToBase64String(tmpNew) + "\r\n";
                                    recurssionB[thread]--;
                                    return;
                                }
                                byte b = (byte)(0 + bufN[a_2]);
                                enumerateB(a_2, b, alreadyN, alreadyIndexN, mappingN, tmpNew, bufN, src, dst, 0, 0, bw, 0);
                                //counter++;
                            }
                        }
                    });
                    System.Threading.Thread a2 = new System.Threading.Thread(() =>
                    {
                        recurssionA[1]++;
                        List<byte> already2 = new List<byte>();
                        List<int> alreadyIndex2 = new List<int>();
                        int[] mapping2 = new int[256];
                        for (int i = 0; i < 256; i++)
                        {
                            mapping2[i] = i;
                        }

                        byte[] ori2 = new byte[256];
                        ori.CopyTo(ori2, 0);
                        byte[] buf2 = new byte[256];
                        buf.CopyTo(buf2, 0);
                        byte a_2 = 1;
                        for (int l = 85; l < 170; l++)
                        {
                            if (recurssionA[1] < 100)
                            {
                                countersA[1, recurssionA[1]] = l;
                            }

                            if (!already2.Contains((byte)l))
                            {
                                byte[] tmpNew = new byte[256];
                                ori2.CopyTo(tmpNew, 0);
                                if (buf2[a_2] != ori[mapping2[a_2]])
                                {
                                }//check if valid
                                tmpNew[mapping2[a_2]] = (byte)l;
                                byte[] bufN = new byte[256];
                                buf2.CopyTo(bufN, 0);
                                bufN[a_2] = (byte)l;
                                List<byte> alreadyN = new List<byte>();
                                alreadyN.AddRange(already2);
                                List<int> alreadyIndexN = new List<int>();
                                alreadyIndexN.AddRange(alreadyIndex2);
                                int[] mappingN = new int[256];
                                mapping2.CopyTo(mappingN, 0);
                                alreadyN.Add((byte)l);
                                alreadyIndexN.Add(mapping2[a_2]);
                                if (alreadyN.Count == 256)
                                {
                                    textBox3.Text += Convert.ToBase64String(tmpNew) + "\r\n";
                                    recurssionB[thread]--;
                                    return;
                                }
                                byte b = (byte)(0 + bufN[a_2]);
                                enumerateB(a_2, b, alreadyN, alreadyIndexN, mappingN, tmpNew, bufN, src, dst, 0, 0, bw, 1);
                                //counter++;
                            }
                        }
                    });
                    System.Threading.Thread a3 = new System.Threading.Thread(() =>
                    {
                        recurssionA[2]++;
                        List<byte> already2 = new List<byte>();
                        List<int> alreadyIndex2 = new List<int>();
                        int[] mapping2 = new int[256];
                        for (int i = 0; i < 256; i++)
                        {
                            mapping2[i] = i;
                        }

                        byte[] ori2 = new byte[256];
                        ori.CopyTo(ori2, 0);
                        byte[] buf2 = new byte[256];
                        buf.CopyTo(buf2, 0);
                        byte a_2 = 1;
                        for (int l = 170; l < 256; l++)
                        {
                            if (recurssionA[2] < 100)
                            {
                                countersA[2, recurssionA[2]] = l;
                            }

                            if (!already2.Contains((byte)l))
                            {
                                byte[] tmpNew = new byte[256];
                                ori2.CopyTo(tmpNew, 0);
                                if (buf2[a_2] != ori[mapping2[a_2]])
                                {
                                }//check if valid
                                tmpNew[mapping2[a_2]] = (byte)l;
                                byte[] bufN = new byte[256];
                                buf2.CopyTo(bufN, 0);
                                bufN[a_2] = (byte)l;
                                List<byte> alreadyN = new List<byte>();
                                alreadyN.AddRange(already2);
                                List<int> alreadyIndexN = new List<int>();
                                alreadyIndexN.AddRange(alreadyIndex2);
                                int[] mappingN = new int[256];
                                mapping2.CopyTo(mappingN, 0);
                                alreadyN.Add((byte)l);
                                alreadyIndexN.Add(mapping2[a_2]);
                                if (alreadyN.Count == 256)
                                {
                                    textBox3.Text += Convert.ToBase64String(tmpNew) + "\r\n";
                                    recurssionB[thread]--;
                                    return;
                                }
                                byte b = (byte)(0 + bufN[a_2]);
                                enumerateB(a_2, b, alreadyN, alreadyIndexN, mappingN, tmpNew, bufN, src, dst, k, 0, bw, 2);
                                //counter++;
                            }
                        }
                    });
                    a1.Start();
                    a2.Start();
                    a3.Start();
                    System.Threading.Thread.Sleep(1000);
                }
                else
                {
                    for (int l = 0; l < 256; l++)
                    {
                        if (recurssionA[thread] < 100)
                        {
                            countersA[thread,recurssionA[thread]] = l;
                        }

                        if (!already.Contains((byte)l))
                        {
                            byte[] tmpNew = new byte[256];
                            ori.CopyTo(tmpNew, 0);
                            tmpNew[mapping[a]] = (byte)l;
                            byte[] bufN = new byte[256];
                            buf.CopyTo(bufN, 0);
                            bufN[a] = (byte)l;
                            List<byte> alreadyN = new List<byte>();
                            alreadyN.AddRange(already);
                            List<int> alreadyIndexN = new List<int>();
                            alreadyIndexN.AddRange(alreadyIndex);
                            int[] mappingN = new int[256];
                            mapping.CopyTo(mappingN, 0);
                            alreadyN.Add((byte)l);
                            alreadyIndexN.Add(mapping[a]);
                            if (alreadyN.Count == 256)
                            {
                                textBox3.Text += Convert.ToBase64String(tmpNew) + "\r\n";
                                recurssionB[thread]--;
                                return;
                            }
                            byte b = (byte)(sum + bufN[a]);
                            enumerateB(a, b, alreadyN, alreadyIndexN, mappingN, tmpNew, bufN, src, dst, k, sum, bw, thread);
                            //counter++;
                        }
                    }
                }

                //Console.WriteLine("Enumerated " + counter + " a");
            }
            recurssionA[thread]--;
        }

        private void enumerateB(byte a, byte b, List<byte> already, List<int> alreadyIndex, int[] mapping, byte[] ori, byte[] buf, byte[] src, byte[] dst, int k, int sum, System.IO.BinaryWriter bw, int thread)
        {
            if (already.Count >= 256)
            {
                return;
            }

            recurssionB[thread]++;
            if (alreadyIndex.Contains(mapping[b]))
            {
                sum = b;
                byte v12 = buf[a];
                buf[a] = buf[b];
                buf[b] = v12;
                mapping[a] = b;
                mapping[b] = a;

                byte c = (byte)(buf[b] + buf[a]);
                byte val = (byte)(dst[k] ^ src[k]);

                if (((buf[c] == 0) && !alreadyIndex.Contains(mapping[c])) || buf[c] == val)
                {
                    if (!alreadyIndex.Contains(mapping[c]))
                    {
                        buf[c] = val;
                        ori[mapping[c]] = val;
                        //bw.Write(ori);
                        //newCount++;
                        already.Add(val);
                        alreadyIndex.Add(mapping[c]);
                        if (already.Count == 256)
                        {
                            textBox3.Text += Convert.ToBase64String(ori) + "\r\n";
                            recurssionB[thread]--;
                            return;
                        }
                    }
                    k++;
                    a = (byte)((k + 1) & 0xff);
                    enumerateA(a, already, alreadyIndex, mapping, ori, buf, src, dst, k, sum, bw, thread, true);
                }
            }
            else
            {
                int counter = 0;
                for (int l = 0; l < 256; l++)
                {
                    if (recurssionB[thread] < 100)
                    {
                        countersB[thread,recurssionB[thread]] = l;
                    }

                    if (!already.Contains((byte)l))
                    {
                        byte[] tmpNew = new byte[256];
                        ori.CopyTo(tmpNew, 0);
                        if (buf[b] != ori[mapping[b]])
                        {
                        }//check if valid
                        tmpNew[mapping[b]] = (byte)l;
                        byte[] bufN = new byte[256];
                        buf.CopyTo(bufN, 0);
                        bufN[b] = (byte)l;
                        List<byte> alreadyN = new List<byte>();
                        alreadyN.AddRange(already);
                        List<int> alreadyIndexN = new List<int>();
                        alreadyIndexN.AddRange(alreadyIndex);
                        int[] mappingN = new int[256];
                        mapping.CopyTo(mappingN, 0);
                        alreadyN.Add((byte)l);
                        alreadyIndexN.Add(b);

                        sum = b;
                        byte v12 = bufN[a];
                        bufN[a] = bufN[b];
                        bufN[b] = v12;
                        mappingN[a] = b;
                        mappingN[b] = a;

                        byte c = (byte)(bufN[b] + bufN[a]);
                        byte val = (byte)(dst[k] ^ src[k]);

                        if (((bufN[c] == 0) && !alreadyIndexN.Contains(mappingN[c])) || bufN[c] == val)
                        {
                            if (!alreadyIndexN.Contains(mappingN[c]))
                            {
                                bufN[c] = val;
                                tmpNew[mappingN[c]] = val;
                                /*bw.Write(tmpNew);
                                newCount++;
                                counter++;*/
                                alreadyN.Add(val);
                                alreadyIndexN.Add(mappingN[c]);
                                if (alreadyN.Count == 256)
                                {
                                    textBox3.Text += Convert.ToBase64String(tmpNew) + "\r\n";
                                    recurssionB[thread]--;
                                    return;
                                }
                            }
                            k++;
                            a = (byte)((k + 1) & 0xff);
                            enumerateA(a, alreadyN, alreadyIndexN, mappingN, tmpNew, bufN, src, dst, k, sum, bw, thread, true);
                        }
                    }
                }
                //Console.WriteLine("Enumerated " + counter + " b");
            }
            recurssionB[thread]--;
        }

        private byte[] BlockEncrypt(byte[] src, byte[] key, ref int counter, ref int sum)
        {
            for (int i = 0; i < src.Length; i++)
            {
                int a = (counter + 1) & 0xFF;
                counter = a;
                int b = (sum + key[a]) & 0xFF;
                sum = b;
                int v12 = key[a];
                key[a] = key[b];
                key[b] = (byte)v12;
                src[i] = (byte)(src[i] ^ key[(key[sum] + key[a]) & 0xFF]);
            }
            return src;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            byte[] src = Encoding.UTF8.GetBytes(textBox1.Text);
            byte[] dst = Mono.Math.Conversions.HexStr2Bytes(textBox2.Text);
            timer1.Start();
            System.Threading.ThreadPool.QueueUserWorkItem((state) =>
            {
                bruteForce(src, dst);
            });
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            for (int i = 0; i < 100; i++)
            {
                if (i < 50)
                {
                    if (listBox1.Items.Count < i + 1)
                    {
                        listBox1.Items.Add(string.Format("{0}:{1},{2},{3}", i, countersA[0, i], countersA[1, i], countersA[2, i]));
                    }
                    else
                    {
                        listBox1.Items[i] = string.Format("{0}:{1},{2},{3}", i, countersA[0, i], countersA[1, i], countersA[2, i]);
                    }

                    if (listBox2.Items.Count < i + 1)
                    {
                        listBox2.Items.Add(string.Format("{0}:{1},{2},{3}", i, countersB[0, i], countersB[1, i], countersB[2, i]));
                    }
                    else
                    {
                        listBox2.Items[i] = string.Format("{0}:{1},{2},{3}", i, countersB[0, i], countersB[1, i], countersB[2, i]);
                    }
                }
                else
                {
                    if (listBox3.Items.Count < i + 1 - 50)
                    {
                        listBox3.Items.Add(string.Format("{0}:{1},{2},{3}", i, countersA[0, i], countersA[1, i], countersA[2, i]));
                    }
                    else
                    {
                        listBox3.Items[i - 50] = string.Format("{0}:{1},{2},{3}", i, countersA[0, i], countersA[1, i], countersA[2, i]);
                    }

                    if (listBox4.Items.Count < i + 1 - 50)
                    {
                        listBox4.Items.Add(string.Format("{0}:{1},{2},{3}", i, countersB[0, i], countersB[1, i], countersB[2, i]));
                    }
                    else
                    {
                        listBox4.Items[i - 50] = string.Format("{0}:{1},{2},{3}", i, countersB[0, i], countersB[1, i], countersB[2, i]);
                    }
                }
            }
        }
    }
}
