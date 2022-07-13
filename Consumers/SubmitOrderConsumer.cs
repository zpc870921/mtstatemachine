using MassTransit;

namespace mtstatemachine.Consumers
{
    public class SubmitOrderConsumer : IConsumer<SubmitOrder>
    {
        private readonly ILogger<SubmitOrderConsumer> logger;

        public SubmitOrderConsumer(ILogger<SubmitOrderConsumer> logger)
        {
            this.logger = logger;
        }
        public async Task Consume(ConsumeContext<SubmitOrder> context)
        {
            if(context.Message.CustomerNumber.Contains("test"))
            {
                if (context.RequestId != null) 
                    await context.RespondAsync<SubmitOrderRejected>(new
                    {
                        OrderId = context.Message.OrderId,
                        Timestamp = InVar.Timestamp,
                        CustomerNumber = context.Message.CustomerNumber,
                        Reason = "invalid customernumber can't order"
                    });
                return;
            }

            await context.Publish<OrderSubmitted>(new
            {
                OrderId = context.Message.OrderId,
                Timestamp = context.Message.Timestamp,
                CustomerNumber = context.Message.CustomerNumber,
                PaymentCardNumber =context.Message.PaymentCardNumber
            });
           if(context.RequestId!=null)
            await context.RespondAsync<SubmitOrderAccepted>(new
            {
                OrderId =context.Message.OrderId,
                Timestamp = context.Message.Timestamp,
                CustomerNumber = context.Message.CustomerNumber
            });
        }
    }
}
