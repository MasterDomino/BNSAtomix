
using SmartEngine.Network;
using SagaBNS.Common.Packets;
using SagaBNS.CharacterServer.Network.Client;

namespace SagaBNS.CharacterServer.Manager
{
    public class CharacterClientManager: ClientManager<CharacterPacketOpcode>
    {
        private static CharacterClientManager instance;

        public static CharacterClientManager Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new CharacterClientManager();
                }

                return instance;
            }
        }

        public CharacterClientManager()
        {
            Encrypt = false;

            RegisterPacketHandler(CharacterPacketOpcode.CM_LOGIN_REQUEST, new Packets.Client.CM_LOGIN_REQUEST());
            RegisterPacketHandler(CharacterPacketOpcode.CM_CHAR_LIST_REQUEST, new Packets.Client.CM_CHAR_LIST_REQUEST());
            RegisterPacketHandler(CharacterPacketOpcode.CM_ACTOR_INFO_REQUEST, new Packets.Client.CM_ACTOR_INFO_REQUEST());
            RegisterPacketHandler(CharacterPacketOpcode.CM_CHAR_CREATE, new Packets.Client.CM_CHAR_CREATE());
            RegisterPacketHandler(CharacterPacketOpcode.CM_CHAR_SAVE, new Packets.Client.CM_CHAR_SAVE());
            RegisterPacketHandler(CharacterPacketOpcode.CM_CHAR_DELETE, new Packets.Client.CM_CHAR_DELETE());
            RegisterPacketHandler(CharacterPacketOpcode.CM_ITEM_CREATE, new Packets.Client.CM_ITEM_CREATE());
            RegisterPacketHandler(CharacterPacketOpcode.CM_ITEM_SAVE, new Packets.Client.CM_ITEM_SAVE());
            RegisterPacketHandler(CharacterPacketOpcode.CM_ITEM_LIST_SAVE, new Packets.Client.CM_ITEM_LIST_SAVE());
            RegisterPacketHandler(CharacterPacketOpcode.CM_ITEM_DELETE, new Packets.Client.CM_ITEM_DELETE());
            RegisterPacketHandler(CharacterPacketOpcode.CM_ITEM_INVENTORY_GET, new Packets.Client.CM_ITEM_INVENTORY_GET());
            RegisterPacketHandler(CharacterPacketOpcode.CM_QUEST_GET, new Packets.Client.CM_QUEST_GET());
            RegisterPacketHandler(CharacterPacketOpcode.CM_QUEST_SAVE, new Packets.Client.CM_QUEST_SAVE());
            RegisterPacketHandler(CharacterPacketOpcode.CM_SKILL_GET, new Packets.Client.CM_SKILL_GET());
            RegisterPacketHandler(CharacterPacketOpcode.CM_SKILL_SAVE, new Packets.Client.CM_SKILL_SAVE());
            RegisterPacketHandler(CharacterPacketOpcode.CM_TELEPORT_GET, new Packets.Client.CM_TELEPORT_GET());
            RegisterPacketHandler(CharacterPacketOpcode.CM_TELEPORT_SAVE, new Packets.Client.CM_TELEPORT_SAVE());
        }

        protected override Session<CharacterPacketOpcode> NewSession()
        {
            return new CharacterSession();
        }
    }
}
