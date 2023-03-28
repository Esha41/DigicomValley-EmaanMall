using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using EmaanMall.Data;
using EmaanMall.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using System.IO;
using Microsoft.AspNetCore.Authorization;

namespace EmaanMall.Controllers
{
    public class CategoriesController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _env;

        public CategoriesController(ApplicationDbContext context, IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
        }

        // GET: Categories
        [Authorize(Roles = "Admin,Staff")]
        public async Task<IActionResult> Index()
        {
            return View(await _context.Categories.ToListAsync());
        }

        // GET: Categories/Details/5
        [Authorize(Roles = "Admin,Staff")]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var category = await _context.Categories
                .FirstOrDefaultAsync(m => m.CategoryId == id);
            if (category == null)
            {
                return NotFound();
            }

            return View(category);
        }
        // GET: Categories/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Categories/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("CategoryId,CategoryIcon,CategoryName,CategoryStatus")] Category category, IFormFile Images)
        {
            if (ModelState.IsValid)
            {
                if (Images != null)
                {
                    var Image = Images;
                    string rootpath = _env.WebRootPath;
                    string fileName = Path.GetFileNameWithoutExtension(Image.FileName);
                    string extention = Path.GetExtension(Image.FileName);
                    category.CategoryIcon = fileName =  DateTime.Now.ToString("yymmssfff") + extention;
                    string path = Path.Combine(rootpath + "/Uploads/", fileName);
                    using (var filestream = new FileStream(path, FileMode.Create))
                    {
                        await Image.CopyToAsync(filestream);
                    }
                    _context.Add(category);

                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
            }
            return Ok(category);
        }
        [Authorize(Roles = "Admin,Staff")]
        // GET: Categories/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var category = await _context.Categories.FindAsync(id);
            if (category == null)
            {
                return NotFound();
            }
            return View(category);
        }

        // POST: Categories/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Category category)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    Category cat = _context.Categories.Where(a => a.CategoryId == category.CategoryId).FirstOrDefault();
                    IFormFile file = Request.Form.Files["photo"];
                    if (file != null)
                    {
                        if (Request.Form.Files != null && Request.Form.Files.Count > 0)
                        {
                            var pathExist = "Uploads/" + DateTime.Now.Date;
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
                                string fileName = $"{category.CategoryName}_{type}_{file.FileName}";
                                String uploadfilePath = Path.Combine(_env.WebRootPath, "Uploads");
                                if (!System.IO.Directory.Exists(uploadfilePath))
                                {
                                    System.IO.Directory.CreateDirectory(uploadfilePath); //Create directory if it doesn't exist
                                }
                                string path = Path.Combine(uploadfilePath, fileName);

                                using (FileStream fs = new FileStream(path, FileMode.Create))
                                {
                                    file.CopyTo(fs);
                                    cat.CategoryIcon = fileName;
                                }
                            }
                        }

                    }
                    cat.CategoryName = category.CategoryName;
                    _context.Categories.Update(cat);
                    _context.SaveChanges();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CategoryExists(category.CategoryId))
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
            return View(category);
        }
        [Authorize(Roles = "Admin,Staff")]
        // GET: Categories/Delete/5
    /*    public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var category = await _context.Categories
                .FirstOrDefaultAsync(m => m.CategoryId == id);
            if (category == null)
            {
                return NotFound();
            }

            return View(category);
        }*/

        // POST: Categories/Delete/5
        public async Task<IActionResult> ChangeStatus(int id)
        {
            try
            {
                var category = await _context.Categories.FindAsync(id);
                if (id == null || category == null)
                {
                    return NotFound();
                }
                category.CategoryStatus = !(category.CategoryStatus);
                _context.Categories.Update(category);
                _context.SaveChanges();

                //status false for productCategories table so that category with that product couldnt appear
                bool temp = true;
                if (category.CategoryStatus == false)
                {
                    temp = false;
                 }
              
                    var prodCat = _context.ProductCategories.Where(a => a.CategoryId == id).ToList();
                    foreach (var item in prodCat)
                    {
                        item.ProductCategoryStatus = temp;
                        _context.ProductCategories.Update(item);
                        _context.SaveChanges();
                    }
              
                return RedirectToAction(nameof(Index));
            }
            catch (Exception e)
            {
                return RedirectToAction(nameof(Index));
            }

          
        }
      /*  public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var prodList = from prod in _context.ProductDetails
                               join prodCat in _context.ProductCategories
                               on prod.ProductDetailId equals prodCat.ProductDetailId 
                               where(prodCat.CategoryId==id)
                               select prod;
                

                var category = await _context.Categories.FindAsync(id);
                if (id == null || category == null)
                {
                    return NotFound();
                }
                _context.Categories.Remove(category);
                _context.SaveChanges();

                _context.ProductDetails.RemoveRange(prodList);
                _context.SaveChanges();

                return RedirectToAction(nameof(Index));
            }
            catch (Exception e)
            {
                return RedirectToAction(nameof(Index));
            }


        }
*/
        private bool CategoryExists(int id)
        {
            return _context.Categories.Any(e => e.CategoryId == id);
        }
    }
}
