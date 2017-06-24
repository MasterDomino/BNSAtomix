using System;
using System.Collections.Generic;
using SagaBNS.Common.Packets.CharacterServer;
using SagaBNS.Common.Network;
using SagaBNS.LobbyServer.Network.Client;

namespace SagaBNS.LobbyServer.Network.CharacterServer
{
    public class CharacterSession : CharacterSession<LobbySession>
    {
        private static readonly CharacterSession instance = new CharacterSession();

        public static CharacterSession Instance { get { return instance; } }
        public CharacterSession()
        {
            Host = Configuration.Instance.CharacterHost;
            Port = Configuration.Instance.CharacterPort;
            CharacterPassword = Configuration.Instance.CharacterPassword;
        }

        protected override void OnCharList(LobbySession client, List<Common.Actors.ActorPC> chars)
        {
            client.OnCharacterList(chars);
        }

        protected override void OnActorInfo(LobbySession client, Common.Actors.ActorPC chara)
        {
            throw new NotImplementedException();
        }

        protected override void OnCharCreateResult(LobbySession client, uint charID, SM_CHAR_CREATE_RESULT.Results result)
        {
            client.OnCharCreateResult(charID, result);
        }

        protected override void OnCharDeleteResult(LobbySession client, SM_CHAR_DELETE_RESULT.Results result)
        {
            client.OnCharDeleteResult(result);
        }

        protected override void OnGotInventoryItem(LobbySession client, Common.Item.Item item, bool end)
        {
            client.OnGotInventoryItem(item, end);
        }

        protected override void OnQuestInfo(LobbySession client, List<Common.Quests.Quest> quests, List<ushort> completed)
        {
            throw new NotImplementedException();
        }

        protected override void OnSkillInfo(LobbySession client, List<Common.Skills.Skill> skills)
        {
            throw new NotImplementedException();
        }

        protected override void OnTeleportInfo(LobbySession client, List<ushort> locations)
        {
            throw new NotImplementedException();
        }
    }
}
