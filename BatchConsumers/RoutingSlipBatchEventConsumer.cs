using MassTransit;
using MassTransit.Courier.Contracts;

namespace mtstatemachine.BatchConsumers
{

    public class RoutingSlipBatchEventConsumer : IConsumer<Batch<RoutingSlipCompleted>>
    {
        private readonly ILogger<RoutingSlipBatchEventConsumer> logger;

        public RoutingSlipBatchEventConsumer(ILogger<RoutingSlipBatchEventConsumer> logger)
        {
            this.logger = logger;
        }
        public async Task Consume(ConsumeContext<Batch<RoutingSlipCompleted>> context)
        {
            if (this.logger.IsEnabled(LogLevel.Information))
            {
                this.logger.LogInformation("routing slip completed:{TrackingNumbers}",String.Join(",", context.Message.Select(m => m.Message.TrackingNumber)));
            }
        }
    }
}
