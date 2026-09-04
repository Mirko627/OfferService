using AutoMapper;
using OfferService.Business.Interfaces;
using OfferService.Kafka.Producer;
using OfferService.Repository.Entities;
using OfferService.Repository.Interfaces;
using OfferService.Shared.dtos;
using OfferService.Shared.enums;
using OfferService.Shared.Kafka.Contracts;
using PropertyService.ClientHttp.Interfaces;
using PropertyService.Shared.dtos;

namespace OfferService.Business.Services
{
    public class OfferService : IOfferService
    {
        private readonly IOfferRepository repository;
        private readonly IPropertyClient propertyClient;
        private readonly IOfferEventPublisher eventPublisher;
        private readonly IMapper mapper;
        private readonly IMapper mapperEvent;

        public OfferService(IOfferRepository repository, IMapper mapper, IPropertyClient propertyClient, IOfferEventPublisher eventPublisher, IMapper mapperEvent)
        {
            this.repository = repository;
            this.mapper = mapper;
            this.propertyClient = propertyClient;
            this.eventPublisher = eventPublisher;
            this.mapperEvent = mapperEvent;
        }

        public async Task AddAsync(CreateOfferDto offerDto, int userId, CancellationToken ct = default)
        {
            PropertyDto p = await propertyClient.GetByIdAsync(offerDto.PropertyId, ct) ?? throw new KeyNotFoundException($"Proprietà con ID {offerDto.PropertyId} non trovata.");
            if (p.Status != PropertyService.Shared.enums.PropertyStatus.Available) throw new InvalidOperationException("La proprietà non è più disponibile");
            if (p.OwnerId == userId) throw new InvalidOperationException("Non è possibile fare un offerta per una propria proprietà");
            Offer o = mapper.Map<Offer>(offerDto);
            o.OfferId = userId;
            o.CreatedAt = DateOnly.FromDateTime(DateTime.Now);
            o.ExpirateDate = (DateOnly.FromDateTime(DateTime.Now)).AddDays(30);
            o.Status = OfferStatus.Pending;

            OfferCreatedDto offerCreatedDto = mapperEvent.Map<OfferCreatedDto>(o);

            OutboxEvent outboxEvent = eventPublisher.CreateOfferCreatedEvent(offerCreatedDto);

            await repository.AddAsync(o, outboxEvent, ct);
        }

        public async Task DeleteAsync(int id, int userId, CancellationToken ct = default)
        {
            Offer o = await repository.GetByIdAsync(id, ct) ?? throw new KeyNotFoundException($"Offerta con ID {id} non trovata.");
            if(o.OfferId != userId) throw new UnauthorizedAccessException("Non hai i permessi per eliminare questa offerta.");

            OfferCancelledDto offerCancelledDto = mapperEvent.Map<OfferCancelledDto>(o);
            
            OutboxEvent outboxEvent = eventPublisher.CreateOfferCancelledEvent(offerCancelledDto);

            await repository.DeleteAsync(id, outboxEvent, ct);
        }

        public async Task<List<OfferDto>> GetAllAsync(CancellationToken ct = default)
        {
            List<Offer> offers = await repository.GetAllAsync(ct);
            foreach (Offer offer in offers)
            {
                await CheckExpired(offer, ct);
            }
            List<OfferDto> offerDtos = mapper.Map<List<OfferDto>>(offers);
            return offerDtos;
        }

        public async Task<OfferDto> GetByIdAsync(int id, CancellationToken ct = default)
        {
            Offer o = await repository.GetByIdAsync(id, ct) ?? throw new KeyNotFoundException($"Offerta con ID {id} non trovata.");
            OfferDto? offerDto = mapper.Map<OfferDto>(o);
            return offerDto;
        }

