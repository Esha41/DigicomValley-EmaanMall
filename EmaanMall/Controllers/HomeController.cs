using EmaanMall.Data;
using EmaanMall.Models;
using EmaanMall.Models.ViewModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace EmaanMall.Controllers
{
    public class HomeController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<HomeController> _logger;
        private readonly UserManager<IdentityUser> _userManager;
        public HomeController(ILogger<HomeController> logger, ApplicationDbContext context, UserManager<IdentityUser> userManager)
        {
            _logger = logger;
            _context = context;
            _userManager = userManager;
        }

        [Authorize(Roles = "Admin,Vendor,Staff")]
        public async Task<IActionResult> Index()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var user = await _userManager.FindByIdAsync(userId);
            var role = await _userManager.GetRolesAsync(user);
            if (userId != null)
            {
                if ((role.ElementAt(0) == "Admin") || (role.ElementAt(0) == "Staff"))
                {
                    var categories = _context.Categories.Where(a => a.CategoryStatus == true).ToList();
                    List<ProductCategory> list = new List<ProductCategory>();
                    foreach (var i in categories)
                    {
                        if (_context.ProductCategories.Where(s => s.CategoryId == i.CategoryId).FirstOrDefault() != null)
                        {
                            list.Add(_context.ProductCategories.Include(a => a.Category).Include(a => a.productDetail).ThenInclude(a => a.Product).Where(s => s.CategoryId == i.CategoryId).FirstOrDefault());
                        }
                    }

                    OrderVM orderVM = new OrderVM()
                    {
                        /*  .Where(a => a.Date.Year == DateTime.Now.Year).Count();
                   .Where(a => a.Date.Year == DateTime.Now.AddYears(-1).Year).Count();*/
                        //  orders = _context.Orders.Where(a => a.OrderDate > DateTime.Now.AddYears(-1)).ToList(),  Where(a => a.OrderDate.Year == DateTime.Now.Year).
                        orders = _context.Orders.Where(a => a.OrderDate >= DateTime.Now.AddYears(-1)).ToList(),
                        Allorders = _context.Orders.ToList(),
                        products = _context.ProductDetails.Include(a=>a.Product).Where(a => a.ProductDetailStatus == true).OrderByDescending(a => a.ProductDetailDate).Take(5).ToList(),  //ProductDate > DateTime.Now.AddMonths(-1)
                        productDetails = _context.ProductDetails.ToList(),
                        productCategories = list.Distinct().Take(5).ToList(),
                        AllproductCategories = _context.ProductCategories.Where(a => a.ProductCategoryStatus == true).Distinct().ToList(),
                        productInquiries = _context.ProductInquiries.Where(s => s.Status == true && s.ProductInquiryStatus == "pending").ToList(),
                        promocodeList = _context.promoCodes.Where(s => s.Status == true).ToList()

                    };
                    return View(orderVM);
                }
               else if (role.ElementAt(0) == "Vendor")
                {
                    var categories = _context.Categories.Where(a => a.CategoryStatus == true).ToList();
                    List<ProductCategory> list = new List<ProductCategory>();
                    foreach (var i in categories)
                    {
                        if (_context.ProductCategories.Where(s => s.CategoryId == i.CategoryId).FirstOrDefault() != null)
                        {
                            list.Add(_context.ProductCategories.Include(a => a.Category).Include(a => a.productDetail).ThenInclude(a => a.Product).Where(s => s.CategoryId == i.CategoryId && s.productDetail.ReferenceUserId.ToString() == userId).FirstOrDefault());
                        }
                    }
                    var ordersList = _context.Orders;
                    var productDetailsList = _context.ProductDetails.Include(s => s.Product).Where(a => a.ReferenceUserId.ToString() == userId).ToList();
                    var orderedProduct = _context.OrderProducts.ToList();
                    List<Order> VendorOrders = new List<Order>();
                    List<OrderProduct> OrdersProduct = new List<OrderProduct>();
                    foreach (var i in productDetailsList)
                    {
                        OrdersProduct.AddRange(orderedProduct.Where(s => s.ProductDetailId == i.ProductDetailId).ToList());
                    }
                    foreach (var j in OrdersProduct)
                    {
                        VendorOrders.AddRange(ordersList.Where(v => v.OrderId == j.OrderId));
                    }

                    OrderVM orderVM = new OrderVM()
                    {
                        orders = VendorOrders.Where(a => a.OrderDate >= DateTime.Now.AddYears(-1)).Distinct().ToList(),
                        Allorders = VendorOrders.Distinct().ToList(),
                        products = productDetailsList.Where(a => a.ProductDetailStatus == true).OrderByDescending(a => a.ProductDetailDate).Take(5).ToList(),  //ProductDate > DateTime.Now.AddMonths(-1)
                        productDetails = productDetailsList,
                        productCategories = list.Distinct().Take(5).ToList(),
                        AllproductCategories = _context.ProductCategories.Include(s=>s.productDetail).Where(a => a.ProductCategoryStatus == true && a.productDetail.ReferenceUserId.ToString() == userId).Distinct().ToList(),
                        productInquiries = _context.ProductInquiries.Include(a=>a.productDetail).Where(s => s.Status == true && s.ProductInquiryStatus == "pending" && s.productDetail.ReferenceUserId.ToString() == userId).ToList(),
                        //promocodeList = _context.promoCodes.Where(s => s.Status == true).ToList()

                    };
                    return View(orderVM);
                }
                string link = Request.Scheme + "://" + Request.Host + "/Identity/Account/Logout";
                return Redirect(link);

            }
            else
            {
                string link = Request.Scheme + "://" + Request.Host + "/Identity/Account/Logout";
                return Redirect(link);
            }
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
