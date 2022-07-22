using MassTransit;

namespace mtstatemachine.Consumers
{
    public interface Doit
    {
        public TimeSpan Duration { get; set; }
    }
    public class DoJobConsumer : IJobConsumer<Doit>
    {
        public async Task Run(JobContext<Doit> context)
        {
            await Task.Delay(1000);
            if(context.RetryAttempt==0)
            {
                throw new Exception("not now,i'm busy!");
            }
            await Task.Delay(context.Job.Duration-TimeSpan.FromSeconds(1),context.CancellationToken);
            Console.WriteLine($"xxxxxxxx=================job completed:{DateTime.Now}xxxxxxxxxxxxx===========================");
        }
    }

    public class DoJobConsumerDefinition:ConsumerDefinition<DoJobConsumer>
    {
        protected override void ConfigureConsumer(IReceiveEndpointConfigurator endpointConfigurator, IConsumerConfigurator<DoJobConsumer> consumerConfigurator)
        {
            consumerConfigurator.Options<JobOptions<Doit>>(options => {
                options.SetJobTimeout(TimeSpan.FromMinutes(5));
                options.SetConcurrentJobLimit(20);
                options.SetRetry(r => r.Interval(5, 5000));
            });
        }
    }
}
