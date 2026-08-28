//Contiene il nome degli eventi pubblicati da OfferService
namespace OfferService.Shared.Kafka.Events;

public static class OfferKafkaEvents
{
    public const string OfferCreated = "OfferCreated";
    public const string OfferAccepted = "OfferAccepted";
    public const string OfferRejected = "OfferRejected";
    public const string OfferUpdated = "OfferUpdated";
    public const string OfferCancelled = "OfferCancelled";
}