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
            => PublishAsync(OfferKafkaEvents.OfferCreated, offer);

        public Task OfferAcceptedAsync(OfferAcceptedDto offer)
            => PublishAsync(OfferKafkaEvents.OfferAccepted, offer);

        public Task OfferRejectedAsync(OfferRejectedDto offer)
            => PublishAsync(OfferKafkaEvents.OfferRejected, offer);

        public Task OfferCancelledAsync(OfferCancelledDto offer)
            => PublishAsync(OfferKafkaEvents.OfferCancelled, offer);

        public Task OfferUpdatedAsync(OfferUpdatedDto offer)
            => PublishAsync(OfferKafkaEvents.OfferUpdated, offer);

        private async Task PublishAsync<T>(string eventName, T offerDto)
        {
            var operationMessage = new OperationMessage<T>
            {
                Operation = eventName,
                Dto = offerDto
            };

            string json = JsonSerializer.Serialize(operationMessage);

            await _producerClient.ProduceAsync(
                _topics.OfferEvents,
                eventName,
                json);
        }
    }
}