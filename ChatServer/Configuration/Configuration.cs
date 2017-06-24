using System;
using System.Xml;

using SmartEngine.Core;
using SmartEngine.Network;

namespace SagaBNS.ChatServer
{
    public class Configuration : Singleton<Configuration>
    {
        private int port;
        /// <summary>
        /// 聊天服务器的监听端口
        /// </summary>
        public int Port { get { return port; } }

        public void Initialization(string path)
        {
            XmlDocument xml = new XmlDocument();
            try
            {
                XmlElement root;
                XmlNodeList list;
                xml.Load(path);
                root = xml["ChatServer"];
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
