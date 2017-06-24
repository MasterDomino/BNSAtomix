using System.Linq;
using SmartEngine.Network;
using SagaBNS.Common.Actors;
using SagaBNS.Common.Packets;
using SagaBNS.Common.Packets.GameServer;
using SagaBNS.GameServer.Map;
using SagaBNS.GameServer.Packets.Client;

namespace SagaBNS.GameServer.Network.Client
{
    public partial class GameSession : Session<GamePacketOpcode>
    {
        public void OnPlayerRevive(CM_PLAYER_REVIVE p)
        {
            if (chara.Status.Dead && !chara.Status.Recovering && !chara.Status.ShouldRespawn)
            {
                chara.Status.ShouldRespawn = true;
                chara.Status.ShouldLoadMap = true;

                var points = from point in map.RespawnPoints
                             orderby chara.DistanceToPoint(point.X, point.Y, point.Z)
                             select point;
                RespawnPoint res = points.FirstOrDefault();
                if (res.MapID == 0)
                {
                    map.SendActorToMap(chara, map, chara.X, chara.Y, chara.Z);
                }
                else
                {
                    Map.Map tMap = MapManager.Instance.GetMap(res.MapID, chara.CharID, chara.PartyID);
                    if (map != null)
                    {
                        chara.Dir = res.Dir;
                        map.SendActorToMap(chara, tMap, res.X, res.Y, res.Z);
                    }
                    else
                    {
                        map.SendActorToMap(chara, map, chara.X, chara.Y, chara.Z);
                    }
                }
            }
        }

        public void OnPlayerRecover(CM_PLAYER_RECOVER p)
        {
            Tasks.Player.RecoverTask task = new Tasks.Player.RecoverTask(this);
            chara.Tasks["RecoverTask"] = task;
            task.Activate();
        }

        public void SendPlayerEquiptStats(Common.Item.Item item, Stats extra2 = Stats.None, bool all = false)
        {
            UpdateEvent evt = new UpdateEvent();
            Stats extra = Utils.FindStat(item.Synthesis);
            /*
            if (item.BaseData.PrimaryStats.ContainsKey(Stats.MinAtk))
                evt.AddActorPara(Common.Packets.GameServer.PacketParameter.EquiptMinAtk, chara.Status.AtkMinExt);
            if (item.BaseData.PrimaryStats.ContainsKey(Stats.MaxAtk))
                evt.AddActorPara(Common.Packets.GameServer.PacketParameter.EquiptMaxAtk, chara.Status.AtkMaxExt);
            if (item.BaseData.PrimaryStats.ContainsKey(Stats.HitB) || Stats.HitB == extra || Stats.HitB == extra2 || all)
                evt.AddActorPara(Common.Packets.GameServer.PacketParameter.HitBase, chara.Status.Hit);
            if (item.BaseData.PrimaryStats.ContainsKey(Stats.AvoidB) || Stats.AvoidB == extra || Stats.AvoidB == extra2 || all)
                evt.AddActorPara(Common.Packets.GameServer.PacketParameter.AvoidBase, chara.Status.Avoid);
            if (item.BaseData.PrimaryStats.ContainsKey(Stats.CritB) || Stats.CritB == extra || Stats.CritB == extra2 || all)
                evt.AddActorPara(Common.Packets.GameServer.PacketParameter.CriticalBase, chara.Status.Critical);
            if (item.BaseData.PrimaryStats.ContainsKey(Stats.DefCritB) || Stats.DefCritB == extra || Stats.DefCritB == extra2 || all)
                evt.AddActorPara(Common.Packets.GameServer.PacketParameter.CriticalResist, chara.Status.CriticalResist);
            if (item.BaseData.PrimaryStats.ContainsKey(Stats.ParryB) || Stats.ParryB == extra || Stats.ParryB == extra2 || all)
                evt.AddActorPara(Common.Packets.GameServer.PacketParameter.ParryBase, chara.Status.Parry);
            if (item.BaseData.PrimaryStats.ContainsKey(Stats.PrcB) || Stats.PrcB == extra || Stats.PrcB == extra2 || all)
                evt.AddActorPara(Common.Packets.GameServer.PacketParameter.PierceBase, chara.Status.Pierce);
             */
            if (item.BaseData.PrimaryStats.ContainsKey(Stats.MaxHp) || Stats.MaxHp == extra || Stats.MaxHp == extra2 || all)
            {
                SendPlayerHP(true);
            }

            SM_PLAYER_UPDATE_LIST p = new SM_PLAYER_UPDATE_LIST()
            {
                Parameters = evt
            };
            Network.SendPacket(p);
        }

