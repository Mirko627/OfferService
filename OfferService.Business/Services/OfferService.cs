using AutoMapper;
using OfferService.Business.Interfaces;
using OfferService.Repository.Entities;
using OfferService.Repository.Interfaces;
using OfferService.Shared.dtos;
using OfferService.Shared.enums;
using PropertyService.ClientHttp.Interfaces;
using PropertyService.Shared.dtos;
using System;

namespace OfferService.Business.Services
{
    public class OfferService : IOfferService
    {
        private readonly IOfferRepository repository;
        private readonly IPropertyClient propertyClient;
        private readonly IMapper mapper;

        public OfferService(IOfferRepository repository, IMapper mapper, IPropertyClient propertyClient)
        {
            this.repository = repository;
            this.mapper = mapper;
            this.propertyClient = propertyClient;
        }

        public async Task AddAsync(CreateOfferDto offerDto, int userId)
        {
            PropertyDto? p = await propertyClient.GetByIdAsync(offerDto.PropertyId);
            if (p == null) throw new Exception("Immobile non esistente");
            if (p.Status == PropertyService.Shared.enums.PropertyStatus.Sold) throw new Exception("L'immobile è già stato venduto");
            if (p.OwnerId == userId) throw new Exception("Non è possibile fare un offerta per una propria proprietà");
            Offer o = mapper.Map<Offer>(offerDto);
            o.OfferId = userId;
            o.CreatedAt = DateOnly.FromDateTime(DateTime.Now);
            o.ExpirateDate = (DateOnly.FromDateTime(DateTime.Now)).AddDays(30);
            o.Status = OfferStatus.Pending;
            await repository.AddAsync(o);
        }

        public async Task DeleteAsync(int id)
        {
            await repository.DeleteAsync(id);
        }

        public async Task<List<OfferDto>> GetAllAsync()
        {
            List<Offer> offers = await repository.GetAllAsync();
            foreach (Offer offer in offers)
            {
                await CheckExpired(offer);
            }
            List<OfferDto> offerDtos = mapper.Map<List<OfferDto>>(offers);
            return offerDtos;
        }

        public async Task<OfferDto?> GetByIdAsync(int id)
        {
            Offer? offer = await repository.GetByIdAsync(id);
            if (offer != null)
                await CheckExpired(offer);
            OfferDto? offerDto = mapper.Map<OfferDto>(offer);
            return offerDto;
        }

        public async Task UpdateAsync(int id, UpdateOfferDto offerDto)
        {
            Offer? offer = await repository.GetByIdAsync(id);
            if (offer == null)
                throw new Exception("Offerta non trovata");
            await CheckExpired(offer);
            mapper.Map(offerDto, offer);
            await repository.UpdateAsync(offer);
        }
        public async Task AcceptOfferAsync(int offerId, int userId)
        {
            Offer? o = await repository.GetByIdAsync(offerId);
            if (o == null) throw new Exception("Offerta non trovata");
            await CheckExpired(o);
            PropertyDto? property = await propertyClient.GetByIdAsync(o.PropertyId);
            if (property == null) throw new Exception("Immobile non esistente");
            if (property.OwnerId != userId) throw new UnauthorizedAccessException("L'attuale utente non e' il proprietario");
            if (property.Status == PropertyService.Shared.enums.PropertyStatus.Sold) throw new Exception("L'immobile non e' più disponibile");
            if (o.Status != OfferStatus.Pending) throw new Exception("L'offerta non e' più valida");
            o.Status = OfferStatus.Accepted;
            await repository.UpdateAsync(o);
            List<Offer> otherOffers = await repository.GetOtherOffersByPropertyAsync(o.PropertyId, o.Id);
            foreach (Offer other in otherOffers)
            {
                if(other.Status == OfferStatus.Pending)
                {
                    other.Status = OfferStatus.Expired;
                    await repository.UpdateAsync(other);
                }
            }
        }
        public async Task RejectOfferAsync(int offerId, int userId)
        {
            Offer? o = await repository.GetByIdAsync(offerId);
            if (o == null) throw new Exception("Offerta non trovata");
            await CheckExpired(o);
            PropertyDto? property = await propertyClient.GetByIdAsync(o.PropertyId);
            if (property == null) throw new Exception("Immobile non esistente");
            if (property.OwnerId != userId) throw new UnauthorizedAccessException("L'attuale utente non e' il proprietario");
            o.Status = OfferStatus.Rejected;
            await repository.UpdateAsync(o);
        }
        public async Task CancelOfferAsync(int offerId, int userId)
        {
            Offer? o = await repository.GetByIdAsync(offerId);
            if (o == null) throw new Exception("Offerta non trovata");
            await CheckExpired(o);
            if (o.OfferId != userId)
                throw new UnauthorizedAccessException("Non puoi ritirare un'offerta non tua.");
            if (o.Status != OfferStatus.Pending)
                throw new InvalidOperationException("Non puoi ritirare un'offerta già elaborata.");
            await repository.DeleteAsync(offerId);
        }
        private async Task CheckExpired(Offer o)
        {
            if (o.ExpirateDate < DateOnly.FromDateTime(DateTime.Now))
            {
                o.Status = OfferStatus.Expired;
                await repository.UpdateAsync(o);
            }
        }
    }
}
