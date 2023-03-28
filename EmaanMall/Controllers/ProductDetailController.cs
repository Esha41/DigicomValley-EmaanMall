using EmaanMall.Data;
using EmaanMall.Models;
using EmaanMall.Models.ViewModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace EmaanMall.Controllers
{
    public class ProductDetailController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _env;
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly UserManager<IdentityUser> _userManager;
        private RoleManager<IdentityRole> _roleManager;
        public ProductDetailController(ApplicationDbContext context, IWebHostEnvironment env, UserManager<IdentityUser> userManager, SignInManager<IdentityUser> signInManager, RoleManager<IdentityRole> roleManager)
        {
            _context = context;
            _env = env;
            _userManager = userManager;
            _signInManager = signInManager;
            _roleManager = roleManager;
        }
        [Authorize(Roles = "Admin,Vendor,Staff")]
        public async Task<IActionResult> Index(int? Prodid, int? Catid)
        {

            ProductsVM pvm = new ProductsVM();
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var user = await _userManager.FindByIdAsync(userId);
            var role = await _userManager.GetRolesAsync(user);
            if (userId != null)
            {
                if ((role.ElementAt(0) == "Admin") || (role.ElementAt(0) == "Staff"))
                {
                    if (Prodid != null)
                    {
                        pvm.productDetailsList = _context.ProductDetails.Include(s => s.Product).Where(a => a.ProductDetailStatus == true && a.ProductId == Prodid).ToList();
                    }
                    else if (Catid != null)
                    {
                        var ProductDetailist = _context.ProductDetails.Include(s => s.Product).ToList();
                        var productCatogories = _context.ProductCategories.Where(a => a.ProductCategoryStatus == true && a.CategoryId == Catid).ToList();
                        var prodDetails = from pd in ProductDetailist
                                          join pc in productCatogories
                                          on pd.ProductDetailId equals pc.ProductDetailId
                                          select pd;
                        pvm.productDetailsList = prodDetails.ToList();

                    }
                    else
                    {
                        pvm.productDetailsList = _context.ProductDetails.Include(s => s.Product).ToList();
                    }
                }
                if (role.ElementAt(0) == "Vendor")
                {
                    pvm.productDetailsList = _context.ProductDetails.Include(s => s.Product).Where(a => a.ReferenceUserId.ToString() == userId).ToList();
                }
                pvm.productImagesList = _context.ProductImages.Include(s => s.productDetail).Where(a => a.ProductImageStatus == true).ToList();
                pvm.productCategoriesList = _context.ProductCategories.Include(s => s.productDetail).Include(s => s.Category).Where(a => a.ProductCategoryStatus == true).ToList();
                pvm.productPricesBundles = _context.ProductBundles.Include(s => s.productDetail).Where(a => a.ProductBundleStatus == true).ToList();

                return View(pvm);
            }
            else
            {
                string link = Request.Scheme + "://" + Request.Host + "/Identity/Account/Login";
                return Redirect(link);
            }

        }
        [Authorize(Roles = "Admin,Vendor,Staff")]
        public async Task<IActionResult> ProductBundles(int? id)
        {
            ViewData["productsSize"] = new SelectList(_context.productSize.Where(s => s.ProductDetailId == id).ToList(), "ProductSizesId", "Size");
            ViewBag.ProductDetailId = id;
            ViewBag.isBundleExist = false;
            if (_context.ProductBundles.Any(a=>a.ProductDetailId==id))
            {
                ProductsVM pvm = new ProductsVM();
                pvm.productPricesBundles = _context.ProductBundles.Where(s => s.ProductDetailId == id).ToList();
                pvm.ProductSizeList = _context.productSize.Where(s => s.ProductDetailId == id).ToList();
                ViewBag.isBundleExist = true;
                return View(pvm);
            }
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ProductBundles(ProductsVM pvm)
        {
            if (pvm.productPricesBundles != null)
            {
                if (pvm.productPricesBundles.Count() != 0)
                {
                    var PrevprodBun = _context.ProductBundles.Where(a => a.ProductDetailId == pvm.ProductDetailId).ToList();
                    if (PrevprodBun.Count() != 0)
                    {
                        _context.ProductBundles.RemoveRange(PrevprodBun);
                       await _context.SaveChangesAsync();
                    }
                    foreach (var price in pvm.productPricesBundles)
                    {
                        price.ProductBundleId = 0;
                        price.ProductDetailId = pvm.ProductDetailId; // pvm.productDetails.ProductDetailId;
                        if(price.DiscountPrice==null)
                        {
                            price.DiscountPrice = 0;
                        }
                        price.ProductBundleStatus = true;
                        _context.ProductBundles.Add(price);
                    }
                    await _context.SaveChangesAsync();

                    var minPrice = pvm.productPricesBundles.OrderBy(a => a.ProductBundlePrice).FirstOrDefault();
                    var productDet = _context.ProductDetails.Where(a => a.ProductDetailId == pvm.ProductDetailId).FirstOrDefault();
                    productDet.ProductDetailUnitPrice = minPrice.ProductBundlePrice;
                    productDet.ProductDetailUnit = minPrice.ProductBundleUnit;
                    //calculate discunt price
                   // double cal = minPrice.DiscountPrice ?? 0/ 100; // * minPrice.ProductBundlePrice;
                    productDet.DiscountPrice = minPrice.DiscountPrice;
                    _context.ProductDetails.Update(productDet);
                   await _context.SaveChangesAsync();

                    return RedirectToAction("Index");
                }
                else
                {
                    ViewData["productsSize"] = new SelectList(_context.productSize.Where(s => s.ProductDetailId == pvm.ProductDetailId).ToList(), "ProductSizesId", "Size");
                    ViewBag.ProductDetailId = pvm.ProductDetailId;
                    return View();
                }
            }
           else
            {
                ViewData["productsSize"] = new SelectList(_context.productSize.Where(s => s.ProductDetailId == pvm.ProductDetailId).ToList(), "ProductSizesId", "Size");
                ViewBag.ProductDetailId = pvm.ProductDetailId;
                return View();
            }
        }
                
          [Authorize(Roles = "Admin,Vendor,Staff")]
        public async Task<IActionResult> Create()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var user = await _userManager.FindByIdAsync(userId);
            var role = await _userManager.GetRolesAsync(user);
            if (userId != null)
            {
                if ((role.ElementAt(0) == "Admin") || (role.ElementAt(0) == "Staff"))
                {
                    ViewData["productsCategories"] = new SelectList(_context.Categories.Where(a => a.CategoryStatus == true), "CategoryId", "CategoryName");

                }
                if (role.ElementAt(0) == "Vendor")
                {
                    var vendorId = _context.Vendor.Where(a => a.UserId == userId).FirstOrDefault().VendorId;
                    var Vencat = from cat in _context.Categories.Where(a => a.CategoryStatus == true)
                                 join vb in _context.VendorBusiness.Where(a => a.VendorId == vendorId).Select(s => new { s.CategoryId })
                                 on cat.CategoryId equals vb.CategoryId
                                 select cat;

                    ViewData["productsCategories"] = new SelectList(Vencat.ToList(), "CategoryId", "CategoryName");

                }
                ViewData["products"] = new SelectList(_context.Products.Where(a => a.ProductStatus == true), "ProductId", "ProductName");
                return View();
            }
              else
            {
                string link = Request.Scheme + "://" + Request.Host + "/Identity/Account/Login";
                return Redirect(link);
            }
        }
        public int createProduct(string name)
        {
            Product product = new Product();
            product.ProductName = name;
            product.ProductDate = DateTime.Now;
            product.ProductStatus = true;
            _context.Products.Add(product);
            _context.SaveChanges();
            return product.ProductId;
        }

        // POST: Products/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ProductsVM pvm, IEnumerable<IFormFile> Images)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    if (Images.Count() != 0)
                    {
                       var productId= createProduct(pvm.productDetails.Product.ProductName);

                        ProductDetail product = new ProductDetail();
                        product.FeatureProduct = pvm.productDetails.FeatureProduct;
                        product.BestSell = pvm.productDetails.BestSell;
                        product.TopRated = pvm.productDetails.TopRated;
                       // product.DiscountPrice = pvm.productDetails.DiscountPrice;
                        product.GuaranteePolicy = pvm.productDetails.GuaranteePolicy;
                        product.ProductDetailDescription = pvm.productDetails.ProductDetailDescription;
                        product.ProductDetailStatus = true;
                        product.ProductDetailDate = DateTime.Now;
                        product.ProductId = productId;
                        if (_context.ProductDetails.Count() == 0)
                        {
                            product.ProductDetailModelNo = "1111";
                        }
                        else
                        {
                            var lastProduct = _context.ProductDetails.ToList().LastOrDefault().ProductDetailModelNo;
                            product.ProductDetailModelNo = Convert.ToString(Convert.ToInt32(lastProduct) + 1);
                        }

                      /*  var minPrice = pvm.productPricesBundles.OrderBy(a => a.ProductBundlePrice).FirstOrDefault();  //get min price nd unit
                        product.ProductDetailUnitPrice = minPrice.ProductBundlePrice;
                        product.ProductDetailUnit = minPrice.ProductBundleUnit;*/
                        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                        product.ReferenceUserId = Guid.Parse(userId);

                        _context.ProductDetails.Add(product);
                        await _context.SaveChangesAsync();

                        if (pvm.ProductCategoryIds.Count != 0)
                        {

                            foreach (var cat in pvm.ProductCategoryIds)
                            {
                                ProductCategory prodCat = new ProductCategory();
                                prodCat.CategoryId = cat;
                                prodCat.ProductDetailId = product.ProductDetailId; // pvm.productDetails.ProductDetailId;
                                prodCat.ProductCategoryStatus = true;
                                _context.ProductCategories.Add(prodCat);
                            }
                            await _context.SaveChangesAsync();
                        }

                        if (pvm.productPricesBundles != null)
                        {
                            if (pvm.productPricesBundles.Count() != 0)
                            {
                                foreach (var price in pvm.productPricesBundles)
                                {
                                    price.ProductDetailId = product.ProductDetailId; // pvm.productDetails.ProductDetailId;
                                    price.ProductBundleStatus = true;
                                    _context.ProductBundles.Add(price);
                                }
                                await _context.SaveChangesAsync();

                            }
                        }
                        if (pvm.ProductColorsList != null)
                        {
                            if (pvm.ProductColorsList.Count() != 0)
                            {
                                foreach (var colors in pvm.ProductColorsList)
                                {
                                    colors.ProductDetailId = product.ProductDetailId;
                                    _context.productColor.Add(colors);
                                }
                                await _context.SaveChangesAsync();

                            }
                        }
                        if (pvm.ProductSizeList != null)
                        {
                            if (pvm.ProductSizeList.Count() != 0)
                            {
                                foreach (var sizes in pvm.ProductSizeList)
                                {
                                    sizes.ProductDetailId = product.ProductDetailId;
                                    _context.productSize.Add(sizes);
                                }
                                await _context.SaveChangesAsync();

                            }
                        }
                        ProductImage prodImg = new ProductImage();
                        var rand = new Random();
                        foreach (IFormFile Image in Images)
                        {
                            prodImg.ProductImageId = 0;
                            prodImg.ProductDetailId = product.ProductDetailId; // pvm.productDetails.ProductDetailId;
                            prodImg.ProductImageStatus = true;
                            if (Image != null)
                            {
                                string rootpath = _env.WebRootPath;
                                //string fileName = Path.GetFileNameWithoutExtension(Image.FileName);
                                //string extention = Path.GetExtension(Image.FileName);
                                prodImg.ProductImageName = DateTime.Now.ToString("yyyyMMddHHmmss") + rand.Next() + ".jpg";// fileName = fileName + DateTime.Now.ToString("yymmssfff") + extention;
                                //string path = Path.Combine(rootpath + "/Uploads/", fileName);
                                string path = Path.Combine(rootpath + "/Uploads/", prodImg.ProductImageName);
                                using (var filestream = new FileStream(path, FileMode.Create))
                                {
                                    await Image.CopyToAsync(filestream);
                                }
                            }
                            _context.ProductImages.Add(prodImg);
                            await _context.SaveChangesAsync();
                        }
                        return RedirectToAction(nameof(Index));
                    }
                    else
                    {
                        ViewBag.isImg = false;
                        ViewData["productsCategories"] = new SelectList(_context.Categories.Where(a => a.CategoryStatus == true), "CategoryId", "CategoryName");
                        ViewData["products"] = new SelectList(_context.Products.Where(a => a.ProductStatus == true), "ProductId", "ProductName");
                        return View(pvm);
                    }
                }
                else
                {
                    ViewData["productsCategories"] = new SelectList(_context.Categories.Where(a => a.CategoryStatus == true), "CategoryId", "CategoryName");
                    ViewData["products"] = new SelectList(_context.Products.Where(a => a.ProductStatus == true), "ProductId", "ProductName");
                    return View(pvm);
                }
            }
         
            catch (Exception e)
            {
                ViewData["productsCategories"] = new SelectList(_context.Categories.Where(a => a.CategoryStatus == true), "CategoryId", "CategoryName");
                ViewData["products"] = new SelectList(_context.Products.Where(a => a.ProductStatus == true), "ProductId", "ProductName");
                return View(pvm);
            }
        }
        [Authorize(Roles = "Admin,Vendor,Staff")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            ProductsVM pvm = new ProductsVM();
            pvm.productDetails =  _context.ProductDetails.Include(a=>a.Product).Where(s=>s.ProductDetailId==id).FirstOrDefault();
            if (pvm.productDetails == null)
            {
                return NotFound();
            }
            pvm.productName = pvm.productDetails.Product.ProductName;
            pvm.productImages = _context.ProductImages.Where(s => s.ProductDetailId == id).ToList();
          /*  pvm.productPricesBundles = _context.ProductBundles.Where(s => s.ProductDetailId == id).ToList();*/
            pvm.ProductColorsList = _context.productColor.Where(s => s.ProductDetailId == id).ToList();
            pvm.ProductSizeList = _context.productSize.Where(s => s.ProductDetailId == id).ToList();

            var categories = _context.ProductCategories.Where(s => s.ProductDetailId == id).Select(a=>new { a.CategoryId}).ToList();

            pvm.ProductCategoryIds = new List<int>();
            foreach(var arr in categories)
            {
                pvm.ProductCategoryIds.Add(arr.CategoryId);
            }
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var user = await _userManager.FindByIdAsync(userId);
            var role = await _userManager.GetRolesAsync(user);
            if (userId != null)
            {
                if ((role.ElementAt(0) == "Admin") || (role.ElementAt(0) == "Staff"))
                {
                    ViewData["productsCategories"] = new SelectList(_context.Categories.Where(a => a.CategoryStatus == true), "CategoryId", "CategoryName");

                }
                if (role.ElementAt(0) == "Vendor")
                {
                    var vendorId = _context.Vendor.Where(a => a.UserId == userId).FirstOrDefault().VendorId;
                    var Vencat = from cat in _context.Categories.Where(a => a.CategoryStatus == true)
                                 join vb in _context.VendorBusiness.Where(a => a.VendorId == vendorId).Select(s => new { s.CategoryId })
                                 on cat.CategoryId equals vb.CategoryId
                                 select cat;

                    ViewData["productsCategories"] = new SelectList(Vencat.ToList(), "CategoryId", "CategoryName");

                }
                ViewData["products"] = new SelectList(_context.Products.Where(a => a.ProductStatus == true), "ProductId", "ProductName");
                return View(pvm);
            }
            else
            {
                string link = Request.Scheme + "://" + Request.Host + "/Identity/Account/Login";
                return Redirect(link);
            }
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(ProductsVM pvm, IEnumerable<IFormFile> Images)
        {
            try
            {
                var chkImg = _context.ProductImages.Where(a => a.ProductDetailId == pvm.productDetails.ProductDetailId).Count();
                if (ModelState.IsValid)
                {
                    if (Images.Count() != 0 || chkImg!=0)
                    {
                        var product = _context.Products.Where(s => s.ProductId == pvm.productDetails.ProductId).FirstOrDefault();
                        product.ProductName = pvm.productName;
                        product.ProductDate = DateTime.Now;
                        _context.Products.Update(product);

                      /*  var minPrice = pvm.productPricesBundles.OrderBy(a => a.ProductBundlePrice).FirstOrDefault();  //get min price nd unit
                    pvm.productDetails.ProductDetailUnitPrice = minPrice.ProductBundlePrice;
                    pvm.productDetails.ProductDetailUnit = minPrice.ProductBundleUnit;*/
                        _context.ProductDetails.Update(pvm.productDetails);
                     _context.SaveChanges();

                    if (pvm.ProductCategoryIds.Count != 0)
                    {
                        var PrevprodCat = _context.ProductCategories.Where(a => a.ProductDetailId == pvm.productDetails.ProductDetailId).ToList();
                        if (PrevprodCat.Count() != 0)
                        {
                            _context.ProductCategories.RemoveRange(PrevprodCat);
                            _context.SaveChanges();
                        }

                        foreach (var cat in pvm.ProductCategoryIds)
                        {
                            ProductCategory prodCat = new ProductCategory();
                            prodCat.CategoryId = cat;
                            prodCat.ProductDetailId = pvm.productDetails.ProductDetailId;
                            prodCat.ProductCategoryStatus = true;
                            _context.ProductCategories.Add(prodCat);
                        }
                        await _context.SaveChangesAsync();
                    }
                        if (pvm.ProductColorsList != null)
                        {
                            var PrevprodBun = _context.productColor.Where(a => a.ProductDetailId == pvm.productDetails.ProductDetailId).ToList();
                            if (PrevprodBun.Count() != 0)
                            {
                                _context.productColor.RemoveRange(PrevprodBun);
                                _context.SaveChanges();
                            }

                            if (pvm.ProductColorsList.Count() != 0)
                            {
                                foreach (var colors in pvm.ProductColorsList)
                                {
                                    colors.ProductDetailId = pvm.productDetails.ProductDetailId;
                                    _context.productColor.Add(colors);
                                }
                                await _context.SaveChangesAsync();

                            }
                        }
                        if (pvm.ProductSizeList != null)
                        {
                            var PrevprodBun = _context.productSize.Where(a => a.ProductDetailId == pvm.productDetails.ProductDetailId).ToList();
                            if (PrevprodBun.Count() != 0)
                            {
                                _context.productSize.RemoveRange(PrevprodBun);
                                _context.SaveChanges();
                            }

                            if (pvm.ProductSizeList.Count() != 0)
                            {
                                foreach (var size in pvm.ProductSizeList)
                                {
                                    size.ProductDetailId = pvm.productDetails.ProductDetailId;
                                    _context.productSize.Add(size);
                                }
                                await _context.SaveChangesAsync();

                            }
                        }
                    /*    if (pvm.productPricesBundles != null)
                    {
                        var PrevprodBun = _context.ProductBundles.Where(a => a.ProductDetailId == pvm.productDetails.ProductDetailId).ToList();
                        if (PrevprodBun.Count() != 0)
                        {
                            _context.ProductBundles.RemoveRange(PrevprodBun);
                            _context.SaveChanges();
                        }

                        if (pvm.productPricesBundles.Count() != 0)
                        {
                            foreach (var price in pvm.productPricesBundles)
                            {
                                price.ProductDetailId = pvm.productDetails.ProductDetailId;
                                price.ProductBundleStatus = true;
                                _context.ProductBundles.Add(price);
                            }
                            await _context.SaveChangesAsync();

                        }
                    }*/
                    if (Images.Count() != 0)
                    {
                        //chk if there is null name image
                        ProductImage prodImg = new ProductImage();
                        var rand = new Random();
                        var chkNullImg = _context.ProductImages.Where(a => a.ProductImageName == null).ToList();
                        var newImage = Images.ToList();
                        if (chkNullImg.Count() == newImage.Count())
                        {
                            foreach (var d in chkNullImg)
                            {
                                foreach (IFormFile Image in newImage)
                                {
                                        string rootpath = _env.WebRootPath;
                                        prodImg.ProductImageName = DateTime.Now.ToString("yyyyMMddHHmmss") + rand.Next() + ".jpg";

                                        string path = Path.Combine(rootpath + "/Uploads/", prodImg.ProductImageName);
                                        using (var filestream = new FileStream(path, FileMode.Create))
                                        {
                                            await Image.CopyToAsync(filestream);
                                        }
                                        d.ProductDetailId = pvm.productDetails.ProductDetailId;
                                    d.ProductImageName = prodImg.ProductImageName;
                                    _context.ProductImages.Update(d);
                                    _context.SaveChanges();
                                    newImage.Remove(Image);
                                    break;
                                }
                            }
                        }
                       else if (chkNullImg.Count() < newImage.Count())
                        {
                            foreach (var d in chkNullImg)
                            {
                                foreach (IFormFile Image in newImage)
                                {

                                        string rootpath = _env.WebRootPath;
                                        prodImg.ProductImageName = DateTime.Now.ToString("yyyyMMddHHmmss") + rand.Next() + ".jpg";

                                        string path = Path.Combine(rootpath + "/Uploads/", prodImg.ProductImageName);
                                        using (var filestream = new FileStream(path, FileMode.Create))
                                        {
                                            await Image.CopyToAsync(filestream);
                                        }
                                        d.ProductDetailId = pvm.productDetails.ProductDetailId;
                                    d.ProductImageName = prodImg.ProductImageName;
                                    _context.ProductImages.Update(d);
                                    _context.SaveChanges();
                                    newImage.Remove(Image);
                                    break;

                                }
                            }
                               
                                foreach (IFormFile Image in newImage)
                            {
                                    string rootpath = _env.WebRootPath;
                                   prodImg.ProductImageName = DateTime.Now.ToString("yyyyMMddHHmmss") + rand.Next() + ".jpg";// fileName = fileName + DateTime.Now.ToString("yymmssfff") + extention;
                                                                                                                              //string path = Path.Combine(rootpath + "/Uploads/", fileName);
                                    string path = Path.Combine(rootpath + "/Uploads/", prodImg.ProductImageName);
                                    using (var filestream = new FileStream(path, FileMode.Create))
                                    {
                                        await Image.CopyToAsync(filestream);
                                    }
                                    prodImg.ProductImageId = 0;
                                prodImg.ProductDetailId = pvm.productDetails.ProductDetailId; // pvm.productDetails.ProductDetailId;
                                prodImg.ProductImageStatus = true;
                                prodImg.ProductImageName = prodImg.ProductImageName;
                                _context.ProductImages.Add(prodImg);
                                _context.SaveChanges();
                                newImage.Remove(Image);
                                break;
                            }

                        }
                        else if (chkNullImg.Count() > newImage.Count())
                        {
                            foreach (var d in chkNullImg)
                            {
                                foreach (IFormFile Image in newImage)
                                {
                                        string rootpath = _env.WebRootPath;
                                        prodImg.ProductImageName = DateTime.Now.ToString("yyyyMMddHHmmss") + rand.Next() + ".jpg";
                                                                                                                                
                                        string path = Path.Combine(rootpath + "/Uploads/", prodImg.ProductImageName);
                                        using (var filestream = new FileStream(path, FileMode.Create))
                                        {
                                            await Image.CopyToAsync(filestream);
                                        }

                                        d.ProductDetailId = pvm.productDetails.ProductDetailId;
                                    d.ProductImageName = prodImg.ProductImageName;
                                    _context.ProductImages.Update(d);
                                    _context.SaveChanges();
                                    newImage.Remove(Image);
                                    break;

                                }
                            }
                        }

                    }

                    /*     foreach (IFormFile Image in Images)
                         {
                             prodImg.ProductImageId = 0;
                             prodImg.ProductDetailId = pvm.productDetails.ProductDetailId; // pvm.productDetails.ProductDetailId;
                             prodImg.ProductImageStatus = true;
                             if (Image != null)
                             {
                                 string rootpath = _env.WebRootPath;
                                 string fileName = Path.GetFileNameWithoutExtension(Image.FileName);
                                 string extention = Path.GetExtension(Image.FileName);
                                 prodImg.ProductImageName = DateTime.Now.ToString("yyyyMMddHHmmss") + rand.Next() + ".jpg";// fileName = fileName + DateTime.Now.ToString("yymmssfff") + extention;
                                 string path = Path.Combine(rootpath + "/Uploads/", fileName);
                                 using (var filestream = new FileStream(path, FileMode.Create))
                                 {
                                     await Image.CopyToAsync(filestream);
                                 }
                             }
                             _context.ProductImages.Add(prodImg);
                             await _context.SaveChangesAsync();
                         }*/

                    return RedirectToAction(nameof(Index));
                }
                    else
                    {
                        ViewBag.isImg = false;
                        ViewData["productsCategories"] = new SelectList(_context.Categories.Where(a => a.CategoryStatus == true), "CategoryId", "CategoryName");
                        ViewData["products"] = new SelectList(_context.Products.Where(a => a.ProductStatus == true), "ProductId", "ProductName");
                        return View(pvm);
                    }
                }
                else
                {
                    ViewData["productsCategories"] = new SelectList(_context.Categories.Where(a => a.CategoryStatus == true), "CategoryId", "CategoryName");
                    ViewData["products"] = new SelectList(_context.Products.Where(a => a.ProductStatus == true), "ProductId", "ProductName");
                    return View(pvm);
                }
            }

            catch (Exception e)
            {
                ViewData["productsCategories"] = new SelectList(_context.Categories.Where(a => a.CategoryStatus == true), "CategoryId", "CategoryName");
                ViewData["products"] = new SelectList(_context.Products.Where(a => a.ProductStatus == true), "ProductId", "ProductName");
                return View(pvm);
            }
        }
        [Authorize(Roles = "Admin,Vendor,Staff")]
        public ActionResult Delete(int? id)
        {
            ProductsVM pvm = new ProductsVM();
            if (id == null)
            {
                return NotFound();
            }
            pvm.productDetails = _context.ProductDetails.Include(s => s.Product).Where(a => a.ProductDetailStatus == true && a.ProductDetailId == id).FirstOrDefault();
            pvm.productImagesList = _context.ProductImages.Include(s => s.productDetail).Where(a => a.ProductImageStatus == true && a.ProductDetailId == pvm.productDetails.ProductDetailId).ToList();
            pvm.productCategoriesList = _context.ProductCategories.Include(s => s.productDetail).Include(s => s.Category).Where(a => a.ProductCategoryStatus == true && a.ProductDetailId == pvm.productDetails.ProductDetailId).ToList();
            pvm.productPricesBundles = _context.ProductBundles.Include(s => s.productDetail).Where(a => a.ProductBundleStatus == true && a.ProductDetailId == pvm.productDetails.ProductDetailId).ToList();
            pvm.ProductColorsList = _context.productColor.Where(a => a.ProductDetailId == pvm.productDetails.ProductDetailId).ToList();
            pvm.ProductSizeList = _context.productSize.Where(a => a.ProductDetailId == pvm.productDetails.ProductDetailId).ToList();

            return View(pvm);
        }
        [HttpPost]
        [Authorize(Roles = "Admin,Vendor,Staff")]
        public async Task<IActionResult> Delete(int id)
       {
           try
           {
                var bundles = _context.ProductBundles.Where(a => a.ProductDetailId == id).ToList();
                _context.ProductBundles.RemoveRange(bundles);

                var images = _context.ProductImages.Where(a => a.ProductDetailId == id).ToList();
                _context.ProductImages.RemoveRange(images);

                var sizes = _context.productSize.Where(a => a.ProductDetailId == id).ToList();
                _context.productSize.RemoveRange(sizes);

                var colors = _context.productColor.Where(a => a.ProductDetailId == id).ToList();
                _context.productColor.RemoveRange(colors);

                var cat = _context.ProductCategories.Where(a => a.ProductDetailId == id).ToList();
                _context.ProductCategories.RemoveRange(cat);

                var promoProd = _context.PromotionsProduct.Where(a => a.ProductDetailId == id).ToList();
                _context.PromotionsProduct.RemoveRange(promoProd);


                var op = _context.OrderProducts.Where(a => a.ProductDetailId == id).ToList();
                foreach(var i in op)
                {
                    var order = _context.Orders.Where(a => a.OrderId == i.OrderId).ToList();
                    _context.Orders.RemoveRange(order);
                }
               await _context.SaveChangesAsync();
                _context.OrderProducts.RemoveRange(op);
                /*
                                var prodInq = _context.ProductInquiries.Where(a => a.ProductDetailId == id).ToList();
                                var inqRem =from rem in _context.ProductInquiryRemark
                                            join inq in prodInq
                                            on rem.ProductInquiryId equals 

                                _context.ProductInquiries.RemoveRange(prodInq);


                                _context.ProductInquiryRemark.RemoveRange(inqRem);*/

                var prod = _context.ProductDetails.Where(a => a.ProductDetailId == id).FirstOrDefault();
                _context.ProductDetails.Remove(prod);
                _context.SaveChanges();


                return RedirectToAction(nameof(Index));
           }
           catch (Exception e)
           {
               return RedirectToAction(nameof(Index));
           }


       }

        [Authorize(Roles = "Admin,Vendor,Staff")]
        public ActionResult Details(int? id)
        {
            ProductsVM pvm = new ProductsVM();
            if (id == null)
            {
                return NotFound();
            }
            pvm.productDetails = _context.ProductDetails.Include(s => s.Product).Where(a => a.ProductDetailStatus == true && a.ProductDetailId==id).FirstOrDefault();
            pvm.productImagesList = _context.ProductImages.Include(s => s.productDetail).Where(a => a.ProductImageStatus == true && a.ProductDetailId==pvm.productDetails.ProductDetailId).ToList();
            pvm.productCategoriesList = _context.ProductCategories.Include(s => s.productDetail).Include(s => s.Category).Where(a => a.ProductCategoryStatus == true && a.ProductDetailId == pvm.productDetails.ProductDetailId).ToList();
            pvm.productPricesBundles = _context.ProductBundles.Include(s => s.productDetail).Where(a => a.ProductBundleStatus == true && a.ProductDetailId == pvm.productDetails.ProductDetailId).ToList();
            pvm.ProductColorsList = _context.productColor.Where(a => a.ProductDetailId == pvm.productDetails.ProductDetailId).ToList();
            pvm.ProductSizeList = _context.productSize.Where(a => a.ProductDetailId == pvm.productDetails.ProductDetailId).ToList();

            return View(pvm);
        }
        public ActionResult DeleteImage(int id, string imgName, IEnumerable<IFormFile> Images)
        {
            string rootpath = _env.WebRootPath;
            string path = Path.Combine(rootpath + "/Uploads/", imgName);
            System.IO.File.Delete(path);
            var emptyImage = _context.ProductImages.Where(a => a.ProductImageName == imgName).FirstOrDefault();
            emptyImage.ProductDetailId = null;
            emptyImage.ProductImageName = null;
            _context.ProductImages.Update(emptyImage);
            _context.SaveChanges();

            return RedirectToAction(nameof(Edit),new { id = id });
        }
        public ActionResult ChangeStatus(int? id) //RemarksForChngStatus
        {
            try
            {
                var productDetail = _context.ProductDetails.Find(id);
                if (id == null || productDetail == null)
                {
                    return NotFound();
                }
                var product = _context.Products.Find(productDetail.ProductId);
                _context.Products.Update(product);
                _context.SaveChanges();

                productDetail.ProductDetailStatus = !(productDetail.ProductDetailStatus);
                _context.ProductDetails.Update(productDetail);
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
