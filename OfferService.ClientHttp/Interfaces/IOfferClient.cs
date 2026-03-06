using OfferService.Shared.dtos;

namespace OfferService.ClientHttp.Interfaces
{
    public interface IOfferClient
    {
        Task<List<OfferDto>> GetAllAsync();
        Task<OfferDto?> GetByIdAsync(int id);
        Task AddAsync(CreateOfferDto dto);
        Task UpdateAsync(int id, UpdateOfferDto dto);
        Task DeleteAsync(int id);
        Task AcceptAsync(int id);
        Task RejectAsync(int id);
    }
}
