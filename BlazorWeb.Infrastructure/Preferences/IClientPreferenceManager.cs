using MudBlazor;

namespace BlazorWeb.Infrastructure.Managers.Preferences
{
    public interface IClientPreferenceManager
    {
        Task<MudTheme> GetCurrentThemeAsync();

        Task<bool> ToggleDarkModeAsync();
    }
}