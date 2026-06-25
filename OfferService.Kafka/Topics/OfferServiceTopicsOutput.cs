using Microsoft.Extensions.DependencyInjection;

namespace OfferService.Kafka.Topics;

public class OfferServiceTopicsOutput : AbstractKafkaTopics
{
    public string OfferEvents { get; set; } = "offer-events";

    public override IEnumerable<string> GetTopics()
    {
        return [OfferEvents];
    }
}