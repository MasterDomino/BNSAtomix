using System;
using System.Collections.Generic;
using SagaBNS.Common.Packets.CharacterServer;
using SagaBNS.Common.Network;
using SagaBNS.GameServer.Network.Client;

namespace SagaBNS.GameServer.Network.CharacterServer
{
    public class CharacterSession : CharacterSession<GameSession>
    {
        private static CharacterSession instance = new CharacterSession();

        /// <summary>
        /// 单实例
        /// </summary>
        public static CharacterSession Instance { get { return instance; } set { instance = value; } }

        public CharacterSession()
        {
            Host = Configuration.Instance.CharacterHost;
            Port = Configuration.Instance.CharacterPort;
            CharacterPassword = Configuration.Instance.CharacterPassword;
        }

        protected override void OnCharList(GameSession client, List<Common.Actors.ActorPC> chars)
        {
            throw new NotImplementedException();
        }

        protected override void OnActorInfo(GameSession client, Common.Actors.ActorPC chara)
        {
            client.OnActorInfo(chara);
        }

        protected override void OnCharCreateResult(GameSession client, uint charID, SM_CHAR_CREATE_RESULT.Results result)
        {
            throw new NotImplementedException();
        }

        protected override void OnCharDeleteResult(GameSession client, SM_CHAR_DELETE_RESULT.Results result)
        {
            throw new NotImplementedException();
        }

        protected override void OnGotInventoryItem(GameSession client, Common.Item.Item item, bool end)
        {
            client.OnGotInventoryItem(item, end);
        }

        protected override void OnQuestInfo(GameSession client, List<Common.Quests.Quest> quests, List<ushort> completed)
        {
            client.OnGotQuestInfo(quests, completed);
        }

        protected override void OnSkillInfo(GameSession client, List<Common.Skills.Skill> skills)
        {
            client.OnGotSkillInfo(skills);
        }

        protected override void OnTeleportInfo(GameSession client, List<ushort> locations)
        {
            client.OnGotTeleportInfo(locations);
        }
    }
}
