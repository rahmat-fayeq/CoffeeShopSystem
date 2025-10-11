namespace CoffeShopSystem.Models
{
    public class Order
    {
        public int Id { get; set; }
        public int TableId { get; set; }
        public decimal Discount { get; set; } = 0;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public string Status { get; set; } = "Pending";

        public Table Table { get; set; }
        public ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();
    }
}
