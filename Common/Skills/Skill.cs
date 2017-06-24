using System;

namespace SagaBNS.Common.Skills
{
    public class Skill
    {
        private readonly SkillData baseData;
        public SkillData BaseData { get { return baseData; } }
        public uint ID { get { return baseData.ID; } }
        public bool Dummy { get; set; }
        public DateTime CoolDownEndTime { get; set; }

        public Skill(SkillData data)
        {
            baseData = data;
            CoolDownEndTime = DateTime.Now;
        }

        public override string ToString()
        {
            return baseData.Name;
        }
    }
}
