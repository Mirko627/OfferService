namespace OfferService.Kafka.Contracts
{
    public class OfferAcceptedDto
    {
        public int OfferId { get; set; }
        public int PropertyId { get; set; }
        public int BuyerId { get; set; }
        public int Amount { get; set; }
    }
}
