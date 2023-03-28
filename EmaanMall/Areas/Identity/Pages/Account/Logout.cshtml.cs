﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;

namespace EmaanMall.Areas.Identity.Pages.Account
{
    [AllowAnonymous]
    public class LogoutModel : PageModel
    {
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly ILogger<LogoutModel> _logger;

        public LogoutModel(SignInManager<IdentityUser> signInManager, ILogger<LogoutModel> logger)
        {
            _signInManager = signInManager;
            _logger = logger;
        }

        public IActionResult OnGet()
        {
            OnPost();
            string link = Request.Scheme + "://" + Request.Host + "/Identity/Account/Login";
            return Redirect(link);
        }

        public async Task<IActionResult> OnPost(string returnUrl = null)
        {
            await _signInManager.SignOutAsync();
            _logger.LogInformation("User logged out.");
            if (returnUrl != null)
            {
                string link = Request.Scheme + "://" + Request.Host + "/Identity/Account/Login";
                return Redirect(link);
            }
            else
            {
                string link = Request.Scheme + "://" + Request.Host + "/Identity/Account/Login";
                return Redirect(link);
            }
        }
    }
}
