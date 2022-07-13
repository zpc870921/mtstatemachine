namespace mtstatemachine.Consumers
{
    public interface FulfillOrderCompleted
    {
        Guid OrderId { get; }
        DateTime Timestamp { get; }
    }
}

