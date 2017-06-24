using System;

using SmartEngine.Core;
using SmartEngine.Network;
using SagaBNS.Common;
using SagaBNS.Common.Packets;
using SagaBNS.Common.Packets.AccountServer;
using SagaBNS.Common.Account;
using SagaBNS.Common.Encryption;
using SagaBNS.LoginServer.Network.AccountServer;

namespace SagaBNS.LoginServer.Network.Client
{
    public partial class LoginSession : Session<LoginPacketOpcode>
    {
        private int loginSerial;
        private string checkHash;
        private Account account;
        private AccountLoginResult lastLoginRes = AccountLoginResult.INVALID_PASSWORD;

        public Account Account { get { return account; } }

        public void OnAuthLoginStart(string loginName,int serial)
        {
            loginSerial = serial;
            AccountSession.Instance.RequestAccountInfo(loginName.Split('@')[0], this);
            Logger.Log.Info(loginName + " is trying to login");
        }

        public void OnAccountInfo(AccountLoginResult res, Account account)
        {
            if (res == AccountLoginResult.NO_SUCH_ACCOUNT)
            {
                Logger.Log.Info("Login Result:" + res.ToString());
            }

            switch (res)
            {
                case AccountLoginResult.OK:
                    {
                        this.account = account;
                        Network.Crypt.KeyExchange.MakePrivateKey();
                        ((BNSKeyExchange)Network.Crypt.KeyExchange).Username = account.UserName;
                        ((BNSKeyExchange)Network.Crypt.KeyExchange).Password = account.Password;
                        byte[] exchange = Network.Crypt.KeyExchange.GetKeyExchangeBytes(Mode.Server);
                        byte[] session = ((BNSKeyExchange)Network.Crypt.KeyExchange).Session.getBytes();
                        System.IO.MemoryStream ms = new System.IO.MemoryStream();
                        System.IO.BinaryWriter bw = new System.IO.BinaryWriter(ms);
                        bw.Write(session.Length);
                        bw.Write(session);
                        bw.Write(exchange.Length);
                        bw.Write(exchange);
                        string key = string.Format("<Reply>\n<KeyData>{0}</KeyData>\n</Reply>\n", Convert.ToBase64String(ms.ToArray()));
                        BNSLoginPacket p = new BNSLoginPacket()
                        {
                            Command = "STS/1.0 200 OK",
                            Serial = loginSerial,
                            Content = key
                        };
                        p.WritePacket();
                        Network.SendPacket(p);
                    }
                    break;
                case AccountLoginResult.NO_SUCH_ACCOUNT:
                case AccountLoginResult.DB_ERROR:
                    {
                        BNSLoginPacket p = new BNSLoginPacket()
                        {
                            Command = "STS/1.0 400 ErrAccountNotFound",
                            Serial = loginSerial,
                            Content = "<Error code=\"3002\" server=\"1008\" module=\"1\" line=\"458\"/>\n"
                        };
                        p.WritePacket();
                        Network.SendPacket(p);
                    }
                    break;
            }
        }

        public void OnAuthKeyData(byte[] exchangeKey, string checkHash, int serial)
        {
            Network.Crypt.KeyExchange.MakeKey(Mode.Server, exchangeKey);
            string hash = ((BNSKeyExchange)Network.Crypt.KeyExchange).Authentication;
            if (checkHash == hash.Split(',')[0])
            {
                this.checkHash = hash.Split(',')[1];
                loginSerial = serial;
                AccountSession.Instance.AccountLogin(account.AccountID, this);
            }
            else
            {
                BNSLoginPacket p = new BNSLoginPacket()
                {
                    Command = "STS/1.0 400 ErrBadPasswd",
                    Serial = serial,
                    Content = "<Error code=\"11\" server=\"1012\" module=\"1\" line=\"1683\"/>\n",
                    Encrypt = false
                };
                p.WritePacket();
                Network.SendPacket(p);
                Logger.Log.Info("Login result for " + account.UserName + ": BadPassword");
            }
        }