        public void SendPlayerHP(bool sendMaxHP = false)
        {
            UpdateEvent evt = new UpdateEvent()
            {
                Actor = chara,
                Target = chara,
                UpdateType = UpdateTypes.Actor
            };
            evt.AddActorPara(Common.Packets.GameServer.PacketParameter.HP, chara.HP);
            if (sendMaxHP)
            {
                //evt.AddActorPara(Common.Packets.GameServer.PacketParameter.MaxHP, chara.MaxHP);
                map.SendEventToAllActorsWhoCanSeeActor(MapEvents.EVENT_BROADCAST, evt, chara, true);
            }
        }

        public void SendPlayerMP()
        {
            UpdateEvent evt = new UpdateEvent()
            {
                Actor = chara,
                Target = chara,
                UpdateType = UpdateTypes.Actor
            };
            switch (chara.ManaType)
            {
                case ManaType.BladeSpirit:
                case ManaType.Chakra:
                case ManaType.CombatSpirit:
                    {
                        //evt.AddActorPara(Common.Packets.GameServer.PacketParameter.BladeSpirit, chara.MP);
                        evt.UserData = new byte[] { 7, 2, 1, 0, 0, 0 };
                    }
                    break;
                case ManaType.Force:
                    {
                        //evt.AddActorPara(Common.Packets.GameServer.PacketParameter.MP, chara.MP);
                        evt.UserData = new byte[] { 7, 2, 1, 0, 0, 0 };
                    }
                    break;
            }
            map.SendEventToAllActorsWhoCanSeeActor(MapEvents.EVENT_BROADCAST, evt, chara, true);
        }

        public void SendPlayerGold()
        {
            UpdateEvent evt = new UpdateEvent();
            evt.AddActorPara(PacketParameter.Gold, chara.Gold);
            SM_PLAYER_UPDATE_LIST p = new SM_PLAYER_UPDATE_LIST()
            {
                Parameters = evt
            };
            Network.SendPacket(p);
        }

        public void SendPlayerLevelUp()
        {
            UpdateEvent evt = new UpdateEvent()
            {
                Actor = chara,
                Target = chara,
                UpdateType = UpdateTypes.Actor
            };
            evt.AddActorPara(Common.Packets.GameServer.PacketParameter.Level, chara.Level);
            evt.AddActorPara(PacketParameter.EXPBC, chara.Exp);
            //evt.AddActorPara(PacketParameter.MaxHP, chara.MaxHP);
            evt.AddActorPara(PacketParameter.HP, chara.HP);
            map.SendEventToAllActorsWhoCanSeeActor(MapEvents.EVENT_BROADCAST, evt, chara, true);
        }

        public void SendPlayerEXP()
        {
            UpdateEvent evt = new UpdateEvent()
            {
                Actor = chara,
                Target = chara,
                UpdateType = UpdateTypes.Actor
            };
            evt.AddActorPara(PacketParameter.EXPBC, chara.Exp);
            map.SendEventToAllActorsWhoCanSeeActor(MapEvents.EVENT_BROADCAST, evt, chara, true);
        }

        public void SendPlayerMovementLock(bool locked)
        {
            SM_PLAYER_UPDATE_LIST p = new SM_PLAYER_UPDATE_LIST()
            {

                //evt.AddActorPara(Common.Packets.GameServer.PacketParameter.NoMove, locked ? 1 : 0);
                //evt.AddActorPara(Common.Packets.GameServer.PacketParameter.NoRotate, locked ? 1 : 0);
                Parameters = new UpdateEvent()
            };
            Network.SendPacket(p);
        }

        public void SendPlayerStats()
        {
            SM_PLAYER_UPDATE_LIST p = new SM_PLAYER_UPDATE_LIST()
            {

                /*
                evt.AddActorPara(PacketParameter.EquiptMinAtk2, chara.Status.AtkMinBase);
                evt.AddActorPara(PacketParameter.EquiptMaxAtk2, chara.Status.AtkMaxBase);
                evt.AddActorPara(PacketParameter.HitBase, chara.Status.Hit);
                evt.AddActorPara(PacketParameter.Penetration, chara.Status.Penetration);
                evt.AddActorPara(PacketParameter.CriticalPercent, 1500);
                evt.AddActorPara(PacketParameter.CriticalBase, chara.Status.Critical);
                evt.AddActorPara(PacketParameter.CriticalResist, chara.Status.CriticalResist);
                evt.AddActorPara(PacketParameter.AvoidBase, chara.Status.Avoid);
                evt.AddActorPara(PacketParameter.ParryBase, chara.Status.Parry);
                evt.AddActorPara(PacketParameter.PierceBase, chara.Status.Pierce);
                evt.AddActorPara(PacketParameter.EquiptMinAtk, chara.Status.AtkMinExt);
                evt.AddActorPara(PacketParameter.EquiptMaxAtk, chara.Status.AtkMaxExt);
                evt.AddActorPara(PacketParameter.Def, chara.Status.Defence);
                */
                Parameters = new UpdateEvent()
            };
            Network.SendPacket(p);
        }
    }
}
