using System;

using SmartEngine.Network.Utils;
namespace SagaBNS.Common.Actors
{
    public enum StanceU1 : long
    {
        Unknown4 = 0x4,
        Poisen = 0x20,
        Dash = 0x40,
        Unknown200 = 0x200,
        Down1 = 0x1000,
        Unknown2000 = 0x2000,
        Unknown40000 = 0x40000,
        Unknown400000 = 0x400000,
        Stun = 0x800000,
        NoMove = 0x2000000,
        Unknown10000000 = 0x10000000,
        TakenDown = 0x100000000,
        Unknown200000000000 = 0x200000000000,
        Unknown400000000000 = 0x400000000000,
    }

    public enum StanceU2 : long
    {
        TakenDown1 = 0x2000000,
        TakenDown2 = 0x400000000000,
    }

    public class Status
    {
        public int MaxHPExt;
        public int MaxHPExt2;
        public int AtkMin;
        public int AtkMinBase = 6;
        public int AtkMinExt;
        public int AtkMinExt2;
        public int AtkMax;
        public int AtkMaxBase = 10;
        public int AtkMaxExt;
        public int AtkMaxExt2;
        public int Penetration;
        public int PenetrationBase;
        public int PenetrationExt;
        public int Pierce;
        public int PierceBase;
        public int PierceExt;
        public int PierceExt2;
        public int Hit;
        public int HitBase = 8;
        public int HitExt;
        public int HitExt2;
        public int Critical;
        public int CriticalBase = 2;
        public int CriticalExt;
        public int CriticalExt2;
        public int Practice;
        public int PracticeBase;
        public int PracticeExt;
        public int Defence;
        public int DefenceBase = 3;
        public int DefenceExt;
        public int DefenceExt2;
        public int Parry;
        public int ParryBase;
        public int ParryExt;
        public int ParryExt2;
        public int Avoid;
        public int AvoidBase = 1;
        public int AvoidExt;
        public int AvoidExt2;
        public int CriticalResist;
        public int CriticalResistBase;
        public int CriticalResistExt;
        public int CriticalResistExt2;
        public int Tough;
        public int ToughBase;
        public int ToughExt;
        public bool CastingSkill;
        public bool IsInCombat;
        public bool Blocking;
        public bool Counter;
        public bool Dead, Dying, Recovering, Down, Stun, TakeDown, TakenDown, Stealth, ShouldLoadMap, ShouldRespawn, Frosen, Catch, Invincible, Dummy;
        public ulong InteractWith;
        public Stances Stance;
        public BitMask64<StanceU1> StanceFlag1 = new BitMask64<StanceU1>();
        public BitMask64<StanceU2> StanceFlag2 = new BitMask64<StanceU2>();
        public ulong CorpseActorID;
        public DateTime SkillCooldownEnd;
        public uint LastSkillID;
        public int DisappearEffect;

        public Status()
        {
            SkillCooldownEnd = DateTime.Now;
        }

        public void ClearStatus()
        {
            AtkMax = 0;
            AtkMin = 0;
            Penetration = 0;
            Pierce = 0;
            Hit = 0;
            Critical = 0;
            Practice = 0;
            Defence = 0;
            Parry = 0;
            Avoid = 0;
            CriticalResist = 0;
            Tough = 0;
        }

        public void ClearBaGuaBonus()
        {
            MaxHPExt2 = 0;
            AtkMinExt2 = 0;
            AtkMaxExt2 = 0;
            PierceExt2 = 0;
            HitExt2 = 0;
            CriticalExt2 = 0;
            DefenceExt2 = 0;
            ParryExt2 = 0;
            AvoidExt2 = 0;
            CriticalResistExt2 = 0;
        }
    }
}
