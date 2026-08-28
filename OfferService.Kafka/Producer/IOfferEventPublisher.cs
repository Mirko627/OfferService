using OfferService.Shared.Kafka.Contracts;

namespace OfferService.Kafka.Producer
{
    public interface IOfferEventPublisher
    {
        Task OfferCreatedAsync(OfferCreatedDto offer);
        Task OfferAcceptedAsync(OfferAcceptedDto offer);
        Task OfferRejectedAsync(OfferRejectedDto offer);
        Task OfferCancelledAsync(OfferCancelledDto offer);
        Task OfferUpdatedAsync(OfferUpdatedDto offer);
    }
}
