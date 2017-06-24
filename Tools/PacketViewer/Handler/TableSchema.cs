using System;
using System.Collections.Generic;
using SagaBNS.Common.Actors;

namespace PacketViewer.Table
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
        Null,
        Uint,
        Ulong,
        Ushort,
        Bytes,
        GUshort,
        Packet,
        Surplus,
    }

    public class TableSchema
    {
        private readonly Dictionary<string, Field> fields = new Dictionary<string, Field>();
        public int ID { get; set; }
        public string ElementName { get; set; }
        public Dictionary<string, Field> Fields { get { return fields; } }

        public PacketParser.PacketVersion Version { get; set; }

        public override string ToString()
        {
            return string.Format("{0}:{1} fields", ID, fields.Count);
        }
    }

    public class Field
    {
        /// <summary>
        /// 名称
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 类型
        /// </summary>
        public FieldType Type { get; set; }
        public FieldType SubType { get; set; }
        /// <summary>
        /// 当前封包位置
        /// </summary>
        public ushort Position { get; set; }
        /// <summary>
        /// 操作前移动封包位置
        /// </summary>
        public ushort AddPosition { get; set; }
        /// <summary>
        /// 操作后移动封包位置
        /// </summary>
        public ushort AfterAddPosition { get; set; }
        /// <summary>
        /// 下方是否需要循环
        /// </summary>
        public bool While { get; set; }
        /// <summary>
        /// 是否需要合并封包
        /// </summary>
        public bool Merge { get; set; }
        /// <summary>
        /// 是否需要丢弃数据
        /// </summary>
        public short Discard { get; set; }

        /// <summary>
        /// 选择
        /// </summary>
        public bool Switch { get; set; }

        public Dictionary<string, Table.Field> List { get; set; }

        public string text { get; set; }
        public string Return
        {
            get
            {
                switch (Name)
                {
                    case "race":
                        return ((Race)byte.Parse(text)).ToString();
                    case "gender":
                        return ((Gender)byte.Parse(text)).ToString();
                    case "job":
                        return ((Job)byte.Parse(text)).ToString();
                    case "mana":
                        return ((ManaType)byte.Parse(text)).ToString();
                    case "stateId":
                        string state;
                        switch (int.Parse(text))
                        {
                            case 2:
                            case 4:
                                state = "Invisible";
                                break;
                            case 3:
                            case 5:
                                state = "Visible";
                                break;
                            default:
                                state = "Unknown: " + text;
                                break;
                        }
                        return state;
                    case "enabled":
                        return text == "1" ? "Enabled" : "Disabled";
                    case "moveType":
                        return text == "1" ? "true" : "false";
                    case "friendly":
                        return Byte.Parse(text).ToString("X");
                    case "skillAttackResult":
                        return ((SagaBNS.Common.Skills.SkillAttackResult)byte.Parse(text)).ToString();
                    default:
                        return Type == FieldType.Ulong ? string.Format("{0}", ulong.Parse(text).ToString("X16"), text) : text;
                }
            }
            set
            {
                text = value;
            }
        }

        public ulong ActorIDMin { get; set; }
        public ulong ActorIDMax { get; set; }

        /// <summary>
        /// 是否检查
        /// </summary>
        public bool RunMORM { get; set; }
        public ulong RunMin { get; set; }
        public ulong RunMax { get; set; }

        /// <summary>
        /// 循环
        /// </summary>
        public ushort WhileLength { get; set; }
        /// <summary>
        /// 读取总结长度
        /// </summary>
        public ushort Length { get; set; }

        /// <summary>
        /// 上级目录
        /// </summary>
        public Field Fu { get; set; }

        public string OutText { get; set; }
        public string OutTextToTemp { get; set; }
        public string TextTemp { get; set; }

        public string[] InText { get; set; }

        public string ActorType
        {
            get
            {
                string type = "Unknown";
                if (Name == "actorID")
                {
                    if (ulong.Parse(text) > 0x4000000000000 && ulong.Parse(text) < 0x5000000000000)
                    {
                        type = "NPC";
                    }
                    else if (ulong.Parse(text) > 0x2000000000000 && ulong.Parse(text) < 0x3000000000000)
                    {
                        type = "Summon";
                    }
                    else if (ulong.Parse(text) > 0x1000000000000 && ulong.Parse(text) < 0x2000000000000)
                    {
                        type = "PC";
                    }
                }
                return type;
            }
        }

        public Field()
        {
            SubType = FieldType.Byte;
            ActorIDMin = ulong.MinValue;
            ActorIDMax = ulong.MaxValue;
            RunMin = ulong.MinValue;
            RunMax = ulong.MaxValue;
            OutText = string.Empty;
            OutTextToTemp = string.Empty;
        }

        public override string ToString()
        {
            bool temTr = false;
            if (OutText == "")
            {
                temTr = true;
            }

            if (OutTextToTemp == "" && temTr)
            {
                return "";
            }

            List<string> temp = new List<string>();
            if (Type != FieldType.Null)
            {
                temp.Add(Return);
            }

            if (InText != null)
            {
                for (int i = 0; InText.Length > i; i++)
                {
                    if (InText[i] == "actortype")
                    {
                        if (Name == "actorID")
                        {
                            temp.Add(ActorType);
                        }
                        else if (Fu.Name == "actorID")
                        {
                            temp.Add(Fu.ActorType);
                        }
                    }
                    else if (InText[i] == "texttemp")
                    {
                        temp.Add(TextTemp);
                    }

                    if (List?.ContainsKey(InText[i]) == true)
                    {
                        temp.Add(List[InText[i]].Return);
                    }
                    else if (Fu.Name == InText[i])
                    {
                        temp.Add(Fu.Return);
                    }
                    else if (Fu.List?.ContainsKey(InText[i]) == true)
                    {
                        temp.Add(Fu.List[InText[i]].Return);
                    }
                    else
                    {
                        temp.Add("");
                    }
                }
            }

            if (temTr)
            {
                Fu.TextTemp = string.Format(OutText, temp.ToArray());
                return "";
            }
            else
            {
                return string.Format(OutText, temp.ToArray());
            }
        }
    }
}
