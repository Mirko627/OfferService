namespace OfferService.Repository.Entities
{
    public class OutboxEvent
    {
        public Guid Id { get; set; }
        public string EventType { get; set; } = null!;
        public string Topic { get; set; } = null!;
        public string Key { get; set; } = null!;
        public string Payload { get; set; } = null!;
        public DateTime CreatedAt { get; set; }
        public DateTime? PublishedAt { get; set; }
    }
}
