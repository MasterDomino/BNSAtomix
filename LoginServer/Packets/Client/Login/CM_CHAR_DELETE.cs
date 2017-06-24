using System;
using System.Xml;

using SmartEngine.Network;
using SagaBNS.Common.Packets;
using SagaBNS.Common;
using SagaBNS.LoginServer.Network.Client;

namespace SagaBNS.LoginServer.Packets.Client
{
    public class CM_CHAR_DELETE : BNSLoginPacket
    {
        public CM_CHAR_DELETE()
        {
            ID = LoginPacketOpcode.CM_CHAR_DELETE;
        }

        public override Packet<LoginPacketOpcode> New()
        {
            return new CM_CHAR_DELETE();
        }

        public override void OnProcess(Session<LoginPacketOpcode> client)
        {
            XmlDocument xml = ReadLoginPacket();
            XmlElement root;
            XmlNodeList list;
            root = xml["bns"];
            list = root.ChildNodes;
            byte slotID = 0;
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
                }
            }
            xml = null;
            ((LoginSession)client).OnCharDelete(GetInt(2), slotID);
        }
    }
}