        public async Task UpdateAsync(int id, UpdateOfferDto offerDto, int userId, CancellationToken ct = default)
        {
            Offer o = await repository.GetByIdAsync(id, ct) ?? throw new KeyNotFoundException($"Offerta con ID {id} non trovata.");
            if (o.OfferId != userId) throw new UnauthorizedAccessException("Non hai i permessi per modificare questa offerta.");
            await CheckExpired(o, ct);
            mapper.Map(offerDto, o);

            OfferUpdatedDto offerUpdatedDto = mapperEvent.Map<OfferUpdatedDto>(o);
            
            OutboxEvent outboxEvent = eventPublisher.CreateOfferUpdatedEvent(offerUpdatedDto);

            await repository.UpdateAsync(o, outboxEvent, ct);
        }
        public async Task AcceptOfferAsync(int offerId, int userId, CancellationToken ct = default)
        {
            Offer o = await repository.GetByIdAsync(offerId, ct) ?? throw new KeyNotFoundException($"Offerta con ID {offerId} non trovata.");
            await CheckExpired(o, ct);
            PropertyDto property = await propertyClient.GetByIdAsync(o.PropertyId, ct) ?? throw new KeyNotFoundException($"Proprietà con ID {o.PropertyId} non trovata.");
            if (property.OwnerId != userId) throw new UnauthorizedAccessException("Non hai i permessi per modificare questa offerta.");
            if (property.Status == PropertyService.Shared.enums.PropertyStatus.Sold) throw new InvalidOperationException("La proprietà non è più disponibile");
            if (o.Status != OfferStatus.Pending) throw new InvalidOperationException("L'offerta non è più disponibile");
            o.Status = OfferStatus.Accepted;

            OfferAcceptedDto offerAcceptedDto = mapperEvent.Map<OfferAcceptedDto>(o);

            OutboxEvent outboxEvent = eventPublisher.CreateOfferAcceptedEvent(offerAcceptedDto);


            List<Offer> otherOffers = await repository.GetOtherOffersByPropertyAsync(o.PropertyId, o.Id, ct);

            List<Offer> offersToUpdate = new List<Offer> { o };
            List<OutboxEvent> outboxEvents = new List<OutboxEvent> { outboxEvent };

            foreach (Offer other in otherOffers)
            {
                if (other.Status != OfferStatus.Pending)
                    continue;
                other.Status = OfferStatus.Rejected;
                OfferRejectedDto rejectedDto = mapperEvent.Map<OfferRejectedDto>(other);
                OutboxEvent rejectedOutboxEvent = eventPublisher.CreateOfferRejectedEvent(rejectedDto);
                offersToUpdate.Add(other);
                outboxEvents.Add(rejectedOutboxEvent);
            }
            await repository.UpdateManyAsync(offersToUpdate, outboxEvents, ct);
        }
        public async Task RejectOfferAsync(int offerId, int userId, CancellationToken ct = default)
        {
            Offer o = await repository.GetByIdAsync(offerId, ct) ?? throw new KeyNotFoundException($"Offerta con ID {offerId} non trovata.");
            await CheckExpired(o, ct);
            PropertyDto property = await propertyClient.GetByIdAsync(o.PropertyId, ct) ?? throw new KeyNotFoundException($"Proprietà con ID {o.PropertyId} non trovata.");
            if (property.OwnerId != userId) throw new UnauthorizedAccessException("Non hai i permessi per modificare questa offerta.");
            o.Status = OfferStatus.Rejected;

            OfferRejectedDto offerRejectedDto = mapperEvent.Map<OfferRejectedDto>(o);

            OutboxEvent outboxEvent = eventPublisher.CreateOfferRejectedEvent(offerRejectedDto);

            await repository.UpdateAsync(o, outboxEvent, ct);
        }
        private async Task CheckExpired(Offer o, CancellationToken ct = default)
        {
            if (o.ExpirateDate < DateOnly.FromDateTime(DateTime.Now))
            {
                o.Status = OfferStatus.Expired;
                await repository.UpdateAsync(o, null,  ct);
            }
        }
    }
}

