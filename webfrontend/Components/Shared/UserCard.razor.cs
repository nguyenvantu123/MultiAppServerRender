using BlazorWeb.Extensions;
using Microsoft.AspNetCore.Components;

namespace BlazorWeb.Components.Shared
{
    public partial class UserCard
    {
        [Parameter] public string Class { get; set; }
        private string FirstName { get; set; }
        private string SecondName { get; set; }
        private string Email { get; set; }
        private char FirstLetterOfName { get; set; }

        [Parameter]
        public string ImageDataUrl { get; set; }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                await LoadDataAsync();
            }
        }
        private async Task LoadDataAsync()
        {
            //var state = await _stateProvider.GetAuthenticationStateAsync();
            //var user = state.User;

            //Email = user.GetEmail().Replace(".com", string.Empty);
            //FirstName = user.GetFirstName();
            //SecondName = user.GetLastName();
            //if (FirstName.Length > 0)
            //{
            //    FirstLetterOfName = FirstName[0];
            //}
            //var UserId = user.GetUserId();
            //var imageResponse = (await _localStorage.GetAsync<string>(StorageConstants.Local.UserImageURL)).Value;
            //if (!string.IsNullOrEmpty(imageResponse))
            //{
            //    ImageDataUrl = imageResponse;
            //}
            //StateHasChanged();
        }
    }
}