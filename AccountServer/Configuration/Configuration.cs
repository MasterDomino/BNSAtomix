using System;
using System.Xml;
using SmartEngine.Core;
using SmartEngine.Network;

namespace SagaBNS.AccountServer
{
    public class Configuration : Singleton<Configuration>
    {
        private string dbhost, dbuser, dbpass, dbname, language, password;
        private int dbport, port, loglevel;

        /// <summary>
        /// Mysql服务器地址
        /// </summary>
        public string DBHost { get { return dbhost; } }

        /// <summary>
        /// Mysql服务器端口
        /// </summary>
        public int DBPort { get { return dbport; } }

        /// <summary>
        /// Mysql数据库用户名
        /// </summary>
        public string DBUser { get { return dbuser; } }

        /// <summary>
        /// Mysql数据库密码
        /// </summary>
        public string DBPassword { get { return dbpass; } }

        /// <summary>
        /// Mysql数据库名称
        /// </summary>
        public string DBName { get { return dbname; } }

        /// <summary>
        /// 内部通讯连接密码
        /// </summary>
        public string Password { get { return password; } }

        /// <summary>
        /// 监听端口
        /// </summary>
        public int Port { get { return port; } }

        /// <summary>
        /// 日志等级
        /// </summary>
        public int LogLevel { get { return loglevel; } }

        public void Initialization(string path)
        {
            XmlDocument xml = new XmlDocument();
            try
            {
                XmlElement root;
                XmlNodeList list;
                xml.Load(path);
                root = xml["AccountServer"];
                list = root.ChildNodes;
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
                        case "port":
                            port = int.Parse(i.InnerText);
                            break;
                        case "dbhost":
                            dbhost = i.InnerText;
                            break;
                        case "dbport":
                            dbport = int.Parse(i.InnerText);
                            break;
                        case "dbuser":
                            dbuser = i.InnerText;
                            break;
                        case "dbpass":
                            dbpass = i.InnerText;
                            break;
                        case "dbname":
                            dbname = i.InnerText;
                            break;
                        case "password":
                            password = i.InnerText;
                            break;
                        case "loglevel":
                            loglevel = int.Parse(i.InnerText);
                            break;
                    }
                }
                Logger.Log.Info("Done reading configuration...");
                xml = null;
            }
            catch (Exception ex)
            {
                Logger.Log.Error(ex);
            }
        }
    }
}
