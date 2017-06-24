using System.Collections.Generic;
using System.Threading;

using SagaBNS.Common.Actors;

using SagaBNS.GameServer.Manager;

namespace SagaBNS.GameServer.PC
{
    public static class Status
    {
        public static void EquipItem(ActorPC pc, Common.Item.Item item,bool add)
        {
            Interlocked.Add(ref pc.Status.AtkMinExt, item.BaseData.PrimaryStats.ContainsKey(Stats.MinAtk) ? (add ? 1 : -1) * item.BaseData.PrimaryStats[Stats.MinAtk] : 0);
            Interlocked.Add(ref pc.Status.AtkMaxExt, item.BaseData.PrimaryStats.ContainsKey(Stats.MaxAtk) ? (add ? 1 : -1) * item.BaseData.PrimaryStats[Stats.MaxAtk] : 0);
            Interlocked.Add(ref pc.Status.HitExt, item.BaseData.PrimaryStats.ContainsKey(Stats.HitB) ? (add ? 1 : -1) * item.BaseData.PrimaryStats[Stats.HitB] : 0);
            Interlocked.Add(ref pc.Status.AvoidExt, item.BaseData.PrimaryStats.ContainsKey(Stats.AvoidB) ? (add ? 1 : -1) * item.BaseData.PrimaryStats[Stats.AvoidB] : 0);
            Interlocked.Add(ref pc.Status.ParryExt, item.BaseData.PrimaryStats.ContainsKey(Stats.ParryB) ? (add ? 1 : -1) * item.BaseData.PrimaryStats[Stats.ParryB] : 0);
            Interlocked.Add(ref pc.Status.CriticalExt, item.BaseData.PrimaryStats.ContainsKey(Stats.CritB) ? (add ? 1 : -1) * item.BaseData.PrimaryStats[Stats.CritB] : 0);
            Interlocked.Add(ref pc.Status.CriticalResistExt, item.BaseData.PrimaryStats.ContainsKey(Stats.DefCritB) ? (add ? 1 : -1) * item.BaseData.PrimaryStats[Stats.DefCritB] : 0);
            Interlocked.Add(ref pc.Status.PierceExt, item.BaseData.PrimaryStats.ContainsKey(Stats.PrcB) ? (add ? 1 : -1) * item.BaseData.PrimaryStats[Stats.PrcB] : 0);
            Interlocked.Add(ref pc.Status.MaxHPExt, item.BaseData.PrimaryStats.ContainsKey(Stats.MaxHp) ? (add ? 1 : -1) * item.BaseData.PrimaryStats[Stats.MaxHp] : 0);
            if (Utils.FindStat(item.Synthesis) != Stats.None
                && Utils.FindStat(item.Synthesis) != Stats.CastTimeB
                && Utils.FindStat(item.Synthesis) != Stats.Defense
                && Utils.FindStat(item.Synthesis) != Stats.MaxAtk
                && Utils.FindStat(item.Synthesis) != Stats.Resist)
            {
                FindSynthesisStat(item, pc, add);
            }
        }

        public static void CalcStatus(ActorPC pc, bool recalcBagua = true)
        {
            Level lv = ExperienceManager.Instance[pc.Level];
            if (pc.Job != Job.Destroyer
                && pc.Job != Job.Shooter
                && pc.Job != Job.Summoner)
            {
                pc.Status.CriticalBase = lv.JobStatus[pc.Job].CriticalBase;
                pc.Status.AvoidBase = lv.JobStatus[pc.Job].DodgeBase;
                pc.Status.ParryBase = lv.JobStatus[pc.Job].ParryBase;
                pc.Status.AtkMinBase = lv.JobStatus[pc.Job].AttackMin;
                pc.Status.AtkMaxBase = lv.JobStatus[pc.Job].AttackMax;
                pc.Status.DefenceBase = lv.JobStatus[pc.Job].Defense;
            }

            if (recalcBagua)
            {
                pc.BaGuaEffect.Clear();
                pc.Status.ClearBaGuaBonus();

                Dictionary<int,int> count = new Dictionary<int,int>();

                /*foreach (KeyValuePair<Common.Inventory.InventoryEquipSlot, Common.Item.Item> i in pc.Inventory.Equipments)
                {
                    if (i.Key == Common.Inventory.InventoryEquipSlot.BaoPai1 ||
                        i.Key == Common.Inventory.InventoryEquipSlot.BaoPai2 ||
                        i.Key == Common.Inventory.InventoryEquipSlot.BaoPai3 ||
                        i.Key == Common.Inventory.InventoryEquipSlot.BaoPai4 ||
                        i.Key == Common.Inventory.InventoryEquipSlot.BaoPai5 ||
                        i.Key == Common.Inventory.InventoryEquipSlot.BaoPai6 ||
                        i.Key == Common.Inventory.InventoryEquipSlot.BaoPai7 ||
                        i.Key == Common.Inventory.InventoryEquipSlot.BaoPai8)
                    {
                        if (i.Value != null)
                        {
                            if (count.ContainsKey(i.Value.BaseData.BaoPaiSetNumber))
                                count[i.Value.BaseData.BaoPaiSetNumber] += 1;
                            else
                                count.Add(i.Value.BaseData.BaoPaiSetNumber, 1);
                        }
                    }
                }

                foreach (KeyValuePair<int, int> i in count)
                {
                    BaGua.BaGuaSet set = BaGua.BaGuaManager.Instance[(uint)i.Key];
                    if (set != null)
                    {
                        Dictionary<short, List<uint>> bonus = set.Bonus;
                        if (bonus != null)
                        {
                            if (i.Value == 8)
                            {
                                pc.BaGuaEffect.AddRange(bonus[8]);
                                pc.BaGuaEffect.AddRange(bonus[5]);
                                pc.BaGuaEffect.AddRange(bonus[3]);
                            }
                            else if (i.Value >= 5)
                            {
                                pc.BaGuaEffect.AddRange(bonus[5]);
                                pc.BaGuaEffect.AddRange(bonus[3]);
                            }
                            else if (i.Value >= 3)
                                pc.BaGuaEffect.AddRange(bonus[3]);
                        }
                    }
                }

                foreach (uint i in pc.BaGuaEffect)
                {
                    Effect.Effect effect = Effect.EffectManager.Instance[i];
                    if (effect != null)
                    {
                        foreach (KeyValuePair<int,int> j in effect.Effects)
                        {
                            FindBaguaStat(j.Key, j.Value, pc);
                        }
                    }
                }*/
            }

            pc.Status.AtkMin = pc.Status.AtkMinBase + pc.Status.AtkMinExt + pc.Status.AtkMinExt2;
            pc.Status.AtkMax = pc.Status.AtkMaxBase + pc.Status.AtkMaxExt + pc.Status.AtkMaxExt2;
            pc.Status.Penetration = pc.Status.PenetrationBase + pc.Status.PenetrationExt;
            pc.Status.Pierce = pc.Status.PierceBase + pc.Status.PierceExt + pc.Status.PierceExt2;
            pc.Status.Hit = pc.Status.HitBase + pc.Status.HitExt + pc.Status.HitExt2;
            pc.Status.Critical = pc.Status.CriticalBase + pc.Status.CriticalExt + pc.Status.CriticalExt2;
            pc.Status.Avoid = pc.Status.AvoidBase + pc.Status.AvoidExt + pc.Status.AvoidExt2;
            pc.Status.Defence = pc.Status.DefenceBase + pc.Status.DefenceExt + pc.Status.DefenceExt2;
            pc.Status.CriticalResist = pc.Status.CriticalResistBase + pc.Status.CriticalResistExt + pc.Status.CriticalResistExt2;
            pc.Status.Parry = pc.Status.ParryBase + pc.Status.ParryExt + pc.Status.ParryExt2;
            //TODO: formula for critical, parry, avoid rate
            CalcHP(pc);
        }

