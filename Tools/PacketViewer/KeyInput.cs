using System;
using System.Windows.Forms;

namespace PacketViewer
{
    public partial class KeyInput : Form
    {
        private string exchangeKey;

        public string ExchangeKey
        {
            get
            {
                return exchangeKey;
            }
            set
            {
                exchangeKey = value;
                textBox1.Text = value;
            }
        }

        public string ServerHost
        {
            set
            {
                lb_Server.Text = value;
            }
        }

        public string ExchangeVersion
        {
            set
            {
                textBox3.Text = value;
            }
        }

        public string Key
        {
            get
            {
                return textBox2.Text;
            }
        }

        public KeyInput()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}
