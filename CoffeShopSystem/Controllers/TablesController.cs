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
    public class TablesController : Controller
    {
        private readonly AppDbContext _context;
        private readonly IMapper _mapper;
        private readonly INotyfService _notfy;

        public TablesController(AppDbContext context, IMapper mapper, INotyfService notfy)
        {
            _context = context;
            _mapper = mapper;
            _notfy = notfy;
        }

        // GET: Tables
        public async Task<IActionResult> Index()
        {
            var tables = await _context.Tables.ToListAsync();

            var tableVMs = _mapper.Map<IEnumerable<TableVM>>(tables);

            return View(tableVMs);
        }

        // GET: Tables/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var table = await _context.Tables
                .FirstOrDefaultAsync(m => m.Id == id);
            if (table == null)
            {
                return NotFound();
            }

            var tableVM = _mapper.Map<TableVM>(table);

            return View(tableVM);
        }

        // GET: Tables/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Tables/Create

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(TableVM model)
        {
            if (ModelState.IsValid)
            {
                var table = _mapper.Map<Table>(model);
                _context.Add(table);
                await _context.SaveChangesAsync();
                _notfy.Success("Table created successfully");
                return RedirectToAction(nameof(Index));
            }
            return View(model);
        }

        // GET: Tables/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var table = await _context.Tables.FindAsync(id);
            if (table == null)
            {
                return NotFound();
            }

            var tableVM = _mapper.Map<TableVM>(table);

            return View(tableVM);
        }

        // POST: Tables/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, TableVM model)
        {
            if (id != model.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var table = _mapper.Map<Table>(model);
                    _context.Update(table);
                    _notfy.Information("Table updated successfully");
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TableExists(model.Id))
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

        // GET: Tables/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var table = await _context.Tables
                .FirstOrDefaultAsync(m => m.Id == id);
            if (table == null)
            {
                return NotFound();
            }
            var tableVM = _mapper.Map<TableVM>(table);
            return View(tableVM);
        }

        // POST: Tables/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var table = await _context.Tables.FindAsync(id);
            if (table != null)
            {
                _context.Tables.Remove(table);
            }

            await _context.SaveChangesAsync();
            _notfy.Warning("Table deleted successfully");
            return RedirectToAction(nameof(Index));
        }

        private bool TableExists(int id)
        {
            return _context.Tables.Any(e => e.Id == id);
        }
    }
}

