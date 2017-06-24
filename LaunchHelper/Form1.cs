using System;
using System.IO;
using System.Text;
using System.Windows.Forms;

namespace LaunchHelper
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            textBox1.Text = Program.path;            
        }

        private void button3_Click(object sender, EventArgs e)
        {
            if (FD.ShowDialog() == DialogResult.OK)
            {
                if (File.Exists(FD.SelectedPath + "\\bin\\Client.exe"))
                {
                    textBox1.Text = FD.SelectedPath;
                    StreamWriter sw = new StreamWriter(Program.startup + "\\path.txt", false, Encoding.Unicode);
                    sw.WriteLine(textBox1.Text);
                    sw.Flush();
                    sw.Close();
                }
                else
                {
                    MessageBox.Show("Please use proper game installation folder", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);                
                }
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (!System.IO.File.Exists(textBox1.Text + "\\bin\\Client.exe"))
            {
                MessageBox.Show("Please use proper game installation folder", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            string arg = $"/LaunchByLauncher /SessKey:{Program.session} /MacAddr:{Program.mac} /UserNick:{Program.nick} /CompanyID:{Program.company} /ChannelGroupIndex:-1 /ServerAddr:  /StartGameID:{Program.game} /RepositorySub:  /GamePath:{textBox1.Text}";
            System.Diagnostics.Process process = System.Diagnostics.Process.Start(textBox1.Text + "\\bin\\Client.exe", arg);
            Close();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process process = System.Diagnostics.Process.Start(Program.startup + "\\PlayNCLauncher_ori.exe", Program.arg);
            Close();
        }
    }
}
