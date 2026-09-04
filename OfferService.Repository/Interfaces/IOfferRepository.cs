using OfferService.Repository.Entities;

namespace OfferService.Repository.Interfaces
{
    public interface IOfferRepository
    {
        Task<List<Offer>> GetAllAsync(CancellationToken ct = default);
        Task<Offer?> GetByIdAsync(int id, CancellationToken ct = default);
        Task AddAsync(Offer offer, OutboxEvent? outboxEvent = null, CancellationToken ct = default);
        Task UpdateAsync(Offer offer, OutboxEvent? outboxEvent = null, CancellationToken ct = default);
        Task DeleteAsync(int id, OutboxEvent? outboxEvent = null, CancellationToken ct = default);
        Task<List<Offer>> GetOtherOffersByPropertyAsync(int propertyId, int id, CancellationToken ct = default);
        Task UpdateManyAsync(IEnumerable<Offer> offers, IEnumerable<OutboxEvent>? outboxEvents = null, CancellationToken ct = default);
    }
}
