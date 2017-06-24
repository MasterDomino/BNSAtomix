using System.Collections.Generic;

using SmartEngine.Network;

using SagaBNS.GameServer.Skills.SkillHandlers;
using SagaBNS.GameServer.Skills.SkillHandlers.Common;

namespace SagaBNS.GameServer.Skills
{
    public partial class SkillManager : Singleton<SkillManager>
    {
        private readonly Dictionary<uint, ISkillHandler> skillHandlers = new Dictionary<uint, ISkillHandler>();

        public void Init()
        {
            skillHandlers.Add(uint.MaxValue, new SkillHandlers.Common.DefaultAttack());//Default handler
            skillHandlers[10100] = new DefaultAttack(true, true);//Rushing wind
            skillHandlers[10400] = new DefaultAttack(true);//Rushing wind Move
            skillHandlers[10410] = new DefaultAttack(true, true);//Rushing wind NoTarget
            skillHandlers[10420] = new DefaultAttack(true);//Rushing wind NoTarget Move
            skillHandlers[10101] = new DefaultAttack(true, true);//Blood wind
            skillHandlers[10401] = new DefaultAttack(true);//Blood wind Move
            skillHandlers[10411] = new DefaultAttack(true, true);//Blood wind NoTarget
            skillHandlers[10421] = new DefaultAttack(true);//Blood wind NoTarget Move
            skillHandlers[10102] = new DefaultAttack(true, true);//Storm wind
            skillHandlers[10402] = new DefaultAttack(true);//Storm wind Move
            skillHandlers[10412] = new DefaultAttack(true, true);//Storm wind NoTarget
            skillHandlers[10422] = new DefaultAttack(true);//Storm wind NoTarget Move
            skillHandlers[10103] = new DefaultAttack();//Sword stab
            skillHandlers[10105] = new SkillHandlers.BladeMaster.SwordBlocking();//Sword Blocking
            skillHandlers[10108] = new SkillHandlers.BladeMaster.TurningStrike(55101061);//Turning Strike  
            skillHandlers[10113] = new SkillHandlers.BladeMaster.CounterAttack();//Counter Attack
            skillHandlers[10126] = new SkillHandlers.BladeMaster.Rush();//Sword Blocking
            skillHandlers[10127] = new DefaultAttack(false, 20, 0);//Front kick
            skillHandlers[10131] = new SkillHandlers.BladeMaster.CounterAttack();//Counter Attack
            skillHandlers[10226] = new SkillHandlers.BladeMaster.CounterAttack();//Counter Attack
            skillHandlers[10227] = new SkillHandlers.BladeMaster.CounterAttack();//Counter Attack

            skillHandlers[11100] = new DefaultAttack(true, true);//Justice punch
            skillHandlers[11600] = new DefaultAttack(true);//Justice punch Move
            skillHandlers[11603] = new DefaultAttack(true, true);//Justice punch NoTarget
            skillHandlers[11606] = new DefaultAttack(true);//Justice punch NoTarget Move
            skillHandlers[11112] = new SkillHandlers.KungfuMaster.TakeDown(11112013, 11112010);
            skillHandlers[11118] = new SkillHandlers.KungfuMaster.JumpPunch();
            skillHandlers[11119] = new DefaultAttack(true);

            skillHandlers[11101] = new DefaultAttack(true, true);//Strong punch
            skillHandlers[11601] = new DefaultAttack(true);//Strong punch Move
            skillHandlers[11604] = new DefaultAttack(true, true);//Strong punch NoTarget
            skillHandlers[11607] = new DefaultAttack(true);//Strong punch NoTarget Move

            skillHandlers[11127] = new DefaultAttack(true, true);//PowerAttack
            skillHandlers[11602] = new DefaultAttack(true);//PowerAttack Move
            skillHandlers[11605] = new DefaultAttack(true, true);//PowerAttack NoTarget
            skillHandlers[11608] = new DefaultAttack(true);//PowerAttack NoTarget Move

            skillHandlers[11103] = new SkillHandlers.KungfuMaster.CounterEnemy();
            skillHandlers[11104] = new SkillHandlers.KungfuMaster.Counter();

            skillHandlers[12235] = new SkillHandlers.ForceMaster.ColdDlast(12000054);//冰龙
            skillHandlers[12237] = new SkillHandlers.ForceMaster.RapidCooling();//冰冻
            skillHandlers[12255] = new SkillHandlers.ForceMaster.FireBall();//Fire ball
            skillHandlers[12256] = new SkillHandlers.ForceMaster.FrosenBall();//冰球
            skillHandlers[12300] = new SkillHandlers.ForceMaster.MagneticCatch(12111013);//鬼影手
            skillHandlers[12311] = new SkillHandlers.ForceMaster.ThrowTarget();//推
            skillHandlers[12312] = new SkillHandlers.ForceMaster.EnergyMissile();//弹指神功

            skillHandlers[15100] = new SkillHandlers.Assassin.AssassinAttack(true);//Mist Slash
            skillHandlers[15300] = new SkillHandlers.Assassin.AssassinAttack(true);//Mist Slash hidden
            skillHandlers[15303] = new SkillHandlers.Assassin.AssassinAttack(true);//Mist Slash Poison
            skillHandlers[15101] = new SkillHandlers.Assassin.AssassinAttack(true);//Dew Slash
            skillHandlers[15301] = new SkillHandlers.Assassin.AssassinAttack(true);//Dew Slash hidden
            skillHandlers[15304] = new SkillHandlers.Assassin.AssassinAttack(true);//Dew Slash Poison
            skillHandlers[15102] = new SkillHandlers.Assassin.AssassinAttack(true);//Moonlight Slash
            skillHandlers[15111] = new SkillHandlers.Assassin.WoodBlock();//Wood Block 
            skillHandlers[15116] = new SkillHandlers.Assassin.WoodBlockCounter();//Wood Block Counter
            skillHandlers[15118] = new SkillHandlers.Assassin.WoodBlockCounter();//Wood Block Counter Force
            skillHandlers[15119] = new SkillHandlers.Assassin.Dash(false);//Dash
            skillHandlers[15120] = new SkillHandlers.Assassin.Dash(true);//Dash Back
            skillHandlers[15128] = new SkillHandlers.Assassin.PoisenBreath();//Poisen Breath
            skillHandlers[15201] = new SkillHandlers.Assassin.SpineStab();//Spine Stab
            skillHandlers[15208] = new SkillHandlers.Assassin.Mine();//Mine
            skillHandlers[15211] = new SkillHandlers.Assassin.MineExplode();//Mine Explode
            skillHandlers[15004] = new SkillHandlers.Assassin.HighHit();
            skillHandlers[15302] = new DefaultAttack(true);//Moonlight Slash hidden
            skillHandlers[15305] = new DefaultAttack(true);//Moonlight Slash Poison

            skillHandlers[5000012] = new SkillHandlers.BladeMaster.Rush();
            skillHandlers[5100604] = new SkillHandlers.Common.Dash();//Monster ME_KangSi_None_Rush
            skillHandlers[5200106] = new SkillHandlers.Common.PestSuicide();//Monster ME_KangSi_None_Rush
            skillHandlers[5203105] = new SkillHandlers.Common.PestPoisen();//Monster ME_Pest_None_Pest360_LV5
            skillHandlers[5203110] = new SkillHandlers.Common.PestPoisen();//Monster ME_Pest_None_Pest360_LV10
            skillHandlers[5203113] = new SkillHandlers.Common.PestPoisen();//Monster ME_Pest_None_Pest360_LV13
            skillHandlers[5203114] = new SkillHandlers.Common.PestPoisen();//Monster ME_Pest_None_Pest360_LV14
            skillHandlers[5400009] = new SkillHandlers.KungfuMaster.JumpPunch();//Monster JumpPunch
            skillHandlers[5400019] = new SkillHandlers.KungfuMaster.LowKick();//Monster low kick
            skillHandlers[5400020] = new SkillHandlers.KungfuMaster.JumpPunch();//Monster JumpPunch
            //skillHandlers[5510001] = new DefaultAttack(false, true);//Monster Rushing wind
            //skillHandlers[5510002] = new DefaultAttack(false, true);//Monster Blood wind
            skillHandlers[5510003] = new SkillHandlers.BladeMaster.SwordBlocking(55100030);
            //skillHandlers[5510005] = new DefaultAttack(false, true);//Monster Storm wind
            skillHandlers[5510106] = new SkillHandlers.BladeMaster.TurningStrike(55101061);//Counter Attack            
            skillHandlers[5510107] = new SkillHandlers.BladeMaster.CounterAttack();//Counter Attack            
            skillHandlers[5510109] = new DefaultAttack(false, 20, 0);//Monster Front kick
            skillHandlers[5510113] = new SkillHandlers.BladeMaster.Rush();
            skillHandlers[5511106] = new SkillHandlers.KungfuMaster.JumpPunch();//Monster Jumppunch
            skillHandlers[5511111] = new SkillHandlers.KungfuMaster.LowKick();//Monster low kick
            skillHandlers[5511114] = new SkillHandlers.KungfuMaster.TakeDown();//Monster take down
            skillHandlers[5511115] = new SkillHandlers.KungfuMaster.Armbar();//Monster Armbar
            //skillHandlers[5513101] = new DefaultAttack(false, 20, 0);//Monster Judgement
            skillHandlers[5513103] = new DefaultAttack(false, 30);//Monster Protection Axe
            skillHandlers[5513112] = new SkillHandlers.BladeMaster.Rush();

            skillHandlers[7100000] = new SkillHandlers.Common.FoodRecovery(45); // Start Zone Mandu & Boiled Mandu & Premium Mandu
            skillHandlers[7100001] = new SkillHandlers.Common.FoodRecovery(55); // Premium Fried Chicken
            skillHandlers[7100002] = new SkillHandlers.Common.FoodRecovery(81); // Premium Pork Noodles
            skillHandlers[7100003] = new SkillHandlers.Common.FoodRecovery(108); // Premium Fried Fox Gyoza
            skillHandlers[7100004] = new SkillHandlers.Common.FoodRecovery(139); // Premium Konnyaku Noodles
            skillHandlers[7100005] = new SkillHandlers.Common.FoodRecovery(180); // Premium Wolf Meatballs
            skillHandlers[7100006] = new SkillHandlers.Common.FoodRecovery(264); // Premium Bottled Deer Soup
            skillHandlers[7100010] = new SkillHandlers.Common.FoodRecovery(47); // Craftsman's Fried Chicken
            skillHandlers[7100011] = new SkillHandlers.Common.FoodRecovery(68); // Craftsman's Pork Noodles
            skillHandlers[7100012] = new SkillHandlers.Common.FoodRecovery(90); // Craftsman's Fried Fox Gyoza
            skillHandlers[7100013] = new SkillHandlers.Common.FoodRecovery(117); // Craftsman's Konnyaku Noodles
            skillHandlers[7100014] = new SkillHandlers.Common.FoodRecovery(150); // Craftsman's Wolf Meatballs
            skillHandlers[7100015] = new SkillHandlers.Common.FoodRecovery(220); // Craftsman's Bottled Deer Soup
            skillHandlers[7400100] = new SkillHandlers.Common.FoodRecovery(61); // Master Craftsman's Fried Chicken
            skillHandlers[7400110] = new SkillHandlers.Common.FoodRecovery(88); // Master Craftsman's Pork Noodles
            skillHandlers[7400120] = new SkillHandlers.Common.FoodRecovery(117); // Master Craftsman's Fried Fox Gyoza
            skillHandlers[7400130] = new SkillHandlers.Common.FoodRecovery(152); // Master Craftsman's Konnyaku Noodles
            skillHandlers[7400140] = new SkillHandlers.Common.FoodRecovery(195); // Master Craftsman's Wolf Meatballs
            skillHandlers[7400150] = new SkillHandlers.Common.FoodRecovery(286); // Master Craftsman's Bottled Deer Soup
            skillHandlers[7400160] = new SkillHandlers.Common.FoodRecovery(19); // Mandu
            skillHandlers[7400170] = new SkillHandlers.Common.FoodRecovery(23); // Fried Chicken
            skillHandlers[7400180] = new SkillHandlers.Common.FoodRecovery(34); // Pork Noodles
            skillHandlers[7400190] = new SkillHandlers.Common.FoodRecovery(45); // Fried Fox Gyoza
            skillHandlers[7400200] = new SkillHandlers.Common.FoodRecovery(58); // Konnyaku Noodles
            skillHandlers[7400210] = new SkillHandlers.Common.FoodRecovery(75); // Master Craftsman's Wolf Meatballs
            skillHandlers[7400220] = new SkillHandlers.Common.FoodRecovery(110); // Bottled Deer Soup
            skillHandlers[7400500] = new SkillHandlers.Common.FoodRecovery(20); // Crab Porridge
            skillHandlers[7400510] = new SkillHandlers.Common.FoodRecovery(26); // 흑룡채 전투식량 
            skillHandlers[7400520] = new SkillHandlers.Common.FoodRecovery(38); // Frog Tongue Mandu
            skillHandlers[7400530] = new SkillHandlers.Common.FoodRecovery(44); // Soratang
            skillHandlers[7400540] = new SkillHandlers.Common.FoodRecovery(19); // 
            skillHandlers[7400550] = new SkillHandlers.Common.FoodRecovery(38); // 
            skillHandlers[7400560] = new SkillHandlers.Common.FoodRecovery(38); // 
            skillHandlers[7400570] = new SkillHandlers.Common.FoodRecovery(15); // 
            skillHandlers[7403020] = new SkillHandlers.Common.FoodRecovery(132); // 
            skillHandlers[7403030] = new SkillHandlers.Common.FoodRecovery(95); // 
            skillHandlers[7403040] = new SkillHandlers.Common.FoodRecovery(85); // 
            skillHandlers[7403050] = new SkillHandlers.Common.FoodRecovery(132); // 

            /* Recovery Potions
            skillHandlers[7200000] = new SkillHandlers.Common.PotionRecovery(377); // Worthy Medium Size Lesser Recovery Potion
            skillHandlers[7200001] = new SkillHandlers.Common.PotionRecovery(616); // Worthy Large Size Lesser Recovery Potion
            skillHandlers[7402000] = new SkillHandlers.Common.PotionRecovery(94);  // Small Size Lesser Recovery Potion
            skillHandlers[7402010] = new SkillHandlers.Common.PotionRecovery(188); // Medium Size Lesser Recovery Potion
            skillHandlers[7402020] = new SkillHandlers.Common.PotionRecovery(308); // Large Size Lesser Recovery Potion
            */
        }
    }
}
