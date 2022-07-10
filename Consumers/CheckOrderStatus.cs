namespace mtstatemachine.Consumers
{
    public interface CheckOrderRequest
    {
        Guid OrderId { get; }
    }

    public interface OrderNotFound
    {
        Guid OrderId { get; }
    }

    public interface CheckOrderStatus
    {
        Guid OrderId { get; set; }
        string State { get; set; }
    }
}
