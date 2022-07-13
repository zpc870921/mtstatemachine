namespace mtstatemachine.Consumers
{
    public interface FulfillOrderFaulted
    {
        Guid OrderId { get; }
        DateTime Timestamp { get; }
    }
}

