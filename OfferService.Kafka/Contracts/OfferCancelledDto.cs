namespace OfferService.Kafka.Contracts
{
    public class OfferCancelledDto
    {
        public int OfferId { get; set; }
        public int PropertyId { get; set; }
        public int BuyerId { get; set; }
    }
}
