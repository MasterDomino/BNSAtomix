using System;
using System.Collections.Generic;
using SagaBNS.Common.Packets.CharacterServer;
using SagaBNS.Common.Network;
using SagaBNS.LoginServer.Network.Client;

namespace SagaBNS.LoginServer.Network.CharacterServer
{
    public class CharacterSession : CharacterSession<LoginSession>
    {
        private static readonly CharacterSession instance = new CharacterSession();

        public static CharacterSession Instance { get { return instance; } }
        public CharacterSession()
        {
            Host = Configuration.Instance.CharacterHost;
            Port = Configuration.Instance.CharacterPort;
            CharacterPassword = Configuration.Instance.CharacterPassword;
        }

        protected override void OnCharList(LoginSession client, List<Common.Actors.ActorPC> chars)
        {
            client.OnGotCharList(chars);
        }

        protected override void OnActorInfo(LoginSession client, Common.Actors.ActorPC chara)
        {
            throw new NotImplementedException();
        }

        protected override void OnCharCreateResult(LoginSession client, uint charID, SM_CHAR_CREATE_RESULT.Results result)
        {
            client.OnCharCreateResult(charID, result);
        }

        protected override void OnCharDeleteResult(LoginSession client, SM_CHAR_DELETE_RESULT.Results result)
        {
            client.OnCharDeleteResult(result);
        }

        protected override void OnGotInventoryItem(LoginSession client, Common.Item.Item item, bool end)
        {
            client.OnGotInventoryItem(item, end);
        }

        protected override void OnQuestInfo(LoginSession client, List<Common.Quests.Quest> quests, List<ushort> completed)
        {
            throw new NotImplementedException();
        }

        protected override void OnSkillInfo(LoginSession client, List<Common.Skills.Skill> skills)
        {
            throw new NotImplementedException();
        }

        protected override void OnTeleportInfo(LoginSession client, List<ushort> locations)
        {
            throw new NotImplementedException();
        }
    }
}
