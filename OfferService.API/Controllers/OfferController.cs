using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OfferService.Business.Interfaces;
using OfferService.Shared.dtos;
using System.Security.Claims;

namespace OfferService.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class OfferController : ControllerBase
    {
        private readonly IOfferService _offerService;

        public OfferController(IOfferService offerService)
        {
            _offerService = offerService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll(CancellationToken ct)
        {
            var offers = await _offerService.GetAllAsync(ct);
            return Ok(offers);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id, CancellationToken ct)
        {
            var offer = await _offerService.GetByIdAsync(id, ct);
            return Ok(offer);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateOfferDto dto, CancellationToken ct)
        {
            var userId = GetUserIdFromClaims();
            await _offerService.AddAsync(dto, userId, ct);
            return Created();
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateOfferDto dto, CancellationToken ct)
        {
            var userId = GetUserIdFromClaims();
            await _offerService.UpdateAsync(id, dto, userId, ct);
            return Ok((new { message = "Offerta aggiornata con successo" }));
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id, CancellationToken ct)
        {
            var userId = GetUserIdFromClaims();
            await _offerService.DeleteAsync(id, userId, ct);
            return Ok((new { message = "Offerta eliminata con successo" }));
        }

        [HttpPatch("{id}/accept")]
        public async Task<IActionResult> Accept(int id, CancellationToken ct)
        {
            var userId = GetUserIdFromClaims();
            await _offerService.AcceptOfferAsync(id, userId, ct);
            return Ok(new { message = "Offerta accettata con successo" });
        }

        [HttpPatch("{id}/reject")]
        public async Task<IActionResult> Reject(int id, CancellationToken ct)
        {
            var userId = GetUserIdFromClaims();
            await _offerService.RejectOfferAsync(id, userId, ct);
            return Ok(new { message = "Offerta rifiutata con successo" });
        }

        private int GetUserIdFromClaims()
        {
            string? userIdClaim =
                User.FindFirst(ClaimTypes.NameIdentifier)?.Value ??
                User.FindFirst("id")?.Value;

            if (string.IsNullOrEmpty(userIdClaim))
                throw new UnauthorizedAccessException("ID utente non trovato nel token");

            return int.Parse(userIdClaim);
        }
    }
}
