using Blazored.LocalStorage;
using BlazorWeb.Components.Pages.Authentication;
using BlazorWeb.Constants.Application;
using BlazorWeb.Extensions;
using BlazorWeb.Identity;
using BlazorWeb.Request.Identity;
using BlazorWeb.Response.ResponseApiBase;
using BlazorWeb.Response.User;
using BlazorWeb.Services.BffApiClients;
using BlazorWebApi.Constants.Storage;
using Microsoft.AspNetCore.Authentication.OAuth;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Server.HttpSys;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.JSInterop;
using MudBlazor;
<<<<<<< HEAD:BlazorWeb/Components/Layout/MainBody.razor.cs
=======
using MultiAppServer.ServiceDefaults.Wrapper;
>>>>>>> 3c6e47b79da1d67715f3c930762656f0a6a8fe2b:BlazorWeb/BlazorWeb/Components/Layout/MainBody.razor.cs
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Refit;
using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Wrapper;

namespace BlazorWeb.Components.Layout
{
    public partial class MainBody
    {
        [Parameter]
        public RenderFragment ChildContent { get; set; }

        [Parameter]
        public EventCallback OnDarkModeToggle { get; set; }

        private bool _drawerOpen = true;
        [Inject] private IBffApiClients _bffApiClients { get; set; }

<<<<<<< HEAD:BlazorWeb/Components/Layout/MainBody.razor.cs
        [Inject] private IAuthenticationManager authenticationManager { get; set; }

        //[Inject] private IResponseApiBase _iResponseApiBase { get; set; }
=======
        [Inject] private IResponseApiBase _iResponseApiBase { get; set; }
>>>>>>> 3c6e47b79da1d67715f3c930762656f0a6a8fe2b:BlazorWeb/BlazorWeb/Components/Layout/MainBody.razor.cs

        //[Inject] private IBffApiClients _bffApiClients { get; set; }

        private string CurrentUserId { get; set; }
        private string ImageDataUrl { get; set; }
        private string FirstName { get; set; }
        private string SecondName { get; set; }
        private string Email { get; set; }
        private char FirstLetterOfName { get; set; }

        public async Task ToggleDarkMode()
        {
            await OnDarkModeToggle.InvokeAsync();
        }

