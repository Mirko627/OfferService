using OfferService.Shared.dtos;

namespace OfferService.Business.Interfaces
{
    public interface IOfferService
    {
        Task<List<OfferDto>> GetAllAsync();
        Task<OfferDto> GetByIdAsync(int id);
        Task AddAsync(CreateOfferDto offerDto, int userId);
        Task UpdateAsync(int id, UpdateOfferDto offerDto, int userId);
        Task DeleteAsync(int id, int userId);
        Task AcceptOfferAsync(int offerId, int userId);
        Task RejectOfferAsync(int offerId, int userId);
    }
}
