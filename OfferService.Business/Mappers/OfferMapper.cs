using AutoMapper;
using OfferService.Repository.Entities;
using OfferService.Shared.dtos;

namespace OfferService.Business.Mappers
{
    public class OfferMapper : Profile
    {
        public OfferMapper() {
            CreateMap<Offer, OfferDto>();

            CreateMap<UpdateOfferDto, Offer>()
                .ForMember(dest => dest.Id, opt => opt.Ignore());

            CreateMap<CreateOfferDto, Offer>();
        }
    }
}
