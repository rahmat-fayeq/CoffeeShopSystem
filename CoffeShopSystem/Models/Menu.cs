namespace CoffeShopSystem.Models
{
    public class Menu
    {
        public int Id { get; set; }
        public string Item { get; set; } = string.Empty;
        public decimal Price { get; set; } = 0;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();
    }
}
