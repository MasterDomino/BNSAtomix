
using SmartEngine.Network;
using SagaBNS.Common.Actors;

namespace SagaBNS.Common.Packets.CharacterServer
{
    public class CM_CHAR_SAVE : Packet<CharacterPacketOpcode>
    {
        public CM_CHAR_SAVE()
        {
            ID = CharacterPacketOpcode.CM_CHAR_SAVE;
        }

        public long SessionID
        {
            get
            {
                return GetLong(2);
            }
            set
            {
                PutLong(value, 2);
            }
        }

        public ActorPC Character
        {
            get
            {
                ActorPC pc = new ActorPC()
                {
                    AccountID = GetUInt(10),
                    CharID = GetUInt(),
                    SlotID = GetByte(),
                    WorldID = GetByte(),
                    Name = GetString(),
                    Level = GetByte(),
                    Exp = GetUInt(),
                    Race = (Race)GetByte(),
                    Gender = (Gender)GetByte(),
                    Job = (Job)GetByte(),
                    Appearence1 = GetBytes(GetByte()),
                    Appearence2 = GetBytes(GetByte()),
                    UISettings = GetString(),
                    PartyID = GetULong(),
                    Offline = GetByte() == 1,
                    HP = GetInt(),
                    MP = GetUShort(),
                    MaxHP = GetInt(),
                    MaxMP = GetUShort(),
                    Gold = GetInt(),
                    MapID = GetUInt(),
                    X = GetShort(),
                    Y = GetShort(),
                    Z = GetShort(),
                    Dir = GetUShort(),
                    InventorySize = GetByte()
                };
                return pc;
            }
            set
            {
                PutUInt(value.AccountID, 10);
                PutUInt(value.CharID);
                PutByte(value.SlotID);
                PutByte(value.WorldID);
                PutString(value.Name);
                PutByte(value.Level);
                PutUInt(value.Exp);
                PutByte((byte)value.Race);
                PutByte((byte)value.Gender);
                PutByte((byte)value.Job);
                PutByte((byte)value.Appearence1.Length);
                PutBytes(value.Appearence1);
                PutByte((byte)value.Appearence2.Length);
                PutBytes(value.Appearence2);
                PutString(value.UISettings);
                PutULong(value.PartyID);
                PutByte(value.Offline ? (byte)1 : (byte)0);
                PutInt(value.HP);
                PutUShort((ushort)value.MP);
                PutInt(value.MaxHP);
                PutUShort(value.MaxMP);
                PutInt(value.Gold);
                PutUInt(value.MapID);
                PutShort((short)value.X);
                PutShort((short)value.Y);
                PutShort((short)value.Z);
                PutUShort(value.Dir);
                PutByte(value.InventorySize);
            }
        }
    }
}
