using Automatonymous;
using MassTransit;
using mtstatemachine.Consumers;

namespace mtstatemachine.StateMachines
{
    public class OrderAcceptedActivity : IStateMachineActivity<OrderState, OrderAccepted>
    {
        private readonly ILogger<OrderAcceptedActivity> logger;
        private readonly IServiceSvc serviceSvc;

        public OrderAcceptedActivity(ILogger<OrderAcceptedActivity> logger,IServiceSvc serviceSvc)
        {
            this.logger = logger;
            this.serviceSvc = serviceSvc;
        }
        public void Probe(ProbeContext context)
        {
            context.CreateScope("order-accepted");
        }
        public void Accept(StateMachineVisitor visitor)
        {
            visitor.Visit(this);
        }

        public async Task Execute(BehaviorContext<OrderState, OrderAccepted> context, IBehavior<OrderState, OrderAccepted> next)
        {
           // Console.WriteLine("hello orderacceptedactivity");
            string msg= this.serviceSvc.Hello("hello actitity");
            Console.WriteLine(msg+" from activity");

            var consumeContext = context.GetPayload<ConsumeContext>();

            var endpoint =await consumeContext.GetSendEndpoint(new Uri("queue:fulfill-order"));
            await endpoint.Send<FulfillOrder>(new
            {
                context.Message.OrderId
            });
            await next.Execute(context).ConfigureAwait(false);
        }

        public async Task Faulted<TException>(BehaviorExceptionContext<OrderState, OrderAccepted, TException> context, IBehavior<OrderState, OrderAccepted> next) where TException : Exception
        {
            await next.Execute(context);
        }

        
    }
}
