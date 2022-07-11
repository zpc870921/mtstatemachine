namespace mtstatemachine.CourierActivities
{
    public interface AllocateInventoryArgument
    {
         Guid OrderId { get; }
         string ItemNumber { get; set; }
         int Quantity { get; set; }
    }
}
