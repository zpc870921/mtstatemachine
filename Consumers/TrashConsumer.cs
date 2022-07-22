using MassTransit;

namespace mtstatemachine.Consumers
{
    public interface TrashBin
    {
        string BinNumber { get; }
    }
        public class TrashConsumer : IConsumer<TrashBin>
        {
            public async Task Consume(ConsumeContext<TrashBin> context)
            {
                Console.WriteLine($"BinNumber:{context.Message.BinNumber}");
            }
        }

        public class TrashConsumerDefinition:ConsumerDefinition<TrashConsumer>
        {
        protected override void ConfigureConsumer(IReceiveEndpointConfigurator endpointConfigurator, IConsumerConfigurator<TrashConsumer> consumerConfigurator)
        {
            endpointConfigurator.ConnectConsumerConfigurationObserver(new ConsoleConsumeMessageFilterConfigurationObserver(endpointConfigurator));
            endpointConfigurator.UseFilter(new ConsoleConsumerFilter());
            consumerConfigurator.UseFilter(new ConsoleConsumeWithConsumerFilter<TrashConsumer>());
            consumerConfigurator.ConsumerMessage<TrashBin>(m=>m.UseFilter(new ConsoleConsumeWithConsumerFilter<TrashConsumer, TrashBin>()));
        }
    }
    }
