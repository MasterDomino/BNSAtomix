using System.Collections.Generic;
using SmartEngine.Network;
using SagaBNS.Common.Packets;
using SagaBNS.Common.Skills;
using SagaBNS.GameServer.Network.CharacterServer;
using SagaBNS.GameServer.Packets.Client;
using SagaBNS.GameServer.Skills.SkillHandlers.Common;

namespace SagaBNS.GameServer.Network.Client
{
    public partial class GameSession : Session<GamePacketOpcode>
    {
        public void SendTeleportAdd()
        {
            ushort location = Command.Commands.Instance.TeleportPoint(this).teleportId;
            if (!chara.Locations.Contains(location))
            {
                SM_LOCATION_ADD_TELEPORT p = new SM_LOCATION_ADD_TELEPORT()
                {
                    LocationId = location
                };
                chara.Locations.Add(location);
                Network.SendPacket(p);
            }
        }

        public void SendTeleportLoad()
        {
            SM_LOCATION_LOAD_TELEPORT p = new SM_LOCATION_LOAD_TELEPORT()
            {
                Locations = chara.Locations
            };
            Network.SendPacket(p);
        }

        public void OnGotTeleportInfo(List<ushort> locations)
        {
            foreach (ushort i in locations)
            {
                chara.Locations.Add(i);
            }
            CharacterSession.Instance.GetSkills(chara.CharID, this);
        }

        public void OnTeleportRequest(CM_TELEPORT_REQUEST p)
        {
            if (chara.Locations.Contains(p.Location))
            {
                Dictionary<Common.Item.Item, ushort> items = new Dictionary<Common.Item.Item, ushort>();
                Common.Item.Item add;
                ushort total = 0;
                foreach (KeyValuePair<ushort, ushort> i in p.ExchangeItems)
                {
                    add = Character.Inventory.Container[Common.Item.Containers.Inventory][i.Key];
                    if (add != null)
                    {
                        if (add.Count >= i.Value && add.ItemID == 65000)
                        {
                            items.Add(add, i.Value);
                            total += i.Value;
                        }
                        else
                        {
                            break;
                        }
                    }
                }

                if (total > 0 && total < 30)
                {
                    SM_TELEPORT_REQUEST r = new SM_TELEPORT_REQUEST()
                    {
                        Location = p.Location,
                        Time = 100
                    };
                    Network.SendPacket(r);

                    Teleport teleport = new Teleport();
                    SkillArg arg = new SkillArg()
                    {
                        Caster = chara,
                        Dir = p.Location
                    };
                    teleport.HandleSkillActivate(arg,items);
                }
            }
        }
    }
}