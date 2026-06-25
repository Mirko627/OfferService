using AutoMapper;
using OfferService.Repository.Entities;
using OfferService.Kafka.Contracts;

namespace OfferService.Business.Mappers
{
    public class OfferEventMapper : Profile
    {
        public OfferEventMapper()
        {

            CreateMap<Offer, OfferCreatedDto>()
                .ForMember(dest => dest.BuyerId,
                    opt => opt.MapFrom(src => src.OfferId));

            CreateMap<Offer, OfferAcceptedDto>()
                .ForMember(dest => dest.BuyerId,
                    opt => opt.MapFrom(src => src.OfferId));

            CreateMap<Offer, OfferRejectedDto>()
                .ForMember(dest => dest.BuyerId,
                    opt => opt.MapFrom(src => src.OfferId));

            CreateMap<Offer, OfferUpdatedDto>()
                .ForMember(dest => dest.BuyerId,
                    opt => opt.MapFrom(src => src.OfferId));

            CreateMap<Offer, OfferCancelledDto>()
                .ForMember(dest => dest.BuyerId,
                    opt => opt.MapFrom(src => src.OfferId));
        }
    }
}