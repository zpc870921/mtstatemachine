using MassTransit;
using MassTransit.Courier.Contracts;

namespace mtstatemachine.Consumers
{
    public class RoutingslipEventConsumer : IConsumer<RoutingSlipCompleted>,IConsumer<RoutingSlipActivityCompleted>,IConsumer<RoutingSlipFaulted>
    {
        private readonly ILogger<RoutingslipEventConsumer> logger;

        public RoutingslipEventConsumer(ILogger<RoutingslipEventConsumer> logger)
        {
            this.logger = logger;
        }
        public Task Consume(ConsumeContext<RoutingSlipCompleted> context)
        {
            if(this.logger.IsEnabled(LogLevel.Information))
            {
                this.logger.Log(LogLevel.Information,"routingslipcompleted:{TrackingNumber}",context.Message.TrackingNumber);
            }
            return Task.CompletedTask;
        }

        public Task Consume(ConsumeContext<RoutingSlipActivityCompleted> context)
        {
            if (this.logger.IsEnabled(LogLevel.Information))
            {
                this.logger.Log(LogLevel.Information, "RoutingSlipActivityCompleted:{ActivityName},trackingnumber:{TrackingNumber}", context.Message.ActivityName,context.Message.TrackingNumber);
            }
            return Task.CompletedTask;
        }

        public Task Consume(ConsumeContext<RoutingSlipFaulted> context)
        {
            if (this.logger.IsEnabled(LogLevel.Information))
            {
                this.logger.Log(LogLevel.Information, "RoutingSlipFaulted:{TrackingNumber},{ExceptionInfo}", context.Message.TrackingNumber,context.Message.ActivityExceptions.FirstOrDefault());
            }
            return Task.CompletedTask;
        }
    }
}
