using OfferService.Shared.dtos;

namespace OfferService.Business.Interfaces
{
    public interface IOfferService
    {
        public Task<List<OfferDto>> GetAllAsync();
        public Task<OfferDto?> GetByIdAsync(int id);
        public Task AddAsync(CreateOfferDto offerDto, int userId);
        public Task UpdateAsync(int id, UpdateOfferDto offerDto);
        public Task DeleteAsync(int id);
        public Task AcceptOfferAsync(int offerId, int userId);
        public Task RejectOfferAsync(int offerId, int userId);
        public Task CancelOfferAsync(int offerId, int userId);
    }
}
