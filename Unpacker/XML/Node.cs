using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using System.Text;

namespace Unpacker.XML
{
    public static class Node
    {
        public static void ToStream(System.IO.BinaryWriter bw, XmlElement node, int index = 1)
        {
            if (node.Attributes.Count > 0)
            {
                bw.Write(node.Attributes.Count);
                foreach (XmlAttribute i in node.Attributes)
                {
                    bw.Write(i.Name.Length);
                    bw.Write(Encoding.Unicode.GetBytes(i.Name));
                    bw.Write(i.Value.Length);
                    bw.Write(Encoding.Unicode.GetBytes(i.Value));
                }
            }
            bw.Write((byte)1);
            bw.Write(node.Name.Length);
            bw.Write(Encoding.Unicode.GetBytes(node.Name));
            bw.Write(node.ChildNodes.Count);
            bw.Write(index);
            if (node.Attributes.Count > 0)
            {
                foreach (XmlElement i in node.ChildNodes)
                {
                    if (node.ParentNode.ParentNode == null)
                    {
                        index++;
                    }

                    if (i.Attributes.Count > 0)
                    {
                        bw.Write(1);
                        ToStream(bw, i, index);
                    }
                    else
                    {
                        bw.Write(2);
                        bw.Write(i.InnerText.Length);
                        bw.Write(Encoding.Unicode.GetBytes(i.InnerText));
                        ToStream(bw, i, index);
                    }
                }
            }
        }

        public static XmlElement FromStream(System.IO.BinaryReader br, XmlDocument xml, bool hasAttributes = true)
        {
            //node.Information = Encoding.Unicode.GetString(br.ReadBytes(br.ReadInt32()*2));
            Dictionary<string, string> attributes = new Dictionary<string, string>();
            if (hasAttributes)
            {
                int attrCount = br.ReadInt32();
                for (int i = 0; i < attrCount; i++)
                {
                    attributes[Encoding.Unicode.GetString(br.ReadBytes(br.ReadInt32() * 2))] = Encoding.Unicode.GetString(br.ReadBytes(br.ReadInt32() * 2));
                }
            }
            Debug.Assert(br.ReadByte() == 1);
            XmlElement node = xml.CreateElement(Encoding.Unicode.GetString(br.ReadBytes(br.ReadInt32() * 2)));
            foreach (string i in attributes.Keys)
            {
                XmlAttribute attr = xml.CreateAttribute(i);
                attr.Value = attributes[i];
                node.Attributes.Append(attr);
            }
            int ChildrenCount = br.ReadInt32();
            int Index = br.ReadInt32();
            for (int i = 0; i < ChildrenCount; i++)
            {
                int type = br.ReadInt32();
                if (type == 1)
                {
                    node.AppendChild(Node.FromStream(br, xml));
                }
                else if (type == 2)
                {
                    string unknown = Encoding.Unicode.GetString(br.ReadBytes(br.ReadInt32() * 2));
                    XmlElement child = Node.FromStream(br, xml, false);
                    child.InnerText = unknown;
                    node.AppendChild(child);
                }
                else
                {
                    Debug.Assert(false);
                }

                if (br.BaseStream.Position == br.BaseStream.Length)
                {
                    break;
                }
            }
            return node;
        }
    }
}
