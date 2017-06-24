using System.Collections.Generic;
using System.Collections.Concurrent;

using SmartEngine.Core;
using SmartEngine.Network;

using SagaBNS.ChatServer.Manager;
using SagaBNS.ChatServer.Packets;
using SagaBNS.ChatServer.Packets.Client;

namespace SagaBNS.ChatServer.Network.Client
{
    public partial class ChatSession : Session<BNSChatOpcodes>
    {
        public string Name { get; set; }
        public string Email { get; set; }
        public ulong ActorID { get; set; }
        public bool Authenticated { get; set; }
        public string Channel { get; set; }
        public string PartyChannel { get; set; }
        public uint ChannelID { get; set; }
        public uint PartyChannelID { get; set; }

        public void OnLoginAuth(CM_LOGIN_AUTH p)
        {
            ActorID = p.ActorID;
            p.GetInfo(out string name, out string email, out string token);
            Name = name;
            Email = email;
            if (token == "4165E727FDE5F82DDA7BB8977CF8A3FBBD5C8A61EF90ED81EDAA98B6ED180740131ABD2EDAEBAB6EDE7B6B2579F3BCF3CDB257E17165D38171FD7632C1F7CC31"
                && ChatClientManager.Instance.Login(this))
            {
                Authenticated = true;
                Logger.Log.Info(string.Format("Player:{0}({1}|0x{2:X}) is logging in", name, email, ActorID));
                SM_LOGIN_AUTH_RESULT p1 = new SM_LOGIN_AUTH_RESULT();
                Network.SendPacket(p1);
            }
        }

        public void OnPlayerChangeChannel(CM_PLAYER_CHANGE_CHANNEL p)
        {
            string channel = p.Channel;
            bool isParty = channel.Contains("Party");
            uint chanID = ChatClientManager.Instance.ChangeChannel(this, p.Channel, isParty);
            if (isParty)
            {
                PartyChannelID = chanID;
            }
            else
            {
                ChannelID = chanID;
            }

            SM_PLAYER_CHANGE_CHANNEL_RESULT p1 = new SM_PLAYER_CHANGE_CHANNEL_RESULT()
            {
                Unknown = p.Type,
                ChannelID = isParty ? PartyChannelID : ChannelID
            };
            Network.SendPacket(p1);
        }

        public void OnChat(CM_CHAT p)
        {
            uint chanID = p.ChannelID;
            string msg = p.Message;
            ConcurrentDictionary<ulong, ChatSession> clients = null;
            if (chanID == ChannelID)
            {
                ChatClientManager.Instance.ClientsByChannel.TryGetValue(Channel, out clients);
            }
            else if (chanID == PartyChannelID)
            {
                ChatClientManager.Instance.ClientsByChannel.TryGetValue(PartyChannel, out clients);
            }
            if (clients != null)
            {
                foreach (KeyValuePair<ulong, ChatSession> i in clients)
                {
                    SM_CHAT p1 = new SM_CHAT()
                    {
                        ChannelID = chanID,
                        ActorID = ActorID
                    };
                    p1.PutMessage(Name, msg);
                    i.Value.Network.SendPacket(p1);
                }
            }
        }

        public void OnPing(CM_PING p)
        {

        }

        public void OnChannelQuit(CM_CHANNEL_QUIT p)
        {
            uint channID = p.ChannelID;
            if (channID == ChannelID)
            {
                ChatClientManager.Instance.RemoveClient(Channel, ActorID);
            }
            if (channID == PartyChannelID)
            {
                ChatClientManager.Instance.RemoveClient(PartyChannel, ActorID);
            }
        }
    }
}
