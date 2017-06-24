using System;
using System.Xml;

using SmartEngine.Network;
using SagaBNS.Common;
using SagaBNS.Common.Packets;
using SagaBNS.LoginServer.Network.Client;

namespace SagaBNS.LoginServer.Packets.Client
{
    public class CM_CHAR_CREATE : BNSLoginPacket
    {
        public CM_CHAR_CREATE()
        {
            ID = LoginPacketOpcode.CM_CHAR_CREATE;
        }

        public override Packet<LoginPacketOpcode> New()
        {
            return new CM_CHAR_CREATE();
        }

        public override void OnProcess(Session<LoginPacketOpcode> client)
        {
            XmlDocument xml = ReadLoginPacket();
            XmlElement root;
            XmlNodeList list;
            byte[] keyData = null;
            root = xml["bns"];
            list = root.ChildNodes;
            byte slotID = 0;
            string charName = string.Empty;
            byte[] charData = null;
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
                    case "slotid":
                        Guid id = Guid.Parse(i.InnerText);
                        for (slotID = 0; slotID < 5; slotID++)
                        {
                            if (((uint)slotID).ToGUID() == id)
                            {
                                break;
                            }
                        }
                        break;
                    case "charname":
                        charName = i.InnerText;
                        break;
                    case "chardata":
                        charData = Convert.FromBase64String(i.InnerText);
                        break;
                }
            }
            xml = null;
            ((LoginSession)client).OnCharCreate(GetInt(2), WorldID, slotID, charName, charData);
        }
    }
}
