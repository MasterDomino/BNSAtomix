using SagaBNS.Common.Skills;
using SmartEngine.Network.Utils;
using System;

namespace SagaBNS.GameServer.Skills
{
    public class SkillFactory : Factory<SkillFactory, SkillData>
    {
        #region Instantiation

        public SkillFactory()
        {
            loadingTab = "Loading skill template database";
            loadedTab = " skill templates loaded.";
            databaseName = "skill Templates";
            FactoryType = FactoryType.XML;
        }

        #endregion

        #region Methods

        public Skill CreateNewSkill(uint skillID)
        {
            if (items.ContainsKey(skillID))
            {
                return new Skill(this[skillID]);
            }
            else
            {
                return null;
            }
        }

        protected override uint GetKey(SkillData item)
        {
            return item.ID;
        }

        protected override void ParseCSV(SkillData item, string[] paras)
        {
            throw new NotImplementedException();
        }

        protected override void ParseXML(System.Xml.XmlElement root, System.Xml.XmlElement current, SkillData item)
        {
            switch (root.Name.ToLower())
            {
                case "skill":
                    {
                        switch (current.Name.ToLower())
                        {
                            case "id":
                                item.ID = uint.Parse(current.InnerText);
                                break;

                            case "effect":
                                item.Effect = uint.Parse(current.InnerText);
                                break;

                            case "name":
                                item.Name = current.InnerText;
                                break;

                            case "skilltype":
                                item.SkillType = (SkillType)Enum.Parse(typeof(SkillType), current.InnerText, true);
                                break;

                            case "minatk":
                                item.MinAtk = float.Parse(current.InnerText) / 100f;
                                break;

                            case "maxatk":
                                item.MaxAtk = float.Parse(current.InnerText) / 100f;
                                break;

                            case "castrangemin":
                                item.CastRangeMin = (int)(int.Parse(current.InnerText) / 3.5f);
                                break;

                            case "castrangemax":
                                item.CastRangeMax = (int)(int.Parse(current.InnerText) / 3.5f);
                                break;

                            case "manacost":
                                item.ManaCost = int.Parse(current.InnerText);
                                break;

                            case "casttime":
                                item.CastTime = int.Parse(current.InnerText);
                                break;

                            case "cooldown":
                                item.CoolDown = int.Parse(current.InnerText);
                                break;

                            case "actiontime":
                                item.ActionTime = int.Parse(current.InnerText);
                                break;

                            case "activationtime":
                                foreach (string i in current.InnerText.Split(','))
                                {
                                    item.ActivationTimes.Add(int.Parse(i));
                                }
                                break;

                            case "notargettype":
                                item.NoTargetType = (NoTargetTypes)Enum.Parse(typeof(NoTargetTypes), current.InnerText);
                                break;

                            case "notargetwidth":
                                item.NoTargetWidth = int.Parse(current.InnerText);
                                break;

                            case "notargetangle":
                                item.NoTargetAngle = int.Parse(current.InnerText);
                                break;

                            case "notargetrange":
                                item.NoTargetRange = int.Parse(current.InnerText);
                                break;

                            case "shouldapproach":
                                item.ShouldApproach = current.InnerText == "1";
                                break;

                            case "approachtimerate":
                                item.ApproachTimeRate = int.Parse(current.InnerText);
                                break;

                            case "duration":
                                item.Duration = int.Parse(current.InnerText);
                                break;

                            case "bonusaddition":
                                item.BonusAddition = current.InnerText;
                                break;

                            case "bonusrate":
                                item.BonusRate = ((float)int.Parse(current.InnerText)) / 100;
                                break;

                            case "movementlockoncasting":
                                item.MovementLockOnCasting = int.Parse(current.InnerText);
                                break;

                            case "movementlockonaction":
                                item.MovementLockOnAction = int.Parse(current.InnerText);
                                break;

                            case "casterstance":
                                item.RequiredCasterStance = (SkillCastStances)Enum.Parse(typeof(SkillCastStances), current.InnerText, true);
                                break;

                            case "targetstance":
                                item.RequiredTargetStance = (SkillCastStances)Enum.Parse(typeof(SkillCastStances), current.InnerText, true);
                                break;

                            case "relatedskills":
                                {
                                    foreach (string i in current.InnerText.Split(','))
                                    {
                                        if (i != "0")
                                        {
                                            item.RelatedSkills.Add(uint.Parse(i));
                                        }
                                    }
                                }
                                break;

                            case "previousskills":
                                {
                                    foreach (string i in current.InnerText.Split(','))
                                    {
                                        if (i != "0")
                                        {
                                            item.PreviousSkills.Add(uint.Parse(i));
                                        }
                                    }
                                }
                                break;
                        }
                    }
                    break;
            }
        }

        #endregion
    }
}