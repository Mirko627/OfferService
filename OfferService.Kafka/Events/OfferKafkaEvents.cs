//Contiene il nome degli eventi pubblicati da OfferService
namespace OfferService.Kafka.Events;

public static class OfferKafkaEvents
{
    public const string OfferCreated = "OfferCreated";
    public const string OfferAccepted = "OfferAccepted";
    public const string OfferRejected = "OfferRejected";
    public const string OfferUpdated = "OfferRejected";
    public const string OfferCancelled = "OfferRejected";
}