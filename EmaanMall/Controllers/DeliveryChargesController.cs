using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using EmaanMall.Data;
using EmaanMall.Models;
using Microsoft.AspNetCore.Authorization;

namespace EmaanMall.Controllers
{
    public class DeliveryChargesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public DeliveryChargesController(ApplicationDbContext context)
        {
            _context = context;
        }
        [Authorize(Roles = "Admin")]
        // GET: DeliveryCharges
        public async Task<IActionResult> Index()
        {
            return View(await _context.DeliveryCharges.ToListAsync());
        }
        [Authorize(Roles = "Admin")]
        // GET: DeliveryCharges/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var deliveryCharges = await _context.DeliveryCharges
                .FirstOrDefaultAsync(m => m.DeliveryChargesId == id);
            if (deliveryCharges == null)
            {
                return NotFound();
            }

            return View(deliveryCharges);
        }
        [Authorize(Roles = "Admin")]
        // GET: DeliveryCharges/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: DeliveryCharges/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("DeliveryChargesId,DeliveryChargesProvince,DeliveryChargesCity,DeliveryChargesAmount,DeliveryChargesStatus,DeliveryChargesDate")] DeliveryCharges deliveryCharges)
        {
            if (ModelState.IsValid)
            {
                _context.Add(deliveryCharges);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(deliveryCharges);
        }
        [Authorize(Roles = "Admin")]
        // GET: DeliveryCharges/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var deliveryCharges = await _context.DeliveryCharges.FindAsync(id);
            if (deliveryCharges == null)
            {
                return NotFound();
            }
            return View(deliveryCharges);
        }

        // POST: DeliveryCharges/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("DeliveryChargesId,DeliveryChargesProvince,DeliveryChargesCity,DeliveryChargesAmount,DeliveryChargesStatus,DeliveryChargesDate")] DeliveryCharges deliveryCharges)
        {
            if (id != deliveryCharges.DeliveryChargesId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(deliveryCharges);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!DeliveryChargesExists(deliveryCharges.DeliveryChargesId))
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
            return View(deliveryCharges);
        }
        [Authorize(Roles = "Admin")]
        // GET: DeliveryCharges/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var deliveryCharges = await _context.DeliveryCharges
                .FirstOrDefaultAsync(m => m.DeliveryChargesId == id);
            if (deliveryCharges == null)
            {
                return NotFound();
            }

            return View(deliveryCharges);
        }

        // POST: DeliveryCharges/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var deliveryCharges = await _context.DeliveryCharges.FindAsync(id);
            _context.DeliveryCharges.Remove(deliveryCharges);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        [Authorize(Roles = "Admin")]
        private bool DeliveryChargesExists(int id)
        {
            return _context.DeliveryCharges.Any(e => e.DeliveryChargesId == id);
        }
    }
}
