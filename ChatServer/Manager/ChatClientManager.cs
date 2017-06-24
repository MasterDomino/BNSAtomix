using System.Collections.Concurrent;
using System.Threading;

using SmartEngine.Network;
using SmartEngine.Core;
using SagaBNS.ChatServer.Network.Client;
using SagaBNS.ChatServer.Packets;
using SagaBNS.ChatServer.Packets.Client;

namespace SagaBNS.ChatServer.Manager
{
    public class ChatClientManager : ClientManager<BNSChatOpcodes>
    {
        private static readonly ChatClientManager instance = new ChatClientManager();

        public static ChatClientManager Instance { get { return instance; } }

        private readonly ConcurrentDictionary<string, ConcurrentDictionary<ulong, ChatSession>> clientsByChannel = new ConcurrentDictionary<string, ConcurrentDictionary<ulong, ChatSession>>();
        private readonly ConcurrentDictionary<string, ChatSession> clientsByName = new ConcurrentDictionary<string, ChatSession>();
        private readonly ConcurrentDictionary<string, uint> channelID = new ConcurrentDictionary<string, uint>();
        private int nextChannelID = unchecked((int)0x80000000);

        public ConcurrentDictionary<string, ConcurrentDictionary<ulong, ChatSession>> ClientsByChannel
        {
            get { return clientsByChannel; }
        }

        public ChatClientManager()
        {
            RegisterPacketHandler(BNSChatOpcodes.CM_LOGIN_AUTH, new CM_LOGIN_AUTH());
            RegisterPacketHandler(BNSChatOpcodes.CM_PLAYER_CHANGE_CHANNEL, new CM_PLAYER_CHANGE_CHANNEL());
            RegisterPacketHandler(BNSChatOpcodes.CM_CHAT, new CM_CHAT());
            RegisterPacketHandler(BNSChatOpcodes.CM_PING, new CM_PING());
            RegisterPacketHandler(BNSChatOpcodes.CM_CHANNEL_QUIT, new CM_CHANNEL_QUIT());
        }

        protected override Session<BNSChatOpcodes> NewSession()
        {
            return new ChatSession();
        }

        public bool Login(ChatSession client)
        {
            if (!clientsByName.ContainsKey(client.Name))
            {
                clientsByName[client.Name] = client;
                return true;
            }
            else
            {
                return false;
            }
        }

        public void Logout(ChatSession client)
        {
            clientsByName.TryRemove(client.Name, out client);
            RemoveClient(client.Channel, client.ActorID);
            RemoveClient(client.PartyChannel, client.ActorID);
        }

        public void RemoveClient(string channel, ulong actorID)
        {
            if (!string.IsNullOrEmpty(channel) && clientsByChannel.TryGetValue(channel, out ConcurrentDictionary<ulong, ChatSession> lst))
            {
                if (lst.TryRemove(actorID, out ChatSession client))
                {
                    Logger.Log.Info(string.Format("Player:{0}({1}) is quit Channel:{2},", client.Name, client.Email, channel));
                }

                if (lst.Count == 0)
                {
                    clientsByChannel.TryRemove(channel, out lst);
                }
            }
        }

        public uint ChangeChannel(ChatSession client, string channel, bool isParty)
        {
            if (client.Authenticated)
            {
                Logger.Log.Info(string.Format("Player:{0}({1}) is changing Channel to {2},", client.Name, client.Email, channel));
                ConcurrentDictionary<ulong, ChatSession> clients;
                if (!isParty)
                {
                    RemoveClient(client.Channel, client.ActorID);

                    if (!string.IsNullOrEmpty(client.Channel))
                    {
                        clientsByChannel.TryGetValue(client.Channel, out clients);
                    }
                    client.Channel = channel;
                }
                else
                {
                    RemoveClient(client.PartyChannel, client.ActorID);
                    if (!string.IsNullOrEmpty(client.PartyChannel))
                    {
                        clientsByChannel.TryGetValue(client.PartyChannel, out clients);
                    }

                    client.PartyChannel = channel;
                }
                uint chanID;
                if (!clientsByChannel.TryGetValue(channel, out clients))
                {
                    clients = new ConcurrentDictionary<ulong, ChatSession>();
                    Interlocked.Increment(ref nextChannelID);
                    chanID = (uint)nextChannelID;
                    clientsByChannel[channel] = clients;
                    channelID[channel] = chanID;
                }
                else
                {
                    chanID = channelID[channel];
                }

                clients[client.ActorID] = client;
                return chanID;
            }
            else
            {
                return 0;
            }
        }
    }
}
