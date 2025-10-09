namespace CoffeShopSystem.ViewModels
{
    public class HomeVM
    {
        // Revenue
        public decimal DailyRevenue { get; set; }
        public decimal WeeklyRevenue { get; set; }
        public decimal MonthlyRevenue { get; set; }
        public decimal SixMonthRevenue { get; set; }
        public decimal YearlyRevenue { get; set; }
        public decimal TotalRevenue { get; set; }

        // Orders
        public int DailyOrders { get; set; }
        public int WeeklyOrders { get; set; }
        public int MonthlyOrders { get; set; }
        public int SixMonthOrders { get; set; }
        public int YearlyOrders { get; set; }
        public int TotalOrders { get; set; }

        // Top menus
        public List<TopMenuVM> TopMenus { get; set; } = new();
    }

    public class TopMenuVM
    {
        public int MenuId { get; set; }
        public string MenuName { get; set; } = string.Empty;
        public int QuantitySold { get; set; }
        public decimal TotalRevenue { get; set; }
    }
}
