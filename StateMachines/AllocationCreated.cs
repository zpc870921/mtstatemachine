namespace mtstatemachine.StateMachines
{
    public interface AllocationCreated
    {
        public Guid AllocationId { get; set; }
        public TimeSpan HoldDuration { get; set; }
    }

    public interface AllocationStatusRequest
    {
         Guid AllocationId { get; set; }
    }

    public interface AllocationStatusResponse
    {
        Guid AllocationId { get; set; }
        string Status { get; set; }
    }
}
