using OfferService.Shared.enums;

namespace OfferService.Shared.dtos
{
    public class OfferDto
    {

        public int Id { get; set; }
        public int PropertyId { get; set; }
        public int OfferId { get; set; }
        public int Amount { get; set; }
        public DateOnly CreatedAt { get; set; }
        public DateOnly ExpirateDate { get; set; }
        public OfferStatus Status { get; set; }
    }
}
