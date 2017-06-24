using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Xml;

namespace Translator
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Google.API.Translate.TranslateClient client = new Google.API.Translate.TranslateClient("http://www.sagaeco.com");
            int counter = 0;
            int total = 0;
            if (OD.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                XmlDocument xml = new XmlDocument();
                xml.Load(OD.FileName);
                XmlElement root = xml["text"];
                XmlNodeList list = root.ChildNodes;
                total = list.Count;
                foreach (object j in list)
                {
                    if (j.GetType() != typeof(XmlElement))
                    {
                        continue;
                    }

                    XmlElement i = (XmlElement)j;
                    if (i.Name == "Entry")
                    {
                        XmlNodeList childre = i.ChildNodes;
                        string name = string.Empty;
                        foreach (object l in childre)
                        {
                            if (l.GetType() != typeof(XmlElement))
                            {
                                continue;
                            }

                            XmlElement k = (XmlElement)l;

                            switch (k.Name)
                            {
                                case "Name":
                                    name = k.InnerText;
                                    break;
                                case "Value":
                                    if (name.Contains("UI.") && !name.Contains("UI.Chat.Sy") && k.InnerText.Length <1000 && !string.IsNullOrEmpty(k.InnerText))
                                    {
                                        k.InnerText = client.Translate(k.InnerText, Google.API.Translate.Language.Korean, Google.API.Translate.Language.Japanese);
                                        System.Threading.Thread.Sleep(1000);
                                    }
                                    break;
                            }
                        }
                    }
                    counter++;
                    pb.Value = (counter * 100) / total;
                    Application.DoEvents();
                }
                xml.Save(OD.FileName);
            }
        }
    }
}
