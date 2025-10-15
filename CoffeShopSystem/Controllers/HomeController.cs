using CoffeShopSystem.Data;
using CoffeShopSystem.Models;
using CoffeShopSystem.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;

namespace CoffeShopSystem.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly AppDbContext _context;

        public HomeController(ILogger<HomeController> logger, AppDbContext context)
        {
            _logger = logger;
            _context = context;
        }

        public IActionResult Index()
        {
            // Normalize to UTC for PostgreSQL
            var now = DateTime.UtcNow;
            var startOfToday = DateTime.SpecifyKind(now.Date, DateTimeKind.Utc);
            var startOfTomorrow = startOfToday.AddDays(1);
            var startOfWeek = DateTime.SpecifyKind(now.AddDays(-7), DateTimeKind.Utc);
            var startOfMonth = DateTime.SpecifyKind(new DateTime(now.Year, now.Month, 1), DateTimeKind.Utc);
            var startOfSixMonths = DateTime.SpecifyKind(now.AddMonths(-6), DateTimeKind.Utc);
            var startOfYear = DateTime.SpecifyKind(new DateTime(now.Year, 1, 1), DateTimeKind.Utc);

            var orders = _context.Orders
                .Include(o => o.OrderItems)
                .ThenInclude(oi => oi.Menu)
                .ToList();

            decimal GetOrderTotal(Order o)
            {
                var total = o.OrderItems.Sum(i => i.PriceAtOrder * i.Quantity);
                return total - o.Discount;
            }

            // Revenue calculations
            decimal dailyRevenue = orders.Where(o => o.CreatedAt >= startOfToday && o.CreatedAt < startOfTomorrow).Sum(GetOrderTotal);
            decimal weeklyRevenue = orders.Where(o => o.CreatedAt >= startOfWeek).Sum(GetOrderTotal);
            decimal monthlyRevenue = orders.Where(o => o.CreatedAt >= startOfMonth).Sum(GetOrderTotal);
            decimal sixMonthRevenue = orders.Where(o => o.CreatedAt >= startOfSixMonths).Sum(GetOrderTotal);
            decimal yearlyRevenue = orders.Where(o => o.CreatedAt >= startOfYear).Sum(GetOrderTotal);
            decimal totalRevenue = orders.Sum(GetOrderTotal);

            // Orders (customers served)
            int dailyOrders = orders.Count(o => o.CreatedAt >= startOfToday && o.CreatedAt < startOfTomorrow);
            int weeklyOrders = orders.Count(o => o.CreatedAt >= startOfWeek);
            int monthlyOrders = orders.Count(o => o.CreatedAt >= startOfMonth);
            int sixMonthOrders = orders.Count(o => o.CreatedAt >= startOfSixMonths);
            int yearlyOrders = orders.Count(o => o.CreatedAt >= startOfYear);
            int totalOrders = orders.Count();

            // 🥇 Top 5 Menus 
            var topMenus = orders
            .SelectMany(o => o.OrderItems)
            .GroupBy(oi => new { oi.MenuId, oi.Menu.Item })
            .Select(g => new TopMenuVM
            {
                MenuId = g.Key.MenuId,
                MenuName = g.Key.Item,
                QuantitySold = g.Sum(x => x.Quantity),
                TotalRevenue = g.Sum(x => x.PriceAtOrder * x.Quantity)
            })
            .OrderByDescending(x => x.QuantitySold)
            .Take(5)
            .ToList();

            var vm = new HomeVM
            {
                DailyRevenue = dailyRevenue,
                WeeklyRevenue = weeklyRevenue,
                MonthlyRevenue = monthlyRevenue,
                SixMonthRevenue = sixMonthRevenue,
                YearlyRevenue = yearlyRevenue,
                TotalRevenue = totalRevenue,

                DailyOrders = dailyOrders,
                WeeklyOrders = weeklyOrders,
                MonthlyOrders = monthlyOrders,
                SixMonthOrders = sixMonthOrders,
                YearlyOrders = yearlyOrders,
                TotalOrders = totalOrders,

                TopMenus = topMenus
            };

            return View(vm);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
