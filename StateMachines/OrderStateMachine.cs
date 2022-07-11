using System;
using MassTransit;
using mtstatemachine.Consumers;

namespace mtstatemachine.StateMachines
{
    public class OrderStateMachine : MassTransitStateMachine<OrderState>
    {
        public OrderStateMachine()
        {
            
            Event(()=>OrderSubmitted,x=>x.CorrelateById(m=>m.Message.OrderId));
            Event(()=> OrderAccepted, x=>x.CorrelateById(m=>m.Message.OrderId));
            Event(()=> CustomerAccountClosed, x => x.CorrelateBy((saga, context) =>
                saga.CustomerNumber == context.Message.CustomerNumber));
            Event(()=>CheckOrderRequest,x=> { 
                x.CorrelateById(m => m.Message.OrderId);
                x.OnMissingInstance(m => m.ExecuteAsync(e => e.RespondAsync<OrderNotFound>(new
                {
                    OrderId = e.Message.OrderId
                })));
            });

          

            InstanceState(x => x.CurrentState);
            Initially(
                When(OrderSubmitted)
                .Then(x => {
                    x.Saga.SubmittedDate = x.Message.Timestamp;
                    x.Saga.CustomerNumber = x.Message.CustomerNumber;
                })
                .TransitionTo(Submitted)
                );

            During(Submitted,
                Ignore(OrderSubmitted),
                When(CustomerAccountClosed).TransitionTo(Canceled),
                When(OrderAccepted)
                .Activity(x=>x.OfType<OrderAcceptedActivity>())
                .TransitionTo(Accepted)
                );

            DuringAny(When(OrderSubmitted)
                .Then(x => {
                x.Saga.SubmittedDate ??= x.Message.Timestamp;
                x.Saga.CustomerNumber ??= x.Message.CustomerNumber;
            }));
            DuringAny(When(CheckOrderRequest).RespondAsync(x => x.Init<CheckOrderStatus>(new
            {
                OrderId = x.Saga.CorrelationId,
                State = x.Saga.CurrentState
            })));

        }
        public State Submitted { get; set; }
        public State Canceled { get; set; }
        public State Accepted { get; set; }
        public State Faulted { get; set; }

        public Event<OrderSubmitted> OrderSubmitted { get; set; }
        public Event<OrderAccepted> OrderAccepted { get; set; }
        public Event<CustomerAccountClosed> CustomerAccountClosed { get; set; }
        public Event<CheckOrderRequest> CheckOrderRequest { get; set; }

       
    }
}
