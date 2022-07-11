using MassTransit;

namespace mtstatemachine.StateMachines
{
    public class AllocationStateDefinition:SagaDefinition<AllocationState>
    {
        public AllocationStateDefinition()
        {
            ConcurrentMessageLimit = 4;
        }
        protected override void ConfigureSaga(IReceiveEndpointConfigurator endpointConfigurator, ISagaConfigurator<AllocationState> sagaConfigurator)
        {
            endpointConfigurator.UseMessageRetry(r => r.Interval(3, 1000));
            endpointConfigurator.UseInMemoryOutbox();
        }
    }
}
