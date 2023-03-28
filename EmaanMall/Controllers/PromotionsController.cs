using EmaanMall.Data;
using EmaanMall.Models;
using EmaanMall.Models.ViewModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace EmaanMall.Controllers
{
    public class PromotionsController : Controller
    {
        private readonly ApplicationDbContext _context;
        public readonly IWebHostEnvironment _WebHost;
        public PromotionsController(ApplicationDbContext context,IWebHostEnvironment WebHost)
        {
            _context = context;
            _WebHost = WebHost;
        }
        [Authorize(Roles = "Admin,Staff")]
        public async Task<IActionResult> Index()
        {
            promotionsVM pvm = new promotionsVM();
            pvm.promotionsList = _context.promotions.ToList();
            pvm.proProdList = _context.PromotionsProduct.Include(a => a.promotions).Include(a => a.productDetail).ThenInclude(a => a.Product).Where(a => a.Status == true && a.productDetail.ProductDetailStatus==true && a.productDetail.Product.ProductStatus==true).ToList();
            return View(pvm);
        }
        [Authorize(Roles = "Admin,Staff")]
        public IActionResult Create()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Promotions pc)
        {
            if (ModelState.IsValid)
            { 
                IFormFile file = Request.Form.Files["photo"];
            if (Request.Form.Files != null && Request.Form.Files.Count > 0)
            {
                if (!string.IsNullOrEmpty(file.Name) && file.Length > 0)
                {
                    String type = "EMP_IMG";
                    string fileName = $"{pc.Title}_{type}_{file.FileName}";
                     
                        String uploadfilePath = Path.Combine(_WebHost.WebRootPath, "Uploads//promotions");
                    if (!System.IO.Directory.Exists(uploadfilePath))
                    {
                        System.IO.Directory.CreateDirectory(uploadfilePath); //Create directory if it doesn't exist
                    }
                    string path = Path.Combine(uploadfilePath, fileName);

                    using (FileStream fs = new FileStream(path, FileMode.Create))
                    {
                        file.CopyTo(fs);
                        pc.Image = fileName;
                    }
                }
            }

            _context.promotions.Add(pc);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return RedirectToAction(nameof(Index));
        }
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var promotions = await _context.promotions.FindAsync(id);
            if (promotions == null)
            {
                return NotFound();
            }
            return View(promotions);
        }

        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Promotions promotion)
        {
            if (id != promotion.PromotionsId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    IFormFile file = Request.Form.Files["photo"];
                    if (file != null)
                    {
                        if (Request.Form.Files != null && Request.Form.Files.Count > 0)
                        {
                            var pathExist = "Uploads/promotions/" + promotion.Title;
                            if (System.IO.File.Exists(Path.Combine(_WebHost.WebRootPath, pathExist)))
                            {
                                try
                                {
                                    System.IO.File.Delete(Path.Combine(_WebHost.WebRootPath, pathExist));
                                }
                                catch
                                {
                                }
                            }
                            if (!string.IsNullOrEmpty(file.Name) && file.Length > 0)
                            {
                                String type = "EMP_IMG";
                                string fileName = $"{promotion.Title}_{type}_{file.FileName}";
                                String uploadfilePath = Path.Combine(_WebHost.WebRootPath, "Uploads//promotions");
                                if (!System.IO.Directory.Exists(uploadfilePath))
                                {
                                    System.IO.Directory.CreateDirectory(uploadfilePath); //Create directory if it doesn't exist
                                }
                                string path = Path.Combine(uploadfilePath, fileName);

                                using (FileStream fs = new FileStream(path, FileMode.Create))
                                {
                                    file.CopyTo(fs);
                                    promotion.Image = fileName;
                                }
                            }
                        }

                    }
                    _context.Update(promotion);
                    await _context.SaveChangesAsync();
                }
                catch (Exception e)
                {
                    return View(promotion);
                }
                return RedirectToAction(nameof(Index));
            }
            return View(promotion);
        }
        [Authorize(Roles = "Admin,Staff")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var productPromotions = _context.PromotionsProduct.Where(a => a.PromotionsId == id).ToList();
            _context.PromotionsProduct.RemoveRange(productPromotions);
            _context.SaveChanges();

            var promotions = _context.promotions.Where(a => a.PromotionsId == id).FirstOrDefault();
            _context.promotions.Remove(promotions);
            _context.SaveChanges();

            return RedirectToAction(nameof(Index));
        }
        [Authorize(Roles = "Admin,Staff")]
        public ActionResult ChangeStatus(int? id)
        {
            try
            {
                var promotions = _context.promotions.Find(id);
                if (id == null || promotions == null)
                {
                    return NotFound();
                }
                promotions.Status = !(promotions.Status);
                _context.promotions.Update(promotions);
                _context.SaveChanges();
                return RedirectToAction(nameof(Index));
            }
            catch (Exception e)
            {
                return RedirectToAction(nameof(Index));
            }
        }




        [Authorize(Roles = "Admin,Staff")]
        public IActionResult Index2()
        {  promotionsVM pvm = new promotionsVM();
            pvm.promotionsList = _context.promotions.Where(a => a.Status == true).ToList();
            pvm.proProdList = _context.PromotionsProduct.Include(a => a.promotions).Include(a => a.productDetail).ThenInclude(a => a.Product).Where(a => a.Status == true).ToList();
            return View(pvm);
        }
        [Authorize(Roles = "Admin,Staff")]
        public IActionResult Create2(int? id)
        {
            ViewData["promotions"] = new SelectList(_context.promotions.Where(a=>a.Status==true), "PromotionsId", "Title");
            promotionsVM pvm = new promotionsVM();
            pvm.promotions = _context.promotions.Where(a => a.PromotionsId == id).FirstOrDefault();
            return View(pvm);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create2(promotionsVM pp)
        {
            if (ModelState.IsValid)
            {
                if (pp.ProductIds != null)
                {
                    PromotionsProducts prod = new PromotionsProducts();
                    foreach (var item in pp.ProductIds)
                    {
                        prod.Id = 0;
                        prod.PromotionsId = pp.promotions.PromotionsId;
                        prod.ProductDetailId = item;
                        prod.Date = DateTime.Now;
                        prod.Status = true;
                        _context.PromotionsProduct.Add(prod);
                        await _context.SaveChangesAsync();
                        
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return RedirectToAction(nameof(Index));
        }
        [Authorize(Roles = "Admin,Staff")]
        public IActionResult Edit2(int? id)
        {
            promotionsVM pvm = new promotionsVM();
            pvm.proProdList = _context.PromotionsProduct.Include(a=>a.productDetail).ThenInclude(a=>a.Product).Where(a => a.PromotionsId == id).ToList();

            pvm.ProductIds.AddRange(pvm.proProdList.Select(a => a.productDetail.ProductId).ToList());
            pvm.promotions = _context.promotions.Where(a => a.PromotionsId == id).FirstOrDefault();

            List<Product> Products = new List<Product>();
            var ProductsDetails = _context.ProductDetails.Include(a => a.Product).Where(a => a.ProductDetailStatus == true).Distinct().ToList();
            var prod = _context.Products.Where(a => a.ProductStatus == true).ToList();
            foreach(var items in ProductsDetails)
            {
                Products.Add(prod.Where(a => a.ProductId == items.ProductId).FirstOrDefault());
            }
            var stands =Products
    .Select(s => new
    {
        StandID = s.ProductId,
        Description = string.Format("{0}  {1}", s.ProductName, ProductsDetails.Where(a=>a.ProductId==s.ProductId).FirstOrDefault().ProductDetailModelNo)
    })
    .ToList();
            ViewData["promotions"] = new SelectList(_context.promotions.Where(a => a.Status == true && a.PromotionsId == id), "PromotionsId", "Title");
            ViewData["ProductIds"] = new SelectList(stands, "StandID", "Description", pvm.ProductIds);
            return View(pvm);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit2(promotionsVM pp)
        {
            if (ModelState.IsValid)
            {
                var promoProd = _context.PromotionsProduct.Where(a => a.PromotionsId == pp.promotions.PromotionsId).ToList();
                _context.PromotionsProduct.RemoveRange(promoProd);
                 _context.SaveChanges();

                if (pp.ProductIds != null)
                {
                    PromotionsProducts prod = new PromotionsProducts();
                    foreach (var item in pp.ProductIds)
                    {
                        var pdId = _context.ProductDetails.Where(a => a.ProductId == item && a.ProductDetailStatus==true).FirstOrDefault().ProductDetailId;
                        prod.Id = 0;
                        prod.PromotionsId = pp.promotions.PromotionsId;
                        prod.ProductDetailId = pdId;
                        prod.Date = DateTime.Now;
                        prod.Status = true;
                        _context.PromotionsProduct.Add(prod);
                        await _context.SaveChangesAsync();

                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return RedirectToAction(nameof(Index));
        }
        [HttpPost]
        public ActionResult GetProducts()
         { 
            var Products = _context.ProductDetails.Include(a=>a.Product).Where(a => a.ProductDetailStatus == true && a.Product.ProductStatus==true).Distinct().ToList();
            List<SelectListItem> ProductNames = new List<SelectListItem>();
          
                Products.ForEach(x =>
                {
                    ProductNames.Add(new SelectListItem { Text = x.Product.ProductName + "  " + x.ProductDetailModelNo, Value = x.ProductDetailId.ToString()});
                });
                ViewData["productId"] = new SelectList(Products, "ProductId", "ProductName");
   
           
            return Json(ProductNames);
        }
        [Authorize(Roles = "Admin,Staff")]
        public async Task<IActionResult> Delete2(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var productPromotions = _context.PromotionsProduct.Where(a => a.PromotionsId == id).ToList();
            _context.PromotionsProduct.RemoveRange(productPromotions);
            _context.SaveChanges();


            return RedirectToAction(nameof(Index2));
        }

        // GET: DeliveryCharges/Details/5
        [Authorize(Roles = "Admin,Staff")]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var productPromotions = _context.PromotionsProduct.Where(a => a.PromotionsId == id).ToList();
            return View(productPromotions);
        }

    }
}
