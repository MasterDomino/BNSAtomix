using System;
using System.Collections.Generic;
using System.Xml;

using SmartEngine.Core;
using SmartEngine.Network;

namespace SagaBNS.LoginServer
{
    public class Configuration : Singleton<Configuration>
    {
        private string accountHost, characterHost, accountPassword = "", characterPassword = string.Empty;
        private int accountPort, characterPort, port, loglevel;
        private readonly List<WorldInfo> worlds = new List<WorldInfo>();
        /// <summary>
        /// 帐号服务器的域名或IP
        /// </summary>
        public string AccountHost { get { return accountHost; } }

        /// <summary>
        /// 帐号服务器的监听端口
        /// </summary>
        public int AccountPort { get { return accountPort; } }

        /// <summary>
        /// 帐号服务器的内部通讯密码
        /// </summary>
        public string AccountPassword { get { return accountPassword; } }

        /// <summary>
        /// 人物服务器的域名或IP
        /// </summary>
        public string CharacterHost { get { return characterHost; } }

        /// <summary>
        /// 人物服务器的监听端口
        /// </summary>
        public int CharacterPort { get { return characterPort; } }

        /// <summary>
        /// 人物服务器的内部通讯密码
        /// </summary>
        public string CharacterPassword { get { return characterPassword; } }

        /// <summary>
        /// 登陆服务器的监听端口
        /// </summary>
        public int Port { get { return port; } }

        /// <summary>
        /// 日志等级
        /// </summary>
        public int LogLevel { get { return loglevel; } }

        public List<WorldInfo> Worlds { get { return worlds; } }
        public void Initialization(string path)
        {
            XmlDocument xml = new XmlDocument();
            try
            {
                XmlElement root;
                XmlNodeList list;
                xml.Load(path);
                root = xml["LoginServer"];
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
                        case "accountport":
                            accountPort = int.Parse(i.InnerText);
                            break;
                        case "accounthost":
                            accountHost = i.InnerText;
                            break;
                        case "accountpassword":
                            accountPassword = i.InnerText;
                            break;
                        case "characterport":
                            characterPort = int.Parse(i.InnerText);
                            break;
                        case "characterhost":
                            characterHost = i.InnerText;
                            break;
                        case "characterpassword":
                            characterPassword = i.InnerText;
                            break;
                        case "loglevel":
                            loglevel = int.Parse(i.InnerText);
                            break;
                        case "world":
                            {
                                WorldInfo info = new WorldInfo();
                                foreach (object l in i.ChildNodes)
                                {
                                    XmlElement k = l as XmlElement;
                                    if (k == null)
                                    {
                                        continue;
                                    }

                                    switch (k.Name.ToLower())
                                    {
                                        case "name":
                                            info.Name = k.InnerText;
                                            break;
                                        case "publicaddress":
                                            info.Address = k.InnerText;
                                            break;
                                    }
                                }
                                worlds.Add(info);
                            }
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
