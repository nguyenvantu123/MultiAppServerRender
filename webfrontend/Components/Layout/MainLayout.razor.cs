using BlazorWeb.Constants.Application;
using BlazorWeb.Extensions;
using BlazorWeb.Request.Identity;
using BlazorWeb.Services;
using BlazorWeb.Services.BffApiClients;
using BlazorWeb.Settings;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.JSInterop;
using MudBlazor;
using System;
using System.Linq;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Threading.Tasks;
using static System.Net.WebRequestMethods;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace BlazorWeb.Components.Layout
{
    public partial class MainLayout
    {
        private MudTheme _currentTheme;

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                _currentTheme = UserTheme.DefaultTheme;

                _currentTheme = await _clientPreferenceManager.GetCurrentThemeAsync();
            }
        }


        private async Task DarkMode()
        {
            bool isDarkMode = await _clientPreferenceManager.ToggleDarkModeAsync();
            _currentTheme = isDarkMode
                ? UserTheme.DefaultTheme
                : UserTheme.DarkTheme;
        }
    }
}