using OfferService.Shared.dtos;

namespace OfferService.ClientHttp.Interfaces
{
    public interface IOfferClient
    {
        Task<List<OfferDto>> GetAllAsync(CancellationToken ct = default);
        Task<OfferDto?> GetByIdAsync(int id, CancellationToken ct = default);
        Task AddAsync(CreateOfferDto dto, CancellationToken ct = default);
        Task UpdateAsync(int id, UpdateOfferDto dto, CancellationToken ct = default);
        Task DeleteAsync(int id, CancellationToken ct = default);
        Task AcceptAsync(int id, CancellationToken ct = default);
        Task RejectAsync(int id, CancellationToken ct = default);
    }
}
