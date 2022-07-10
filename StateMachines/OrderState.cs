using MassTransit;
using MongoDB.Bson.Serialization.Attributes;

namespace mtstatemachine.StateMachines
{
    public class OrderState : SagaStateMachineInstance,ISagaVersion
    {
        [BsonId]
        public Guid CorrelationId { get; set; }
        public string CurrentState { get; set; }

        public DateTime? AcceptedDate { get; set; }
        public DateTime? SubmittedDate { get; set; }
        public string CustomerNumber { get; set; }
        public int Version { get; set; }

    }
}
