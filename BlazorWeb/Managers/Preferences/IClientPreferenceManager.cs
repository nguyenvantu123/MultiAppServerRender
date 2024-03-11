using BlazorWeb.Wrapper;
using MudBlazor;
using Shared.Settings;

namespace BlazorWeb.Managers.Preferences
{
    public interface IClientPreferenceManager
    {
        Task<bool> ToggleDarkModeAsync();

        Task<bool> ToggleLayoutDirection();

        Task<IResultBase<string>> ChangeLanguageAsync(string languageCode);

        Task<MudTheme> GetCurrentThemeAsync();

        Task<bool> IsRTL();


        Task<ClientPreference> GetPreference();

        Task SetPreference(ClientPreference preference);
    }
}
