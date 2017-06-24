namespace SagaBNS.Common.Actors
{
    public class ActorNPC : ActorExt
    {
        private readonly NPCData data;
        public NPCData BaseData
        {
            get { return data; }
        }

        public ActorNPC(NPCData data)
        {
            this.data = data;
            type = SmartEngine.Network.Map.ActorType.NPC;
            SightRange = data.SightRange;
            HP = data.MaxHP;
            MaxHP = data.MaxHP;
            MP = data.MaxMP;
            MaxMP = data.MaxMP;
            Level = data.Level;
            ManaType = data.ManaType;
            Faction = data.Faction;
            Speed = 1000;
            Status.AtkMin = data.AtkMin;
            Status.AtkMax = data.AtkMax;
        }

        public ushort NpcID
        {
            get
            {
                return data.NpcID;
            }
        }

        public ushort StandartMotion { get; set; }

        public int AppearEffect { get; set; }

        public int DisappearEffect { get; set; }

        public int MoveRange { get; set; }

        public short X_Ori { get; set; }
        public short Y_Ori { get; set; }
        public short Z_Ori { get; set; }
    }
}
