namespace mtstatemachine.Consumers
{
    public interface CustomerAccountClosed
    {
        Guid CustomerId { get; set; }
        string CustomerNumber { get; set; }
    }
}
