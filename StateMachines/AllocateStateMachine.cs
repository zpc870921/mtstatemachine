using MassTransit;
using Automatonymous;
using System.Text.Json;

namespace mtstatemachine.StateMachines
{
    public class AllocateStateMachine:MassTransitStateMachine<AllocationState>
    {
        public AllocateStateMachine(ILogger<AllocateStateMachine> logger)
        {
            InstanceState(x => x.CurrentState);
            Event(()=>AllocationCreated,x=>x.CorrelateById(m=>m.Message.AllocationId));
            Event(()=>AllocationStatusRequest,x=> {
                x.CorrelateById(m => m.Message.AllocationId);
                x.OnMissingInstance(m => m.ExecuteAsync(a => a.RespondAsync<AllocationStatusResponse>(new
                {
                    AllocationId =a.Message.AllocationId,
                    Status = "not found"
                })));
            });

            Schedule(() => HoldExpiration, x => x.HoldDurationToken, s => {
                s.Delay = TimeSpan.FromDays(8);
                s.Received = m => m.CorrelateById(x => x.Message.AlloctionId);
            });
            Initially(
                When(AllocationCreated)
                .Then(async x => {
                    await Console.Out.WriteLineAsync($"initially.AllocationCreated-data:{JsonSerializer.Serialize(x.Message)},saga:{JsonSerializer.Serialize(x.Saga)}");
                })
                .Schedule(HoldExpiration, context => context.Init<AlloctionTimeoutExpiration>(new { AllocationId = context.Message.AllocationId }),x=>x.Message.HoldDuration)
                .TransitionTo(Allocated)                
                );
            DuringAny(When(AllocationStatusRequest).RespondAsync(c => c.Init<AllocationStatusResponse>(new {AllocationId=c.Message.AllocationId,Status=c.Saga.CurrentState })));

            During(Allocated,
                When(HoldExpiration.Received)
                .ThenAsync(async x => {
                    logger.Log( LogLevel.Information, $"allocatestatemachine.AlloctionTimeoutExpiration.Received xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx:{DateTime.Now}");
                }));
        }

        public Schedule<AllocationState, AlloctionTimeoutExpiration> HoldExpiration { get; set; }
        public State Allocated { get; set; }
        public Event<AllocationCreated> AllocationCreated { get; set; }
        public Event<AllocationStatusRequest> AllocationStatusRequest { get; set; }
    }
}
