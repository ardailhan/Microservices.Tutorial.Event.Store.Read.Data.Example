namespace Product.Application.Models.ViewModels
{
    public class ListProductVM
    {
        public int ProductId { get; set; }
        public string ProductName { get; set; }
        public int Count { get; set; }
        public bool IsAvailable { get; set; }
        public decimal Price { get; set; }
    }
}
