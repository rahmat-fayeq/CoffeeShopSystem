namespace CoffeShopSystem.ViewModels
{
    public class OrderVM
    {
        public class OrderItemVM
        {
            public int MenuId { get; set; }
            public string ItemName { get; set; } = string.Empty;
            public decimal Price { get; set; }
            public int Quantity { get; set; } = 1;
            public string Status { get; set; } = string.Empty;
        }

        public class CreateOrderVM
        {
            public int TableId { get; set; }
            public decimal Discount { get; set; } = 0;
            public List<OrderItemVM> Items { get; set; } = new();
        }

        public class EditOrderVM
        {
            public int Id { get; set; }
            public int TableId { get; set; }
            public decimal Discount { get; set; } = 0;
            public List<OrderItemVM> Items { get; set; } = new();
        }

        public class OrderDetailsVM
        {
            public int Id { get; set; }
            public string TableName { get; set; } = string.Empty;
            public decimal Discount { get; set; }
            public DateTime CreatedAt { get; set; }
            public string Status { get; set; } = string.Empty;
            public List<OrderItemVM> Items { get; set; } = new();
            public decimal Total => Items.Sum(i => i.Price * i.Quantity) - Discount;
        }

    }
}
