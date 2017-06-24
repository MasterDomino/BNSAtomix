using System.Xml;

using SmartEngine.Network;
using SagaBNS.Common.Packets;
using SagaBNS.LoginServer.Network.Client;

namespace SagaBNS.LoginServer.Packets.Client
{
    public class CM_AUTH_LOGIN_START : BNSLoginPacket
    {
        public CM_AUTH_LOGIN_START()
        {
            ID = LoginPacketOpcode.CM_AUTH_LOGIN_START;
        }

        public override Packet<LoginPacketOpcode> New()
        {
            return new CM_AUTH_LOGIN_START();
        }

        public override void OnProcess(Session<LoginPacketOpcode> client)
        {
            XmlDocument xml = ReadLoginPacket();
            XmlElement root;
            XmlNodeList list;
            string loginName = string.Empty;
            root = xml["Request"];
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
                    case "loginname":
                        loginName = i.InnerText;
                        break;
                }
            }
            xml = null;

            ((LoginSession)client).OnAuthLoginStart(loginName, Serial);
        }
    }
}
