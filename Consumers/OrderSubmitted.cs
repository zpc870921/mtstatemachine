namespace mtstatemachine.Consumers
{
    public interface OrderSubmitted
    {
        Guid OrderId { get; }
        DateTime Timestamp { get; }
        string CustomerNumber { get; }
    }
}