        protected override async Task OnInitializedAsync()
        {

<<<<<<< HEAD:BlazorWeb/Components/Layout/MainBody.razor.cs
            hubConnection = hubConnection.TryInitialize(_navigationManager, _localStorage);
=======
            hubConnection = hubConnection.TryInitialize(_navigationManager, _localStorageService);
            await hubConnection.StartAsync();
>>>>>>> 3c6e47b79da1d67715f3c930762656f0a6a8fe2b:BlazorWeb/BlazorWeb/Components/Layout/MainBody.razor.cs
            hubConnection.On<string, string, string>(ApplicationConstants.SignalR.ReceiveChatNotification, (message, receiverUserId, senderUserId) =>
            {
                if (CurrentUserId == receiverUserId)
                {
                    _jsRuntime.InvokeAsync<string>("PlayAudio", "notification");
                    _snackBar.Add(message, Severity.Info, config =>
                    {
                        config.VisibleStateDuration = 10000;
                        config.HideTransitionDuration = 500;
                        config.ShowTransitionDuration = 500;
                        config.Action = _localizer["Chat?"];
                        config.ActionColor = Color.Primary;
                        config.Onclick = snackBar =>
                        {
                            _navigationManager.NavigateTo($"chat/{senderUserId}");
                            return Task.CompletedTask;
                        };
                    });
                }
            });
            hubConnection.On(ApplicationConstants.SignalR.ReceiveRegenerateTokens, async () =>
            {
                try
                {
                    var token = (await _localStorageService.GetItemAsync<string>(StorageConstants.Local.AuthToken));
                    var refreshToken = (await _localStorageService.GetItemAsync<string>(StorageConstants.Local.RefreshToken));

                    if (string.IsNullOrEmpty(token))
                    {
                        _snackBar.Add(_localizer["You are Logged Out."], Severity.Error);
                        await _stateProvider.Logout();
                        _navigationManager.NavigateTo("/");
                    }

<<<<<<< HEAD:BlazorWeb/Components/Layout/MainBody.razor.cs
                    var data = await _stateProvider.RefreshToken();
                    if (!string.IsNullOrEmpty(data.AccessToken))
=======
                    var data = await _authenticationManager.TryRefreshToken();
                    if (!string.IsNullOrEmpty(data))
>>>>>>> 3c6e47b79da1d67715f3c930762656f0a6a8fe2b:BlazorWeb/BlazorWeb/Components/Layout/MainBody.razor.cs
                    {
                        await _stateProvider.Login(data.AccessToken, data.RefreshToken);
                        _snackBar.Add(_localizer["Refreshed Token."], Severity.Success);
                        //_httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                    }
                    else
                    {
                        _snackBar.Add(_localizer["You are Logged Out."], Severity.Error);
                        await _stateProvider.Logout();
                        _navigationManager.NavigateTo("/");
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    _snackBar.Add(_localizer["You are Logged Out."], Severity.Error);
                    await _stateProvider.Logout();
                    _navigationManager.NavigateTo("/");
                }
            });
            hubConnection.On<string, string>(ApplicationConstants.SignalR.LogoutUsersByRole, async (userId, roleId) =>
            {
                if (CurrentUserId != userId)
                {
                    var rolesResponse = await _bffApiClients.GetAllRole();
                    if (rolesResponse.Success)
                    {
                        var role = rolesResponse.Result.FirstOrDefault(x => x.Id == roleId);
                        if (role != null)
                        {
                            var currentUserRolesResponse = await _bffApiClients.GetRolesAsync();
                            if (currentUserRolesResponse.Success && currentUserRolesResponse.Result.UserRoles.Any(x => x.RoleName == role.Name))
                            {
                                _snackBar.Add(_localizer["You are logged out because the Permissions of one of your Roles have been updated."], Severity.Error);
                                await hubConnection.SendAsync(ApplicationConstants.SignalR.OnDisconnect, CurrentUserId);
                                await _stateProvider.Logout();
                                _navigationManager.NavigateTo("/login");
                            }
                        }
                    }
                }
            });
            //hubConnection.On<string>(ApplicationConstants.SignalR.PingRequest, async (userName) =>
            //{
            //    await hubConnection.SendAsync(ApplicationConstants.SignalR.PingResponse, CurrentUserId, userName);

            //});

            //await hubConnection.SendAsync(ApplicationConstants.SignalR.OnConnect, CurrentUserId);

            await hubConnection.StartAsync();

            _snackBar.Add(string.Format(_localizer["Welcome {0}"], FirstName), Severity.Success);
        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                await LoadDataAsync();
            }
        }

        private async Task LoadDataAsync()
        {

<<<<<<< HEAD:BlazorWeb/Components/Layout/MainBody.razor.cs
            var token = await _localStorage.GetItemAsync<string>(StorageConstants.Local.AuthToken) ?? "";

            var state = await _bffApiClients.UserProfile(authorization: token);

            //if (state.Success)
=======
            var token = await _localStorageService.GetItemAsync<string>(StorageConstants.Local.AuthToken) ?? "";

            var state = await _bffApiClients.UserProfile(authorization: token);

            var httpResponse = JsonConvert.DeserializeObject<ResultBase<bool>>(await state.Content.ReadAsStringAsync());

            if (httpResponse.StatusCode == (int)HttpStatusCode.Unauthorized)
            {

                var refreshToken = await _authenticationManager.RefreshToken();

                if (string.IsNullOrEmpty(refreshToken))
                {
                    await _authenticationManager.Logout();

                }
            }

            if (httpResponse.StatusCode == (int)HttpStatusCode.OK)
            {
                var responseReturn = JsonConvert.DeserializeObject<ResultBase<UserProfileResponse>>(await state.Content.ReadAsStringAsync());

                if (responseReturn.Success)
                {
                    FirstName = responseReturn.Result.FirstName;
                    if (FirstName.Length > 0)
                    {
                        FirstLetterOfName = FirstName[0];
                    }

                    SecondName = responseReturn.Result.LastName;
                    Email = responseReturn.Result.Email;
                }
                else
                {
                    foreach (var item in responseReturn.ErrorMessages)
                    {
                        _snackBar.Add(
                            _localizer[item],
                            Severity.Error);
                    }
                }
            }

            //else
            //{
            //    for
            //}

            //CurrentUserId = state.id();

            //var imageResponse = await _accountManager.GetProfilePictureAsync(CurrentUserId);
            //if (imageResponse.Succeeded)
>>>>>>> 3c6e47b79da1d67715f3c930762656f0a6a8fe2b:BlazorWeb/BlazorWeb/Components/Layout/MainBody.razor.cs
            //{
            //    FirstName = state.Result.FirstName;
            //    if (FirstName.Length > 0)
            //    {
            //        FirstLetterOfName = FirstName[0];
            //    }

            //    SecondName = state.Result.LastName;
            //    Email = state.Result.Email;
            //}
            //else
            //{
            //    foreach (var item in state.ErrorMessages)
            //    {
            //        _snackBar.Add(
            //            _localizer[item],
            //            Severity.Error);
            //    }
            //}
        }

        //else
        //{
        //    for
        //}

        //CurrentUserId = state.id();

        //var imageResponse = await _accountManager.GetProfilePictureAsync(CurrentUserId);
        //if (imageResponse.Succeeded)
        //{
        //    ImageDataUrl = imageResponse.Data;
        //}

        //var currentUserResult = await _userManager.GetAsync(CurrentUserId);
        //if (!currentUserResult.Succeeded || currentUserResult.Data == null)
        //{
        //    _snackBar.Add(
        //        _localizer["You are logged out because the user with your Token has been deleted."],
        //        Severity.Error);
        //    CurrentUserId = string.Empty;
        //    ImageDataUrl = string.Empty;
        //    FirstName = string.Empty;
        //    SecondName = string.Empty;
        //    Email = string.Empty;
        //    FirstLetterOfName = char.MinValue;
        //    await _stateProvider.Logout();
        //}


        private void DrawerToggle()
        {
            _drawerOpen = !_drawerOpen;
        }

        private void Logout()
        {
            var parameters = new DialogParameters
            {
                {nameof(Dialogs.Logout.ContentText), $"{_localizer["Logout Confirmation"]}"},
                {nameof(Dialogs.Logout.ButtonText), $"{_localizer["Logout"]}"},
                {nameof(Dialogs.Logout.Color), Color.Error},
                {nameof(Dialogs.Logout.CurrentUserId), CurrentUserId},
                {nameof(Dialogs.Logout.HubConnection), hubConnection}
            };

            var options = new DialogOptions { CloseButton = true, MaxWidth = MaxWidth.Small, FullWidth = true };

            _dialogService.Show<Dialogs.Logout>(_localizer["Logout"], parameters, options);
        }

        private HubConnection hubConnection;
        public bool IsConnected => hubConnection.State == HubConnectionState.Connected;
    }
}