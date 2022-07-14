using MassTransit;

namespace mtstatemachine.Consumers
{
    public class FulfillOrderConsumerDefinition:ConsumerDefinition<FulfillOrderConsumer>
    {
        public FulfillOrderConsumerDefinition()
        {
            ConcurrentMessageLimit = 4;
        }
        protected override void ConfigureConsumer(IReceiveEndpointConfigurator endpointConfigurator, IConsumerConfigurator<FulfillOrderConsumer> consumerConfigurator)
        {
            endpointConfigurator.UseMessageRetry(r =>
            {
                r.Ignore<InvalidOperationException>();
                r.Interval(3, 1000);
            });
        }
    }
}

