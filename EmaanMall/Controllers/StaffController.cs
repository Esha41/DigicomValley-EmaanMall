using EmaanMall.Data;
using EmaanMall.Models;
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
    public class StaffController : Controller
    {
        private readonly ApplicationDbContext _context;

        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly UserManager<IdentityUser> _userManager;
        private RoleManager<IdentityRole> _roleManager;
        private readonly IWebHostEnvironment _env;
        public StaffController(ApplicationDbContext context, UserManager<IdentityUser> userManager, SignInManager<IdentityUser> signInManager, RoleManager<IdentityRole> roleManager, IWebHostEnvironment env)
        {
            _context = context;
            _userManager = userManager;
            _roleManager = roleManager;
            _env = env;
        }
        [Authorize(Roles = "Admin")]
        public IActionResult Create(string email, string password)
        {
            ViewBag.Email = email;
            ViewBag.password = password;
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Staff stf, IFormFile Images)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    Staff s = new Staff();
                    s.StaffName = stf.StaffName;
                    s.Phone = stf.Phone;
                    s.Email = stf.Email;
                    s.Address = stf.Address;
                    s.Password = stf.Password;
                    s.Status = true;
                    var user = _context.Users.Where(a => a.Email == stf.Email).FirstOrDefault();
                    s.UserId = user.Id;

                    if (Images != null)
                    {
                        var Image = Images;
                        string rootpath = _env.WebRootPath;
                        string fileName = Path.GetFileNameWithoutExtension(Image.FileName);
                        string extention = Path.GetExtension(Image.FileName);
                        s.Image = fileName = DateTime.Now.ToString("yymmssfff") + extention;
                        string path = Path.Combine(rootpath + "/Uploads/", fileName);
                        using (var filestream = new FileStream(path, FileMode.Create))
                        {
                            await Image.CopyToAsync(filestream);
                        }

                    }

                    _context.Staffs.Add(s);
                    _context.SaveChanges();
                    return RedirectToAction("StaffList");
                }
                else
                {
                    ViewBag.Email = stf.Email;
                    return View(stf);
                }
            }
            catch (Exception e)
            {
                ViewBag.Email = stf.Email;
                return View(stf);
            }

        }
        [Authorize(Roles = "Admin,Staff")]
        public async Task<IActionResult> Profile(int? id)
        {
            if (id != null)
            {
                var staff = _context.Staffs.Where(a => a.StaffId == id).FirstOrDefault();
                return View(staff);
            }
            else
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                var user = await _userManager.FindByIdAsync(userId);
                var role = await _userManager.GetRolesAsync(user);
                if (userId != null)
                {
                    var staff = _context.Staffs.Where(a => a.Email == user.Email).FirstOrDefault();
                    return View(staff);
                }
                else
                {
                    string link = Request.Scheme + "://" + Request.Host + "/Identity/Account/Login";
                    return Redirect(link);
                }
            }
        }
        [Authorize(Roles = "Staff")]
        public async Task<IActionResult> Edit()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var user = await _userManager.FindByIdAsync(userId);
            var role = await _userManager.GetRolesAsync(user);
            if (userId != null)
            {
                var staff = _context.Staffs.Where(a => a.Email == user.Email).FirstOrDefault();
                return View(staff);
            }
            else
            {
                string link = Request.Scheme + "://" + Request.Host + "/Identity/Account/Login";
                return Redirect(link);
            }
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditPost(Staff stf)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    Staff s = _context.Staffs.Where(a => a.StaffId == stf.StaffId).FirstOrDefault();
                    s.StaffName = stf.StaffName;
                    s.Address = stf.Address;
                    s.Phone = stf.Phone;
                    s.Email = stf.Email;
                    IFormFile file = Request.Form.Files["vendorphoto"];
                    if (file != null)
                    {
                        if (Request.Form.Files != null && Request.Form.Files.Count > 0)
                        {
                            var pathExist = "Uploads/" + stf.Email;
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
                                string fileName = $"{stf.Email}_{type}_{file.FileName}";
                                String uploadfilePath = Path.Combine(_env.WebRootPath, "Uploads");
                                if (!System.IO.Directory.Exists(uploadfilePath))
                                {
                                    System.IO.Directory.CreateDirectory(uploadfilePath); //Create directory if it doesn't exist
                                }
                                string path = Path.Combine(uploadfilePath, fileName);

                                using (FileStream fs = new FileStream(path, FileMode.Create))
                                {
                                    file.CopyTo(fs);
                                    s.Image = fileName;
                                }
                            }
                        }

                    }

                    _context.Staffs.Update(s);
                    _context.SaveChanges();

                    string link = Request.Scheme + "://" + Request.Host + "/Identity/Account/Login";
                    return Redirect(link);
                }
                else
                {
                    ViewBag.Email = stf.Email;
                    return View(stf);
                }
            }
            catch (Exception e)
            {
                ViewBag.Email = stf.Email;
                return View(stf);
            }

        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> StaffList()
        {
            var staff = _context.Staffs.ToList();
            return View(staff);
        }
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> ChangeStaffStatus(int id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var staff = _context.Staffs.Where(a => a.StaffId == id).FirstOrDefault();
            var userId = User.FindFirstValue(staff.UserId);
            var user = await _userManager.FindByIdAsync(staff.UserId);


            staff.Status = !(staff.Status);
            if (staff.Status == false)
            {
                var oldPass = staff.Password;
                staff.Password = staff.Password + 23;
                var changePasswordResult = await _userManager.ChangePasswordAsync(user, oldPass, staff.Password);
                if (!changePasswordResult.Succeeded)
                {
                    foreach (var error in changePasswordResult.Errors)
                    {
                        ModelState.AddModelError(string.Empty, error.Description);
                    }
                }

            }
            _context.Staffs.Update(staff);
            _context.SaveChanges();
            return RedirectToAction("StaffList");
        }
    }
}
