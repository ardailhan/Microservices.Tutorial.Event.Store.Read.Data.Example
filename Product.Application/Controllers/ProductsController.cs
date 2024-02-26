using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis;
using MongoDB.Driver;
using Product.Application.Models.ViewModels;
using Shared.Events;
using Shared.Models;
using Shared.Services.Abstractions;

namespace Product.Application.Controllers
{
    public class ProductsController(IEventStoreService eventStoreService, IMongoDBService mongoDBService) : Controller
    {
        public async Task<IActionResult> Index()
        {
            var productCollection = mongoDBService.GetCollection<Shared.Models.Product>("Products");
            var products = await (await productCollection.FindAsync(t => true)).ToListAsync();
            return View(products);
        }

        public IActionResult Create()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Create(CreateProductVM createProductVM)
        {
            NewProductAddedEvent newProductAddedEvent = new()
            {
                ProductId = Guid.NewGuid().ToString(),
                InitialCount = createProductVM.Count,
                InitialPrice = createProductVM.Price,
                IsAvailable = createProductVM.IsAvailable,
                ProductName = createProductVM.ProductName
            };

            await eventStoreService.AppendToStreamAsync("products-stream", new[] { eventStoreService.GenerateEventData(newProductAddedEvent) });
            return RedirectToAction("Index");
        }
        public async Task<IActionResult> Edit(string productId)
        {
            var productCollection = mongoDBService.GetCollection<Shared.Models.Product>("Products");
            var product = await (await productCollection.FindAsync(p => p.Id == productId)).FirstOrDefaultAsync();
            return View(product);
        }
        [HttpPost]
        public async Task<IActionResult> CountUpdate(Shared.Models.Product model)
        {
            var productCollection = mongoDBService.GetCollection<Shared.Models.Product>("Products");
            var product = await (await productCollection.FindAsync(p => p.Id == model.Id)).FirstOrDefaultAsync();
            if (product.Count > model.Count)
            {
                CountDecreasedEvent countDecreasedEvent = new()
                {
                    ProductId = model.Id,
                    DecrementAmount = model.Count
                };
                await eventStoreService.AppendToStreamAsync("products-stream", new[] { eventStoreService.GenerateEventData(countDecreasedEvent) });
            }
            else if (product.Count < model.Count)
            {
                CountIncreasedEvent countIncreasedEvent = new()
                {
                    ProductId = model.Id,
                    IncrementAmount = model.Count
                };
                await eventStoreService.AppendToStreamAsync("products-stream", new[] { eventStoreService.GenerateEventData(countIncreasedEvent) });
            }

            return RedirectToAction("Index");
        }
        [HttpPost]
        public async Task<IActionResult> PriceUpdate(Shared.Models.Product model)
        {
            var productCollection = mongoDBService.GetCollection<Shared.Models.Product>("Products");
            var product = await (await productCollection.FindAsync(p => p.Id == model.Id)).FirstOrDefaultAsync();
            if (product.Price > model.Price)
            {
                PriceDecreasedEvent priceDecreasedEvent = new()
                {
                    ProductId = model.Id,
                    DecrementAmount = model.Price
                };
                await eventStoreService.AppendToStreamAsync("products-stream", new[] { eventStoreService.GenerateEventData(priceDecreasedEvent) });
            }
            else if (product.Price < model.Price)
            {
                PriceIncreasedEvent priceIncreasedEvent = new()
                {
                    ProductId = model.Id,
                    IncrementAmount = model.Price
                };
                await eventStoreService.AppendToStreamAsync("products-stream", new[] { eventStoreService.GenerateEventData(priceIncreasedEvent) });
            }
            return RedirectToAction("Index");
        }
        [HttpPost]
        public async Task<IActionResult> AvailableUpdate(Shared.Models.Product model)
        {
            var productCollection = mongoDBService.GetCollection<Shared.Models.Product>("Products");
            var product = await (await productCollection.FindAsync(p => p.Id == model.Id)).FirstOrDefaultAsync();
            if (product.IsAvailable != model.IsAvailable)
            {
                AvailabilityChangedEvent availabilityChangedEvent = new()
                {
                    ProductId = model.Id,
                    IsAvailable = model.IsAvailable
                };
                await eventStoreService.AppendToStreamAsync("products-stream", new[] { eventStoreService.GenerateEventData(availabilityChangedEvent) });
            }

            return RedirectToAction("Index");
        }
    }
}
