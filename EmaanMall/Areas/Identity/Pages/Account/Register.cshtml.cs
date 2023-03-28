using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using EmaanMall.Utility;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Logging;

namespace EmaanMall.Areas.Identity.Pages.Account
{
    [AllowAnonymous]
    public class RegisterModel : PageModel
    {
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly ILogger<RegisterModel> _logger;
        private readonly IEmailSender _emailSender;
        private readonly RoleManager<IdentityRole> _roleManager;

        public RegisterModel(
            UserManager<IdentityUser> userManager,
            SignInManager<IdentityUser> signInManager,
            ILogger<RegisterModel> logger,
            IEmailSender emailSender,
            RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _logger = logger;
            _emailSender = emailSender;
            _roleManager = roleManager;
        }

        [BindProperty]
        public InputModel Input { get; set; }

        public string ReturnUrl { get; set; }

        public IList<AuthenticationScheme> ExternalLogins { get; set; }

        public class InputModel
        {
            [Required]
            [EmailAddress]
            [Display(Name = "Email")]
            public string Email { get; set; }

            [Required]
            [StringLength(100, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 6)]
            [DataType(DataType.Password)]
            [Display(Name = "Password")]
            public string Password { get; set; }
            public string Role { get; set; }
            [DataType(DataType.Password)]
            [Display(Name = "Confirm password")]
            [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
            public string ConfirmPassword { get; set; }
          
           
        }
        public async Task<IActionResult> OnGetAsync(string returnUrl = null)
        {
            if(User.IsInRole("Vendor"))
            {
                string link = Request.Scheme + "://" + Request.Host + "/Identity/Account/Logout";
                return Redirect(link);
            }
            ReturnUrl = returnUrl;
            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();
            return Page();


            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId != null)
            {
                var user = await _userManager.FindByIdAsync(userId);
                var role = await _userManager.GetRolesAsync(user);
                if (role.ElementAt(0) == "Admin")
                {
                    ReturnUrl = returnUrl;
                    ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();
                    return Page();
                }
                else
                {
                    string link = Request.Scheme + "://" + Request.Host + "/Identity/Account/Logout";
                    return Redirect(link);
                }
            }
            else
            {
                string link = Request.Scheme + "://" + Request.Host + "/Identity/Account/Logout";
                return Redirect(link);
            }
            /* ReturnUrl = returnUrl;
             ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();*/
        }

        public async Task<IActionResult> OnPostAsync(string returnUrl = null)
        {
            returnUrl = returnUrl ?? Url.Content("~/");
            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();
            if (ModelState.IsValid)
            {
                if (_roleManager != null)
                {
                    if (!await _roleManager.RoleExistsAsync(Input.Role))
                    {
                        await _roleManager.CreateAsync(new IdentityRole(Input.Role));
                    }
                }

                var user = new IdentityUser { UserName = Input.Email, Email = Input.Email };
                user.EmailConfirmed = true;
                var result = await _userManager.CreateAsync(user, Input.Password);
                if (result.Succeeded)
                {
                    await _userManager.AddToRoleAsync(user, Input.Role);
                    if(Input.Role=="Vendor")
                    {
                        string link1 = Request.Scheme + "://" + Request.Host + "/Vendors/Create/?email="+Input.Email +"&password="+Input.Password;
                        return Redirect(link1);
                    }
                    if (Input.Role == "Staff")
                    {
                        string link1 = Request.Scheme + "://" + Request.Host + "/Staff/Create/?email=" + Input.Email + "&password=" + Input.Password;
                        return Redirect(link1);
                    }
                    string link = Request.Scheme + "://" + Request.Host + "/Identity/Account/Login";
                    return Redirect(link);
                    var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                    code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
                    var callbackUrl = Url.Page(
                        "/Account/ConfirmEmail",
                        pageHandler: null,
                        values: new { area = "Identity", userId = user.Id, code = code, returnUrl = returnUrl },
                        protocol: Request.Scheme);

                    await _emailSender.SendEmailAsync(Input.Email, "Confirm your email",
                        $"Please confirm your account by <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>clicking here</a>.");

                    if (_userManager.Options.SignIn.RequireConfirmedAccount)
                    {
                        return RedirectToPage("RegisterConfirmation", new { email = Input.Email, returnUrl = returnUrl });
                    }
                    else
                    {
                        await _signInManager.SignInAsync(user, isPersistent: false);
                        return LocalRedirect(returnUrl);
                    }
                }
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }

            // If we got this far, something failed, redisplay form
            return Page();
            /*   returnUrl = returnUrl ?? Url.Content("~/");
               ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();
               if (ModelState.IsValid)
               {
                   var user = new IdentityUser { UserName = Input.Email, Email = Input.Email, PhoneNumber = Input.PhoneNumber, NormalizedUserName = Input.Name};
                   var result = await _userManager.CreateAsync(user, Input.Password);
                   if (result.Succeeded)
                   {


                       if (!await _roleManager.RoleExistsAsync(SD.AdminEndUser))
                       {
                           await _roleManager.CreateAsync(new IdentityRole(SD.AdminEndUser));
                       }
                       if (!await _roleManager.RoleExistsAsync(SD.CustomerEndUser))
                       {
                           await _roleManager.CreateAsync(new IdentityRole(SD.CustomerEndUser));
                       }

                       if (Input.IsAdmin)
                       {
                           await _userManager.AddToRoleAsync(user, SD.CustomerEndUser);
                       }
                       else
                       {
                           await _userManager.AddToRoleAsync(user, SD.AdminEndUser);
                       }









                       _logger.LogInformation("User created a new account with password.");

                       var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                       code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
                       var callbackUrl = Url.Page(
                           "/Account/ConfirmEmail",
                           pageHandler: null,
                           values: new { area = "Identity", userId = user.Id, code = code, returnUrl = returnUrl },
                           protocol: Request.Scheme);

                       await _emailSender.SendEmailAsync(Input.Email, "Confirm your email",
                           $"Please confirm your account by <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>clicking here</a>.");

                       if (_userManager.Options.SignIn.RequireConfirmedAccount)
                       {
                           return RedirectToPage("RegisterConfirmation", new { email = Input.Email, returnUrl = returnUrl });
                       }
                       else
                       {
                           await _signInManager.SignInAsync(user, isPersistent: false);
                           return LocalRedirect(returnUrl);
                       }
                   }
                   foreach (var error in result.Errors)
                   {
                       ModelState.AddModelError(string.Empty, error.Description);
                   }
               }

               // If we got this far, something failed, redisplay form
               return Page();*/
        }
    }
}
