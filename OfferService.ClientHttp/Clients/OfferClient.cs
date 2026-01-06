using OfferService.ClientHttp.Interfaces;
using OfferService.Shared.dtos;
using System.Net.Http.Json;

namespace OfferService.ClientHttp.Clients
{
    public class OfferClient : IOfferClient
    {
        private readonly HttpClient _httpClient;

        public OfferClient(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<List<OfferDto>> GetAllAsync()
        {
            return await _httpClient.GetFromJsonAsync<List<OfferDto>>("api/offer") ?? [];
        }

        public async Task<OfferDto?> GetByIdAsync(int id)
        {
            return await _httpClient.GetFromJsonAsync<OfferDto>($"api/offer/{id}");
        }

        public async Task AddAsync(CreateOfferDto dto)
        {
            var response = await _httpClient.PostAsJsonAsync("api/offer", dto);
            response.EnsureSuccessStatusCode();
        }

        public async Task UpdateAsync(int id, UpdateOfferDto dto)
        {
            var response = await _httpClient.PutAsJsonAsync($"api/offer/{id}", dto);
            response.EnsureSuccessStatusCode();
        }

        public async Task DeleteAsync(int id)
        {
            var response = await _httpClient.DeleteAsync($"api/offer/{id}");
            response.EnsureSuccessStatusCode();
        }

        public async Task AcceptAsync(int id)
        {
            var response = await _httpClient.PatchAsync($"api/offer/{id}/accept", null);
            response.EnsureSuccessStatusCode();
        }

        public async Task RejectAsync(int id)
        {
            var response = await _httpClient.PatchAsync($"api/offer/{id}/reject", null);
            response.EnsureSuccessStatusCode();
        }
    }
}
