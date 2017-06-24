namespace SagaBNS.GameServer.Item
{
    public enum ItemUpdateMethod
    {
        List,
        Add,
        Update,
        Move = 4,
        Remove,
        Repair = 13,
        Use = 16,
        Sold,
        Delete = 18,
        RemoveSeal = 19,
        Disassembled = 20,
        Synthesis = 22,
        Give = 24,
    }
}
