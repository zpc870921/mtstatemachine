using MassTransit;

namespace mtstatemachine.BatchConsumers
{
    public class RoutingSlipBatchEventConsumerDefinition:ConsumerDefinition<RoutingSlipBatchEventConsumer>
    {

        public RoutingSlipBatchEventConsumerDefinition()
        {
            Endpoint(cfg=>cfg.PrefetchCount=50);
        }
        protected override void ConfigureConsumer(IReceiveEndpointConfigurator endpointConfigurator, IConsumerConfigurator<RoutingSlipBatchEventConsumer> consumerConfigurator)
        {
            consumerConfigurator.Options<BatchOptions>(options => {
                options.MessageLimit = 10;
                options.TimeLimit = TimeSpan.FromSeconds(5);
            });
        }
    }
}
