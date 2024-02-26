using Microsoft.AspNetCore.Mvc;
using Product.Application.Models.ViewModels;
using Shared.Events;
using Shared.Services.Abstractions;

namespace Product.Application.Controllers
{
    public class ProductsController(IEventStoreService eventStoreService) : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult CreateProduct()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> CreateProduct(CreateProductVM createProductVM)
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
    }
}
