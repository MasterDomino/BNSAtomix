using System.Collections.Generic;
using System.Linq;
using System.Threading;

using SmartEngine.Core;
using SmartEngine.Network.Map;
using SmartEngine.Network;
using SagaBNS.Common.Actors;
using SagaBNS.Common.Skills;
using SagaBNS.Common.Packets;
using SagaBNS.GameServer.Map;
using SagaBNS.GameServer.Skills;
using SagaBNS.GameServer.Packets.Client;

namespace SagaBNS.GameServer.Network.Client
{
    public partial class GameSession : Session<GamePacketOpcode>
    {
        public void SendSkillAdd(uint skillID)
        {
            SM_SKILL_ADD p = new SM_SKILL_ADD()
            {
                SkillID = skillID
            };
            Network.SendPacket(p);
        }

        public void SendSkillLoad()
        {
            SM_SKILL_LOAD p = new SM_SKILL_LOAD()
            {
                Skills = chara.Skills.Values.ToList()
            };
            Network.SendPacket(p);

            //Manager.ExperienceManager.Instance.SendMissingSkills(chara);
        }

        public void OnGotSkillInfo(List<Skill> skills)
        {
            foreach (Skill i in skills)
            {
                SkillManager.Instance.PlayerAddSkill(chara, i.ID, false);
            }
            SendAuthFinish();
        }

        private Actor curTarget;
        public void OnTargetSwitch(CM_TARGET_SWITCH p)
        {
            curTarget = map.GetActor(p.ActorID);
            UpdateEvent evt = new UpdateEvent()
            {
                UpdateType = UpdateTypes.Actor,
                Actor = chara,
                Target = chara
            };
            //evt.AddActorPara(Common.Packets.GameServer.PacketParameter.FaceTo, (long)p.ActorID);
            map.SendEventToAllActorsWhoCanSeeActor(MapEvents.EVENT_BROADCAST, evt, chara, true);
        }

        public void OnSkillCastCoordinate(CM_SKILL_CAST_COORDINATE p)
        {
            //Logger.Log.Info(string.Format("SkillCast:ID{0} X:{1} Y:{2} Z:{3} Dir:{4}", p.SkillID, p.X, p.Y, p.Z, p.Dir));
            if (chara.Skills.ContainsKey(p.SkillID))
            {
                Skill skill = chara.Skills[p.SkillID];

                SkillArg arg = new SkillArg()
                {
                    Caster = chara,
                    Target = chara,
                    CastMode = SkillCastMode.Coordinate,
                    Skill = skill,
                    X = p.X,
                    Y = p.Y,
                    Z = p.Z,
                    Dir = p.Dir
                };
                Interlocked.Increment(ref skillSession);
                arg.SkillSession = (byte)skillSession;

                if (SkillManager.Instance.SkillCast(arg))
                {
                    /*SM_SKILL_CAST_RESULT p1 = new SM_SKILL_CAST_RESULT();
                    p1.SkillSession = arg.SkillSession;
                    p1.SkillID = skill.ID;
                    this.Network.SendPacket(p1);*/

                    ChangeCombatStatus(true);
                }
                else
                {
                    SM_SKILL_CAST_RESULT p1 = new SM_SKILL_CAST_RESULT()
                    {
                        SkillSession = 0x60,
                        Unknown = 1,
                        SkillID = p.SkillID
                    };
                    Network.SendPacket(p1);
                }
            }
            else
            {
                Logger.Log.Warn(string.Format("Player:{0}({1}) does not have SkillID:{2}", chara.Name, chara.CharID, p.SkillID));
                SM_SKILL_CAST_RESULT p1 = new SM_SKILL_CAST_RESULT()
                {
                    SkillSession = 0x60,
                    Unknown = 1,
                    SkillID = p.SkillID
                };
                Network.SendPacket(p1);
            }
        }

        private int skillSession;
        public void OnSkillCast(CM_SKILL_CAST p)
        {
            //Logger.Log.Info(string.Format("SkillCast:ID{0} Target:0x{1:X} Dir:{2}", p.SkillID, p.ActorID, p.Dir));
            if (chara.Skills.ContainsKey(p.SkillID))
            {
                Skill skill = chara.Skills[p.SkillID];
                Actor target = map.GetActor(p.ActorID);

                if (skill.ID == 12300 && ((ActorNPC)target).NpcID == 218)
                {
                    SM_SERVER_MESSAGE r = new SM_SERVER_MESSAGE()
                    {
                        MessagePosition = SM_SERVER_MESSAGE.Positions.ChatWindow,
                        Message = "This skill has been disabled for this boss due to abuse."
                    };
                    Network.SendPacket(r);
                    return;
                }

                SkillArg arg = new SkillArg()
                {
                    Caster = chara,
                    Target = (ActorExt)target,
                    Skill = skill,
                    Dir = p.Dir
                };
                Interlocked.Increment(ref skillSession);
                arg.SkillSession = (byte)skillSession;

                if (SkillManager.Instance.SkillCast(arg))
                {
                    /*SM_SKILL_CAST_RESULT p1 = new SM_SKILL_CAST_RESULT();
                    p1.SkillSession = arg.SkillSession;
                    p1.SkillID = skill.ID;
                    this.Network.SendPacket(p1);*/

                    ChangeCombatStatus(true);
                }
                else
                {
                    Skills.SkillManager.Instance.BroadcastSkillCast(arg, SkillMode.DurationEnd);
                }
            }
            else
            {
                Logger.Log.Warn(string.Format("Player:{0}({1}) does not have SkillID:{2}", chara.Name, chara.CharID, p.SkillID));
                SM_SKILL_CAST_RESULT p1 = new SM_SKILL_CAST_RESULT()
                {
                    SkillSession = 0x60,
                    Unknown = 1,
                    SkillID = p.SkillID
                };
                Network.SendPacket(p1);
            }
        }

        public void ChangeCombatStatus(bool combat)
        {
            if (chara.Status.IsInCombat != combat || !combat)
            {
                chara.Status.IsInCombat = combat;
                UpdateEvent evt = new UpdateEvent()
                {
                    Actor = chara,
                    Target = chara,
                    UpdateType = UpdateTypes.Actor
                };
                //evt.AddActorPara(PacketParameter.CombatStatus, combat ? 1 : 0);
                map.SendEventToAllActorsWhoCanSeeActor(MapEvents.EVENT_BROADCAST, evt, chara, true);
            }
        }
    }
}
