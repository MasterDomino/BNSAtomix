using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Unpacker.Table
{
    public enum FieldType
    {
        Integer = 3,
        Byte = 1,
        String = 7,
        Short8 = 8,
        Short = 2,
        Short6 = 6,
        Integer22 = 22,
        Long = 9,//8 bytes
        SizedString = 25,
        Type27 = 27,//8+4
        Array,
    }

    public class TableSchema
    {
        private readonly Dictionary<string, Field> fields = new Dictionary<string, Field>();
        public string Name { get; set; }
        public string ElementName { get; set; }
        public Dictionary<string, Field> Fields { get { return fields; } }

        public override string ToString()
        {
            return string.Format("{0}:{1} fields", Name, fields.Count);
        }
    }

    public class Field
    {
        public string Name { get; set; }
        public FieldType Type { get; set; }
        public FieldType SubType { get; set; }
        public int Size { get; set; }
        public bool Export { get; set; }

        public Field()
        {
            SubType = FieldType.Byte;
        }

        public override string ToString()
        {
            return Size == 0 ? string.Format("{0}:{1}", Name, Type) : string.Format("{0}:{1} size:{2}", Name, Type, Size);
        }
    }
}
