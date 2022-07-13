using MassTransit;
using MassTransit.Courier.Contracts;

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
            builder.AddActivity("PaymentActiviy", new Uri("queue:payment_execute"), new
            {
                Amount =10,
                CardNumber =context.Message.PaymentCardNumber?? "5999-4222-2080"
            });
            builder.AddVariable("OrderId", orderId);
            await builder.AddSubscription(context.SourceAddress, RoutingSlipEvents.Faulted| RoutingSlipEvents.Supplemental, MassTransit.Courier.Contracts.RoutingSlipEventContents.None, endpoint =>
                endpoint.Send<FulfillOrderFaulted>(new { context.Message.OrderId })
            );
            await builder.AddSubscription(context.SourceAddress, RoutingSlipEvents.Completed | RoutingSlipEvents.Supplemental, MassTransit.Courier.Contracts.RoutingSlipEventContents.None, endpoint =>
                 endpoint.Send<FulfillOrderCompleted>(new { context.Message.OrderId })
            );
            await context.Execute(builder.Build());
        }
    }
}

