using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Threading.Tasks;

using Mono.Math;
namespace BruteForce
{
    public partial class Form1 : Form
    {
        public static BigInteger N = new BigInteger("E306EBC02F1DC69F5B437683FE3851FD9AAA6E97F4CBD42FC06C72053CBCED68EC570E6666F529C58518CF7B299B5582495DB169ADF48ECEB6D65461B4D7C75DD1DA89601D5C498EE48BB950E2D8D5E0E0C692D613483B38D381EA9674DF74D67665259C4C31A29E0B3CFF7587617260E8C58FFA0AF8339CD68DB3ADB90AAFEE");
        private readonly List<BigInteger> list = new List<BigInteger>();
        private readonly List<BigInteger> border = new List<BigInteger>();
        private BigInteger step;
        private readonly BigInteger Two = new BigInteger(2);
        private readonly bool found;
        private BigInteger result;
        private BigInteger Max;
        private DateTime startTime;
        private bool ended;
        private BigInteger Module;
        private bool shouldBreak;
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            this.FormClosing += new FormClosingEventHandler(Form1_FormClosing);
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (timer1.Enabled)
            {
                if (SD.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    shouldBreak = true;
                    timer1.Stop();
                    System.IO.FileStream fs = new System.IO.FileStream(SD.FileName, System.IO.FileMode.Create);
                    System.IO.BinaryWriter bw = new System.IO.BinaryWriter(fs);
                    bw.Write(Module.getBytes().Length);
                    bw.Write(Module.getBytes());
                    bw.Write(list.Count);
                    foreach (BigInteger i in list)
                    {
                        bw.Write(i.getBytes().Length);
                        bw.Write(i.getBytes());
                    }
                    foreach (BigInteger i in border)
                    {
                        bw.Write(i.getBytes().Length);
                        bw.Write(i.getBytes());
                    }
                    fs.Close();
                }
            }
        }

        private void btnStart_Click(object sender, EventArgs e)
        {
            ParallelOptions option = new ParallelOptions()
            {
                MaxDegreeOfParallelism = int.Parse(tb_Threads.Text)
            };
            byte[] tmp = new byte[32];
            for (int i = 0; i < 32; i++)
            {
                tmp[i] = 0xFF;
            }

            Max = new BigInteger(tmp);
            byte[] keys = Convert.FromBase64String(tb_Input.Text);
            
            using (System.IO.MemoryStream ms2 = new System.IO.MemoryStream(keys))
            {
                using (System.IO.BinaryReader br3 = new System.IO.BinaryReader(ms2))
                {
                    Module = new BigInteger(br3.ReadBytes(br3.ReadInt32()));
                }
            }
            
            ConcurrentQueue<BigInteger> queue = new ConcurrentQueue<BigInteger>();
            BigInteger counter = new BigInteger(0);
            step = Max / int.Parse(tb_Truc.Text);
            Action[] actions = new Action[int.Parse(tb_Truc.Text)];
            for (int i = 0; i < int.Parse(tb_Truc.Text); i++)
            {
                BigInteger big=new BigInteger(counter);
                queue.Enqueue(big);
                list.Add(big);
                border.Add(counter);
                counter += step;
                actions[i] = () =>
                {
                    while (!shouldBreak && queue.Count > 0)
                    {
                        if (queue.TryDequeue(out BigInteger current))
                        {
                            BigInteger stop = current + step;
                            int index = list.IndexOf(current);
                            for (; (current < stop) && !shouldBreak; current += 1)
                            {
                                list[index] = current;

                                if (Two.modPow(current, N) == Module)
                                {
                                    shouldBreak = true;
                                    result = current;
                                    break;
                                }
                            }
                        }
                    }
                };
            }

            timer1.Start();
            startTime = DateTime.Now;
            btnStart.Enabled = false;
            System.Threading.ThreadPool.QueueUserWorkItem((state) =>
            {
                shouldBreak = false;
                ended = false;
                Parallel.Invoke(option, actions);
                ended = true;
            });
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            BigInteger counter = new BigInteger(0);
            for (int i = 0; i < list.Count; i++)
            {
                counter += (list[i] - border[i]);
            }
            //counter = (counter * 1000) / Max;
            lbCur.Text = counter.ToString();
            lbMax.Text = Max.ToString();
            counter = (counter * 10000) / Max;
            int perc = int.Parse(counter.ToString());
            float percent = ((float)perc) / 100;
            lb_Perc.Text = string.Format("{0:0.00}", percent);
            TimeSpan span = DateTime.Now - startTime;
            lbTime.Text = span.ToString();
            if (percent > 0.01f)
            {
                TimeSpan rem = new TimeSpan(0, 0, 0, (int)(span.TotalSeconds / percent));
                lb_Remain.Text = rem.ToString();
            }
            if (found)
            {
                timer1.Stop();
                tbResult.Text = Convert.ToBase64String(result.getBytes());
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            ParallelOptions option = new ParallelOptions()
            {
                MaxDegreeOfParallelism = int.Parse(tb_Threads.Text)
            };
            byte[] tmp = new byte[32];
            for (int i = 0; i < 32; i++)
            {
                tmp[i] = 0xFF;
            }

            Max = new BigInteger(tmp);
            if (OD.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                System.IO.FileStream fs = new System.IO.FileStream(OD.FileName, System.IO.FileMode.Open);
                System.IO.BinaryReader br = new System.IO.BinaryReader(fs);
                Module = new BigInteger(br.ReadBytes(br.ReadInt32()));
                tb_Input.Text = Convert.ToBase64String(Module.getBytes());
                int count = br.ReadInt32();
                step = Max / count;
                ConcurrentQueue<BigInteger> queue = new ConcurrentQueue<BigInteger>();
                Action[] actions = new Action[count];
                for (int i = 0; i < count; i++)
                {
                    BigInteger big = new BigInteger(br.ReadBytes(br.ReadInt32()));
                    queue.Enqueue(big);
                    list.Add(big);
                    actions[i] = () =>
                    {
                        while (!shouldBreak && queue.Count > 0)
                        {
                            if (queue.TryDequeue(out BigInteger current))
                            {
                                int index = list.IndexOf(current);
                                BigInteger stop = border[index] + step;

                                for (; (current < stop) && !shouldBreak; current += 1)
                                {
                                    list[index] = current;

                                    if (Two.modPow(current, N) == Module)
                                    {
                                        shouldBreak = true;
                                        result = current;
                                        break;
                                    }
                                }
                            }
                        }
                    };
                }
                for (int i = 0; i < count; i++)
                {
                    BigInteger big = new BigInteger(br.ReadBytes(br.ReadInt32()));
                    border.Add(big);
                }
                timer1.Start();
                startTime = DateTime.Now;
                btnStart.Enabled = false;
                System.Threading.ThreadPool.QueueUserWorkItem((state) =>
                {
                    ended = false;
                    shouldBreak = false;
                    Parallel.Invoke(option, actions);
                    ended = true;
                });
            }
        }
    }
}
