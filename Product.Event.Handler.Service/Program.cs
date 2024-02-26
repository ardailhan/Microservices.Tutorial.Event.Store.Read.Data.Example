using Product.Event.Handler.Service.Services;
using Shared.Services;
using Shared.Services.Abstractions;

var builder = Host.CreateApplicationBuilder(args);
//builder.Services.AddHostedService<Worker>();

builder.Services.AddSingleton<IEventStoreService, IEventStoreService>();
builder.Services.AddSingleton<IMongoDBService, MongoDBService>();

var host = builder.Build();
host.Run();
