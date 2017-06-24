using System;
using System.Xml;

using SmartEngine.Network;
using SagaBNS.Common.Packets;
using SagaBNS.LoginServer.Network.Client;

namespace SagaBNS.LoginServer.Packets.Client
{
    public class CM_AUTH_KEY_DATA : BNSLoginPacket
    {
        public CM_AUTH_KEY_DATA()
        {
            ID = LoginPacketOpcode.CM_AUTH_KEY_DATA;
        }

        public override Packet<LoginPacketOpcode> New()
        {
            return new CM_AUTH_KEY_DATA();
        }

        public override void OnProcess(Session<LoginPacketOpcode> client)
        {
            XmlDocument xml = ReadLoginPacket();
            XmlElement root;
            XmlNodeList list;
            byte[] keyData = null;
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
                    case "keydata":
                        keyData = Convert.FromBase64String(i.InnerText);
                        break;
                }
            }
            xml = null;
            System.IO.MemoryStream ms = new System.IO.MemoryStream(keyData);
            System.IO.BinaryReader br = new System.IO.BinaryReader(ms);
            byte[] exchangeKey = br.ReadBytes(br.ReadInt32());
            byte[] hash = br.ReadBytes(br.ReadInt32());
            string checkHash = Convert.ToBase64String(hash);

            ((LoginSession)client).OnAuthKeyData(exchangeKey, checkHash, Serial);
        }
    }
}
