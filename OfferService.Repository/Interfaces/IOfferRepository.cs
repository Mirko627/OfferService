using OfferService.Repository.Entities;

namespace OfferService.Repository.Interfaces
{
    public interface IOfferRepository
    {
        Task<List<Offer>> GetAllAsync();
        Task<Offer?> GetByIdAsync(int id);
        Task AddAsync(Offer offer);
        Task UpdateAsync(Offer offer);
        Task DeleteAsync(int id);
        Task<List<Offer>> GetOtherOffersByPropertyAsync(int propertyId, int id);
    }
}
