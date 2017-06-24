namespace SagaBNS.GameServer.NPC
{
    public class NPCStore
    {
        public uint ID { get; set; }
        public float BuyRate { get; set; }
        public float SellRate { get; set; }
        public float BuyBackRate { get; set; }
        public bool IsStoreByItem { get; set; }
        public uint[] Items { get; set; }
        public uint[] Materials { get; set; }
        public ushort[] MaterialCounts { get; set; }
    }
}
