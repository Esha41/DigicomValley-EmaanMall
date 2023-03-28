using EmaanMall.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace EmaanMall.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }
        public DbSet<Customer> Customers { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<ProductDetail> ProductDetails { get; set; }
        public DbSet<ProductImage> ProductImages { get; set; }
        public DbSet<ProductBundle> ProductBundles { get; set; }
        public DbSet<ProductCategory> ProductCategories { get; set; }
        public DbSet<FavoriteProduct> FavoriteProducts { get; set; }
        public DbSet<ProductInquiry> ProductInquiries { get; set; }
        public DbSet<Notification> Notifications { get; set; }
        public DbSet<DeliveryCharges> DeliveryCharges { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderProduct> OrderProducts { get; set; }
        public DbSet<PromoCodes> promoCodes { get; set; }
        public DbSet<ProductInquiryRemarks> ProductInquiryRemark { get; set; }
        public DbSet<Promotions> promotions { get; set; }
        public DbSet<CustomerPromoCodeLog> CustomerPromoCodeLogs { get; set; }
        public DbSet<PromotionsProducts> PromotionsProduct { get; set; }
        public DbSet<ProductColors> productColor { get; set; }
        public DbSet<ProductSizes> productSize { get; set; }
        public DbSet<Vendor> Vendor { get; set; }
        public DbSet<Staff> Staffs { get; set; }
        public DbSet<VendorBusiness> VendorBusiness { get; set; }
        public DbSet<RatingsReviews> ratingsReview { get; set; }
    }
}
