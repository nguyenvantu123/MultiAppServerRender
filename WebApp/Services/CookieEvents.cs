﻿using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;

namespace WebApp.Services
{
    public class CookieEvents: CookieAuthenticationEvents
    {
        public override Task RedirectToLogin(RedirectContext<CookieAuthenticationOptions> context)
        {

            context.RedirectUri = "/login";
            return base.RedirectToLogin(context);
        }
    }
}
