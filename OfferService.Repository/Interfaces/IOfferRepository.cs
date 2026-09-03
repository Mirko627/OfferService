using OfferService.Repository.Entities;

namespace OfferService.Repository.Interfaces
{
    public interface IOfferRepository
    {
        Task<List<Offer>> GetAllAsync();
        Task<Offer?> GetByIdAsync(int id);
        Task AddAsync(Offer offer, OutboxEvent? outboxEvent = null);
        Task UpdateAsync(Offer offer, OutboxEvent? outboxEvent = null);
        Task DeleteAsync(int id, OutboxEvent? outboxEvent = null);
        Task<List<Offer>> GetOtherOffersByPropertyAsync(int propertyId, int id);
        Task UpdateManyAsync(IEnumerable<Offer> offers, IEnumerable<OutboxEvent>? outboxEvents = null);
    }
}
