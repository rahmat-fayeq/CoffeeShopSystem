namespace CoffeShopSystem.Models
{
    public class OrderItem
    {
        public int Id { get; set; }
        public int OrderId { get; set; }
        public int MenuId { get; set; }
        public int Quantity { get; set; } = 1;
        public decimal PriceAtOrder { get; set; } = 0;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public Order Order { get; set; }
        public Menu Menu { get; set; }
    }
}
