using EmaanMall.Data;
using EmaanMall.Models;
using EmaanMall.Models.ViewModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EmaanMall.Controllers
{
    public class ProductInquiries : Controller
    {
        private readonly ApplicationDbContext _context;

        public ProductInquiries(ApplicationDbContext context)
        {
            _context = context;
        }
        [Authorize(Roles = "Admin,Staff")]
        public IActionResult Index()
        {
            var inquiries = _context.ProductInquiries.Include(a=>a.customer).Include(a=>a.productDetail).ThenInclude(a=>a.Product).Where(a => a.Status == true).OrderBy(a=>a.ProductInquiryDate).ToList();
            return View(inquiries);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> IndexP(int id,string remarks)
        {
            ProductInquiryRemarks pr = new ProductInquiryRemarks();
            pr.Date = DateTime.Now;
            pr.Status = true;
            pr.ProductInquiryId =id;
            pr.AdminRemarks =remarks;
           _context.ProductInquiryRemark.Add(pr);
            _context.SaveChanges();
            return RedirectToAction(nameof(Index));
        }
        [Authorize(Roles = "Admin,Staff")]
        public async Task<IActionResult> Details(int? id)
        {

            if (id == null)
            {
                return NotFound();
            }
            ProductsVM pvm = new ProductsVM();
            pvm.productInquiries = _context.ProductInquiries.Include(a => a.customer).Include(a => a.productDetail).ThenInclude(a => a.Product).Where(a => a.Status == true && a.ProductInquiryId == id).FirstOrDefault();
            pvm.productInquiryRemarks = _context.ProductInquiryRemark.Where(a => a.ProductInquiryId == pvm.productInquiries.ProductInquiryId).ToList();
            if (pvm.productInquiries == null)
            {
                return NotFound();
            }

            return View(pvm);
        }
        [Authorize(Roles = "Admin,Staff")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var ProductInquiry = _context.ProductInquiries.Where(a => a.ProductInquiryId == id).FirstOrDefault();
            ProductInquiry.Status = false;
            _context.ProductInquiries.Update(ProductInquiry);
            _context.SaveChanges();

            return RedirectToAction(nameof(Index));
        }
        [Authorize(Roles = "Admin,Staff")]
        public ActionResult ChangeInquiryStatus(int? id, string Status)
        {
            try
            {
                var inquiry = _context.ProductInquiries.Find(id);
                if (id == null || inquiry == null)
                {
                    return NotFound();
                }
                inquiry.ProductInquiryStatus = Status;
                _context.ProductInquiries.Update(inquiry);
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
