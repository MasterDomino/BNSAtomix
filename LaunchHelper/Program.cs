using System;
using System.Windows.Forms;
using System.IO;

namespace LaunchHelper
{
    internal static class Program
    {
        public static string session, nick, mac, company, game, path, arg, startup;

        /// <summary>
        /// 应用程序的主入口点。
        /// </summary>
        /// <param name="arg"></param>
        [STAThread]
        private static void Main(string[] arg)
        {
            Program.arg = string.Empty;
            bool wait = false;
            foreach (string i in arg)
            {
                if (string.Equals(i, "waitgame", StringComparison.CurrentCultureIgnoreCase))
                {
                    wait = true;
                    continue;
                }
                Program.arg += " " + i;
                startup = Path.GetDirectoryName(Application.ExecutablePath);
                if (i.Contains("/SessKey:"))
                {
                    session = i.Replace("/SessKey:", string.Empty);
                }
                if (i.Contains("/UserNick:"))
                {
                    nick = i.Replace("/UserNick:", string.Empty);
                }
                if (i.Contains("/MacAddr:"))
                {
                    mac = i.Replace("/MacAddr:", string.Empty);
                }
                if (i.Contains("/CompanyID:"))
                {
                    company = i.Replace("/CompanyID:", string.Empty);
                }
                if (i.Contains("/StartGameID:"))
                {
                    game = i.Replace("/StartGameID:", string.Empty);
                }
            }
            if (!wait)
            {
                return;
            }
            if (File.Exists(startup + "\\path.txt"))
            {
                StreamReader sr = new StreamReader(startup + "\\path.txt", System.Text.Encoding.Unicode);
                path = sr.ReadLine();
                sr.Close();
            }
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Form1());
        }
    }
}
