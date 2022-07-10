using MassTransit;
using mtstatemachine.Consumers;

namespace mtstatemachine.CourierActivities
{
    public class AllocateInventoryActivity : IActivity<AllocateInventoryArgument, AllocateInventoryLog>
    {
        private readonly IRequestClient<AllocateInventory> allocateInventoryRequest;
       

        public AllocateInventoryActivity(IRequestClient<AllocateInventory> allocateInventoryRequest)
        {
            this.allocateInventoryRequest = allocateInventoryRequest;
        }
        public async Task<CompensationResult> Compensate(CompensateContext<AllocateInventoryLog> context)
        {
            var allocationId = context.Log.AllocationId;
            await context.Publish<AllocateReleaseRequest>(new
            {
                AllocationId = allocationId,
                Reason = "Order fault"
            });
            return context.Compensated();
        }

        public async Task<ExecutionResult> Execute(ExecuteContext<AllocateInventoryArgument> context)
        {
            var orderid = context.Arguments.OrderId;
            var itemnumber = context.Arguments.ItemNumber;
            if(string.IsNullOrWhiteSpace(itemnumber))
            {
                throw new ArgumentNullException(nameof(context.Arguments.ItemNumber));
            }

            var quantity = context.Arguments.Quantity;
            if (quantity <= 0)
            {
                throw new ArgumentException(nameof(context.Arguments.Quantity));
            }
            var allocationId = NewId.NextGuid();
            await this.allocateInventoryRequest.GetResponse<InventoryAllocated>(new
            {
                AllocationId=allocationId,
                ItemNumber = itemnumber,
                Quantity = quantity
            });

            return context.Completed(new {AllocationId= allocationId });
        }
    }

    public interface AllocateInventoryArgument
    {
         Guid OrderId { get; }
         string ItemNumber { get; set; }
         int Quantity { get; set; }
    }
    public interface AllocateInventoryLog
    {
        Guid AllocationId { get; set; }
    }
}