        public void OnAccountLoginResult(AccountLoginResult result)
        {
            lastLoginRes = result;
            if (result == AccountLoginResult.OK)
            {
                account.LastLoginTime = DateTime.Now;
                account.LastLoginIP = Network.Socket.RemoteEndPoint.ToString().Split(':')[0];
                System.IO.MemoryStream ms = new System.IO.MemoryStream();
                System.IO.BinaryWriter bw = new System.IO.BinaryWriter(ms);
                bw.Write(32);
                bw.Write(Convert.FromBase64String(checkHash));
                string res = Convert.ToBase64String(ms.ToArray());
                string key = string.Format("<Reply>\n<KeyData>{0}</KeyData>\n</Reply>\n", res);
                BNSLoginPacket p = new BNSLoginPacket()
                {
                    Command = "STS/1.0 200 OK",
                    Serial = loginSerial,
                    Content = key,
                    Encrypt = false
                };
                p.WritePacket();
                Network.SendPacket(p);
                Logger.Log.Info(account.UserName + " login successful!");
            }
            else
            {
                BNSLoginPacket p = new BNSLoginPacket()
                {
                    Command = "STS/1.0 400 ErrBadPasswd",//should be already log in
                    Serial = loginSerial,
                    Content = "<Error code=\"11\" server=\"1012\" module=\"1\" line=\"1683\"/>\n",
                    Encrypt = false
                };
                p.WritePacket();
                Network.SendPacket(p);
                Logger.Log.Info("Login result for " + account.UserName + ": ALREADY_LOG_IN");
                AccountSession.Instance.AccountLogout(account.AccountID, this);
            }
        }

        public void OnAuthLoginFinish(int serial)
        {
            Guid accountGuid = account.AccountID.ToGUID();
            BNSLoginPacket p = new BNSLoginPacket()
            {
                Command = "STS/1.0 200 OK",
                Serial = serial,
                Content = string.Format("<Reply>\n<UserId>{0}</UserId>\n<UserCenter>1</UserCenter>\n<Roles type=\"array\">\n<RoleId>6</RoleId>\n</Roles>\n</Reply>\n", accountGuid.ToString().ToUpper())
            };
            Logger.Log.Info("account:" + accountGuid.ToString());
            p.WritePacket();

            Network.SendPacket(p);

            p = new BNSLoginPacket()
            {
                Command = "POST /Presence/UserInfo STS/1.0",
                Serial = 0,
                Content = string.Format("<Message>\n<UserId>{0}</UserId>\n<UserCenter>1</UserCenter>\n<UserName>:{1}</UserName>\n<Status>online</Status>\n<Alias>{1}</Alias>\n<Alias>bns:{1}</Alias>\n</Message>\n", accountGuid.ToString().ToUpper(), account.UserName)
            };
            p.WritePacket();
            Network.SendPacket(p);
        }

        public void OnAccountList(int serial)
        {
            BNSLoginPacket p = new BNSLoginPacket()
            {
                Command = "STS/1.0 200 OK",
                Serial = serial,
                Content = string.Format("<Reply type=\"array\">\n<GameAccount>\n<Alias>{0}</Alias>\n<Created>2012-04-20T10:04:46Z</Created>\n</GameAccount>\n</Reply>\n", account.AccountID.ToGUID().ToString().ToUpper())
            };
            p.WritePacket();

            Network.SendPacket(p);
        }

        public void OnWorldList(int serial)
        {
            BNSLoginPacket p = new BNSLoginPacket()
            {
                Command = "STS/1.0 200 OK",
                Serial = serial
            };
            string worlds = string.Empty;
            int idx = 1;
            foreach (WorldInfo i in Configuration.Instance.Worlds)
            {
                worlds += string.Format("<World>\n<WorldCode>{0}</WorldCode>\n<WorldName>{1}</WorldName>\n<PublicNetAddress>{2}</PublicNetAddress>\n<Property>\n<EnableCharacterCreate>true</EnableCharacterCreate>\n<WorldScore>0</WorldScore>\n</Property>\n<UserCounts>\n<PlayingUsers>426</PlayingUsers>\n<WaitingUsers>0</WaitingUsers>\n<MaxUsers>4000</MaxUsers>\n</UserCounts>\n</World>\n",
                    idx++, i.Name, i.Address);
            }
            p.Content = string.Format("<Reply type=\"array\">\n{0}\n</Reply>\n", worlds);
            p.WritePacket();

            Network.SendPacket(p);
        }
    }
}
