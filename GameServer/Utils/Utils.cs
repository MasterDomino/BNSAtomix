using System;

using SmartEngine.Network.Map;
using SmartEngine.Core.Math;
using SagaBNS.Common.Actors;
using SagaBNS.GameServer.Network.Client;
using SagaBNS.GameServer.Map;
using SagaBNS.Common.Skills;

namespace SagaBNS.GameServer
{
    public static class Utils
    {
        public static GameSession Client(this ActorPC pc)
        {
            if (pc.EventHandler == null)
            {
                return null;
            }

            return ((ActorEventHandlers.PCEventHandler)pc.EventHandler).Client;
        }

        public static ulong ToULong(this ActorMapObj mapObj)
        {
            return (ulong)mapObj.MapID << 32 | mapObj.ObjectID;
        }

        public static ushort DirectionFromTarget(this Actor actor, Actor a2)
        {
            return DirectionFromTarget(actor, a2.X, a2.Y);
        }

        public static ushort DirectionFromTarget(this Actor actor, int x, int y)
        {
            return DirectionFromTarget(actor.X, actor.Y, x, y);
        }

        public static ushort DirectionFromTarget(int selfX, int selfY, int x, int y)
        {
            Vec3 vec = new Vec3(x - selfX, y - selfY, 0);
            Quat rot = Quat.FromDirectionZAxisUp(vec);
            Angles ang = rot.ToAngles();
            float deg = -ang.Yaw;
            if (deg < 0)
            {
                deg = 360 + deg;
            }

            return (ushort)deg;
        }

        public static ushort DirectionRelativeToTarget(this Actor actor, Actor target)
        {
            return DirectionRelativeToTarget(actor, target.X, target.Y);
        }

        public static ushort DirectionRelativeToTarget(this Actor actor, int x, int y)
        {
            return actor.Dir.DirectionRelativeToTarget(actor.X, actor.Y, x, y);
        }

        public static ushort DirectionRelativeToTarget(this ushort direction, int selfX,int selfY, int x, int y)
        {
            int dir = Math.Abs(direction - DirectionFromTarget(selfX, selfY, x, y));
            if (dir >= 180)
            {
                dir = 360 - dir;
            }

            return (ushort)dir;
        }

        public static void ChangeStance(this ActorExt actor, Stances stance, byte skillSession, uint additionID, int stance2 = -1)
        {
            if (actor.Status.Stance != stance)
            {
                actor.Status.Stance = stance;
                UpdateEvent evt = UpdateEvent.NewActorAdditionEvent(actor, actor, skillSession, 0, additionID, UpdateEvent.ExtraUpdateModes.None);
                //evt.AddActorPara(PacketParameter.Stance, (int)stance);
                if (stance2 >= 0)
                {
                    //evt.AddActorPara(PacketParameter.Stance2, stance2);
                    MapManager.Instance.GetMap(actor.MapInstanceID).SendEventToAllActorsWhoCanSeeActor(MapEvents.EVENT_BROADCAST, evt, actor, true);
                }
            }
        }

        public static Vec3 DirectionToVector(this ushort dir)
        {
            Angles ang = new Angles();
            if (dir >= 180)
            {
                dir -= 360;
            }

            ang.Yaw = -dir;
            return ang.ToQuat() * new Vec3(1, 0, 0);
        }

        public static long DistanceToPoint2DLong(this Actor actor, long x, long y)
        {
            long dX = x - actor.X;
            long dY = y - actor.Y;
            return (long)(Math.Sqrt(dX * dX + dY * dY));
        }

        public static uint PartyID2CreatorID(this ulong id)
        {
            return 0x80000000 | (uint)id;
        }

        public static bool IsHit(this SkillAttackResult res)
        {
            return res != SkillAttackResult.Miss && res != SkillAttackResult.Avoid;
        }

        public static Stats FindStat(byte stat)
        {
            switch (stat)
            {
                case 0x01:
                    return Stats.MaxAtk;
                case 0x02:
                    return Stats.Defense;
                case 0x03:
                    return Stats.Resist;
                case 0x05:
                    return Stats.HitB;
                case 0x07:
                    return Stats.CritB;
                case 0x09:
                    return Stats.DefCritB;
                case 0x0B:
                    return Stats.AvoidB;
                case 0x0D:
                    return Stats.ParryB;
                case 0x13:
                    return Stats.CastTimeB;
                case 0x1A:
                    return Stats.MaxHp;
                case 0x20:
                    return Stats.PrcB;
                default:
                    return Stats.None;
            }
        }
    }
}
