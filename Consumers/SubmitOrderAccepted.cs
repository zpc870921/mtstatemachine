namespace mtstatemachine.Consumers
{
    public interface SubmitOrderAccepted
    {
        Guid OrderId { get; }
        DateTime Timestamp { get; }
        string CustomerNumber { get; }
    }
}
