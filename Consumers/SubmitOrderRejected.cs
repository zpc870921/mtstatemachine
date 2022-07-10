namespace mtstatemachine.Consumers
{
    public interface SubmitOrderRejected
    {
        Guid OrderId { get; }
        DateTime Timestamp { get; }
        string CustomerNumber { get; }
        string Reason { get; }
    }
}
