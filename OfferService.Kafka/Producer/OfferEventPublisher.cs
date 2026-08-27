using Microsoft.Extensions.Options;
using OfferService.Business.Interfaces;
using OfferService.Kafka.Contracts;
using OfferService.Kafka.Events;
using OfferService.Kafka.Topics;
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

        public Task OfferCreatedAsync(OfferCreatedDto offer)
            => PublishAsync(OfferKafkaEvents.OfferCreated, Insert, offer);

        public Task OfferAcceptedAsync(OfferAcceptedDto offer)
            => PublishAsync(OfferKafkaEvents.OfferAccepted, Insert, offer);

        public Task OfferRejectedAsync(OfferRejectedDto offer)
            => PublishAsync(OfferKafkaEvents.OfferRejected, Insert, offer);

        public Task OfferCancelledAsync(OfferCancelledDto offer)
            => PublishAsync(OfferKafkaEvents.OfferCancelled, Insert, offer);

        public Task OfferUpdatedAsync(OfferUpdatedDto offer)
            => PublishAsync(OfferKafkaEvents.OfferUpdated, Insert, offer);

        private async Task PublishAsync<T>(string kafkaKey, string crudOperation, T offerDto)
        {
            var operationMessage = new OperationMessage<T>
            {
                Operation = crudOperation,
                Dto = offerDto
            };

            string json = JsonSerializer.Serialize(operationMessage);

            await _producerClient.ProduceAsync(
                _topics.OfferEvents,
                kafkaKey,
                json);
        }
    }
}