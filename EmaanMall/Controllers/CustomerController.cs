using EmaanMall.Data;
using EmaanMall.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EmaanMall.Controllers
{
    public class CustomerController : Controller
    {
        private readonly ApplicationDbContext _context;

        public CustomerController(ApplicationDbContext context)
        {
            _context = context;
        }
        [Authorize(Roles = "Admin,Staff")]
        // GET: Products
        public async Task<IActionResult> Index()
        {
            return View(_context.Customers.ToList());
        }
        [Authorize(Roles = "Admin,Staff")]
        // GET: Products/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var customer =  _context.Customers
                .FirstOrDefault(m => m.CustomerId == id);
            if (customer == null)
            {
                return NotFound();
            }

            return View(customer);
        }
      
        [Authorize(Roles = "Admin,Staff")]
        // GET: Products/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var customer = await _context.Customers.FindAsync(id);
            if (customer == null)
            {
                return NotFound();
            }
            return View(customer);
        }

        // POST: Products/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Customer customer)
        {
            if (id != customer.CustomerId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                var customerData = _context.Customers.Where(a => a.CustomerId == id).FirstOrDefault();
                customerData.FirstName = customer.FirstName;
                customerData.LastName = customer.LastName;
                customerData.CustomerPhone = customer.CustomerPhone;
                customerData.CustomerEmail = customer.CustomerEmail;
                customerData.CustomerAddress = customer.CustomerAddress;
                customerData.CustomerStatus = customer.CustomerStatus;
                _context.Customers.Update(customerData);
                    await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(customer);
        }
      /*  [Authorize(Roles = "Admin")]
        // GET: Products/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var customer = await _context.Customers
                .FirstOrDefaultAsync(m => m.CustomerId == id);
            if (customer == null)
            {
                return NotFound();
            }

            return View(customer);
        }
*/
        // POST: Products/Delete/5
        [Authorize(Roles = "Admin,Staff")]
        public async Task<IActionResult> Delete(int id)
        {
            var customer = await _context.Customers.FindAsync(id);
            customer.CustomerStatus = !(customer.CustomerStatus);
            _context.Customers.Update(customer);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
    }
}
