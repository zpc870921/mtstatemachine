namespace mtstatemachine.Consumers
{
    public interface AllocateReleaseRequest
    {
        Guid AllocationId { get; }
        string Reason { get; }
    }
}
