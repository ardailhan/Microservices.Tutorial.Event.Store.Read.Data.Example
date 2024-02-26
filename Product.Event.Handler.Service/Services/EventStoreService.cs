
using EventStore.Client;
using Shared.Events;
using Shared.Services.Abstractions;
using System.Reflection;
using System.Text.Json;

namespace Product.Event.Handler.Service.Services
{
    public class EventStoreService(IEventStoreService eventStoreService) : BackgroundService
    {
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            await eventStoreService.SubscribeToStreamAsync("products-stream", async (streamSubscription, resolvedEvent, cancellationToken) =>
            {
                string eventType = resolvedEvent.Event.EventType;
                object @event = JsonSerializer.Deserialize(resolvedEvent.Event.Data.ToArray(),
                    Assembly.Load("Shared").GetTypes().FirstOrDefault(t => t.Name == eventType));

                switch (@event)
                {
                    case NewProductAddedEvent e:

                        break;
                }
            });
        }
    }
}
