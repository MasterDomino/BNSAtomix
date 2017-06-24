using System.Linq;

using SmartEngine.Network;
using SagaBNS.Common.Packets;
using SagaBNS.Common.Packets.CharacterServer;
using SagaBNS.Common.Actors;
using SagaBNS.Common.Skills;
using SagaBNS.CharacterServer.Cache;

namespace SagaBNS.CharacterServer.Network.Client
{
    public partial class CharacterSession : Session<CharacterPacketOpcode>
    {
        public void OnSkillGet(Packets.Client.CM_SKILL_GET p)
        {
            ActorPC pc = CharacterCache.Instance[p.CharID];
            SM_SKILL_INFO p1 = new SM_SKILL_INFO()
            {
                SessionID = p.SessionID,
                Skills = pc.Skills.Values.ToList()
            };
            Network.SendPacket(p1);
        }

        public void OnSkillSave(Packets.Client.CM_SKILL_SAVE p)
        {
            ActorPC pc = CharacterCache.Instance[p.CharID];
            if (pc != null)
            {
                lock (pc.Skills)
                {
                    pc.Skills.Clear();
                    foreach (Skill i in p.Skills)
                    {
                        pc.Skills[i.ID] = i;
                    }
                }
            }
        }
    }
}
