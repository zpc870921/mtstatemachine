using MassTransit;
using MassTransit.Configuration;
using MassTransit.Metadata;

public class ConsoleConsumerFilter : IFilter<ConsumeContext>
{
    public void Probe(ProbeContext context)
    {
        context.CreateFilterScope("consoleconsumefilter");
    }

    public async Task Send(ConsumeContext context, IPipe<ConsumeContext> next)
    {
        Console.WriteLine($"messageid:{context.MessageId}");
        await next.Send(context).ConfigureAwait(false);
    }
}


public class ConsoleConsumerMessageFilter<TMessage> : IFilter<ConsumeContext<TMessage>>
    where TMessage : class
{
    public void Probe(ProbeContext context)
    {
        context.CreateFilterScope("consoleconsumefilter");

        context.Add("output", "console");
    }

    public async Task Send(ConsumeContext<TMessage> context, IPipe<ConsumeContext<TMessage>> next)
    {
        Console.WriteLine($"consume messageid:{context.MessageId}");
        await next.Send(context).ConfigureAwait(false);
    }
}

public class ConsoleConsumeMessageFilterConfigurationObserver : IConsumerConfigurationObserver
{
    private readonly IConsumePipeConfigurator consumePipeConfigurator;

    public ConsoleConsumeMessageFilterConfigurationObserver(IConsumePipeConfigurator consumePipeConfigurator)
    {
        this.consumePipeConfigurator = consumePipeConfigurator;
    }
    public void ConsumerConfigured<TConsumer>(IConsumerConfigurator<TConsumer> configurator) where TConsumer : class
    {
    }

    public void ConsumerMessageConfigured<TConsumer, TMessage>(IConsumerMessageConfigurator<TConsumer, TMessage> configurator)
        where TConsumer : class
        where TMessage : class
    {
        this.consumePipeConfigurator.AddPipeSpecification(new FilterPipeSpecification<ConsumeContext<TMessage>>(new ConsoleConsumerMessageFilter<TMessage>()));
    }
}

public class ConsoleConsumeWithConsumerFilter<TConsumer> : IFilter<ConsumerConsumeContext<TConsumer>> where TConsumer : class
{
    public void Probe(ProbeContext context)
    {
        context.CreateFilterScope("consolewithconsumerconsumefilter");
    }

    public async Task Send(ConsumerConsumeContext<TConsumer> context, IPipe<ConsumerConsumeContext<TConsumer>> next)
    {
        Console.WriteLine($"consumer messageid:{context.MessageId}");
        await next.Send(context).ConfigureAwait(false);
    }


    }


public interface IMessageValidator<TMessage>
    where TMessage : class
{
    Task Validate(ConsumeContext<TMessage> context);
}

public class MessageValidator<TMessage> : IMessageValidator<TMessage>
    where TMessage: class
{
    public Task Validate(ConsumeContext<TMessage> context)
    {
        Console.WriteLine($"validate message:{context.MessageId}");
        return Task.CompletedTask;
    }
}

public class ConsoleConsumeWithConsumerFilter<TConsumer, TMessage> : IFilter<ConsumerConsumeContext<TConsumer, TMessage>>
        where TConsumer : class
        where TMessage : class
{
    public void Probe(ProbeContext context)
    {
        context.CreateFilterScope("consolewithconsumerconsumefilter");
    }

    public async Task Send(ConsumerConsumeContext<TConsumer, TMessage> context, IPipe<ConsumerConsumeContext<TConsumer, TMessage>> next)
    {
        Console.WriteLine($"consumer/message messageid:{context.MessageId},{TypeMetadataCache<TMessage>.ShortName}");

        var serviceprovider = context.GetPayload<IServiceProvider>();
        var validator = serviceprovider.GetService<IMessageValidator<TMessage>>();
        if (validator != null)
        {
            await validator.Validate(context);
        }

        await next.Send(context).ConfigureAwait(false);
    }
}