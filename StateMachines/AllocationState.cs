using MassTransit;
using MongoDB.Bson.Serialization.Attributes;

namespace mtstatemachine.StateMachines
{
    public class AllocationState : SagaStateMachineInstance,ISagaVersion
    {
        [BsonId]
        public Guid CorrelationId { get; set; }
        public string CurrentState { get; set; }

        public int Version { get; set; }

        public Guid? HoldDurationToken { get; set; }
    }
}
