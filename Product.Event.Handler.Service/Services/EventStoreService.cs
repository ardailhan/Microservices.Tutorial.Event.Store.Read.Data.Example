using EventStore.Client;
using MongoDB.Driver;
using Shared.Events;
using Shared.Services.Abstractions;
using System.Reflection;
using System.Text.Json;

namespace Product.Event.Handler.Service.Services
{
    public class EventStoreService(IEventStoreService eventStoreService, IMongoDBService mongoDBService) : BackgroundService
    {
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            await eventStoreService.SubscribeToStreamAsync("products-stream", async (streamSubscription, resolvedEvent, cancellationToken) =>
            {
                string eventType = resolvedEvent.Event.EventType;
                object @event = JsonSerializer.Deserialize(resolvedEvent.Event.Data.ToArray(),
                    Assembly.Load("Shared").GetTypes().FirstOrDefault(t => t.Name == eventType));

                var productCollection = mongoDBService.GetCollection<Models.Product>("Products");

                switch (@event)
                {
                    case NewProductAddedEvent e:
                        var hasProduct = await (await productCollection.FindAsync(p => p.Id == e.ProductId)).AnyAsync();
                        if (!hasProduct)
                            await productCollection.InsertOneAsync(new()
                            {
                                Id = e.ProductId,
                                ProductName = e.ProductName,
                                Count = e.InitialCount,
                                IsAvailable = e.IsAvailable,
                                Price = e.InitialPrice
                            });

                        break;
                }
            });
        }
    }
}
