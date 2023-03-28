using EmaanMall.Data;
using EmaanMall.Models;
using EmaanMall.Models.ViewModel;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace EmaanMall.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserAppAPIController : ControllerBase
    {

        private readonly ApplicationDbContext _db;
        private readonly IWebHostEnvironment _env;
        public UserAppAPIController(ApplicationDbContext db, IWebHostEnvironment env)
        {

            _env = env;
            _db = db;
        }
        public class CategoryList
        {
            public int cat_arr { get; set; }
        }
        public class ProductDetails
        {
            public int startPrice { get; set; }
            public int endPrice { get; set; }
            public string sort { get; set; }
            public int pgNo { get; set; }
            public int? VendorId { get; set; }
            public List<CategoryList> CategoryList { get; set; }
        }
        [HttpGet("isUserExist")]
        public async Task<IActionResult> isUserExist(string phoneNo)
        {
            bool IsResponse = true;
            bool IsStatus = true;
            try
            {
                if (phoneNo != null)
                {
                    var pho ="+"+ phoneNo;
                    if (_db.Customers.Any(s => s.CustomerPhone == pho))
                    {
                        var User = _db.Customers.Where(a => a.CustomerPhone == pho).FirstOrDefault();

                        var items = new
                        {
                            IsResponse = true,
                            IsStatus = true,
                            User = new
                            {
                                User.CustomerId,
                                User.FirstName,
                                User.LastName,
                                User.CustomerPhone
                            }
                        };
                        return Ok(items);
                    }
                }
                var item = new
                {
                    IsResponse = true,
                    IsStatus = false
                };
                return Ok(item);
            }
            catch (Exception e)
            {
                var item = new
                {
                    IsResponse = false,
                    IsStatus = false
                };
                return BadRequest(item);
            }

        }

       [HttpPost("CreateUserProfile")] 
        public async Task<IActionResult> CreateUserProfile([Microsoft.AspNetCore.Mvc.FromBody] Customer User)
        {
            bool IsResponse = true;
            bool IsStatus = true;
            bool IsUserExist = false;
            try
            {
                if (ModelState.IsValid)
                {
                    //add image strt
                    //  var rand = new Random();
                    if (_db.Customers.Any(a => a.CustomerPhone == User.CustomerPhone))
                    {
                        var show = new
                        {
                            IsResponse = true,
                            IsStatus = true,
                            IsUserExist = true,
                            Msg = "User's phone number already exists"

                        };
                        return Ok(show);

                    }
                    /*   if (User.ProfilePhoto != "0")
                       {
                           var path = Path.Combine(_env.ContentRootPath, "wwwroot/Images");

                           if (!System.IO.Directory.Exists(path))
                           {
                               System.IO.Directory.CreateDirectory(path); //Create directory if it doesn't exist
                           }
                           string Imagename = DateTime.Now.ToString("yyyyMMddHHmmss") + rand.Next() + ".jpg";
                           string imgPath = Path.Combine(path, Imagename);
                           byte[] imageBytes = Convert.FromBase64String(User.ProfilePhoto);
                           System.IO.File.WriteAllBytes(imgPath, imageBytes);

                           User.ProfilePhoto = Imagename;
                       }
   */
                    User.CustomerStatus = true;
                    User.CustomerDate = DateTime.Now;
                    _db.Customers.Add(User);
                    await _db.SaveChangesAsync();
                }
                else
                {
                    var show = new
                    {
                        IsResponse = true,
                        IsStatus = false,

                    };
                    return Ok(show);
                }
                var item = new
                {
                    IsResponse,
                    IsStatus,
                  //  IsUserExist = false,
                    User = new
                    {
                        User.CustomerId,
                        User.FirstName,
                        User.LastName,
                        User.CustomerPhone
                    }
                };
                return Ok(item);
            }
            catch (Exception e)
            {
                var item = new
                {
                    IsResponse = false,
                    IsStatus = false
                };
                return BadRequest(item);
            }
        }
        [HttpPost("EdituserProfile")]
        public async Task<IActionResult> EdituserProfile(Customer updatedProfile)
        {
            bool IsStatus = true;
            string IsResponse = "";
            try
            {
                var Profile = _db.Customers.FirstOrDefault(b => b.CustomerId == updatedProfile.CustomerId);
               // var rand = new Random();
                if (Profile != null)
                {
                   /* if (updatedProfile.ProfilePhoto != "0" && updatedProfile.ProfilePhoto != null)
                    {
                        var path = Path.Combine(_env.ContentRootPath, "wwwroot/Images");

                        if (!System.IO.Directory.Exists(path))
                        {
                            System.IO.Directory.CreateDirectory(path); //Create directory if it doesn't exist
                        }
                        string Imagename = DateTime.Now.ToString("yyyyMMddHHmmss") + rand.Next() + ".jpg";
                        string imgPath = Path.Combine(path, Imagename);
                        byte[] imageBytes = Convert.FromBase64String(updatedProfile.ProfilePhoto);
                        System.IO.File.WriteAllBytes(imgPath, imageBytes);

                        updatedProfile.ProfilePhoto = Imagename;
                        Profile.ProfilePhoto = updatedProfile.ProfilePhoto;
                    }*/

                    Profile.FirstName = updatedProfile.FirstName;
                    Profile.LastName = updatedProfile.LastName;
                    Profile.CustomerPhone = updatedProfile.CustomerPhone;
                    Profile.CustomerEmail = updatedProfile.CustomerEmail;
                    Profile.CustomerAddress = updatedProfile.CustomerAddress;

                    /*
                    Profile.CustomerGender = updatedProfile.CustomerGender;
                    Profile.CustomerDateOfBirth = updatedProfile.CustomerDateOfBirth;
                    Profile.CustomerPassword = updatedProfile.CustomerPassword;
                    Profile.CustomerCNIC = updatedProfile.CustomerCNIC;
                    Profile.CustomerAddress = updatedProfile.CustomerAddress;
                    Profile.CustomerStatus = updatedProfile.CustomerStatus;
                    Profile.CustomerDate = updatedProfile.CustomerDate;*/
                    IsStatus = true;
                    IsResponse = "updated";

                    _db.Customers.Update(Profile);
                    await _db.SaveChangesAsync();

                    var items = new
                    {
                        IsStatus,
                        IsResponse,
                        User = new
                        {
                            updatedProfile.CustomerId,
                            updatedProfile.FirstName,
                            updatedProfile.LastName,
                            updatedProfile.CustomerPhone,
                            updatedProfile.CustomerEmail,
                            updatedProfile.CustomerAddress
                        }
                    };

                    return Ok(items);
                }
                else
                {
                    IsStatus = false;
                    IsResponse = "Not updated";
                }
                var item = new
                {
                    IsResponse,
                    IsStatus
                };

                return Ok(item);
            }
            catch (Exception e)
            {
                var item = new
                {
                    IsResponse = false,
                    IsStatus = false
                };

                return BadRequest(item);
            }


        }
      
/*
       [HttpGet("GetuserProfile")]
        public IActionResult GetuserProfile(int userId)  
        {
            bool IsResponse = true;
            bool IsStatus = true;
            try
            {
                var UserProfile = _db.Customers.Where(b => b.CustomerId == userId && b.CustomerStatus).Select(md => new
                {
                    md.CustomerId,
                    md.FirstName,
                    md.LastName,
                    md.CustomerPhone

                }).FirstOrDefault();

                if (UserProfile == null)
                {
                    var item = new
                    {
                        IsResponse = true,
                        IsStatus = false
                    };

                    return Ok(item);
                }
                var profile = new
                {
                    IsResponse,
                    IsStatus,
                    User = UserProfile
                };

                return Ok(profile);
            }
            catch (Exception e)
            {
                var profile = new
                {
                    IsResponse = false,
                    IsStatus = false

                };

                return BadRequest(profile);
            }
        }*/
        [HttpGet("AllProducts")]
        public IActionResult AllProducts(int pgNo)
        {
            try
            {
                bool IsStatus = true;
                bool IsResponse = true;
                bool IsNextPageExist = true;
                int list = 10;
                //pagination
                int skip = list * (pgNo);     
                int count = _db.ProductDetails.Where(a => a.ProductDetailStatus == true).Count(); 
                int take = count - skip;     
                if (take <= list)
                {
                    IsNextPageExist = false;
                }
                skip = skip > count ? 0 : skip;
                take = take > list ? list : take < 0 ? 0 : take;    //now take is just 5
             //pagination
                var AllProducts = _db.ProductDetails.Include(a => a.Product).Where(s => s.ProductDetailStatus == true).Select(s => new
                {
                    s.ProductDetailId,
                    s.Product.ProductName,
                    ProductPrice = s.ProductDetailUnitPrice,
                    s.DiscountPrice,
                    ProductImage = _db.ProductImages.Where(v => v.ProductDetailId == s.ProductDetailId && v.ProductImageStatus == true).FirstOrDefault().ProductImageName,

                       ProductColors = _db.productColor.Where(a => a.ProductDetailId == s.ProductDetailId).Select(a => new
                       {
                           a.ProductColorsId,
                           a.ColorName,
                           a.ColorCode
                       }).ToList(),
                    ProductSize = _db.productSize.Where(a => a.ProductDetailId == s.ProductDetailId).Select(a => new
                    {
                        a.ProductSizesId,
                        a.Size
                    }).ToList(),
                }).ToList();


                var items = new
                {
                    IsResponse,
                    IsStatus,
                    IsNextPageExist,
                    AllProducts = AllProducts.Skip(skip).Take(take).ToList()
                };
                return Ok(items);
            }
            catch (Exception e)
            {
                var items = new
                {
                    IsStatus = false,
                    IsResponse = false
                };
                return Ok(items);
            }
        }
        [HttpGet("MainScreen")]
        public IActionResult MainScreen()
        {
            try
            {
                bool IsStatus = true;
                bool IsResponse = true;
                bool IsNextPageExist = true;
                int pgNo = 0;
                int list = 10;
                //pagination
                int skip = list * (pgNo);
                int count = _db.ProductDetails.Where(a => a.ProductDetailStatus == true).Count();
                int take = count - skip;
                if (take <= list)
                {
                    IsNextPageExist = false;
                }


               
                var CategoriesList = _db.Categories.Where(s => s.CategoryStatus == true).Select(a => new
                {
                    a.CategoryId,
                    a.CategoryName,
                    a.CategoryIcon
                }).ToList();

                var FeaturedProducts = _db.ProductDetails.Include(a => a.Product).Where(s => s.FeatureProduct == true && s.ProductDetailStatus == true).Select(s => new
                {
                    s.ProductDetailId,
                    s.Product.ProductName,
                    ProductImage = _db.ProductImages.Where(v => v.ProductImageStatus == true && v.ProductDetailId == s.ProductDetailId && v.productDetail.FeatureProduct == true).FirstOrDefault().ProductImageName,
                    ProductPrice = s.ProductDetailUnitPrice,
                    s.DiscountPrice,
                    ProductColors = _db.productColor.Where(a => a.ProductDetailId == s.ProductDetailId).Select(a => new
                    {
                        a.ProductColorsId,
                        a.ColorName,
                        a.ColorCode
                    }).ToList(),
                    ProductSize = _db.productSize.Where(a => a.ProductDetailId == s.ProductDetailId).Select(a => new
                    {
                        a.ProductSizesId,
                        a.Size
                    }).ToList(),
                }).Take(5).ToList();

                var BestSellProducts = _db.ProductDetails.Include(a => a.Product).Where(s => s.BestSell == true && s.ProductDetailStatus == true).Select(s => new
                {
                    s.ProductDetailId,
                    s.Product.ProductName,
                    ProductImage = _db.ProductImages.Where(v => v.ProductImageStatus == true && v.ProductDetailId == s.ProductDetailId && v.productDetail.BestSell == true).FirstOrDefault().ProductImageName,
                    ProductPrice = s.ProductDetailUnitPrice,
                    s.DiscountPrice,
                    ProductColors = _db.productColor.Where(a => a.ProductDetailId == s.ProductDetailId).Select(a => new
                    {
                        a.ProductColorsId,
                        a.ColorName,
                        a.ColorCode
                    }).ToList(),
                    ProductSize = _db.productSize.Where(a => a.ProductDetailId == s.ProductDetailId).Select(a => new
                    {
                        a.ProductSizesId,
                        a.Size
                    }).ToList(),
                }).Take(5).ToList();

                var TopRatedProducts = _db.ProductDetails.Include(a => a.Product).Where(s => s.TopRated == true && s.ProductDetailStatus == true).Select(s => new
                {
                    s.ProductDetailId,
                    s.Product.ProductName,
                    ProductImage = _db.ProductImages.Where(v => v.ProductImageStatus == true && v.ProductDetailId == s.ProductDetailId && v.productDetail.TopRated == true).FirstOrDefault().ProductImageName,
                    ProductPrice = s.ProductDetailUnitPrice,
                    s.DiscountPrice,
                    ProductColors = _db.productColor.Where(a => a.ProductDetailId == s.ProductDetailId).Select(a => new
                    {
                        a.ProductColorsId,
                        a.ColorName,
                        a.ColorCode
                    }).ToList(),
                    ProductSize = _db.productSize.Where(a => a.ProductDetailId == s.ProductDetailId).Select(a => new
                    {
                        a.ProductSizesId,
                        a.Size
                    }).ToList(),
                }).Take(5).ToList();

                var AllProducts = _db.ProductDetails.Include(a => a.Product).Where(s => s.ProductDetailStatus == true).Select(s => new
                {
                    s.ProductDetailId,
                    s.Product.ProductName,
                    ProductPrice = s.ProductDetailUnitPrice,
                    s.DiscountPrice,
                    ProductImage = _db.ProductImages.Where(v => v.ProductDetailId == s.ProductDetailId && v.ProductImageStatus == true).FirstOrDefault().ProductImageName,
                    ProductColors = _db.productColor.Where(a => a.ProductDetailId == s.ProductDetailId).Select(a => new
                    {
                        a.ProductColorsId,
                        a.ColorName,
                        a.ColorCode
                    }).ToList(),
                    ProductSize = _db.productSize.Where(a => a.ProductDetailId == s.ProductDetailId).Select(a => new
                    {
                        a.ProductSizesId,
                        a.Size
                    }).ToList(),
                }).Take(10).ToList();

                /*    var Promotions = _db.PromotionsProduct.Include(a=>a.promotions).Include(a=>a.productDetail).ThenInclude(a => a.Product).Where(s => s.promotions.Status == true && s.promotions.StartDate.Date>=DateTime.Now.Date).Select(s => new
                    {*/
                var PromotionsList = _db.promotions.Where(s => s.Status == true && s.StartDate.Date >= DateTime.Now.Date || s.EndDate>=DateTime.Now.Date).Select(s => new
                {
                    s.PromotionsId,
                    s.Title,
                    s.Image,
                  startDate=  s.StartDate.Date,
                  endDate=  s.EndDate.Date,
               /*    Products = _db.PromotionsProduct.Include(a => a.promotions).Include(a => a.productDetail).ThenInclude(a => a.Product).Where(c => c.PromotionsId==s.PromotionsId).Select(k => new
                    {
                       ProductId=k.ProductDetailId,
                       ProductName=   k.productDetail.Product.ProductName,
                   }).ToList()*/
                       
                   
                }).ToList();

                var items = new
                {
                    IsResponse,
                    IsStatus,
                    IsNextPageExist,
                    PromotionsList,
                    CategoriesList,
                    FeaturedProducts,
                    BestSellProducts,
                    TopRatedProducts,
                    AllProducts,
                    lati = "31.505355",
                    longi = "74.34995",
                    DeliveryBasePrice = 5.00
                };
                return Ok(items);
            }
            catch (Exception e)
            {
                var items = new
                {
                    IsStatus = false,
                    IsResponse = false
                };
                return Ok(items);
            }
        }
       
        public double getProductRatings(int prodId)
        {
            var vendorId = _db.ProductDetails.Where(a => a.ProductDetailId == prodId).FirstOrDefault().ReferenceUserId.ToString();

            var orderProd = _db.OrderProducts.Include(s => s.productDetail).Where(a => a.productDetail.ReferenceUserId.ToString() == vendorId);
            var TotalReviews2 = from rr in _db.ratingsReview
                                from op in orderProd
                                where (rr.OrderId == op.OrderId)
                                select rr;
            var TotalReviews = TotalReviews2.Distinct().Count();
            var TotalReviewsL = TotalReviews2.Distinct();

            double TotalRatings = 0.0;
            if (TotalReviews == 0)
            {
                TotalRatings = 0.0;
            }
            else
            {
                double sum = TotalReviewsL.Sum(s => s.Value);
                TotalRatings = sum / Convert.ToDouble(TotalReviews);
            }
            return (double)System.Math.Round(TotalRatings,1);
        }
        
             public int getProductReviews(int prodId)
        {
            var vendorId = _db.ProductDetails.Where(a => a.ProductDetailId == prodId).FirstOrDefault().ReferenceUserId.ToString();

            var orderProd = _db.OrderProducts.Include(s => s.productDetail).Where(a => a.productDetail.ReferenceUserId.ToString() == vendorId);
            var TotalReviews2 = from rr in _db.ratingsReview
                                from op in orderProd
                                where (rr.OrderId == op.OrderId)
                                select rr;
            var TotalReviews = TotalReviews2.Distinct().Count();
            return TotalReviews;
        }
        [HttpPost("GetProductList")]
        public IActionResult GetProductList([FromBody] ProductDetails pvm)// (int? startPrice, int? endPrice,string sort, int[] catArray, int pgNo) 
        {
            try
            {
                bool IsStatus = true;
                bool IsResponse = true;
                bool IsNextPageExist = true;
                int list = 10;
                //pagination
                int skip = list * (pvm.pgNo);
                int count = 0;
                //object vendorRefId = null;
                object VendorInfo = null;
                var TotalReviews = 0;
                double TotalRatings = 0.0;
                var vendorRefId = _db.Vendor.Where(s => s.VendorId == pvm.VendorId).FirstOrDefault();
                if (pvm.VendorId != null)
                {
                 
                    count = _db.ProductDetails.Where(a => a.ProductDetailStatus == true && a.ReferenceUserId.ToString() == vendorRefId.UserId).Count();


                    var vendorId = _db.ProductDetails.Where(a => a.ReferenceUserId.ToString()== vendorRefId.UserId).FirstOrDefault().ReferenceUserId.ToString();

                    var orderProd = _db.OrderProducts.Include(s => s.productDetail).Where(a => a.productDetail.ReferenceUserId.ToString() == vendorId);
                    var TotalReviews2 = from rr in _db.ratingsReview
                                        from op in orderProd
                                        where (rr.OrderId == op.OrderId)
                                        select rr;
                     TotalReviews = TotalReviews2.Distinct().Count();
                    var TotalReviewsL = TotalReviews2.Distinct();

                 
                    if (TotalReviews == 0)
                    {
                        TotalRatings = 0.0;
                    }
                    else
                    {
                        TotalRatings =Convert.ToDouble(TotalReviewsL.Sum(s => s.Value))/Convert.ToDouble(TotalReviews);
                       TotalRatings= (double)System.Math.Round(TotalRatings, 1);
                    }
                   
                    var VendorDetail = from ven in _db.Vendor
                                       join prod in _db.ProductDetails
                                       on ven.UserId equals prod.ReferenceUserId.ToString()
                                       where prod.ReferenceUserId.ToString() == vendorRefId.UserId
                                       select ven;
                    VendorInfo = new
                        {
                            VendorDetail.FirstOrDefault().BusinessName,
                            VendorDetail.FirstOrDefault().Address,
                            VendorDetail.FirstOrDefault().Image,
                            TotalReviews,
                            TotalRatings,
                            TotalOrders=orderProd.Count(),
                            TotalProducts= count
                    };
                    
                }
                else
                {
                    count = _db.ProductDetails.Where(a => a.ProductDetailStatus == true).Count();
                }
                int take = count - skip;
                if (take <= list)
                {
                    IsNextPageExist = false;
                }
                skip = skip > count ? 0 : skip;
                take = take > list ? list : take < 0 ? 0 : take;

              
                if (pvm.VendorId != null)
                {
                    //first get all products 
                    var productList1 = _db.ProductCategories.Include(a => a.productDetail).ThenInclude(a => a.Product).Where(s => s.productDetail.ProductDetailStatus == true && s.productDetail.ReferenceUserId.ToString() == vendorRefId.UserId).ToList();//_db.ProductDetails.Include(a => a.Product).Where(s => s.ProductDetailStatus == true && (pvm.startPrice!=null && pvm.startPrice!=0 )? s.ProductDetailUnitPrice >= pvm.startPrice : true && (pvm.endPrice != null && pvm.endPrice != 0) ? s.ProductDetailUnitPrice <= pvm.endPrice : true).ToList();
                                                                                                                                                                                       //filter all productlist for categories
                    if (pvm.CategoryList.Count() != 0)
                        productList1 = productList1.Where(a => pvm.CategoryList.Any(b => b.cat_arr == a.CategoryId)).ToList();
                    //filter all productlist for price range & get selective data
                    var allProducts1 = productList1.Where(a => a.productDetail.ProductDetailUnitPrice >= pvm.startPrice && a.productDetail.ProductDetailUnitPrice <= pvm.endPrice && a.productDetail.ReferenceUserId.ToString()==vendorRefId.UserId).Select(s => new
                    {
                        s.ProductDetailId,
                        s.productDetail.Product.ProductName,
                        ProductPrice = s.productDetail.ProductDetailUnitPrice,
                        s.productDetail.DiscountPrice,
                        ProductImage = _db.ProductImages.Where(s => s.ProductDetailId == s.ProductDetailId && s.ProductImageStatus == true).FirstOrDefault().ProductImageName,
                        ProductColors = _db.productColor.Where(a => a.ProductDetailId == s.ProductDetailId).Select(a => new
                        {
                            a.ProductColorsId,
                            a.ColorName,
                            a.ColorCode
                        }).ToList(),
                        ProductSize = _db.productSize.Where(a => a.ProductDetailId == s.ProductDetailId).Select(a => new
                        {
                            a.ProductSizesId,
                            a.Size
                        }).ToList(),
                        Ratings = TotalReviews,
                        Reviews = TotalRatings,
                    }).Distinct().ToList();
                    if (pvm.sort != null && pvm.sort.ToLower().Equals("htl"))
                    {
                        allProducts1 = allProducts1.OrderByDescending(a => a.ProductPrice).ToList();
                    }
                    if (pvm.sort != null && pvm.sort.ToLower().Equals("lth"))
                    {
                        allProducts1 = allProducts1.OrderBy(a => a.ProductPrice).ToList();
                    }
                    var item1 = new
                    {
                        IsResponse,
                        IsStatus,
                        IsNextPageExist,
                        VendorInfo,
                        ProductList = allProducts1.Skip(skip).Take(take).ToList()
                    };
                    return Ok(item1);
                }
                //getting reviews and ratings
                //first get all products 
                var productList = _db.ProductCategories.Include(a => a.productDetail).ThenInclude(a => a.Product).Where(s => s.productDetail.ProductDetailStatus == true).ToList();//_db.ProductDetails.Include(a => a.Product).Where(s => s.ProductDetailStatus == true && (pvm.startPrice!=null && pvm.startPrice!=0 )? s.ProductDetailUnitPrice >= pvm.startPrice : true && (pvm.endPrice != null && pvm.endPrice != 0) ? s.ProductDetailUnitPrice <= pvm.endPrice : true).ToList();
                //filter all productlist for categories
                if (pvm.CategoryList.Count() != 0)
                    productList = productList.Where(a => pvm.CategoryList.Any(b => b.cat_arr == a.CategoryId)).ToList();
                //filter all productlist for price range & get selective data
                var allProducts = productList.Where(a => a.productDetail.ProductDetailUnitPrice >= pvm.startPrice && a.productDetail.ProductDetailUnitPrice <= pvm.endPrice).Select(s => new
                {
                    s.ProductDetailId,
                    s.productDetail.Product.ProductName,
                    ProductPrice = s.productDetail.ProductDetailUnitPrice,
                    s.productDetail.DiscountPrice,
                    ProductImage = _db.ProductImages.Where(s => s.ProductDetailId == s.ProductDetailId && s.ProductImageStatus == true).FirstOrDefault().ProductImageName,
                    ProductColors = _db.productColor.Where(a => a.ProductDetailId == s.ProductDetailId).Select(a => new
                    {
                        a.ProductColorsId,
                        a.ColorName,
                        a.ColorCode
                    }).ToList(),
                    ProductSize = _db.productSize.Where(a => a.ProductDetailId == s.ProductDetailId).Select(a => new
                    {
                        a.ProductSizesId,
                        a.Size
                    }).ToList(),
                    Ratings = getProductRatings(s.ProductDetailId),
                    Reviews = getProductReviews(s.ProductDetailId),
                }).Distinct().ToList();
                if (pvm.sort != null && pvm.sort.ToLower().Equals("htl"))
                {
                    allProducts=allProducts.OrderByDescending(a => a.ProductPrice).ToList();
                }
                if (pvm.sort != null && pvm.sort.ToLower().Equals("lth"))
                {
                    allProducts = allProducts.OrderBy(a => a.ProductPrice).ToList();
                }
                var item = new
                {
                    IsResponse,
                    IsStatus,
                    IsNextPageExist,
                    VendorInfo,
                    ProductList = allProducts.Skip(skip).Take(take).ToList()
                };
                return Ok(item);
                ////pagination
                ////category Array
                //if (pvm.CategoryList.Count() != 0)
                //{
                //    var categoryList = _db.Categories.Where(s=>s.CategoryStatus==true).ToList();


                //    List<ProductCategory> ProductCategoryList = new List<ProductCategory>();
                //    foreach (var id in pvm.CategoryList)
                //    {
                //        if(!_db.Categories.Any(c=>c.CategoryId==id.cat_arr))
                //        {
                //            var item1 = new
                //            {
                //                IsResponse,
                //                IsStatus,
                //                Message="Category Id does not exist"
                //            };
                //            return Ok(item1);
                //        }

                //        ProductCategoryList.AddRange(_db.ProductCategories.Include(a => a.productDetail).ThenInclude(a => a.Product).Where(a => a.CategoryId == id.cat_arr).ToList());
                //    }
                //   var productList= ProductCategoryList.Where(s => s.productDetail.ProductDetailStatus == true).Select(s => new
                //    {
                //        s.ProductDetailId,
                //        s.productDetail.Product.ProductName,
                //        ProductPrice = s.productDetail.ProductDetailUnitPrice,
                //        ProductImage = _db.ProductImages.Where(s => s.ProductDetailId == s.ProductDetailId && s.ProductImageStatus == true).FirstOrDefault().ProductImageName,
                //        Ratings = "null",
                //        Reviews = "null"

                //    }).Distinct().ToList();
                //    var item = new
                //    {
                //        IsResponse,
                //        IsStatus,
                //        IsNextPageExist,
                //        ProductList = productList.Skip(skip).Take(take).ToList()
                //    };
                //    return Ok(item);

                //}
                ////price range
                //if (pvm.startPrice != null && pvm.endPrice != null && pvm.startPrice != 0 && pvm.endPrice != 0)
                //{
                //    var ProductListPriceRange = _db.ProductDetails.Include(a => a.Product).Where(s => s.ProductDetailStatus == true && s.ProductDetailUnitPrice >= pvm.startPrice && s.ProductDetailUnitPrice <= pvm.endPrice).Select(s => new
                //    {
                //        s.ProductDetailId,
                //        s.Product.ProductName,
                //        ProductPrice = s.ProductDetailUnitPrice,
                //        ProductImage = _db.ProductImages.Where(s => s.ProductDetailId == s.ProductDetailId && s.ProductImageStatus == true).FirstOrDefault().ProductImageName,
                //        Ratings = "null",
                //        Reviews = "null"

                //    }).ToList();
                //    var item = new
                //    {
                //        IsResponse,
                //        IsStatus,
                //        IsNextPageExist,
                //        ProductList = ProductListPriceRange.Skip(skip).Take(take).ToList()
                //    };
                //    return Ok(item);

                //}
                ////sort
                //if (pvm.sort.ToLower().Equals("htl"))
                //{
                //    var ProductListPriceRange = _db.ProductDetails.Include(a => a.Product).Where(s => s.ProductDetailStatus == true).Select(s => new
                //    {
                //        s.ProductDetailId,
                //        s.Product.ProductName,
                //        ProductPrice = s.ProductDetailUnitPrice,
                //        ProductImage = _db.ProductImages.Where(s => s.ProductDetailId == s.ProductDetailId && s.ProductImageStatus == true).FirstOrDefault().ProductImageName,
                //        Ratings = "null",
                //        Reviews = "null"

                //    }).OrderByDescending(a => a.ProductPrice).ToList();
                //    var item = new
                //    {
                //        IsResponse,
                //        IsStatus,
                //        IsNextPageExist,
                //        ProductList = ProductListPriceRange.Skip(skip).Take(take).ToList()
                //    };
                //    return Ok(item);

                //}
                //if (pvm.sort.ToLower().Equals("lth"))
                //{
                //    var ProductListPriceRange = _db.ProductDetails.Include(a => a.Product).Where(s => s.ProductDetailStatus == true).Select(s => new
                //    {
                //        s.ProductDetailId,
                //        s.Product.ProductName,
                //        ProductPrice = s.ProductDetailUnitPrice,
                //        ProductImage = _db.ProductImages.Where(s => s.ProductDetailId == s.ProductDetailId && s.ProductImageStatus == true).FirstOrDefault().ProductImageName,
                //        Ratings = "null",
                //        Reviews = "null"

                //    }).OrderBy(a => a.ProductPrice).ToList();
                //    var item = new
                //    {
                //        IsResponse,
                //        IsStatus,
                //        IsNextPageExist,
                //        ProductList = ProductListPriceRange.Skip(skip).Take(take).ToList()
                //    };
                //    return Ok(item);

                //}

                var items = new
                {
                    IsResponse = true,
                    IsStatus = false
                };
                return Ok(items);
            }
            catch (Exception e)
            {
                var items = new
                {
                    IsStatus = false,
                    IsResponse = false
                };
                return Ok(items);
            }
        }
        [HttpPost("SetCustomerReviews")]
        public IActionResult SetCustomerReviews([FromBody] RatingsReviews rvm)   //?
        {
            bool IsResponse = true;
            bool IsStatus = true;
            var ProductName = "";
            try
            {
                if (ModelState.IsValid)
                {
                    if (!_db.Orders.Any(a => a.OrderId == rvm.OrderId)) 
                    {
                        var show = new
                        {
                            IsResponse = true,
                            IsStatus = false,

                        };
                        return Ok(show);

                    }
                    rvm.OrderId = rvm.OrderId;
                    rvm.Value = rvm.Value;
                    rvm.Reviews = rvm.Reviews;
                    rvm.Date = DateTime.Now;
                    _db.ratingsReview.Add(rvm);
                    _db.SaveChanges();

                    var order = _db.Orders.Where(a => a.OrderId == rvm.OrderId ).FirstOrDefault();
                    order.isCustomerReviewed = true;
                    _db.Orders.Update(order);
                    _db.SaveChanges();
                //    ProductName = _db.ProductDetails.Include(s => s.Product).Where(a => a.ProductDetailId == rvm.ProductDetailId).FirstOrDefault().Product.ProductName;
                }
                else
                {
                    var show = new
                    {
                        IsResponse = true,
                        IsStatus = false,

                    };
                    return Ok(show);
                }
                var item = new
                {
                    IsResponse,
                    IsStatus,
                    Reviews = new
                    {
                      //  Name = ProductName,
                        Value = rvm.Value,
                        Reviews = rvm.Reviews
                    }
                };
                return Ok(item);
            }
            catch (Exception e)
            {
                var item = new
                {
                    IsResponse = false,
                    IsStatus = false
                };
                return BadRequest(item);
            }
        }
            [HttpGet("GetVendorProducts")]
        public IActionResult GetVendorProducts(int vendorId)  //? 3
        {
            try
            {
                if (!_db.Vendor.Any(a => a.VendorId == vendorId))
                {
                    var show = new
                    {
                        IsResponse = true,
                        IsStatus = false,

                    };
                    return Ok(show);

                }
                bool IsStatus = true;
                bool IsResponse = true;
                var Products = from pd in _db.ProductDetails.Where(a => a.ProductDetailStatus == true)
                               join ven in _db.Vendor
                               on pd.ReferenceUserId.ToString() equals ven.UserId
                               where ven.VendorId == vendorId
                               select new
                               {
                                   ProductDetailId=pd.ProductDetailId,
                                   ProductName=pd.Product.ProductName,
                                   ProductImage = _db.ProductImages.Where(v => v.ProductImageStatus == true && v.ProductDetailId == pd.ProductDetailId && v.productDetail.FeatureProduct == true).FirstOrDefault().ProductImageName,
                                   ProductPrice = pd.ProductDetailUnitPrice,
                                   DiscountPrice=pd.DiscountPrice,
                                   ProductColors = _db.productColor.Where(a => a.ProductDetailId == pd.ProductDetailId).Select(a => new
                                   {
                                       a.ProductColorsId,
                                       a.ColorName,
                                       a.ColorCode
                                   }).ToList(),
                                   ProductSize = _db.productSize.Where(a => a.ProductDetailId == pd.ProductDetailId).Select(a => new
                                   {
                                       a.ProductSizesId,
                                       a.Size
                                   }).ToList(),
                               };
                /*     var Products = _db.ProductDetails.Include(a => a.Product).Where(s => s.FeatureProduct == true && s.ProductDetailStatus == true).Select(s => new
                     {
                         s.ProductDetailId,
                         s.Product.ProductName,
                         ProductImage = _db.ProductImages.Where(v => v.ProductImageStatus == true && v.ProductDetailId == s.ProductDetailId && v.productDetail.FeatureProduct == true).FirstOrDefault().ProductImageName,
                         ProductPrice = s.ProductDetailUnitPrice,
                         s.DiscountPrice,
                     }).ToList();*/
                return Ok(new { IsStatus, IsResponse, Products });
              
            }
            catch (Exception e)
            {
                return Ok(new { IsStatus = false, IsResponse = false });
            }
        }
       /* [HttpGet("SellerInfo")]
        public IActionResult SellerInfo(int vendorId)
        {
            bool IsStatus = true;
            bool IsResponse = true;
            try
            {
                if (!_db.Vendor.Any(a => a.VendorId == vendorId))
                {
                    var show = new
                    {
                        IsResponse = true,
                        IsStatus = false,

                    };
                    return Ok(show);
                }
                //get rating reviews
                var venRefId = _db.Vendor.Where(s => s.VendorId == vendorId).FirstOrDefault().UserId;
                var vendorProd = _db.ProductDetails.Where(a => a.ReferenceUserId.ToString() == venRefId).ToList();

                var orderProd = _db.OrderProducts.Include(s => s.productDetail).Where(a => a.productDetail.ReferenceUserId.ToString() == venRefId);
                var TotalReviews2 = from rr in _db.ratingsReview
                                    from op in orderProd
                                    where (rr.OrderId == op.OrderId)
                                    select rr;
                var TotalReviews = TotalReviews2.Distinct().Count();
                var TotalReviewsL = TotalReviews2.Distinct();

                double TotalRatings = 0.0;
                if (TotalReviews == 0)
                {
                    TotalRatings = 0.0;
                }
                else
                {
                    TotalRatings = TotalReviewsL.Sum(s => s.Value) / TotalReviews;
                }
                object ReviewsList = null;
                ReviewsList = TotalReviewsL.Select(s => new
                {
                    s.RatingsReviewsId,
                    s.Value,
                    s.Reviews,
                    CustomerId = _db.Orders.Where(a => a.OrderId == s.OrderId).FirstOrDefault().CustomerId,
                    CustomerName = _db.Orders.Include(d => d.Customer).Where(a => a.OrderId == s.OrderId).FirstOrDefault().Customer.FirstName + " " + _db.Orders.Where(a => a.OrderId == s.OrderId).FirstOrDefault().Customer.LastName,
                    Image = _db.Orders.Include(d => d.Customer).Where(a => a.OrderId == s.OrderId).FirstOrDefault().Customer.Image,
                }).ToList();
                //

                var VendorDetail = from ven in _db.Vendor
                                   join prod in _db.ProductDetails
                                   on ven.UserId equals prod.ReferenceUserId.ToString()
                                   where prod.ProductDetailId == productId
                                   select ven;
                object Vendor = null;
                if (VendorDetail.Count() == 0)
                {
                    Vendor = null;
                }
                else
                {
                    Vendor = new
                    {
                        VendorDetail.FirstOrDefault().VendorId,
                        VendorDetail.FirstOrDefault().VendorName,
                        VendorDetail.FirstOrDefault().Address,
                        VendorDetail.FirstOrDefault().Image,
                        VendorDetail.FirstOrDefault().IsVerified,
                        TotalReviews,
                        TotalRatings
                    };
                }

                return Ok();
            }
            catch (Exception e)
            {
                var items = new
                {
                    IsStatus = false,
                    IsResponse = false
                };
                return Ok(items);
            }
        }
         */   [HttpGet("GetVendorReviews")]
        public IActionResult GetVendorReviews(int productId)
        {
            bool IsStatus = true;
            bool IsResponse = true;

            if (!_db.ProductDetails.Any(a => a.ProductDetailId == productId))
            {
                var show = new
                {
                    IsResponse = true,
                    IsStatus = false,

                };
                return Ok(show);
            }
                var vendorId = _db.ProductDetails.Where(a => a.ProductDetailId == productId).FirstOrDefault().ReferenceUserId.ToString();

            var orderProd = _db.OrderProducts.Include(s=>s.productDetail).Where(a => a.productDetail.ReferenceUserId.ToString() == vendorId);
            var TotalReviews2 = from rr in _db.ratingsReview
                               from op in orderProd
                               where (rr.OrderId == op.OrderId)
                               select rr;
            var TotalReviews = TotalReviews2.Distinct().Count();
            var TotalReviewsL = TotalReviews2.Distinct();

            double TotalRatings = 0.0;
            if (TotalReviews == 0)
            {
                TotalRatings = 0.0;
            }
            else
            {
                double sum = TotalReviewsL.Sum(s => s.Value);
                TotalRatings = sum / Convert.ToDouble(TotalReviews);
               TotalRatings= (double)System.Math.Round(TotalRatings, 1);
            }
            object ReviewsList = null;
                ReviewsList = TotalReviewsL.Select(s => new
                {
                    s.RatingsReviewsId,
                    s.Value,
                    s.Reviews,
                    CustomerId=_db.Orders.Where(a=>a.OrderId==s.OrderId).FirstOrDefault().CustomerId,
                    CustomerName = _db.Orders.Include(d=>d.Customer).Where(a => a.OrderId == s.OrderId).FirstOrDefault().Customer.FirstName+" " + _db.Orders.Where(a => a.OrderId == s.OrderId).FirstOrDefault().Customer.LastName,
                    Image = _db.Orders.Include(d => d.Customer).Where(a => a.OrderId == s.OrderId).FirstOrDefault().Customer.Image,
                }).ToList();

                var RatingsReviews = new
                {
                    IsStatus,
                    IsResponse,
                    TotalReviews,
                    TotalRatings,
                    ReviewsList
                };
                return Ok(RatingsReviews);
            
        }
        [HttpGet("GetProductDetail")]
        public IActionResult GetProductDetail(int productId)
        {
            try
            {
                bool IsStatus = true;
                bool IsResponse = true;
                if (!_db.ProductDetails.Any(s => s.ProductDetailId == productId))
                {
                    var item = new
                    {
                        IsResponse,
                        IsStatus = false
                    };
                    return Ok(item);
                }
                //get rating reviews
                var vendorId = _db.ProductDetails.Where(a => a.ProductDetailId == productId).FirstOrDefault().ReferenceUserId.ToString();

                var orderProd = _db.OrderProducts.Include(s => s.productDetail).Where(a => a.productDetail.ReferenceUserId.ToString() == vendorId);
                var TotalReviews2 = from rr in _db.ratingsReview
                                    from op in orderProd
                                    where (rr.OrderId == op.OrderId)
                                    select rr;
                var TotalReviews = TotalReviews2.Distinct().Count();
                var TotalReviewsL = TotalReviews2.Distinct();

                double TotalRatings = 0.0;
                if (TotalReviews == 0)
                {
                    TotalRatings = 0.0;
                }
                else
                {
                    double sum = TotalReviewsL.Sum(s => s.Value);
                  TotalRatings =sum / Convert.ToDouble(TotalReviews);
                    TotalRatings=(double)System.Math.Round(TotalRatings, 1);
                }
                object ReviewsList = null;
                ReviewsList = TotalReviewsL.Select(s => new
                {
                    s.RatingsReviewsId,
                    s.Value,
                    s.Reviews,
                    CustomerId = _db.Orders.Where(a => a.OrderId == s.OrderId).FirstOrDefault().CustomerId,
                    CustomerName = _db.Orders.Include(d => d.Customer).Where(a => a.OrderId == s.OrderId).FirstOrDefault().Customer.FirstName + " " + _db.Orders.Where(a => a.OrderId == s.OrderId).FirstOrDefault().Customer.LastName,
                    Image = _db.Orders.Include(d => d.Customer).Where(a => a.OrderId == s.OrderId).FirstOrDefault().Customer.Image,
                }).ToList();
                //

                var VendorDetail = from ven in _db.Vendor
                             join prod in _db.ProductDetails
                             on ven.UserId equals prod.ReferenceUserId.ToString()
                             where prod.ProductDetailId==productId
                             select ven;
                object Vendor = null;
                if (VendorDetail.Count() == 0)
                {
                    Vendor = null;
                }
                else
                {
                     Vendor = new
                    {
                        VendorDetail.FirstOrDefault().VendorId,
                        VendorDetail.FirstOrDefault().VendorName,
                         VendorDetail.FirstOrDefault().Address,
                         VendorDetail.FirstOrDefault().Image,
                        VendorDetail.FirstOrDefault().IsVerified,
                         TotalReviews,
                         TotalRatings
                     };
                }
               
               

                var ProductDetail = _db.ProductDetails.Include(a => a.Product).Where(a => a.ProductDetailId == productId).Select(s => new
                {
                    s.ProductDetailId,
                    s.Product.ProductName,
                   ProductPrice= s.ProductDetailUnitPrice,
                   s.DiscountPrice,
                   s.GuaranteePolicy,
                    s.ProductDetailDescription,
                    ProductImage = _db.ProductImages.Where(a => a.ProductDetailId == s.ProductDetailId && a.ProductImageStatus == true).Select(a => new
                    {
                        a.ProductImageName
                    }).ToList(),
                    ProductColors = _db.productColor.Where(a => a.ProductDetailId == s.ProductDetailId).Select(a => new
                    {
                        a.ProductColorsId,
                        a.ColorName,
                        a.ColorCode
                    }).ToList(),
                    ProductSize = _db.productSize.Where(a => a.ProductDetailId == s.ProductDetailId).Select(a => new
                    {
                        a.ProductSizesId,
                        a.Size
                    }).ToList(),
                    ProductBundles = _db.ProductBundles.Where(t => t.ProductDetailId == s.ProductDetailId && t.ProductBundleStatus == true).Select(a => new
                     {
                         a.ProductBundleId,
                        Price= a.ProductBundlePrice,
                        Quantity=a.ProductBundleQuantity,
                        Unit= a.ProductBundleUnit,
                        DiscountPrice = a.DiscountPrice==null?0:a.DiscountPrice,
                        ProductSizesId= a.SizeId
                        
                     }).ToList()
                }).FirstOrDefault();
                
              /*  var RatingsReviews = new
                {
                    IsStatus,
                    IsResponse,
                    TotalReviews,
                    TotalRatings,
                    ReviewsList
                };*/
                //
                var FeaturedProducts = _db.ProductDetails.Include(a => a.Product).Where(s => s.FeatureProduct == true && s.ProductDetailStatus == true).Select(s => new
                {
                    s.ProductDetailId,
                    s.Product.ProductName,
                    ProductImage = _db.ProductImages.Where(a => a.ProductImageStatus == true && a.ProductDetailId == s.ProductDetailId && a.productDetail.FeatureProduct == true).FirstOrDefault().ProductImageName,
                    ProductPrice = s.ProductDetailUnitPrice,
                        s.DiscountPrice,
                        //ProductColors = _db.productColor.Where(a => a.ProductDetailId == s.ProductDetailId).Select(a => new
                        //{
                        //    a.ProductColorsId,
                        //    a.ColorName,
                        //    a.ColorCode
                        //}).ToList(),
                        //ProductSize = _db.productSize.Where(a => a.ProductDetailId == s.ProductDetailId).Select(a => new
                        //{
                        //    a.ProductSizesId,
                        //    a.Size
                        //}).ToList(),
                    }).Take(5).ToList();
                var items = new
                {
                    IsResponse,
                    IsStatus,
                    ProductDetail,
                    Vendor,
                    ReviewsList,
                    FeaturedProducts
                };
                return Ok(items);
            }
            catch (Exception e)
            {
                var items = new
                {
                    IsStatus = false,
                    IsResponse = false
                };
                return Ok(items);
    }
}
        [HttpPost("ProductInquiryForm")]
        public IActionResult ProductInquiryForm(ProductInquiry ProdIn)
        {

            bool IsResponse = true;
            bool IsStatus = true;
            var ProductName = "";
            try
            {
                if (ModelState.IsValid)
                {
                    if ((!_db.ProductDetails.Any(a => a.ProductDetailId == ProdIn.ProductDetailId)) || (!_db.Customers.Any(a => a.CustomerId == ProdIn.CustomerId)))
                    {
                        var show = new
                        {
                            IsResponse = true,
                            IsStatus = false,

                        };
                        return Ok(show);

                    }
                    ProdIn.Status = true;
                    ProdIn.ProductInquiryDate = DateTime.Now;
                    ProdIn.ProductInquiryStatus="pending";
                    ProdIn.Status = true;
                    _db.ProductInquiries.Add(ProdIn);
                     _db.SaveChanges();
                    ProductName = _db.ProductDetails.Include(s => s.Product).Where(a => a.ProductDetailId == ProdIn.ProductDetailId).FirstOrDefault().Product.ProductName;
                }
                else
                {
                    var show = new
                    {
                        IsResponse = true,
                        IsStatus = false,

                    };
                    return Ok(show);
                }
                var item = new
                {
                    IsResponse,
                    IsStatus,
                    ProductInquiry = new
                    {
                       Name= ProductName,// ProdIn.productDetail.Product.ProductName,
                      Quantity=  ProdIn.ProductInquiryQuantity,
                        Description= ProdIn.ProductInquiryDescription,
                        Status=ProdIn.ProductInquiryStatus
                    }
                };
                return Ok(item);
            }
            catch (Exception e)
            {
                var item = new
                {
                    IsResponse = false,
                    IsStatus = false
                };
                return BadRequest(item);
            }
        }
        [HttpGet("UserFavouriteProduct")]
        public IActionResult UserFavouriteProduct(int userId, int productId)
        {
         
        try
            {
                bool IsResponse = true;
                bool IsStatus = true;
                bool IsFavourite = true;
                if ((!_db.ProductDetails.Any(a => a.ProductDetailId == productId)) || (!_db.Customers.Any(a => a.CustomerId == userId)))
                    {

                        var show = new
                        {
                            IsResponse = true,
                            IsStatus = false,

                        };
                        return Ok(show);

                    }

                if (_db.FavoriteProducts.Any(a => a.ProductDetailId == productId && a.CustomerId == userId))
                {
                    var fvrtProd = _db.FavoriteProducts.Where(a => a.ProductDetailId == productId && a.CustomerId == userId).FirstOrDefault();
                    fvrtProd.FavoriteProductStatus = !(fvrtProd.FavoriteProductStatus);
                    _db.FavoriteProducts.Update(fvrtProd);
                    _db.SaveChanges();
                    if (fvrtProd.FavoriteProductStatus == false)
                    {
                        IsFavourite = false;
                    }
                    else
                    {
                        IsFavourite = true;
                    }

                    var item = new
                    {
                        IsResponse ,
                        IsStatus ,
                        IsFavourite
                    };
                    return Ok(item);
                }
                FavoriteProduct fp = new FavoriteProduct();
                fp.ProductDetailId = productId;
                fp.CustomerId = userId;
                fp.FavoriteProductStatus = true;
                _db.FavoriteProducts.Add(fp);
                _db.SaveChanges();

                if (fp.FavoriteProductStatus == false)
                {
                    IsFavourite = false;
                }
                else
                {
                    IsFavourite = true;
                }
                var items = new
                {
                    IsResponse,
                    IsStatus,
                    IsFavourite
                };
                return Ok(items);
            }
            catch (Exception e)
            {
                var item = new
                {
                    IsResponse=false,
                    IsStatus=false,
                };
                return BadRequest(item);
            }
          
        }

        [HttpGet("GetFavouriteProducts")]
        public IActionResult GetFavouriteProducts(int userId)  //pg 41
        {
            bool IsResponse = true;
            bool IsStatus = true;
            if (!_db.Customers.Any(s => s.CustomerId == userId))
            {
                var item = new
                {
                    IsResponse,
                    IsStatus=false,
                };
                return Ok(item);
            }
            try
            {
                var favouriteProducts = _db.FavoriteProducts.Include(a => a.productDetail).ThenInclude(a=>a.Product).Where(s => s.CustomerId == userId && s.FavoriteProductStatus == true).Select(sd => new 
                {
                    ProductDetailId = sd.ProductDetailId,
                    ProductName = sd.productDetail.Product.ProductName,
                   ProductPrice= sd.productDetail.ProductDetailUnitPrice,
                    ProductImage = _db.ProductImages.Where(s => s.ProductDetailId == sd.ProductDetailId && s.ProductImageStatus == true).FirstOrDefault().ProductImageName

                }).Distinct().ToList();
                if (favouriteProducts.Count() == 0)
                {
                    favouriteProducts = null;
                    var item = new
                    {
                        IsResponse,
                        IsStatus,
                        FavouriteProducts = favouriteProducts
                    };

                    return Ok(item);
                }
                var fvrtList = new
                {
                    IsResponse,
                    IsStatus ,
                    FavouriteProducts = favouriteProducts
                };

                return Ok(fvrtList);
            }
            catch (Exception e)
            {
                var profile = new
                {
                    IsResponse = false,
                    IsStatus = false,

                };

                return BadRequest(profile);
            }


        }
        [HttpPost("PlaceOrder")]
        public async Task<IActionResult> PlaceOrder([FromBody] OrderVM ovm)
        {
            bool IsResponse = true;
            bool IsStatus = true;
            try
            {
                if (ModelState.IsValid)
                {
                    if(!_db.Customers.Any(d=>d.CustomerId==ovm.order.CustomerId))
                    {
                        var show = new
                        {
                            IsResponse = true,
                            IsStatus = false,
                            Msg="Customer Id does not exist"
                        };
                        return Ok(show);
                    }
                    if (ovm.orderProduct.Count() == 0)
                    {
                        var show = new
                        {
                            IsResponse = true,
                            IsStatus = false,
                            Msg = "No product entered"
                        };
                        return Ok(show);
                    }
                    if (!_db.ProductDetails.Any(d => d.ProductDetailId == ovm.orderProduct.FirstOrDefault().ProductDetailId))
                    {
                        var show = new
                        {
                            IsResponse = true,
                            IsStatus = false,
                            Msg = "product Id does not exist"
                        };
                        return Ok(show);
                    }
                  

                    var details = ovm.order;
                    details.OrderStatus = "processing";
                    if (_db.Orders.Count() == 0)
                    {
                        details.OrderNo = "1111";
                    }
                    else
                    {
                        var lastProduct = _db.Orders.ToList().LastOrDefault().OrderNo;
                        details.OrderNo =Convert.ToString(Convert.ToInt32(lastProduct)+1);
                    }
                    details.OrderDate = DateTime.Now.Date;
                    details.Status = true;
                    details.OrderId = 0;

                    _db.Orders.Add(details);
                    await _db.SaveChangesAsync();

                    foreach (var item1 in ovm.orderProduct)
                    {
                        item1.OrderId = details.OrderId;
                        _db.OrderProducts.Add(item1);
                    }
                    await _db.SaveChangesAsync();
                    //var customer = _db.Customers.Find(details.CustomerId);
                    //var msg = "Dear " + customer.FirstName + ",\n" + "Your Order No : " + details.OrderNo + " is successfully placed. \nThanks to use our app!!";
                    //SendNotification(new { Msg = msg }, "client");

                    var item = new
                    {
                        IsResponse,
                        IsStatus,
                        details.OrderId,
                        details.OrderNo
                    };
                    return Ok(item);

                }
                else
                {
                    var show = new
                    {
                        IsResponse = true,
                        IsStatus = false,

                    };
                    return Ok(show);
                }
               
            }
            catch (Exception e)
            {
                var item = new
                {
                    IsResponse = false,
                    IsStatus = false
                };
                return BadRequest(item);
            }
        }
        [HttpGet("GetPromoCodeStatus")]
        public IActionResult GetPromoCodeStatus(string promoCode, int customerId)  //pg 41
        {
            bool IsResponse = true;
            bool IsStatus = true;
            if ((!_db.promoCodes.Any(s => s.promoCode == promoCode))|| (!_db.Customers.Any(s => s.CustomerId == customerId)))
            {
                var item = new
                {
                    IsResponse,
                    IsStatus = false,
                };
                return Ok(item);
            }
            try
            {
              /*  if(!_db.CustomerPromoCodeLogs.Include(a=>a.PromoCode).Any(s => s.PromoCode.promoCode== promoCode && s.CustomerId == customerId))
                {
                    CustomerPromoCodeLog log = new CustomerPromoCodeLog();
                    var getId = _db.promoCodes.Where(a => a.promoCode == promoCode).FirstOrDefault();
                    log.CustomerId = customerId;
                    log.PromoCodesId = getId.PromoCodesId;
                    log.Date = DateTime.Now;
                    _db.CustomerPromoCodeLogs.Add(log);
                    _db.SaveChanges();
                    var item = new
                    {
                        IsResponse,
                        IsStatus,
                        Msg = "Promo Code Applied"
                    };

                    return Ok(item);
                }*/
                var getLimit = _db.promoCodes.Where(a => a.promoCode == promoCode).FirstOrDefault();
                var getData= _db.CustomerPromoCodeLogs.Include(a => a.PromoCode).Where(s => s.PromoCode.promoCode == promoCode && s.CustomerId==customerId).ToList();
              if(getData.Count()<getLimit.NoOfUsage && getLimit.StartDate.Date<=DateTime.Now.Date && getLimit.EndDate>=DateTime.Now.Date)
                {
                    CustomerPromoCodeLog log = new CustomerPromoCodeLog();
                    log.CustomerId = customerId;
                    log.PromoCodesId = getLimit.PromoCodesId;
                    log.Date = DateTime.Now;
                    _db.CustomerPromoCodeLogs.Add(log);
                    _db.SaveChanges();
                    var item = new
                    {
                        IsResponse,
                        IsStatus,
                        Msg = "Promo Code Applied",
                        DiscountAmount=getLimit.discountPrice
                    };

                    return Ok(item);
                }
               else
                {
                    var item = new
                    {
                        IsResponse,
                        IsStatus,
                        Msg = "Promo Code Access Denied"
                    };

                    return Ok(item);
                }

            }
            catch (Exception e)
            {
                var profile = new
                {
                    IsResponse = false,
                    IsStatus = false,

                };

                return BadRequest(profile);
            }


        }
      
        [HttpGet("GetPromotionList")]
        public IActionResult GetPromotionList()
        {

            try
            {
                bool IsResponse = true;
                bool IsStatus = true;

                var promotionList = _db.promotions.Where(a => a.Status == true).Select(s => new
                {
                    s.PromotionsId,
                    s.Title,
                    s.Image,
                    s.StartDate,
                    s.EndDate
                }).ToList();
                var items = new
                {
                    IsResponse,
                    IsStatus,
                    promotionList
                };
                return Ok(items);
            }
            catch (Exception e)
            {
                var item = new
                {
                    IsResponse = false,
                    IsStatus = false,
                };
                return BadRequest(item);
            }

        }
        [HttpGet("GetPromotionProducts")]
        public IActionResult GetPromotionProducts(int promotionId)
        {

            try
            {
                bool IsResponse = true;
                bool IsStatus = true;
                if (!_db.promotions.Any(s => s.PromotionsId == promotionId))
                {
                    var item = new
                    {
                        IsResponse,
                        IsStatus = false,
                    };
                    return Ok(item);
                }
                var promotionProducts = _db.PromotionsProduct.Include(a=>a.promotions).Include(a=>a.productDetail).ThenInclude(a=>a.Product).Where(a => a.Status == true && a.PromotionsId== promotionId).Select(s => new
                {
                    s.ProductDetailId,
                    s.productDetail.Product.ProductName,
                    ProductPrice = s.productDetail.ProductDetailUnitPrice,
                    ProductImage = _db.ProductImages.Where(v => v.ProductDetailId == s.ProductDetailId && v.ProductImageStatus == true).FirstOrDefault().ProductImageName,

                    /* productId = s.ProductDetailId,
                     s.productDetail.Product.ProductName*/
                }).ToList();
                var items = new
                {
                    IsResponse,
                    IsStatus,
                    promotionProducts
                };
                return Ok(items);
            }
            catch (Exception e)
            {
                var item = new
                {
                    IsResponse = false,
                    IsStatus = false,
                };
                return BadRequest(item);
            }

        }
        [HttpGet("ChangeOrderStatus")]
        public IActionResult ChangeOrderStatus(int orderId,string status)  //pg 41
        {
            bool IsResponse = true;
            bool IsStatus = true;
            if (!_db.Orders.Any(s => s.OrderId == orderId))
            {
                var item = new
                {
                    IsResponse,
                    IsStatus = false,
                };
                return Ok(item);
            }
            try
            {
                var getOrder = _db.Orders.Include(a=>a.Customer).Where(s => s.OrderId == orderId).FirstOrDefault();
                getOrder.OrderStatus = status;
                _db.Orders.Update(getOrder);
                _db.SaveChanges();

                string msg = "Dear " + getOrder.Customer.FirstName + ", Your order " + getOrder.OrderNo + " has been " + getOrder.OrderStatus + "!";
                SendNotification(msg, getOrder.Customer.FCMToken);

                var fvrtList = new
                {
                    IsResponse,
                    IsStatus,
                    Msg="Order status changed"
                };

                return Ok(fvrtList);
            }
            catch (Exception e)
            {
                var profile = new
                {
                    IsResponse = false,
                    IsStatus = false,

                };

                return BadRequest(profile);
            }


        }
        [HttpGet("GetNotifications")]
        public IActionResult GetNotifications(int userId)
        {

            bool IsResponse = true;
            bool IsStatus = true;
            try
            {
                var user = _db.Customers.Find(userId);
                if (user == null)
                {
                    var item = new
                    {
                        IsResponse = true,
                        IsStatus = false,
                    };
                    return Ok(item);
                }
                var notifications = _db.Notifications.Where(s => s.CustomerId == userId && s.NotificationStatus == true).Select(a => new
                {
                    a.NotificationId,
                    a.NotificationTitle,
                    a.NotificationDescription,
                    a.NotificationImageName

                }).ToList();

                var items = new
                {
                    IsResponse = true,
                    IsStatus = true,
                    Notifications = notifications
                };
                return Ok(items);
            }
            catch (Exception e)
            {
                var items = new
                {
                    IsResponse = false,
                    IsStatus = false,
                };
                return Ok(items);
            }
        }
        [HttpGet("OrderHistory")]
        public IActionResult OrderHistory(int userId)
        {

            bool IsResponse = true;
            bool IsStatus = true;
           // bool isCustomerReviewed = false;
            try
            {
                var user = _db.Customers.Find(userId);
                if (user == null)
                {
                    var item = new
                    {
                        IsResponse = true,
                        IsStatus = false,
                    };
                    return Ok(item);
                }  //??order status
           /*   if(_db.ratingsReview.Any(p => p.CustomerId == userId))
                {
                    isCustomerReviewed = true;
                }*/
                var PastOrders = _db.Orders.Where(s => s.CustomerId == userId && (s.OrderStatus.ToLower()=="delivered" || s.OrderStatus.ToLower() == "rejected")).Select(a => new //reject
                {
                    a.OrderId,
                    a.OrderNo,
                    a.OrderStatus,
                    a.OrderDate,
                    a.isCustomerReviewed,
                   RatingValue= !_db.ratingsReview.Any(s=>s.OrderId==a.OrderId)==true?-1 : _db.ratingsReview.Where(s => s.OrderId == a.OrderId).FirstOrDefault().Value,
                    OrderProduct=_db.OrderProducts.Include(s=>s.productDetail).ThenInclude(s=>s.Product).Where(d=>d.OrderId==a.OrderId).Select(d=>new
                    {
                        d.OrderProductId,
                        ProductImage = _db.ProductImages.Where(s => s.ProductImageStatus == true && s.ProductDetailId == s.ProductDetailId).FirstOrDefault().ProductImageName,
                        d.productDetail.Product.ProductName,
                        Price = d.OrderProductPrice,
                        Quantity =d.OrderProductQuantity,
                        Unit = d.OrderProductUnit,

                    }).ToList()

                }).ToList();
                var ActiveOrders= _db.Orders.Where(s => s.CustomerId == userId && (s.OrderStatus.ToLower() == "dispatched" || s.OrderStatus.ToLower() == "processing")).Select(a => new
                {
                    a.OrderId,
                    a.OrderNo,
                    a.OrderStatus,
                    a.OrderDate,
                    OrderProduct = _db.OrderProducts.Include(s => s.productDetail).ThenInclude(s => s.Product).Where(d => d.OrderId == a.OrderId).Select(d => new
                    {
                        d.OrderProductId,
                        ProductImage = _db.ProductImages.Where(s => s.ProductImageStatus == true && s.ProductDetailId == s.ProductDetailId).FirstOrDefault().ProductImageName,
                        d.productDetail.Product.ProductName,
                        Price = d.OrderProductPrice,
                        Quantity = d.OrderProductQuantity,
                        Unit = d.OrderProductUnit
                    }).ToList()

                }).ToList();
                var items = new
                {
                    IsResponse = true,
                    IsStatus = true,
                    PastOrders,
                    ActiveOrders
                };
                return Ok(items);
            }
            catch (Exception e)
            {
                var items = new
                {
                    IsResponse = false,
                    IsStatus = false,
                };
                return Ok(items);
            }
        }
        [HttpGet("GetCustomerDetail")]
        public IActionResult GetCustomerDetail(int userId)
        {

            bool IsResponse = true;
            bool IsStatus = true;
            try
            {
                var user = _db.Customers.Find(userId);
                if (user == null)
                {
                    var item = new
                    {
                        IsResponse = true,
                        IsStatus = false,
                    };
                    return Ok(item);
                }  //??order status
                var CustomerDetail = _db.Customers.Where(s => s.CustomerId == userId ).Select(a => new //reject
                {
                    a.CustomerId,
                     a.FirstName,
                    a.LastName,
                    a.CustomerPhone,
                    a.CustomerEmail,
                    a.CustomerAddress

                }).FirstOrDefault();
                var items = new
                {
                    IsResponse = true,
                    IsStatus = true,
                    CustomerDetail
                };
                return Ok(items);
            }
            catch (Exception e)
            {
                var items = new
                {
                    IsResponse = false,
                    IsStatus = false,
                };
                return Ok(items);
            }
        }

        [HttpGet("DeleteUserProfile")]
        public async Task<IActionResult> DeleteUserProfile(string phoneNo)
        {
            try
            {
                if (phoneNo != null)
                {
                    var pho = "+" + phoneNo;
                    if (_db.Customers.Any(s => s.CustomerPhone == pho))
                    {
                        _db.Customers.Remove(_db.Customers.FirstOrDefault(a => a.CustomerPhone == pho));
                        _db.SaveChanges();
                        var show = new
                        {
                            IsStatus = true,
                            Msg = "User's Delete"

                        };
                        return Ok(show);
                    }
                }
                return Ok(new { IsStatus = false, Msg = "User's phone number not exist" });
            }
            catch (Exception e)
            {
                var item = new
                {
                    IsStatus = false
                };
                return BadRequest(item);
            }
        }

        [HttpGet("GetAllProducts")]
        public async Task<IActionResult> GetAllProducts()
        {
            try
            {
                var allProducts = _db.ProductDetails.Include(a => a.Product).Where(s => s.ProductDetailStatus == true).Select(s => new
                {
                    s.ProductDetailId,
                    s.Product.ProductName,
                    ProductPrice = s.ProductDetailUnitPrice,
                    s.DiscountPrice,
                    ProductImage = _db.ProductImages.Where(v => v.ProductDetailId == s.ProductDetailId && v.ProductImageStatus == true).FirstOrDefault().ProductImageName,
                       ProductColors = _db.productColor.Where(a => a.ProductDetailId == s.ProductDetailId).Select(a => new
                       {
                           a.ProductColorsId,
                           a.ColorName,
                           a.ColorCode
                       }).ToList(),
                    ProductSize = _db.productSize.Where(a => a.ProductDetailId == s.ProductDetailId).Select(a => new
                    {
                        a.ProductSizesId,
                        a.Size
                    }).ToList(),
                }).ToList();
                var items = new
                {
                    IsResponse=true,
                    IsStatus=true,
                    AllProducts=allProducts
                };
                return Ok(items);
            }
            catch (Exception e)
            {
                var item = new
                {
                    IsResponse = false,
                    IsStatus = false
                };
                return BadRequest(item);
            }
        }
        [HttpGet("RefreshFCMToken")]
        public async Task<IActionResult> RefreshFCMToken(int customerId,string FCMToken)
        {
            try
            {
                var customer = _db.Customers.Find(customerId);
                customer.FCMToken = FCMToken;
                _db.SaveChanges();

                var items = new
                {
                    IsResponse = true,
                    IsStatus = true,
                    Msg = "Successfully Update"
                };
                return Ok(items);
            }
            catch (Exception e)
            {
                var item = new
                {
                    IsResponse = false,
                    IsStatus = false
                };
                return BadRequest(item);
            }
        }

        [HttpGet("OrderDetails")]
        public async Task<IActionResult> OrderDetails(int orderId)
        {
            try
            {
                var order = _db.Orders.Where(a => a.OrderId == orderId).Select(x => new
                {
                    x.OrderNo,
                    x.OrderDeliveryCharges,
                    x.OrderDiscountPrice,
                    x.OrderGST,
                    x.Longitude,
                    x.Latitude,
                    x.Description,
                    x.OrderPaymentMethod,
                    x.OrderReceiveAmount,
                    x.OrderRecipentEmail,
                    x.OrderRecipentName,
                    x.OrderRecipentPhone,
                    x.OrderRemainingAmount,
                    x.OrderShippingAddress,
                    x.OrderShippingCity,
                    x.OrderShippingProvince,
                    x.OrderSubTotal,
                    x.OrderTotalPrice,
                    x.OrderStatus,
                    OrderProduct = _db.OrderProducts.Include(a => a.productDetail).ThenInclude(a => a.Product).Where(a => a.OrderId == orderId).Select(y => new
                    {
                        y.OrderProductId,
                        y.OrderProductPrice,
                        y.OrderProductQuantity,
                        y.OrderProductUnit,
                        y.ProductDetailId,
                        y.productDetail.Product.ProductName,
                        ProductImage = _db.ProductImages.Where(s => s.ProductDetailId == s.ProductDetailId && s.ProductImageStatus == true).FirstOrDefault().ProductImageName,
                    }).ToList()
                }).FirstOrDefault(); 

                var items = new
                {
                    IsResponse = true,
                    IsStatus = true,
                    order
                };
                return Ok(items);
            }
            catch (Exception e)
            {
                var item = new
                {
                    IsResponse = false,
                    IsStatus = false
                };
                return BadRequest(item);
            }
        }


        [HttpGet("SeeAllProducts")]
        public async Task<IActionResult> SeeAllProducts(int flag)
        {
            try
            {
                bool IsStatus = true;
                bool IsResponse = true;
                if (flag == 0)
                {
                    var Products = _db.ProductDetails.Include(a => a.Product).Where(s => s.FeatureProduct == true && s.ProductDetailStatus == true).Select(s => new
                    {
                        s.ProductDetailId,
                        s.Product.ProductName,
                        ProductImage = _db.ProductImages.Where(v => v.ProductImageStatus == true && v.ProductDetailId == s.ProductDetailId && v.productDetail.FeatureProduct == true).FirstOrDefault().ProductImageName,
                        ProductPrice = s.ProductDetailUnitPrice,
                        s.DiscountPrice,
                        ProductColors = _db.productColor.Where(a => a.ProductDetailId == s.ProductDetailId).Select(a => new
                        {
                            a.ProductColorsId,
                            a.ColorName,
                            a.ColorCode
                        }).ToList(),
                        ProductSize = _db.productSize.Where(a => a.ProductDetailId == s.ProductDetailId).Select(a => new
                        {
                            a.ProductSizesId,
                            a.Size
                        }).ToList(),
                    }).ToList();
                    return Ok(new { IsStatus, IsResponse, Products });
                }
                else if (flag == 1)
                {
                    var Products = _db.ProductDetails.Include(a => a.Product).Where(s => s.BestSell == true && s.ProductDetailStatus == true).Select(s => new
                    {
                        s.ProductDetailId,
                        s.Product.ProductName,
                        ProductImage = _db.ProductImages.Where(v => v.ProductImageStatus == true && v.ProductDetailId == s.ProductDetailId && v.productDetail.BestSell == true).FirstOrDefault().ProductImageName,
                        ProductPrice = s.ProductDetailUnitPrice,
                        s.DiscountPrice,
                        ProductColors = _db.productColor.Where(a => a.ProductDetailId == s.ProductDetailId).Select(a => new
                        {
                            a.ProductColorsId,
                            a.ColorName,
                            a.ColorCode
                        }).ToList(),
                        ProductSize = _db.productSize.Where(a => a.ProductDetailId == s.ProductDetailId).Select(a => new
                        {
                            a.ProductSizesId,
                            a.Size
                        }).ToList(),
                    }).ToList();
                    return Ok(new { IsStatus, IsResponse, Products });
                }
                else
                {
                    var Products = _db.ProductDetails.Include(a => a.Product).Where(s => s.TopRated == true && s.ProductDetailStatus == true).Select(s => new
                    {
                        s.ProductDetailId,
                        s.Product.ProductName,
                        ProductImage = _db.ProductImages.Where(v => v.ProductImageStatus == true && v.ProductDetailId == s.ProductDetailId && v.productDetail.TopRated == true).FirstOrDefault().ProductImageName,
                        ProductPrice = s.ProductDetailUnitPrice,
                        s.DiscountPrice,
                        ProductColors = _db.productColor.Where(a => a.ProductDetailId == s.ProductDetailId).Select(a => new
                        {
                            a.ProductColorsId,
                            a.ColorName,
                            a.ColorCode
                        }).ToList(),
                        ProductSize = _db.productSize.Where(a => a.ProductDetailId == s.ProductDetailId).Select(a => new
                        {
                            a.ProductSizesId,
                            a.Size
                        }).ToList(),
                    }).ToList();
                    return Ok(new { IsStatus, IsResponse, Products });
                }

               
            }
            catch (Exception e)
            {
                return Ok(new { IsStatus=false, IsResponse=false });
            }
        }
        public void SendNotification(string msg, string FCMToken)
        {
            //var serializer = new JavaScriptSerializer();
            //var json = serializer.Serialize(data);

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
                //first code part comment 
                // string SERVER_API_KEY = "";
                // var SENDER_ID = "";
                //if (user.ToLower() == "client")
                //{
                //    SERVER_API_KEY = "AAAAYTmDMbI:APA91bFFWgViFVH8obf5ILwkUeVhvYcfsjKwX3sZAXW4HVkKCP9Z1ni3s_pjm1xvh_Bpi0rH--AGFOrVj5cF2yyGg5AeavAzzRcaRIIofIgxt16b08735fahFN8TOAhMqjMXTR18CCmh";
                //    SENDER_ID = "417576726962";
                //}
                //else
                //{
                //    SERVER_API_KEY = "AAAAYTmDMbI:APA91bFFWgViFVH8obf5ILwkUeVhvYcfsjKwX3sZAXW4HVkKCP9Z1ni3s_pjm1xvh_Bpi0rH--AGFOrVj5cF2yyGg5AeavAzzRcaRIIofIgxt16b08735fahFN8TOAhMqjMXTR18CCmh";
                //    SENDER_ID = "417576726962";
                //}
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
             /*   using (Stream dataStream = tRequest.GetRequestStream())
                {

                    dataStream.Write(byteArray, 0, byteArray.Length);


                    using (WebResponse tResponse = tRequest.GetResponse())
                    {

                        using (Stream dataStreamResponse = tResponse.GetResponseStream())
                        {

                            using (StreamReader tReader = new StreamReader(dataStreamResponse))
                            {

                                String sResponseFromServer = tReader.ReadToEnd();

                                string str = sResponseFromServer;

                            }
                        }
                    }
                }*/
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
