using System;
using System.Collections.Generic;
using System.Linq;

using SagaBNS.Common.Actors;
using SagaBNS.Common.Skills;
using SagaBNS.GameServer.Map;
using SmartEngine.Network;

namespace SagaBNS.GameServer.NPC.AI.AICommands
{
    public class Attack : IAICommand
    {
        private readonly AI ai;
        private readonly ActorNPC self;
        private ActorExt target;
        private readonly Map.Map map;
        private DateTime nextTime = DateTime.Now;
        private Skill curSkill;
        public Attack(AI ai, ActorNPC self, ActorExt target)
        {
            this.ai = ai;
            this.self = self;
            this.target = target;
            map = Map.MapManager.Instance.GetMap(self.MapInstanceID);
            ai.Period = 400;
            Init();
        }

        #region IAICommand 成员
        private CommandStatus status;
        public CommandTypes Type
        {
            get { return CommandTypes.Attack; }
        }

        public CommandStatus Status
        {
            get { return status; }
        }

        public ActorExt Target
        {
            get
            {
                return target;
            }
            set
            {
                if (target != value)
                {
                    if (value == null)
                    {
                        Finish();
                    }

                    target = value;
                }
            }
        }

        private void Init()
        {
            UpdateEvent evt = new UpdateEvent()
            {
                Actor = self,
                Target = self,
                UpdateType = UpdateTypes.Actor
            };
            //evt.AddActorPara(PacketParameter.CombatStatus, 1);
            map.SendEventToAllActorsWhoCanSeeActor(MapEvents.EVENT_BROADCAST, evt, self, false);
            self.Status.IsInCombat = true;

            evt = new UpdateEvent()
            {
                Actor = self,
                Target = self,
                UpdateType = UpdateTypes.Actor
            };
            //evt.AddActorPara(PacketParameter.FaceTo, (long)target.ActorID);
            map.SendEventToAllActorsWhoCanSeeActor(MapEvents.EVENT_BROADCAST, evt, self, false);
        }

        private void Finish()
        {
            if (self.Status.CastingSkill)
            {
                UpdateEvent evt = new UpdateEvent()
                {
                    Actor = self,
                    UpdateType = UpdateTypes.Skill,
                    Target = target,
                    Skill = curSkill,
                    SkillCastMode = SkillCastMode.Single,
                    SkillMode = SkillMode.End
                };
                map.SendEventToAllActorsWhoCanSeeActor(MapEvents.EVENT_BROADCAST, evt, self, false);
                self.Status.CastingSkill = false;
            }
            status = CommandStatus.Finished;
            {
                UpdateEvent evt = new UpdateEvent()
                {
                    Actor = self,
                    Target = self,
                    UpdateType = UpdateTypes.Actor
                };
                //evt.AddActorPara(PacketParameter.CombatStatus, 0);
                map.SendEventToAllActorsWhoCanSeeActor(MapEvents.EVENT_BROADCAST, evt, self, false);
                self.Status.IsInCombat = false;

                evt = new UpdateEvent()
                {
                    Actor = self,
                    Target = self,
                    UpdateType = UpdateTypes.Actor
                };
                //evt.AddActorPara(PacketParameter.FaceTo, 0);
                map.SendEventToAllActorsWhoCanSeeActor(MapEvents.EVENT_BROADCAST, evt, self, false);
            }
            ai.DueTime = 100;
            ai.Period = 100;
            if (!ai.Activated && !ai.NoPlayer)
            {
                ai.Activate();
            }
        }

        public void Update()
        {
            if (self.DistanceToActor(target) <= ai.GetCurrentCastRange(target) || self.Status.CastingSkill)
            {
                DateTime now = DateTime.Now;
                if (!self.Status.CastingSkill)
                {
                    if (now > self.Status.SkillCooldownEnd && self.BaseData.Skill.Count > 0)
                    {
                        if (DateTime.Now < nextTime)
                        {
                            return;
                        }

                        nextTime = DateTime.Now.AddMilliseconds(self.BaseData.CombatThinkPeriod);

                        self.Dir = self.DirectionFromTarget(target);
                        curSkill = ChooseSkill(target);
                        if (curSkill == null)
                        {
                            return;
                        }

                        SkillArg arg = new SkillArg()
                        {
                            Caster = self,
                            Dir = self.Dir,
                            Target = (curSkill.BaseData.SkillType == SkillType.NoTarget || curSkill.BaseData.SkillType == SkillType.Self) ? self : target,
                            Skill = curSkill,
                            SkillSession = (byte)Global.Random.Next(0, 255)
                        };
                        Skills.SkillManager.Instance.SkillCast(arg);
                    }
                }
            }
            else
            {
                Finish();
            }
        }

        private Skill ChooseSkill(ActorExt target)
        {
            List<Skill> possibleSkills = ai.GetPossibleSkills(target);
            List<Skill> comboSkills = (from skill in possibleSkills
                                       where skill.BaseData.PreviousSkills.Count > 0 || skill.BaseData.RequiredCasterStance != SkillCastStances.None || skill.BaseData.RequiredTargetStance != SkillCastStances.None
                                       select skill).ToList();
            List<Skill> skills = comboSkills.Count > 0 ? comboSkills : possibleSkills;

            int idx = Global.Random.Next(1, skills.Count) - 1;
            if (idx >= skills.Count)
            {
                return null;
            }

            return skills[idx];
        }

        #endregion
    }
}
