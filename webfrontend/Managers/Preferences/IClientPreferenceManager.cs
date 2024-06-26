﻿using BlazorWeb.Settings;
using BlazorWeb.Wrapper;
using MudBlazor;

namespace BlazorWeb.Managers.Preferences
{
    public interface IClientPreferenceManager
    {
        Task<bool> ToggleDarkModeAsync();

        Task<bool> ToggleLayoutDirection();

        Task<ResultBase<string>> ChangeLanguageAsync(string languageCode);

        Task<MudTheme> GetCurrentThemeAsync();

        Task<bool> IsRTL();


        Task<ClientPreference> GetPreference();

        Task SetPreference(ClientPreference preference);
    }
}
