using System;
using System.Xml;
using SmartEngine.Network;
using SmartEngine.Network.Utils;
using SmartEngine.Core;

namespace SagaBNS.AuctionServer
{
    public class Configuration : Singleton<Configuration>
    {
        private string accountHost, characterHost, accountPassword = "", characterPassword = string.Empty;
        private int accountPort, characterPort, port, loglevel;
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

        public void Initialization(string path)
        {
            XmlDocument xml = new XmlDocument();
            try
            {
                XmlElement root;
                XmlNodeList list;
                xml.Load(path);
                root = xml["AuctionServer"];
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

                        case "encryptionkeypair":
                            {
                                string handshake = i.Attributes[0].Value.Substring(260);
                                foreach (object l in i.ChildNodes)
                                {
                                    XmlElement k = l as XmlElement;
                                    if (k == null)
                                    {
                                        continue;
                                    }

                                    switch (k.Name.ToLower())
                                    {
                                        case "key":
                                            byte[][] keypair = new byte[2][];
                                            keypair[0] = Conversions.HexStr2Bytes(k.Attributes[0].Value);
                                            keypair[1] = Conversions.HexStr2Bytes(k.InnerText.Replace("\r", "").Replace("\n", "").Replace(" ", ""));
                                            Common.BNSGameNetwork<Common.Packets.LobbyPacketOpcode>.AddKeyPair(handshake, keypair);
                                            break;
                                    }
                                }
                            }
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
