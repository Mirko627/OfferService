using Microsoft.AspNetCore.Http;
using OfferService.ClientHttp.Interfaces;
using OfferService.Shared.dtos;
using System.Net.Http.Headers;
using System.Net.Http.Json;

namespace OfferService.ClientHttp.Clients
{
    public class OfferClient : IOfferClient
    {
        private readonly HttpClient _httpClient;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public OfferClient(HttpClient httpClient, IHttpContextAccessor httpContextAccessor)
        {
            _httpClient = httpClient;
            _httpContextAccessor = httpContextAccessor;
        }
        public async Task<List<OfferDto>> GetAllAsync()
        {
            var request = new HttpRequestMessage(HttpMethod.Get, "api/offer");
            AddAuthorizationHeader(request);

            var response = await _httpClient.SendAsync(request);
            response.EnsureSuccessStatusCode();

            return await response.Content.ReadFromJsonAsync<List<OfferDto>>() ?? [];
        }

        public async Task<OfferDto?> GetByIdAsync(int id)
        {
            var request = new HttpRequestMessage(HttpMethod.Get, $"api/offer/{id}");
            AddAuthorizationHeader(request);

            var response = await _httpClient.SendAsync(request);
            if (response.StatusCode == System.Net.HttpStatusCode.NotFound) return null;

            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<OfferDto>();
        }

        public async Task AddAsync(CreateOfferDto dto)
        {
            var request = new HttpRequestMessage(HttpMethod.Post, "api/offer");
            request.Content = JsonContent.Create(dto);
            AddAuthorizationHeader(request);

            var response = await _httpClient.SendAsync(request);
            response.EnsureSuccessStatusCode();
        }

        public async Task UpdateAsync(int id, UpdateOfferDto dto)
        {
            var request = new HttpRequestMessage(HttpMethod.Put, $"api/offer/{id}");
            request.Content = JsonContent.Create(dto);
            AddAuthorizationHeader(request);

            var response = await _httpClient.SendAsync(request);
            response.EnsureSuccessStatusCode();
        }

        public async Task DeleteAsync(int id)
        {
            var request = new HttpRequestMessage(HttpMethod.Delete, $"api/offer/{id}");
            AddAuthorizationHeader(request);

            var response = await _httpClient.SendAsync(request);
            response.EnsureSuccessStatusCode();
        }

        public async Task AcceptAsync(int id)
        {
            var request = new HttpRequestMessage(HttpMethod.Patch, $"api/offer/{id}/accept");
            AddAuthorizationHeader(request);

            var response = await _httpClient.SendAsync(request);
            response.EnsureSuccessStatusCode();
        }

        public async Task RejectAsync(int id)
        {
            var request = new HttpRequestMessage(HttpMethod.Patch, $"api/offer/{id}/reject");
            AddAuthorizationHeader(request);

            var response = await _httpClient.SendAsync(request);
            response.EnsureSuccessStatusCode();
        }
        private void AddAuthorizationHeader(HttpRequestMessage request)
        {
            string? authHeader = _httpContextAccessor.HttpContext?.Request.Headers["Authorization"].ToString();

            if (!string.IsNullOrEmpty(authHeader) && authHeader.StartsWith("Bearer "))
            {
                string token = authHeader.Substring("Bearer ".Length).Trim();

                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
            }
        }
    }
}
