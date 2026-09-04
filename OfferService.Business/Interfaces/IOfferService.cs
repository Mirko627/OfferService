using OfferService.Shared.dtos;

namespace OfferService.Business.Interfaces
{
    public interface IOfferService
    {
        Task<List<OfferDto>> GetAllAsync(CancellationToken ct = default);
        Task<OfferDto> GetByIdAsync(int id, CancellationToken ct = default);
        Task AddAsync(CreateOfferDto offerDto, int userId, CancellationToken ct = default);
        Task UpdateAsync(int id, UpdateOfferDto offerDto, int userId, CancellationToken ct = default);
        Task DeleteAsync(int id, int userId, CancellationToken ct = default);
        Task AcceptOfferAsync(int offerId, int userId, CancellationToken ct = default);
        Task RejectOfferAsync(int offerId, int userId, CancellationToken ct = default);
    }
}
