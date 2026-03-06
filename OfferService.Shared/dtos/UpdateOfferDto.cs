using OfferService.Shared.enums;

namespace OfferService.Shared.dtos
{
    public class UpdateOfferDto
    {
        public int PropertyId { get; set; }
        public int Amount { get; set; }
    }
}
