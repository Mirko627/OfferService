namespace OfferService.Kafka.Contracts
{
    public class OfferCreatedDto
    {
        public int PropertyId { get; set; }
        public int BuyerId { get; set; }
        public int Amount { get; set; }
        public DateOnly CreatedAt { get; set; }
    }
}
