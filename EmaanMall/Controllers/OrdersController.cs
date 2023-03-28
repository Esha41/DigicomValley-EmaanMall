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
using System.Net;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using Newtonsoft.Json;
using System.Text;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;

namespace EmaanMall.Controllers
{
    public class OrdersController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly UserManager<IdentityUser> _userManager;
        private RoleManager<IdentityRole> _roleManager;
        public OrdersController(ApplicationDbContext context, UserManager<IdentityUser> userManager, SignInManager<IdentityUser> signInManager, RoleManager<IdentityRole> roleManager)
        {
            _context = context;
            _userManager = userManager;
            _signInManager = signInManager;
            _roleManager = roleManager;
        }
        [Authorize(Roles = "Admin,Vendor,Staff")]
        // GET: Orders
        public async Task<IActionResult> Index()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var user = await _userManager.FindByIdAsync(userId);
            var role = await _userManager.GetRolesAsync(user);
            if (userId != null)
            {
                if ((role.ElementAt(0) == "Admin") || (role.ElementAt(0) == "Staff"))
                {
                    var applicationDbContext = _context.Orders.Include(o => o.Customer).Where(s => (s.OrderStatus.ToLower() == "processing" || s.OrderStatus.ToLower() == "dispatched") && (s.Customer.CustomerStatus == true));
                    return View(await applicationDbContext.ToListAsync());
                }
                if (role.ElementAt(0) == "Vendor")
                {
                    var applicationDbContext = _context.Orders.Include(o => o.Customer).Where(s => (s.OrderStatus.ToLower() == "processing" || s.OrderStatus.ToLower() == "dispatched") && (s.Customer.CustomerStatus == true));

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
                         VendorOrders.AddRange(applicationDbContext.Where(v => v.OrderId ==j.OrderId));
                    }
                    return View(VendorOrders.Distinct());
                }
                string link = Request.Scheme + "://" + Request.Host + "/Identity/Account/Login";
                return Redirect(link);
            }
            else
            {
                string link = Request.Scheme + "://" + Request.Host + "/Identity/Account/Login";
                return Redirect(link);
            }
          
            
        }
        [Authorize(Roles = "Admin,Vendor,Staff")]
        public async Task<IActionResult> OrderHistory(string? type)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var user = await _userManager.FindByIdAsync(userId);
            var role = await _userManager.GetRolesAsync(user);
            List<Order> applicationDbContext = new List<Order>();
            if (userId != null)
            {
                if ((role.ElementAt(0) == "Admin") || (role.ElementAt(0) == "Staff"))
                {
                    if (type == "all")
                    {
                         applicationDbContext = _context.Orders.Include(o => o.Customer).ToList();
                    }
                    else if (type == "delivered")
                    {
                         applicationDbContext = _context.Orders.Include(o => o.Customer).Where(s => (s.OrderStatus.ToLower() == "delivered")).ToList();
                    }
                    else if (type == "rejected")
                    {
                         applicationDbContext = _context.Orders.Include(o => o.Customer).Where(s => (s.OrderStatus.ToLower() == "rejected")).ToList();
                    }
                    else
                    {
                        applicationDbContext = _context.Orders.Include(o => o.Customer).Where(s => (s.OrderStatus.ToLower() == "delivered" || s.OrderStatus.ToLower() == "rejected")).OrderBy(s => s.OrderStatus).ToList();
                    }

                }
                else if (role.ElementAt(0) == "Vendor")
                {
                    List<Order> orderList = new List<Order>();
                    if (type == "all")
                    {
                        orderList = _context.Orders.Include(o => o.Customer).ToList();
                    }
                    else if (type == "delivered")
                    {
                        orderList = _context.Orders.Include(o => o.Customer).Where(s => (s.OrderStatus.ToLower() == "delivered")).ToList();
                    }
                    else if (type == "rejected")
                    {
                        orderList = _context.Orders.Include(o => o.Customer).Where(s => (s.OrderStatus.ToLower() == "rejected")).ToList();
                    }
                    else
                    {
                        orderList = _context.Orders.Include(o => o.Customer).Where(s => (s.OrderStatus.ToLower() == "delivered" || s.OrderStatus.ToLower() == "rejected")).OrderBy(s => s.OrderStatus).ToList();
                    }
                  /*  var orderList = _context.Orders.Include(o => o.Customer).Where(s => (s.OrderStatus.ToLower() == "delivered" || s.OrderStatus.ToLower() == "rejected")).OrderBy(s => s.OrderStatus).ToList();
*/
                    var productDetailsList = _context.ProductDetails.Include(s => s.Product).Where(a => a.ReferenceUserId.ToString() == userId).ToList();
                    var orderedProduct = _context.OrderProducts.ToList();
                    List<OrderProduct> OrdersProduct = new List<OrderProduct>();
                    foreach (var i in productDetailsList)
                    {
                        OrdersProduct.AddRange(orderedProduct.Where(s => s.ProductDetailId == i.ProductDetailId).ToList());
                    }
                    foreach (var j in OrdersProduct)
                    {
                        applicationDbContext.AddRange(orderList.Where(v => v.OrderId == j.OrderId).ToList());
                    }
                   
                }
                else
                {
                    string link = Request.Scheme + "://" + Request.Host + "/Identity/Account/Login";
                    return Redirect(link);
                }
                return View(applicationDbContext.Distinct());
            }
            else
            {
                string link = Request.Scheme + "://" + Request.Host + "/Identity/Account/Login";
                return Redirect(link);
            }

        }
        // GET: Orders/Details/5
        [Authorize(Roles = "Admin,Vendor,Staff")]
        public async Task<IActionResult> Details(long? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            /*
                        var order = await _context.Orders
                            .Include(o => o.Customer)
                            .FirstOrDefaultAsync(m => m.OrderId == id);*/

            var order =  _context.OrderProducts.Include(a=>a.productDetail).ThenInclude(a=>a.Product).Include(a=>a.Order)
                .ThenInclude(o => o.Customer)
                .Where(m => m.OrderId == id).ToList();

            if (order == null)
            {
                return NotFound();
            }

            return View(order);
        }
        [Authorize(Roles = "Admin,Staff")]
        // GET: Orders/Create
        public IActionResult Create()
        {
           
            ViewData["CustomerId"] = new SelectList(_context.Customers, "CustomerId", "FirstName");
            return View();
        }

        // POST: Orders/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("OrderId,CustomerId,OrderSubTotal,OrderTotalPrice,OrderDeliveryCharges,OrderDiscountPrice,OrderGST,OrderPaymentMethod,OrderDate,OrderShippingAddress,OrderShippingProvince,OrderShippingCity,OrderRecipentName,OrderRecipentPhone,OrderRecipentEmail,OrderStatus,OrderNo,OrderReceiveAmount,OrderRemainingAmount")] Order order)
        {
            if (ModelState.IsValid)
            {
                order.OrderStatus = "processing";
                _context.Add(order);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["CustomerId"] = new SelectList(_context.Customers, "CustomerId", "CustomerId", order.CustomerId);
            return View(order);
        }
        [Authorize(Roles = "Admin,Staff")]
        // GET: Orders/Edit/5
        public async Task<IActionResult> Edit(long? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var order = await _context.Orders.FindAsync(id);
            if (order == null)
            {
                return NotFound();
            }
            ViewData["CustomerId"] = new SelectList(_context.Customers, "CustomerId", "CustomerId", order.CustomerId);
            return View(order);
        }

        // POST: Orders/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(long id, [Bind("OrderId,CustomerId,OrderSubTotal,OrderTotalPrice,OrderDeliveryCharges,OrderDiscountPrice,OrderGST,OrderPaymentMethod,OrderDate,OrderShippingAddress,OrderShippingProvince,OrderShippingCity,OrderRecipentName,OrderRecipentPhone,OrderRecipentEmail,OrderStatus,OrderNo,OrderReceiveAmount,OrderRemainingAmount")] Order order)
        {
            if (id != order.OrderId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(order);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!OrderExists(order.OrderId))
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
            ViewData["CustomerId"] = new SelectList(_context.Customers, "CustomerId", "CustomerId", order.CustomerId);
            return View(order);
        }
        [Authorize(Roles = "Admin,Staff")]
        // GET: Orders/Delete/5
        public async Task<IActionResult> Delete(long? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var order = await _context.Orders
                .Include(o => o.Customer)
                .FirstOrDefaultAsync(m => m.OrderId == id);
            if (order == null)
            {
                return NotFound();
            }

            return View(order);
        }

        // POST: Orders/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(long id)
        {
            var order = await _context.Orders.FindAsync(id);
            _context.Orders.Remove(order);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        [Authorize(Roles = "Admin,Staff")]
        private bool OrderExists(long id)
        {
            return _context.Orders.Any(e => e.OrderId == id);
        }
        [Authorize(Roles = "Admin,Vendor,Staff")]
        public ActionResult ChangeOrderStatus(long? id, string Status)
        {
            try
            {
                var orders = _context.Orders.Include(s=>s.Customer).Where(a => a.OrderId == id).FirstOrDefault();
                if (id == null || orders == null)
                {
                    return NotFound();
                }
                orders.OrderStatus = Status;
                _context.Orders.Update(orders);
                _context.SaveChanges();

                if (Status.ToLower() != "processing")
                {
                    string msg = "Dear " + orders.Customer.FirstName + ", Your order " + orders.OrderNo + " has been " + orders.OrderStatus + "!";
                    SendNotification(msg, orders.Customer.FCMToken);
                }
                return RedirectToAction(nameof(Index));
            }
            catch (Exception e)
            {
                return RedirectToAction(nameof(Index));
            }
        }

        public void SendNotification(string msg, string FCMToken)
        {
            var data = new
            {
                to = FCMToken,

                notification = new
                {
                    body = msg,
                    title = "Notification"
                }
            };

            var json = JsonConvert.SerializeObject(data, Formatting.Indented);
            Byte[] byteArray = Encoding.UTF8.GetBytes(json);
            SendNotification(byteArray, FCMToken);
        }
        public HttpResponseMessage SendNotification(Byte[] byteArray, string FCMToken)
        {
            var response = new HttpResponseMessage(HttpStatusCode.OK);
            try
            {
                //now code is
                string SERVER_API_KEY = "AAAAYTmDMbI:APA91bFFWgViFVH8obf5ILwkUeVhvYcfsjKwX3sZAXW4HVkKCP9Z1ni3s_pjm1xvh_Bpi0rH--AGFOrVj5cF2yyGg5AeavAzzRcaRIIofIgxt16b08735fahFN8TOAhMqjMXTR18CCmh";
                var SENDER_ID = "417576726962";
                // Create Request
                WebRequest tRequest;
                //first code 

                tRequest = WebRequest.Create("https://fcm.googleapis.com/fcm/send");     // FCM link ??
                tRequest.Method = "post";
                tRequest.ContentType = "application/json";

                tRequest.Headers.Add(string.Format($"Authorization: key ={SERVER_API_KEY}"));     //Server Api Key Header
                tRequest.Headers.Add(string.Format($"Sender: id ={SENDER_ID}"));     // Sender Id Header
                tRequest.ContentLength = byteArray.Length;
                Stream dataStream = tRequest.GetRequestStream();
                dataStream.Write(byteArray, 0, byteArray.Length);
                dataStream.Close();
                WebResponse tResponse = tRequest.GetResponse();
                dataStream = tResponse.GetResponseStream();
                StreamReader tReader = new StreamReader(dataStream);
                String sResponseFromServer = tReader.ReadToEnd();
                System.Diagnostics.Debug.WriteLine(sResponseFromServer);
                tReader.Close();
                dataStream.Close();
                tResponse.Close();
                response.Content = new StringContent(JsonConvert.SerializeObject("Successfull"));

                response.Content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
                return response;
            }
            catch (Exception ex)
            {
                // throw ex;
                response.Content = new StringContent(JsonConvert.SerializeObject("Successfull"));

                response.Content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
                return response;
            }
        }

    }
}
