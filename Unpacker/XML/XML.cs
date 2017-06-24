using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace Unpacker.XML
{
    public static class XML
    {
        public static void ToStream(System.IO.Stream stream, XmlDocument xml, string path)
        {
            System.IO.BinaryWriter bw = new System.IO.BinaryWriter(stream);
            bw.Write(Encoding.ASCII.GetBytes("LMXBOSLB"));
            bw.Write(2);
            long lenPos = stream.Position;
            stream.Position += 68;
            bw.Write((byte)1);
            bw.Write(path.Length);
            bw.Write(Encoding.Unicode.GetBytes(path));
            Node.ToStream(bw, xml.ChildNodes[0] as XmlElement);
            long endPos = stream.Position;
            stream.Position = lenPos;
            bw.Write((int)(endPos - lenPos) + 12);
            stream.Position = endPos;
        }

        public static XmlDocument FromStream(System.IO.Stream stream)
        {
            XmlDocument xml = new XmlDocument();
            System.IO.BinaryReader br = new System.IO.BinaryReader(stream);
            stream.Position += 12;
            int length = br.ReadInt32() - 80;
            stream.Position += 64;
            Debug.Assert(br.ReadByte() == 1);
            string FileName = Encoding.Unicode.GetString(br.ReadBytes(br.ReadInt32() * 2));
            xml.AppendChild(Node.FromStream(br, xml));
            Debug.Assert(stream.Position == stream.Length);
            return xml;
        }
    }
}
