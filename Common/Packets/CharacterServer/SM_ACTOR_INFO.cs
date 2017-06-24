using SagaBNS.Common.Actors;
using SagaBNS.Common.Network;
using SmartEngine.Network;

namespace SagaBNS.Common.Packets.CharacterServer
{
    public class SM_ACTOR_INFO : Packet<CharacterPacketOpcode>
    {
        #region Instantiation

        public SM_ACTOR_INFO()
        {
            ID = CharacterPacketOpcode.SM_ACTOR_INFO;
        }

        #endregion

        #region Properties

        public ActorPC Character
        {
            get
            {
                return new ActorPC()
                {
                    CharID = GetUInt(),
                    AccountID = GetUInt(),
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
            }
            set
            {
                PutUInt(value.CharID);
                PutUInt(value.AccountID);
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
                PutUInt(value.Exp);
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

        #endregion

        #region Classes

        internal class SM_ACTOR_INFO_INTERNAL<T> : SM_ACTOR_INFO
        {
            #region Methods

            public override Packet<CharacterPacketOpcode> New()
            {
                return new SM_ACTOR_INFO_INTERNAL<T>();
            }

            public override void OnProcess(Session<CharacterPacketOpcode> client)
            {
                ((CharacterSession<T>)client).OnActorInfo(this);
            }

            #endregion
        }

        #endregion
    }
}