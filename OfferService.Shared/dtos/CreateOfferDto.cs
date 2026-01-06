using OfferService.Shared.enums;

namespace OfferService.Shared.dtos
{
    public class CreateOfferDto
    {
        public int PropertyId { get; set; }
        public int Amount { get; set; }
    }
}
