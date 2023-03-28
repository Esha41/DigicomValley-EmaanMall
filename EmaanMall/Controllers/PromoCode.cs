using EmaanMall.Data;
using EmaanMall.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EmaanMall.Controllers
{
    public class PromoCode : Controller
    {
        private readonly ApplicationDbContext _context;

        public PromoCode(ApplicationDbContext context)
        {
            _context = context;
        }
        [Authorize(Roles = "Admin,Staff")]
        public async Task<IActionResult> Index()
        {
            return View(_context.promoCodes.ToList());
        }
        [Authorize(Roles = "Admin,Staff")]
        public IActionResult Create()
        {
            return View();
        }

        // POST: DeliveryCharges/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(PromoCodes pc)
        {
            if (ModelState.IsValid)
            {
                _context.promoCodes.Add(pc);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return RedirectToAction(nameof(Index));
        }
        [Authorize(Roles = "Admin,Staff")]
        // GET: Categories/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var promo = await _context.promoCodes.FindAsync(id);
            if (promo == null)
            {
                return NotFound();
            }
            return View(promo);
        }

        // POST: Categories/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(PromoCodes promo)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var promoCode = _context.promoCodes.Where(a => a.PromoCodesId == promo.PromoCodesId).FirstOrDefault();
                    promoCode.Title = promo.Title;
                    promoCode.promoCode = promo.promoCode;
                    promoCode.NoOfUsage = promo.NoOfUsage;
                    promoCode.discountPrice = promo.discountPrice;
                    promoCode.StartDate = promo.StartDate;
                    promoCode.EndDate = promo.EndDate;
                    _context.promoCodes.Update(promoCode);
                    _context.SaveChanges();
                }
                catch (Exception E)
                {
                    return View(promo);
                    
                }
                return RedirectToAction(nameof(Index));
            }
            return View(promo);
        }

        // GET: DeliveryCharges/Details/5
        [Authorize(Roles = "Admin,Staff")]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var promocode =  _context.promoCodes
                .FirstOrDefault(m => m.PromoCodesId == id);
            if (promocode == null)
            {
                return NotFound();
            }

            return View(promocode);
        }
        [Authorize(Roles = "Admin,Staff")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var promoCode = _context.promoCodes.Where(a => a.PromoCodesId == id).FirstOrDefault();
            _context.promoCodes.Remove(promoCode);
            _context.SaveChanges();

            return RedirectToAction(nameof(Index));
        }
        [Authorize(Roles = "Admin,Staff")]
        public ActionResult ChangeStatus(int? id) //RemarksForChngStatus
        {
            try
            {
                var promocode = _context.promoCodes.Find(id);
                if (id == null || promocode == null)
                {
                    return NotFound();
                }
                promocode.Status = !(promocode.Status);
                _context.promoCodes.Update(promocode);
                _context.SaveChanges();
                return RedirectToAction(nameof(Index));
            }
            catch (Exception e)
            {
                return RedirectToAction(nameof(Index));
            }
        }

    }
}