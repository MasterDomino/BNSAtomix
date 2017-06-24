using System.Collections.Generic;
using System.Linq;

using SmartEngine.Network;
using SagaBNS.Common.Packets;
using SagaBNS.GameServer.Network.Client;
using SagaBNS.GameServer.Packets.Client;

namespace SagaBNS.GameServer.Manager
{
    public class GameClientManager : ClientManager<GamePacketOpcode>
    {
        private static readonly GameClientManager instance = new GameClientManager();

        public static GameClientManager Instance { get { return instance; } }

        public GameClientManager()
        {
            RegisterPacketHandler(GamePacketOpcode.CM_ACTOR_MOVEMENT, new CM_ACTOR_MOVEMENT());
            RegisterPacketHandler(GamePacketOpcode.CM_ACTOR_TURN, new CM_ACTOR_TURN());
            //RegisterPacketHandler(GamePacketOpcode.CM_CHARACTER_SELECT, new CM_CHARACTER_SELECT());
            RegisterPacketHandler(GamePacketOpcode.CM_CHAT, new CM_CHAT());
            RegisterPacketHandler((GamePacketOpcode)0xA, new CM_UNKNOWN());
            //RegisterPacketHandler(GamePacketOpcode.CM_ITEM_LOAD_INVENTORY, new CM_ITEM_LOAD_INVENTORY());
            //RegisterPacketHandler(GamePacketOpcode.CM_HAMMER_REPAIR, new CM_HAMMER_REPAIR());
            //RegisterPacketHandler(GamePacketOpcode.CM_TELEPORT_REQUEST, new CM_TELEPORT_REQUEST());
            //RegisterPacketHandler(GamePacketOpcode.CM_ADD_CRAFT, new CM_ADD_CRAFT());
            //RegisterPacketHandler(GamePacketOpcode.CM_DELETE_CRAFT, new CM_DELETE_CRAFT());
            //RegisterPacketHandler(GamePacketOpcode.CM_TIME_ONLINE_REQUEST, new CM_TIME_ONLINE_REQUEST());
            //RegisterPacketHandler(GamePacketOpcode.CM_DRAGON_STREAM, new CM_DRAGON_STREAM());
            //RegisterPacketHandler(GamePacketOpcode.CM_DECOMPOSE_BAGUA, new CM_DECOMPOSE_BAGUA());
            //RegisterPacketHandler(GamePacketOpcode.CM_COMPOSE_BAGUA, new CM_COMPOSE_BAGUA());
            //RegisterPacketHandler(GamePacketOpcode.CM_ITEM_EQUIP, new CM_ITEM_EQUIP());
            //RegisterPacketHandler(GamePacketOpcode.CM_ITEM_MOVE, new CM_ITEM_MOVE());
            //RegisterPacketHandler(GamePacketOpcode.CM_BAGUA_SET_REMOVE, new CM_BAGUA_SET_REMOVE());
            //RegisterPacketHandler(GamePacketOpcode.CM_BAGUA_SET_CHANGE, new CM_BAGUA_SET_CHANGE());
            //RegisterPacketHandler(GamePacketOpcode.CM_ITEM_BUY, new CM_ITEM_BUY());
            //RegisterPacketHandler(GamePacketOpcode.CM_ITEM_EXCHANGE, new CM_ITEM_EXCHANGE());
            //RegisterPacketHandler(GamePacketOpcode.CM_ITEM_SELL, new CM_ITEM_SELL());
            //RegisterPacketHandler(GamePacketOpcode.CM_ITEM_DROP, new CM_ITEM_DROP());
            //RegisterPacketHandler(GamePacketOpcode.CM_ITEM_UNEQUIP, new CM_ITEM_UNEQUIP());
            //RegisterPacketHandler(GamePacketOpcode.CM_ITEM_BUYBACK, new CM_ITEM_BUYBACK());
            RegisterPacketHandler(GamePacketOpcode.CM_LOGIN_AUTH, new CM_LOGIN_AUTH());
            RegisterPacketHandler(GamePacketOpcode.CM_LOGIN_START, new CM_LOGIN_START());
            RegisterPacketHandler(GamePacketOpcode.CM_LOGIN_CHAT_AUTH, new CM_LOGIN_CHAT_AUTH());
            //RegisterPacketHandler(GamePacketOpcode.CM_LOGIN_AUCTION_START, new CM_LOGIN_AUCTION_START());
            RegisterPacketHandler(GamePacketOpcode.CM_LOGIN_UNKNOWN1, new CM_LOGIN_UNKNOWN1());
            RegisterPacketHandler(GamePacketOpcode.CM_LOGIN_UNKNOWN2, new CM_LOGIN_UNKNOWN2());
            RegisterPacketHandler(GamePacketOpcode.CM_MAP_LOAD_FINISHED, new CM_MAP_LOAD_FINISHED());
            RegisterPacketHandler(GamePacketOpcode.CM_MAPOBJECT_GET_ITEM, new CM_MAPOBJECT_GET_ITEM());
            RegisterPacketHandler(GamePacketOpcode.CM_MAPOBJECT_OPEN, new CM_MAPOBJECT_OPEN());
            RegisterPacketHandler(GamePacketOpcode.CM_MAPOBJECT_INVENTORY_OPEN, new CM_MAPOBJECT_INVENTORY_OPEN());
            RegisterPacketHandler(GamePacketOpcode.CM_MAPOBJECT_CLOSE, new CM_MAPOBJECT_CLOSE());
            //RegisterPacketHandler(GamePacketOpcode.CM_ACTOR_CORPSE_OPEN, new CM_ACTOR_CORPSE_OPEN());
            //RegisterPacketHandler(GamePacketOpcode.CM_ACTOR_CORPSE_CLOSE, new CM_ACTOR_CORPSE_CLOSE());
            //RegisterPacketHandler(GamePacketOpcode.CM_ACTOR_CORPSE_LOOT, new CM_ACTOR_CORPSE_LOOT());
            //RegisterPacketHandler(GamePacketOpcode.CM_ACTOR_CORPSE_PICK_UP, new CM_ACTOR_CORPSE_PICK_UP());
            RegisterPacketHandler(GamePacketOpcode.CM_QUEST_NPC_OPEN, new CM_QUEST_NPC_OPEN());
            //RegisterPacketHandler(GamePacketOpcode.CM_QUEST_ACCEPT, new CM_QUEST_ACCEPT());
            //RegisterPacketHandler(GamePacketOpcode.CM_QUEST_DROP, new CM_QUEST_DROP());
            RegisterPacketHandler(GamePacketOpcode.CM_QUEST_UPDATE_REQUEST, new CM_QUEST_UPDATE_REQUEST());
            RegisterPacketHandler(GamePacketOpcode.CM_QUEST_UPDATE_REQUEST_MAPOBJECT, new CM_QUEST_UPDATE_REQUEST_MAPOBJECT());
            //RegisterPacketHandler(GamePacketOpcode.CM_QUEST_LOOT_QUEST_ITEM, new CM_QUEST_LOOT_QUEST_ITEM());
            //RegisterPacketHandler(GamePacketOpcode.CM_SAVE_UI_SETTING, new CM_SAVE_UI_SETTING());
            //RegisterPacketHandler(GamePacketOpcode.CM_SKILL_LOAD, new CM_SKILL_LOAD());
            /*RegisterPacketHandler(GamePacketOpcode.CM_TARGET_SWITCH, new CM_TARGET_SWITCH());
            RegisterPacketHandler(GamePacketOpcode.CM_PLAYER_RECOVER, new CM_PLAYER_RECOVER());
            RegisterPacketHandler(GamePacketOpcode.CM_PLAYER_REVIVE, new CM_PLAYER_REVIVE());
            RegisterPacketHandler(GamePacketOpcode.CM_PARTY_INVITE, new CM_PARTY_INVITE());
            RegisterPacketHandler(GamePacketOpcode.CM_PARTY_QUIT, new CM_PARTY_QUIT());
            RegisterPacketHandler(GamePacketOpcode.CM_PARTY_KICK, new CM_PARTY_KICK());
            RegisterPacketHandler(GamePacketOpcode.CM_PARTY_CHANGE_LOOT_MODE, new CM_PARTY_CHANGE_LOOT_MODE());
            RegisterPacketHandler(GamePacketOpcode.CM_PARTY_GIVE_LEADERSHIP, new CM_PARTY_GIVE_LEADERSHIP());
            RegisterPacketHandler(GamePacketOpcode.CM_PARTY_REPLY_INVITATION, new CM_PARTY_REPLY_INVITATION());
            RegisterPacketHandler(GamePacketOpcode.CM_PARTY_REPLY_INVITATION_DENIED, new CM_PARTY_REPLY_INVITATION_DENIED());
            RegisterPacketHandler(GamePacketOpcode.CM_SKILL_CAST, new CM_SKILL_CAST());
            RegisterPacketHandler(GamePacketOpcode.CM_SKILL_CAST_COORDINATE, new CM_SKILL_CAST_COORDINATE());
            RegisterPacketHandler(GamePacketOpcode.CM_ACTOR_ITEM_PICK_UP, new CM_ACTOR_ITEM_PICK_UP());
            RegisterPacketHandler(GamePacketOpcode.CM_ACTOR_ITEM_DROP, new CM_ACTOR_ITEM_DROP());
            RegisterPacketHandler(GamePacketOpcode.CM_ITEM_USE, new CM_ITEM_USE());
            RegisterPacketHandler(GamePacketOpcode.CM_MAP_PORTAL_ENTER, new CM_MAP_PORTAL_ENTER());
            RegisterPacketHandler(GamePacketOpcode.CM_MAP_PORTAL_SOME_REQUEST, new CM_MAP_PORTAL_SOME_REQUEST());*/
            //RegisterPacketHandler(GamePacketOpcode.CM_HELP_UNSTUCK, new CM_HELP_UNSTUCK());
            //RegisterPacketHandler(GamePacketOpcode.CM_LOCATION_DISCOVERY, new CM_LOCATION_DISCOVERY());
            //RegisterPacketHandler(GamePacketOpcode.CM_LOCATION_LOAD_TELEPORT, new CM_LOCATION_LOAD_TELEPORT());
            //RegisterPacketHandler(GamePacketOpcode.CM_EXPAND_INVENTORY, new CM_EXPAND_INVENTORY());
            //RegisterPacketHandler(GamePacketOpcode.CM_UNKNOWN, new CM_UNKNOWN());
        }

        public List<GameSession> ValidClients
        {
            get
            {
                List<GameSession> res = new List<GameSession>();
                foreach (GameSession i in Clients.ToArray())
                {
                    if (i.Character != null && i.Account != null)
                    {
                        res.Add(i);
                    }
                }
                return res;
            }
        }

        public GameSession FindClient(string name)
        {
            foreach (GameSession i in ValidClients)
            {
                if (i.Character.Name.ToLower() == name.ToLower())
                {
                    return i;
                }
            }
            return null;
        }

        protected override Session<GamePacketOpcode> NewSession()
        {
            return new GameSession();
        }
    }
}
