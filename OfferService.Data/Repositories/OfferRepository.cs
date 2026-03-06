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

        public async Task<List<Offer>> GetAllAsync()
        {
            List<Offer> offers = await _context.Offers.ToListAsync();
            return offers;
        }

        public async Task<Offer?> GetByIdAsync(int id)
        {
            Offer? offer = await _context.Offers.FindAsync(id);
            return offer;
        }
        public async Task AddAsync(Offer offer)
        {
            await _context.Offers.AddAsync(offer);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            Offer? o = await GetByIdAsync(id);
            if (o == null)
                throw new Exception("Utente non trovato");
            _context.Offers.Remove(o);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Offer offer)
        {
            _context.Offers.Update(offer);
            await _context.SaveChangesAsync();
        }

        public async Task<List<Offer>> GetOtherOffersByPropertyAsync(int propertyId, int id)
        {
            return await _context.Offers.Where(o => (o.PropertyId == propertyId && o.Id != id)).ToListAsync();
        }
    }
}