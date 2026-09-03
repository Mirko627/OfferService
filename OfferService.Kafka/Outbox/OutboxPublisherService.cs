using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using OfferService.Data.Context;
using OfferService.Repository.Entities;
using Utility.Kafka.Abstractions.Clients;

namespace OfferService.Kafka.Outbox
{
    public class OutboxPublisherService : BackgroundService
    {
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly ILogger<OutboxPublisherService> _logger;

        public OutboxPublisherService(IServiceScopeFactory scopeFactory, ILogger<OutboxPublisherService> logger)
        {
            _scopeFactory = scopeFactory;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("OutboxPublisherService avviato");

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    await PublishPendingEvents(stoppingToken);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Errore durante la pubblicazione degli eventi Outbox");
                }
                await Task.Delay(TimeSpan.FromSeconds(1), stoppingToken);
            }
        }

        private async Task PublishPendingEvents(CancellationToken ct){
            using IServiceScope scope = _scopeFactory.CreateScope();

            OfferDBContext db = scope.ServiceProvider.GetRequiredService<OfferDBContext>();

            IProducerClient<string, string> producer = scope.ServiceProvider.GetRequiredService<IProducerClient<string, string>>();

            List<OutboxEvent> events =
                await db.OutboxEvents
                    .Where(e => e.PublishedAt == null)
                    .OrderBy(e => e.CreatedAt)
                    .Take(50)
                    .ToListAsync(ct);

            foreach (OutboxEvent e in events)
            {
                try
                {
                    await producer.ProduceAsync(e.Topic, e.Key, e.Payload, ct);

                    e.PublishedAt = DateTime.UtcNow;

                    await db.SaveChangesAsync(ct);

                    _logger.LogInformation("Outbox event {EventId} pubblicato su {Topic}", e.Id, e.Topic);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Errore nella pubblicazione dell'evento Outbox {EventId}", e.Id);
                }
            }
        }
    }
}
