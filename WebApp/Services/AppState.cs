

using BlazorIdentity.Users.Models;
using WebApp.Interfaces;

namespace WebApp.Services
{
    public class AppState
    {

        private readonly AccountApiClient _apiClient;
        public UserProfile _userProfile { get; set; }


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

        public bool IsDarkMode
        {
            get
            {
                if (_userProfile == null)
                    return false;

                return _userProfile.IsDarkMode;
            }
            set
            {
                _userProfile.IsDarkMode = value;
            }
        }


        public async Task<UserProfile> GetUserProfile()
        {
            if (_userProfile == null)
                _userProfile = (await _apiClient.GetUserProfile()).Result;

            return _userProfile;
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
            }
        }

    }
}