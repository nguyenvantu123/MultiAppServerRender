using MudBlazor;
using Shared.Settings;
using Wrapper;
<<<<<<< HEAD:BlazorWeb/Managers/Preferences/IClientPreferenceManager.cs
=======
//using IResult = Wrapper.IResult;
>>>>>>> 3c6e47b79da1d67715f3c930762656f0a6a8fe2b:BlazorWeb/BlazorWeb/Managers/Preferences/IClientPreferenceManager.cs

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
