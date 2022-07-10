using MassTransit;

namespace mtstatemachine.Consumers
{
    public class FulfillOrderConsumer : IConsumer<FulfillOrder>
    {
        public async Task Consume(ConsumeContext<FulfillOrder> context)
        {
            var orderId = context.Message.OrderId;
            var builder = new RoutingSlipBuilder(NewId.NextGuid());
            builder.AddActivity("AllocateInventory", new Uri("queue:allocate-inventory_execute"), new
            {
                ItemNumber = "item123",
                Quantity = 10
            });
            builder.AddVariable("OrderId", orderId);
            await context.Execute(builder.Build());
        }
    }
}

