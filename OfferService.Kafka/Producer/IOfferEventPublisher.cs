using OfferService.Kafka.Contracts;
using OfferService.Repository.Entities;

namespace OfferService.Business.Interfaces
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
