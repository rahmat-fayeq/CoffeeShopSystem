using AutoMapper;
using AutoMapper.QueryableExtensions;
using CoffeShopSystem.Data;
using CoffeShopSystem.Models;
using CoffeShopSystem.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using static CoffeShopSystem.ViewModels.OrderVM;

public class OrdersController : Controller
{
    private readonly AppDbContext _context;
    private readonly IMapper _mapper;

    public OrdersController(AppDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    // GET: Orders
    public async Task<IActionResult> Index()
    {
        var orders = await _context.Orders
            .Include(o => o.Table)
            .Include(o => o.OrderItems)
                .ThenInclude(oi => oi.Menu)
            .OrderByDescending(o => o.CreatedAt)
            .ToListAsync();

        var vm = orders.Select(o => _mapper.Map<OrderDetailsVM>(o)).ToList();
        return View(vm);
    }

    // GET: Orders/Details/5
    public async Task<IActionResult> Details(int? id)
    {
        if (id == null) return NotFound();

        var order = await _context.Orders
            .Include(o => o.Table)
            .Include(o => o.OrderItems)
                .ThenInclude(oi => oi.Menu)
            .FirstOrDefaultAsync(o => o.Id == id);

        if (order == null) return NotFound();

        var vm = _mapper.Map<OrderDetailsVM>(order);
        return View(vm);
    }

    // GET: Orders/Create
    public async Task<IActionResult> Create()
    {
        var tables = await _context.Tables.ToListAsync();
        var menus = await _context.Menus.ToListAsync();

        var vm = new CreateOrderVM();
        vm.Items = menus.Select(m => new OrderItemVM
        {
            MenuId = m.Id,
            ItemName = m.Item,
            Price = m.Price,
            Quantity = 0
        }).ToList();

        ViewBag.Tables = tables;
        ViewBag.Menus = menus;
        return View(vm);
    }


    // POST: Orders/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(CreateOrderVM vm)
    {
        if (!ModelState.IsValid)
        {
            ViewBag.Tables = await _context.Tables.ToListAsync();
            ViewBag.Menus = await _context.Menus.ToListAsync();
            return View(vm);
        }

        var order = new Order
        {
            TableId = vm.TableId,
            Discount = vm.Discount,
            CreatedAt = DateTime.UtcNow,
            OrderItems = vm.Items
                .Where(i => i.Quantity > 0)
                .Select(i => new OrderItem
                {
                    MenuId = i.MenuId,
                    Quantity = i.Quantity,
                    PriceAtOrder = i.Price,
                    CreatedAt = DateTime.UtcNow
                })
                .ToList()
        };

        _context.Orders.Add(order);
        await _context.SaveChangesAsync();

        return RedirectToAction(nameof(Index));
    }

    // GET: Orders/Edit/5
    public IActionResult Edit(int id)
    {
        var order = _context.Orders
            .Include(o => o.OrderItems)
            .ThenInclude(oi => oi.Menu)
            .FirstOrDefault(o => o.Id == id);

        if (order == null) return NotFound();

        // Map to EditOrderVM
        var vm = new EditOrderVM
        {
            Id = order.Id,
            TableId = order.TableId,
            Discount = order.Discount,
            Items = order.OrderItems.Select(oi => new OrderItemVM
            {
                MenuId = oi.MenuId,
                ItemName = oi.Menu.Item,
                Price = oi.PriceAtOrder,
                Quantity = oi.Quantity
            }).ToList()
        };

        // **Populate all menus** for the select list
        ViewBag.Menus = _context.Menus.ToList();

        // Populate tables if needed
        ViewBag.Tables = _context.Tables.ToList();

        return View(vm);
    }


    // POST: Orders/Edit/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, EditOrderVM vm)
    {
        if (id != vm.Id) return BadRequest();

        if (!ModelState.IsValid)
        {
            ViewBag.Tables = await _context.Tables.ToListAsync();
            return View(vm);
        }

        var order = await _context.Orders
            .Include(o => o.OrderItems)
            .FirstOrDefaultAsync(o => o.Id == id);

        if (order == null) return NotFound();

        // Update table and discount
        order.TableId = vm.TableId;
        order.Discount = vm.Discount;

        // Update order items
        foreach (var itemVm in vm.Items)
        {
            var existingItem = order.OrderItems.FirstOrDefault(oi => oi.MenuId == itemVm.MenuId);
            if (existingItem != null)
            {
                if (itemVm.Quantity > 0)
                {
                    existingItem.Quantity = itemVm.Quantity;
                    existingItem.PriceAtOrder = itemVm.Price;
                }
                else
                {
                    // Remove item if quantity set to 0
                    _context.OrderItems.Remove(existingItem);
                }
            }
            else if (itemVm.Quantity > 0)
            {
                // Add new item if quantity > 0
                order.OrderItems.Add(new OrderItem
                {
                    MenuId = itemVm.MenuId,
                    Quantity = itemVm.Quantity,
                    PriceAtOrder = itemVm.Price,
                    CreatedAt = DateTime.UtcNow
                });
            }
        }

        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }


    // GET: Orders/Delete/5
    public async Task<IActionResult> Delete(int? id)
    {
        if (id == null) return NotFound();

        var order = await _context.Orders
            .Include(o => o.Table)
            .Include(o => o.OrderItems) 
            .FirstOrDefaultAsync(o => o.Id == id);

        if (order == null) return NotFound();
        return View(order);
    }

    // POST: Orders/Delete/5
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var order = await _context.Orders
            .Include(o => o.OrderItems)
            .FirstOrDefaultAsync(o => o.Id == id);

        if (order != null)
        {
            _context.OrderItems.RemoveRange(order.OrderItems);

            _context.Orders.Remove(order);

            await _context.SaveChangesAsync();
        }

        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    public async Task<IActionResult> ToggleStatus(int id)
    {
        var order = await _context.Orders.FindAsync(id);
        if (order == null)
            return NotFound();

        order.Status = order.Status == "Pending" ? "Completed" : "Pending";
        await _context.SaveChangesAsync();

        return RedirectToAction(nameof(Index));
    }


}
