using Microsoft.EntityFrameworkCore;
using OfferService.Repository.Entities;
using OfferService.Repository.Interfaces;
using System;
using OfferService.Data.Context;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace OfferService.Data.Repositories
{
    public class OfferRepository : IOfferRepository
    {
        private readonly OfferDBContext _context;

        public OfferRepository(OfferDBContext context)
        {
            _context = context;
        }

        public async Task<List<Offer>> GetAllAsync(CancellationToken ct = default)
        {
            List<Offer> offers = await _context.Offers.ToListAsync(ct);
            return offers;
        }

        public async Task<Offer?> GetByIdAsync(int id, CancellationToken ct = default)
        {
            Offer? offer = await _context.Offers.FindAsync(id, ct);
            return offer;
        }
        public async Task AddAsync(Offer offer, OutboxEvent? outboxEvent = null, CancellationToken ct = default)
        {
            await _context.Offers.AddAsync(offer, ct);
            
            if(outboxEvent != null)
                await _context.OutboxEvents.AddAsync(outboxEvent, ct);

            await _context.SaveChangesAsync(ct);
        }

        public async Task DeleteAsync(int id, OutboxEvent? outboxEvent = null, CancellationToken ct = default)
        {
            Offer o = await GetByIdAsync(id, ct) ?? throw new Exception("Offerta non trovato");
            _context.Offers.Remove(o);

            if (outboxEvent != null)
                await _context.OutboxEvents.AddAsync(outboxEvent, ct);

            await _context.SaveChangesAsync(ct);
        }

        public async Task UpdateAsync(Offer offer, OutboxEvent? outboxEvent = null, CancellationToken ct = default)
        {
            _context.Offers.Update(offer);
            
            if (outboxEvent != null)
                await _context.OutboxEvents.AddAsync(outboxEvent, ct);

            await _context.SaveChangesAsync(ct);
        }

        public async Task<List<Offer>> GetOtherOffersByPropertyAsync(int propertyId, int id, CancellationToken ct = default)
        {
            return await _context.Offers.Where(o => (o.PropertyId == propertyId && o.Id != id)).ToListAsync(ct);
        }

        public async Task UpdateManyAsync(IEnumerable<Offer> offers, IEnumerable<OutboxEvent>? outboxEvents = null, CancellationToken ct = default)
        {
            _context.Offers.UpdateRange(offers);

            if (outboxEvents != null)
                await _context.OutboxEvents.AddRangeAsync(outboxEvents, ct);

            await _context.SaveChangesAsync(ct);
        }
    }
}