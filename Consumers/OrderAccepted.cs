namespace mtstatemachine.Consumers
{
    public interface OrderAccepted
    {
        Guid OrderId { get; set; }
        DateTime Timestamp { get; set; }
    }
}
