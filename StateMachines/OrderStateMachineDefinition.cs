using MassTransit;

namespace mtstatemachine.StateMachines
{
    public class OrderStateMachineDefinition : SagaDefinition<OrderState>
    {
        public OrderStateMachineDefinition()
        {

        }

        protected override void ConfigureSaga(IReceiveEndpointConfigurator endpointConfigurator, ISagaConfigurator<OrderState> sagaConfigurator)
        {
            endpointConfigurator.UseMessageRetry(r=>r.Interval(3,TimeSpan.FromSeconds(1)));
        }
    }
}
