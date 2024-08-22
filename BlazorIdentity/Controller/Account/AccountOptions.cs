// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace BlazorIdentity.Users.Controller.Account;

public class AccountOptions
{
    public static bool AllowLocalLogin = true;
    public static bool AllowRememberLogin = false;
    public static TimeSpan RememberMeLoginDuration = TimeSpan.FromDays(30);

    public static bool ShowLogoutPrompt = false;
    public static bool AutomaticRedirectAfterSignOut = true;

    public static string InvalidCredentialsErrorMessage = "Invalid username or password";

    // specify the Windows authentication scheme being used
    public static readonly string WindowsAuthenticationSchemeName = Microsoft.AspNetCore.Server.IISIntegration.IISDefaults.AuthenticationScheme;
    // if user uses windows auth, should we load the groups from windows
    public static bool IncludeWindowsGroups = false;

}
