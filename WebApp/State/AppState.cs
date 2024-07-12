//using BlazorBoilerplate.Shared.Dto.Db;
//using BlazorBoilerplate.Shared.Interfaces;
//using BlazorBoilerplate.Shared.Localizer;
//using Humanizer;

using WebApp.Constant;
using WebApp.Interfaces;
using WebApp.Models;

namespace WebApp.State
{
    public class AppState
    {
        public static BlazorRuntime Runtime { get; set; } = BlazorRuntime.Server;

        public event Action OnChange;

        private readonly AccountApiClient _apiClient;
        private UserProfileViewModel _userProfile { get; set; }

        //private readonly IStringLocalizer<Global> L;

        //public readonly string AppName = string.Empty;
        //public readonly string AppShortName = string.Empty;

        public AppState(AccountApiClient apiClient)
        {
            _apiClient = apiClient;
        }

        public bool IsNavOpen
        {
            get
            {
                if (_userProfile == null)
                    return true;

                return _userProfile.IsNavOpen;
            }
            set
            {
                _userProfile.IsNavOpen = value;
            }
        }
        public bool IsNavMinified { get; set; }

        public async Task UpdateUserProfile()
        {
            //await _apiClient.SaveChanges();
        }

        public void ClearUserProfile()
        {
            _userProfile = null;
        }

        public async Task<UserProfileViewModel> GetUserProfile()
        {
            if (_userProfile == null)
                _userProfile = await _apiClient.GetUserProfile();

            return _userProfile;
        }

        public async Task UpdateUserProfileCount(int count)
        {
            _userProfile.Count = count;
            await UpdateUserProfile();
            NotifyStateChanged();
        }

        public async Task<int> GetUserProfileCount()
        {
            if (_userProfile == null)
            {
                _userProfile = await GetUserProfile();
                return _userProfile.Count;
            }

            return _userProfile.Count;
        }

        public async Task SaveLastVisitedUri(string uri)
        {
            if (_userProfile == null)
            {
                _userProfile = await GetUserProfile();
            }
            else
            {
                _userProfile.LastPageVisited = uri;
                await UpdateUserProfile();

                NotifyStateChanged();
            }
        }

        private void NotifyStateChanged() => OnChange?.Invoke();
    }
}