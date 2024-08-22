// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.


namespace BlazorIdentity.Users.Controller.Account;

public class LoginInputModel
{
    [Required]
    public string Username { get; set; }
    [Required]
    public string Password { get; set; }
    public bool RememberLogin { get; set; }

    private string returnUrl;

    public string ReturnUrl
    {
        get => returnUrl ?? "/";
        set
        {
            if (!string.IsNullOrEmpty(value))
            {
                if (value.StartsWith("http"))
                    value = new Uri(value).LocalPath;

                if (!value.StartsWith("/"))
                    value = $"/{value}";
            }

            returnUrl = value;
        }
    }

    public string __RequestVerificationToken { get; set; }
}
