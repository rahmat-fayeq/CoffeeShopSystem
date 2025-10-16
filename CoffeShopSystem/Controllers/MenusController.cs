using AspNetCoreHero.ToastNotification.Abstractions;
using AutoMapper;
using CoffeShopSystem.Data;
using CoffeShopSystem.Models;
using CoffeShopSystem.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CoffeShopSystem.Controllers
{
    [Authorize]
    public class MenusController : Controller
    {
        private readonly AppDbContext _context;
        private readonly IMapper _mapper;
        private readonly INotyfService _notyf;

        public MenusController(AppDbContext context, IMapper mapper, INotyfService notyf)
        {
            _context = context;
            _mapper = mapper;
            _notyf = notyf;
        }

        // GET: Menus
        public async Task<IActionResult> Index()
        {
            var menus = await _context.Menus.ToListAsync();
            var menuVMs = _mapper.Map<List<MenuVM>>(menus);

            return View(menuVMs);
        }

        // GET: Menus/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var menu = await _context.Menus
                .FirstOrDefaultAsync(m => m.Id == id);
            if (menu == null)
            {
                return NotFound();
            }
            var menuVM = _mapper.Map<MenuVM>(menu);

            return View(menuVM);
        }

        // GET: Menus/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Menus/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(MenuVM model)
        {
            if (ModelState.IsValid)
            {
                var menu = _mapper.Map<Menu>(model);

                _context.Add(menu);
                await _context.SaveChangesAsync();
                _notyf.Success("Menu item created successfully");
                return RedirectToAction(nameof(Index));
            }
            return View(model);
        }

        // GET: Menus/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var menu = await _context.Menus.FindAsync(id);
            if (menu == null)
            {
                return NotFound();
            }

            var menuVM = _mapper.Map<MenuVM>(menu);

            return View(menuVM);
        }

        // POST: Menus/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id,MenuVM model)
        {
            if (id != model.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var menu = _mapper.Map<Menu>(model);
                    _context.Update(menu);
                    await _context.SaveChangesAsync();
                    _notyf.Information("Menu item updated successfully");
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!MenuExists(model.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(model);
        }

        // GET: Menus/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var menu = await _context.Menus
                .FirstOrDefaultAsync(m => m.Id == id);
            if (menu == null)
            {
                return NotFound();
            }
            var menuVM = _mapper.Map<MenuVM>(menu);

            return View(menuVM);
        }

        // POST: Menus/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var menu = await _context.Menus.FindAsync(id);
            if (menu == null)
            {
                return NotFound();
            }

            try
            {
                _context.Menus.Remove(menu);
                await _context.SaveChangesAsync();
                _notyf.Success("Menu item deleted successfully.");
            }
            catch(DbUpdateException)
            {
                _notyf.Error("This menu cannot be deleted because it has related orders.");
            }
            
            return RedirectToAction(nameof(Index));
        }

        private bool MenuExists(int id)
        {
            return _context.Menus.Any(e => e.Id == id);
        }
    }
}
