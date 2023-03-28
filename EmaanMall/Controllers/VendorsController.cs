using EmaanMall.Data;
using EmaanMall.Models;
using EmaanMall.Models.ViewModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace EmaanMall.Controllers
{
    public class VendorsController : Controller
    {
        private readonly ApplicationDbContext _context;

        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly UserManager<IdentityUser> _userManager;
        private RoleManager<IdentityRole> _roleManager;
        private readonly IWebHostEnvironment _env;
        public VendorsController(ApplicationDbContext context, UserManager<IdentityUser> userManager, SignInManager<IdentityUser> signInManager, RoleManager<IdentityRole> roleManager, IWebHostEnvironment env)
        {
            _context = context;
            _userManager = userManager;
            _roleManager = roleManager;
            _env = env;
        }

        [Authorize(Roles = "Admin,Staff")]
        public IActionResult Create(string email,string password)
        {
            ViewData["productsCategories"] = new SelectList(_context.Categories.Where(a => a.CategoryStatus == true), "CategoryId", "CategoryName");
            ViewBag.Email = email;
            ViewBag.password = password;
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(VendorVM vvm, IFormFile Images)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    Vendor vendors = new Vendor();
                    vendors.VendorName = vvm.vendors.VendorName;
                    vendors.BusinessName = vvm.vendors.BusinessName;
                    vendors.Address = vvm.vendors.Address;
                    vendors.Phone = vvm.vendors.Phone;
                    vendors.Email = vvm.vendors.Email;
                    vendors.Password = vvm.vendors.Password;
                    vendors.Status = true;
                    var user = _context.Users.Where(a => a.Email == vvm.vendors.Email).FirstOrDefault();
                    vendors.UserId = user.Id;

                    if (Images != null)
                    {
                        var Image = Images;
                        string rootpath = _env.WebRootPath;
                        string fileName = Path.GetFileNameWithoutExtension(Image.FileName);
                        string extention = Path.GetExtension(Image.FileName);
                       vendors.Image = fileName = DateTime.Now.ToString("yymmssfff") + extention;
                        string path = Path.Combine(rootpath + "/Uploads/", fileName);
                        using (var filestream = new FileStream(path, FileMode.Create))
                        {
                            await Image.CopyToAsync(filestream);
                        }

                    }

                    _context.Vendor.Add(vendors);
                    _context.SaveChanges();
                    if (vvm.ProductCategoryIds.Count != 0 && vvm.ProductCategoryIds != null)
                    {
                        foreach (var cat in vvm.ProductCategoryIds)
                        {
                            VendorBusiness vb=new VendorBusiness();
                            vb.BusinessId = 0;
                            vb.CategoryId = cat;
                            vb.VendorId =vendors.VendorId;
                            _context.VendorBusiness.Add(vb);
                        }
                         _context.SaveChanges();
                    }
                    return RedirectToAction("VendorList");
                   /* string link = Request.Scheme + "://" + Request.Host + "/Identity/Account/Login";
                    return Redirect(link);*/
                }
                else
                {
                    ViewData["productsCategories"] = new SelectList(_context.Categories.Where(a => a.CategoryStatus == true), "CategoryId", "CategoryName");
                    ViewBag.Email = vvm.vendors.Email;
                    return View(vvm);
                }
            }
            catch (Exception e)
            {
                ViewData["productsCategories"] = new SelectList(_context.Categories.Where(a => a.CategoryStatus == true), "CategoryId", "CategoryName");
                ViewBag.Email = vvm.vendors.Email;
                return View(vvm);
            }

        }
        [Authorize(Roles = "Vendor,Admin,Staff")]
        public async Task<IActionResult> Profile(int? id)
        {
            if (id != null)
            {
                var vendor = _context.Vendor.Where(a => a.VendorId == id).FirstOrDefault();
                VendorVM vvm = new VendorVM();
                vvm.vendors = vendor;

                var categories = _context.VendorBusiness.Where(s => s.VendorId == vendor.VendorId).ToList();
                vvm.ProductCategoryIds = new List<int>();
                foreach (var arr in categories)
                {
                    vvm.ProductCategoryIds.Add(arr.CategoryId);
                }
                ViewData["productsCategories"] = new SelectList(_context.Categories.Where(a => a.CategoryStatus == true), "CategoryId", "CategoryName");
                return View(vvm);
            }
            else
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                var user = await _userManager.FindByIdAsync(userId);
                var role = await _userManager.GetRolesAsync(user);
                if (userId != null)
                {
                    var vendor = _context.Vendor.Where(a => a.Email == user.Email).FirstOrDefault();
                    VendorVM vvm = new VendorVM();
                    vvm.vendors = vendor;

                    var categories = _context.VendorBusiness.Where(s => s.VendorId == vendor.VendorId).ToList();
                    vvm.ProductCategoryIds = new List<int>();
                    foreach (var arr in categories)
                    {
                        vvm.ProductCategoryIds.Add(arr.CategoryId);
                    }
                    ViewData["productsCategories"] = new SelectList(_context.Categories.Where(a => a.CategoryStatus == true), "CategoryId", "CategoryName");
                    return View(vvm);
                }
                else
                {
                    string link = Request.Scheme + "://" + Request.Host + "/Identity/Account/Login";
                    return Redirect(link);
                }
            }
        }
        [Authorize(Roles = "Vendor")]
        public async Task<IActionResult> Edit()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var user = await _userManager.FindByIdAsync(userId);
            var role = await _userManager.GetRolesAsync(user);
            if (userId != null)
            {
                var vendor = _context.Vendor.Where(a => a.Email == user.Email).FirstOrDefault();
                VendorVM vvm = new VendorVM();
                vvm.vendors = vendor;

                var categories = _context.VendorBusiness.Where(s => s.VendorId == vendor.VendorId).ToList();
                vvm.ProductCategoryIds = new List<int>();
                foreach (var arr in categories)
                {
                    vvm.ProductCategoryIds.Add(arr.CategoryId);
                }

                ViewData["productsCategories"] = new SelectList(_context.Categories.Where(a => a.CategoryStatus == true), "CategoryId", "CategoryName");
                return View(vvm);
            }
            else
            {
                string link = Request.Scheme + "://" + Request.Host + "/Identity/Account/Login";
                return Redirect(link);
            }
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditPost(VendorVM vvm)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    Vendor vendors = _context.Vendor.Where(a => a.VendorId == vvm.vendors.VendorId).FirstOrDefault();
                    vendors.VendorName = vvm.vendors.VendorName;
                    vendors.BusinessName = vvm.vendors.BusinessName;
                    vendors.Address = vvm.vendors.Address;
                    vendors.Phone = vvm.vendors.Phone;
                    vendors.Email = vvm.vendors.Email;
                    IFormFile file = Request.Form.Files["vendorphoto"];
                   if (file != null)
                    {
                        if (Request.Form.Files != null && Request.Form.Files.Count > 0)
                        {
                            var pathExist = "Uploads/" + vendors.Email;
                            if (System.IO.File.Exists(Path.Combine(_env.WebRootPath, pathExist)))
                            {
                                try
                                {
                                    System.IO.File.Delete(Path.Combine(_env.WebRootPath, pathExist));
                                }
                                catch
                                {
                                }
                            }
                            if (!string.IsNullOrEmpty(file.Name) && file.Length > 0)
                            {
                                String type = "EMP_IMG";
                                string fileName = $"{vendors.Email}_{type}_{file.FileName}";
                                String uploadfilePath = Path.Combine(_env.WebRootPath, "Uploads");
                                if (!System.IO.Directory.Exists(uploadfilePath))
                                {
                                    System.IO.Directory.CreateDirectory(uploadfilePath); //Create directory if it doesn't exist
                                }
                                string path = Path.Combine(uploadfilePath, fileName);

                                using (FileStream fs = new FileStream(path, FileMode.Create))
                                {
                                    file.CopyTo(fs);
                                    vendors.Image = fileName;
                                }
                            }
                        }

                    }

                    _context.Vendor.Update(vendors);
                    _context.SaveChanges();

                    if (vvm.ProductCategoryIds.Count != 0 && vvm.ProductCategoryIds != null)
                    {
                        var PrevprodCat = _context.VendorBusiness.Where(a => a.VendorId == vendors.VendorId).ToList();
                        if (PrevprodCat.Count() != 0)
                        {
                            _context.VendorBusiness.RemoveRange(PrevprodCat);
                            _context.SaveChanges();
                        }

                        foreach (var cat in vvm.ProductCategoryIds)
                        {
                            VendorBusiness vb = new VendorBusiness();
                            vb.BusinessId = 0;
                            vb.CategoryId = cat;
                            vb.VendorId = vendors.VendorId;
                            _context.VendorBusiness.Add(vb);
                        }
                        _context.SaveChanges();
                    }
                    string link = Request.Scheme + "://" + Request.Host + "/Identity/Account/Login";
                    return Redirect(link);
                }
                else
                {
                    ViewData["productsCategories"] = new SelectList(_context.Categories.Where(a => a.CategoryStatus == true), "CategoryId", "CategoryName");
                    ViewBag.Email = vvm.vendors.Email;
                    return View(vvm);
                }
            }
            catch (Exception e)
            {
                ViewData["productsCategories"] = new SelectList(_context.Categories.Where(a => a.CategoryStatus == true), "CategoryId", "CategoryName");
                ViewBag.Email = vvm.vendors.Email;
                return View(vvm);
            }

        }
       
        [Authorize(Roles = "Admin,Staff")]
        public async Task<IActionResult> VendorList()
        {
            var vendors = _context.Vendor.ToList();
            return View(vendors);
        }
        [Authorize(Roles = "Admin,Staff")]
        public async Task<IActionResult> ChangeVendorStatus(int id)
        {
            if(id==null)
            {
                return NotFound();
            }
            var vendors = _context.Vendor.Where(a => a.VendorId == id).FirstOrDefault();
            var userId = User.FindFirstValue(vendors.UserId);
            var user = await _userManager.FindByIdAsync(vendors.UserId);

           
            vendors.Status = !(vendors.Status);
            if(vendors.Status==false)
            {
                var oldPass = vendors.Password;
                vendors.Password = vendors.Password + 23;
                var changePasswordResult = await _userManager.ChangePasswordAsync(user, oldPass, vendors.Password);
                if (!changePasswordResult.Succeeded)
                {
                    foreach (var error in changePasswordResult.Errors)
                    {
                        ModelState.AddModelError(string.Empty, error.Description);
                    }
                }

            }
            _context.Vendor.Update(vendors);
            _context.SaveChanges();
            return RedirectToAction("VendorList");
        }
        [Authorize(Roles = "Admin,Staff")]
        public async Task<IActionResult> ChangeVerifyStatus(int id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var vendors = _context.Vendor.Where(a => a.VendorId == id).FirstOrDefault();
            vendors.IsVerified = !(vendors.IsVerified);
            _context.Vendor.Update(vendors);
            _context.SaveChanges();
            return RedirectToAction("VendorList");
        }
    }
}
