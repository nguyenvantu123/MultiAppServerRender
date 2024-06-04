using System.Collections.Generic;
using Blazored.LocalStorage;
using MudBlazor;
using System.Threading.Tasks;
using BlazorWeb.Constants.Storage;
using Microsoft.Extensions.Localization;
using BlazorWeb.Settings;
using BlazorWeb.Managers.Preferences;
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;
using BlazorWeb.Wrapper;

namespace BlazorWeb.Manager.Preferences
{
    public class ClientPreferenceManager : IClientPreferenceManager
    {
        private readonly ILocalStorageService _localStorageService;
        private readonly IStringLocalizer<ClientPreferenceManager> _localizer;

        public ClientPreferenceManager(
            ILocalStorageService localStorageService,
            IStringLocalizer<ClientPreferenceManager> localizer)
        {
            _localStorageService = localStorageService;
            _localizer = localizer;
        }

        public async Task<bool> ToggleDarkModeAsync()
        {
            var preference = await GetPreference() as ClientPreference;
            if (preference != null)
            {
                preference.IsDarkMode = !preference.IsDarkMode;
                await SetPreference(preference);
                return !preference.IsDarkMode;
            }

            return false;
        }
        public async Task<bool> ToggleLayoutDirection()
        {
            var preference = await GetPreference() as ClientPreference;
            if (preference != null)
            {
                preference.IsRTL = !preference.IsRTL;
                await SetPreference(preference);
                return preference.IsRTL;
            }
            return false;
        }

        public async Task<ResultBase<string>> ChangeLanguageAsync(string languageCode)
        {
            var preference = await GetPreference() as ClientPreference;
            if (preference != null)
            {
                preference.LanguageCode = languageCode;
                await SetPreference(preference);
                return new ResultBase<string>
                {
                    Success = true,
                    ErrorMessages = new List<string> { _localizer["Client Language has been changed"] }
                };
            }

            return new ResultBase<string>
            {
                Success = false,
                ErrorMessages = new List<string> { _localizer["Failed to get client preferences"] }
            };
        }

        public async Task<MudTheme> GetCurrentThemeAsync()
        {
            var preference = await GetPreference() as ClientPreference;
            if (preference != null)
            {
                if (preference.IsDarkMode == true) return UserTheme.DarkTheme;
            }
            return UserTheme.DefaultTheme;
        }
        public async Task<bool> IsRTL()
        {
            var preference = await GetPreference() as ClientPreference;
            if (preference != null)
            {
                if (preference.IsDarkMode == true) return false;
            }
            return false;
        }

        public async Task<ClientPreference> GetPreference()
        {
            return (await _localStorageService.GetItemAsync<ClientPreference>(StorageConstants.Local.Preference)) as ClientPreference ?? new ClientPreference();
        }

        public async Task SetPreference(ClientPreference preference)
        {
            await _localStorageService.SetItemAsync(StorageConstants.Local.Preference, preference as ClientPreference);
        }
    }
}