        private static void CalcHP(ActorPC pc)
        {
            Level lv = ExperienceManager.Instance[pc.Level];
            if (lv.JobStatus[pc.Job].HP > 0)
            {
                pc.MaxHP = lv.JobStatus[pc.Job].HP;
                pc.MaxHP += pc.Status.MaxHPExt + pc.Status.MaxHPExt2;
            }
            else
            {
                pc.MaxHP = 500;
                pc.MaxHP += pc.Status.MaxHPExt + pc.Status.MaxHPExt2;
            }
            if (pc.HP > pc.MaxHP)
            {
                Interlocked.Exchange(ref pc.HP, pc.MaxHP);
            }
        }

        public static void FindBaguaStat(int stat,int statvalue, ActorPC pc)
        {
            switch (stat)
            {
                case 159:
                    Interlocked.Add(ref pc.Status.MaxHPExt2, statvalue);
                    break;
                case 169:
                    Interlocked.Add(ref pc.Status.HitExt2, statvalue);
                    break;
                case 170:
                    Interlocked.Add(ref pc.Status.PierceExt2, statvalue);
                    break;
                case 173:
                    Interlocked.Add(ref pc.Status.CriticalExt2, statvalue);
                    break;
                case 175:
                    Interlocked.Add(ref pc.Status.CriticalResistExt2, statvalue);
                    break;
                case 178:
                    Interlocked.Add(ref pc.Status.AvoidExt2, statvalue);
                    break;
                case 180:
                    Interlocked.Add(ref pc.Status.ParryExt2, statvalue);
                    break;
                default:
                    break;
            }
        }

        public static void FindSynthesisStat(Common.Item.Item item,ActorPC pc, bool add)
        {
            switch (item.Synthesis)
            {
                case 0x05:
                    Interlocked.Add(ref pc.Status.HitExt, (add ? 1 : -1) * item.BaseData.SecondaryStats[Stats.HitB]);
                    break;
                case 0x07:
                    Interlocked.Add(ref pc.Status.CriticalExt, (add ? 1 : -1) * item.BaseData.SecondaryStats[Stats.CritB]);
                    break;
                case 0x09:
                    Interlocked.Add(ref pc.Status.CriticalResistExt, (add ? 1 : -1) * item.BaseData.SecondaryStats[Stats.DefCritB]);
                    break;
                case 0x0B:
                    Interlocked.Add(ref pc.Status.AvoidExt, (add ? 1 : -1) * item.BaseData.SecondaryStats[Stats.AvoidB]);
                    break;
                case 0x0D:
                    Interlocked.Add(ref pc.Status.ParryExt, (add ? 1 : -1) * item.BaseData.SecondaryStats[Stats.ParryB]);
                    break;
                case 0x1A:
                    Interlocked.Add(ref pc.Status.MaxHPExt, (add ? 1 : -1) * item.BaseData.SecondaryStats[Stats.MaxHp]);
                    break;
                case 0x20:
                    Interlocked.Add(ref pc.Status.PierceExt, (add ? 1 : -1) * item.BaseData.SecondaryStats[Stats.PrcB]);
                    break;
                default:
                    break; //Shouldn't be reachable
                /*
            case 0x01:
                return Stats.MaxAtk;
            case 0x02:
                return Stats.Defense;
            case 0x03:
                return Stats.Resist;
            case 0x13:
                return Stats.CastTimeB;
                 */
            }
        }
    }
}
