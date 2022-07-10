namespace mtstatemachine.Consumers
{
    public interface InventoryAllocated
    {
        Guid AllocationId { get; }
        string ItemNumber { get; }
        int Quantity { get; }
    }
}
