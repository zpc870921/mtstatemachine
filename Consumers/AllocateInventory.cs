namespace mtstatemachine.Consumers
{
    public interface AllocateInventory
    {
        Guid AllocationId { get; }
        string ItemNumber { get; }
        int Quantity { get; }
    }
}
