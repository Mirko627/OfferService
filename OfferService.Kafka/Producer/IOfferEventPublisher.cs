using OfferService.Repository.Entities;
using OfferService.Shared.Kafka.Contracts;

namespace OfferService.Kafka.Producer
{
    public interface IOfferEventPublisher
    {
        OutboxEvent CreateOfferCreatedEvent(OfferCreatedDto offer);
        OutboxEvent CreateOfferAcceptedEvent(OfferAcceptedDto offer);
        OutboxEvent CreateOfferRejectedEvent(OfferRejectedDto offer);
        OutboxEvent CreateOfferCancelledEvent(OfferCancelledDto offer);
        OutboxEvent CreateOfferUpdatedEvent(OfferUpdatedDto offer);
    }
}
