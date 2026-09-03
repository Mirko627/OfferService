using Microsoft.Extensions.Options;
using OfferService.Kafka.Topics;
using OfferService.Repository.Entities;
using OfferService.Shared.Kafka.Contracts;
using OfferService.Shared.Kafka.Events;
using System.Text.Json;
using Utility.Kafka.Abstractions.Clients;
using Utility.Kafka.Messages;

namespace OfferService.Kafka.Producer
{
    public class OfferEventPublisher : IOfferEventPublisher
    {
        private const string Insert = "I";
        private const string Update = "U";
        private const string Delete = "D";
        
        private readonly IProducerClient<string, string> _producerClient;
        private readonly OfferServiceTopicsOutput _topics;

        public OfferEventPublisher(
            IProducerClient<string, string> producerClient,
            IOptions<OfferServiceTopicsOutput> topics)
        {
            _producerClient = producerClient;
            _topics = topics.Value;
        }

        public OutboxEvent CreateOfferCreatedEvent(OfferCreatedDto offer)
            => CreateEvent(OfferKafkaEvents.OfferCreated,Insert,offer);

        public OutboxEvent CreateOfferAcceptedEvent(OfferAcceptedDto offer)
            => CreateEvent(OfferKafkaEvents.OfferAccepted, Insert, offer);

        public OutboxEvent CreateOfferRejectedEvent(OfferRejectedDto offer)
            => CreateEvent(OfferKafkaEvents.OfferRejected, Insert, offer);

        public OutboxEvent CreateOfferCancelledEvent(OfferCancelledDto offer)
            => CreateEvent(OfferKafkaEvents.OfferCancelled, Insert, offer);

        public OutboxEvent CreateOfferUpdatedEvent(OfferUpdatedDto offer)
            => CreateEvent(OfferKafkaEvents.OfferUpdated, Insert, offer);

        private OutboxEvent CreateEvent<T>(string eventType, string operation, T dto)
        {
            var operationMessage = new OperationMessage<T>
            {
                Operation = operation,
                Dto = dto
            };

            string json = JsonSerializer.Serialize(operationMessage);

            return new OutboxEvent
            {
                Id = Guid.NewGuid(),
                EventType = eventType,
                Topic = _topics.OfferEvents,
                Key = eventType,
                Payload = json,
                CreatedAt = DateTime.UtcNow,
                PublishedAt = null
            };
        }
    }
}