using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Unpacker.Table
{
    internal class Table
    {
        public byte Unknown1 { get; set; }
        public byte Unknown2 { get; set; }
        public byte Unknown3 { get; set; }
        public byte Unknown4 { get; set; }
        public string Name { get; set; }
        public int Size { get; set; }
        public int EntryCount { get; set; }
        public int EntryOffset { get; set; }
        public int EntrySize { get; set; }
        public int DataOffset { get; set; }
        public int DataSize { get; set; }

        public override string ToString()
        {
            return string.Format("{0},U1:{1},U2:{2}", Name, Unknown1, Unknown2);
        }
    }

    public class Entry
    {
        private readonly List<int> offsets = new List<int>();
        private readonly List<string> values=new List<string>();
        public int Unknown1 { get; set; }
        public short Unknown2 { get; set; }
        public short Unknown3 { get; set; }
        public int Size { get; set; }
        public int Index { get; set; }
        public int Index2 { get; set; }
        public List<int> Offsets { get { return offsets; } }
        public List<string> Values { get { return values; } }
    }
}
