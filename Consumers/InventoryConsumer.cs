using MassTransit;
using mtstatemachine.StateMachines;

namespace mtstatemachine.Consumers
{
    public class InventoryConsumer : IConsumer<AllocateInventory>
    {
        public async Task Consume(ConsumeContext<AllocateInventory> context)
        {
          //  await Task.Delay(500);

           // var alloctionId = NewId.NextGuid();
            await context.Publish<AllocationCreated>(new
            {
                AllocationId =context.Message.AllocationId,
                HoldDuration = TimeSpan.FromSeconds(8)
            });

            await context.RespondAsync<InventoryAllocated>(new
            {
                AllocationId =context.Message.AllocationId,
                ItemNumber = context.Message.ItemNumber,
                Quantity = context.Message.Quantity
            });
        }
    }
}
