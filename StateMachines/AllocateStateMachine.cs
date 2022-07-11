using MassTransit;
using Automatonymous;
using System.Text.Json;
using mtstatemachine.Consumers;

namespace mtstatemachine.StateMachines
{
    public class AllocateStateMachine:MassTransitStateMachine<AllocationState>
    {
        public AllocateStateMachine(ILogger<AllocateStateMachine> logger)
        {
            InstanceState(x => x.CurrentState);
            Event(()=>AllocationCreated,x=>x.CorrelateById(m=>m.Message.AllocationId));
            Event(()=>ReleaseRequest,x=>x.CorrelateById(m=>m.Message.AllocationId));
            //Event(()=>AllocationStatusRequest,x=> {
            //    x.CorrelateById(m => m.Message.AllocationId);
            //    x.OnMissingInstance(m => m.ExecuteAsync(a => a.RespondAsync<AllocationStatusResponse>(new
            //    {
            //        AllocationId =a.Message.AllocationId,
            //        Status = "not found"
            //    })));
            //});

            Schedule(() => HoldExpiration, x => x.HoldDurationToken, s => {
                s.Delay = TimeSpan.FromDays(8);
                s.Received = m => m.CorrelateById(x => x.Message.AllocationId);
            });
            Initially(
                When(AllocationCreated)
                .Schedule(HoldExpiration, context => context.Init<AlloctionTimeoutExpiration>(new { context.Message.AllocationId }),x=>x.Message.HoldDuration)
                .TransitionTo(Allocated),
                When(ReleaseRequest)
                .TransitionTo(Released)
                );
            //DuringAny(When(AllocationStatusRequest).RespondAsync(c => c.Init<AllocationStatusResponse>(new {AllocationId=c.Message.AllocationId,Status=c.Saga.CurrentState })));

            During(Released,
                When(AllocationCreated).Then(x => {
                    Console.WriteLine($"allocation was released:{x.Saga.CorrelationId}");
                }).Finalize());
            

            During(Allocated,
                When(AllocationCreated)
                .Schedule(HoldExpiration, context => context.Init<AlloctionTimeoutExpiration>(new { context.Message.AllocationId }), x => x.Message.HoldDuration),
                When(HoldExpiration.Received)
                .ThenAsync(async x => {
                    logger.Log( LogLevel.Information, $"allocation is expired");
                }),
                When(ReleaseRequest)
                .Unschedule(HoldExpiration)
                .Then(x => {
                    Console.WriteLine($"allocation release request,granted:{x.Saga.CorrelationId}");
                }).Finalize()
                );
        }

        public Schedule<AllocationState, AlloctionTimeoutExpiration> HoldExpiration { get; set; }
        public State Allocated { get; set; }
        public State Released { get; set; }
        public Event<AllocationCreated> AllocationCreated { get; set; }
        public Event<AllocateReleaseRequest> ReleaseRequest { get; set; }


        //public Event<AllocationStatusRequest> AllocationStatusRequest { get; set; }
    }
}
