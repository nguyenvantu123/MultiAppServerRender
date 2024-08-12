using BlazorWebApi.Users.Models;
using Breeze.Sharp;
using Microsoft.JSInterop;
using MultiAppServer.ServiceDefaults;
using System.Linq.Expressions;
using System.Net.Http.Json;
using WebApp.Components.Pages.Admin;
using WebApp.Constant;
using WebApp.DataModels;
using WebApp.Extensions;
using WebApp.Models;
using WebApp.Settings;
using WebApp.State;
using static System.Net.WebRequestMethods;
using static WebApp.Components.Pages.Admin.UserProfile;

namespace WebApp.Interfaces
{
    public class AccountApiClient
    {
        private readonly HttpClient _httpClient;

        public AccountApiClient(HttpClient httpClient, IJSRuntime jsRuntime)
        {
            _httpClient = httpClient;
        }

        public async Task<ApiResponseDto<LoginViewModel>> BuildLoginViewModel(string returnUrl)
        {
            return await _httpClient.PostJsonAsync<ApiResponseDto<LoginViewModel>>("/api/account/build-login-view-model", returnUrl);
        }

        public async Task<ApiResponseDto<LoginResponseModel>> Login(LoginInputModel parameters)
        {
            var response = await _httpClient.PostJsonAsync<ApiResponseDto<LoginResponseModel>>("/api/account/login", parameters);

            //if (AppState.Runtime == BlazorRuntime.Server)
            //if (AppState.Runtime == BlazorRuntime.Server)
            //    if (response.IsSuccessStatusCode)
            //        await SubmitServerForm("/server/login/", parameters);

            return response;
        }

        public async Task<ApiResponseDto> LoginWith2fa(LoginWith2faInputModel parameters)
        {
            var response = await _httpClient.PostJsonAsync<ApiResponseDto>("api/account/login-with2fa", parameters);

            //if (AppState.Runtime == BlazorRuntime.Server)
            //    if (response.Success)
            //        await SubmitServerForm("/server/loginwith2fa/", parameters);

            return response;
        }
        public async Task<ApiResponseDto> LoginWithRecoveryCode(LoginWithRecoveryCodeInputModel parameters)
        {
            var response = await _httpClient.PostJsonAsync<ApiResponseDto>("api/account/login-with-recovery-code", parameters);

            //if (AppState.Runtime == BlazorRuntime.Server)
            //    if (response.Success)
            //        await SubmitServerForm("/server/loginwith2fa/", parameters);

            return response;
        }

        public async Task<ApiResponseDto> Logout(string returnUrl = null)
        {
            var response = await _httpClient.PostJsonAsync<ApiResponseDto>("api/account/logout", null);

            if (response.IsSuccessStatusCode)
            {
                var logoutModel = new AccountFormModel() { ReturnUrl = returnUrl };

                //await SubmitServerForm("/server/logout/", logoutModel);
            }

            return response;
        }

        public async Task<ApiResponseDto> CreateUser(RegisterViewModel parameters)
        {
            return await _httpClient.PostJsonAsync<ApiResponseDto>("api/account/create", parameters);
        }

        public async Task<ApiResponseDto<LoginResponseModel>> Register(RegisterViewModel parameters)
        {
            return await _httpClient.PostJsonAsync<ApiResponseDto<LoginResponseModel>>("api/account/register", parameters);
        }

        public async Task<ApiResponseDto> ConfirmEmail(ConfirmEmailViewModel parameters)
        {
            return await _httpClient.PostJsonAsync<ApiResponseDto>("api/account/confirm-email", parameters);
        }

        public async Task<ApiResponseDto> ResetPassword(ResetPasswordViewModel parameters)
        {
            return await _httpClient.PostJsonAsync<ApiResponseDto>("api/account/reset-password", parameters);
        }

        public async Task<ApiResponseDto> UpdatePassword(UpdatePasswordViewModel parameters)
        {
            return await _httpClient.PostJsonAsync<ApiResponseDto>("api/account/update-password", parameters);
        }
        public async Task<ApiResponseDto> AdminChangePassword(ChangePasswordViewModel parameters)
        {
            return await _httpClient.PostJsonAsync<ApiResponseDto>($"api/account/addmin-user-password-reset", parameters);
        }

        public async Task<ApiResponseDto> ForgotPassword(ForgotPasswordViewModel parameters)
        {
            return await _httpClient.PostJsonAsync<ApiResponseDto>("api/account/forgot-password", parameters);
        }

        public async Task<ApiResponseDto<UserViewModel>> EnableAuthenticator(AuthenticatorVerificationCodeViewModel parameters)
        {
            return await _httpClient.PostJsonAsync<ApiResponseDto<UserViewModel>>("api/account/enable-authenticator", parameters);
        }
        public async Task<ApiResponseDto<UserViewModel>> DisableAuthenticator()
        {

            return await _httpClient.PostJsonAsync<ApiResponseDto<UserViewModel>>("api/account/disable-authenticator", null);
        }
        public async Task<ApiResponseDto<UserViewModel>> ForgetTwoFactorClient()
        {
            var response = await _httpClient.PostJsonAsync<ApiResponseDto<UserViewModel>>("api/account/forget-two-factor-client", null);

            //if (AppState.Runtime == BlazorRuntime.Server)
            //    if (response.Success)
            //        await SubmitServerForm("/server/ForgetTwoFactorClient/", new AccountFormModel());

            return response;
        }
        public async Task<ApiResponseDto<UserViewModel>> Enable2fa(string userId = null)
        {
            return await _httpClient.PostJsonAsync<ApiResponseDto<UserViewModel>>("api/account/enable2fa", userId);
        }
        public async Task<ApiResponseDto<UserViewModel>> Disable2fa(string userId = null)
        {
            return await _httpClient.PostJsonAsync<ApiResponseDto<UserViewModel>>("api/account/disable2fa", userId);
        }

        public async Task<UserViewModel> GetUserViewModel()
        {
            UserViewModel userViewModel = new() { IsAuthenticated = false };

            var apiResponse = await _httpClient.GetNewtonsoftJsonAsync<ApiResponseDto<UserViewModel>>("api/account/user-view-model");

            if (apiResponse.IsSuccessStatusCode)
                userViewModel = apiResponse.Result;

            return userViewModel;
        }

        public async Task<ApiResponseDto<UserViewModel>> GetUserViewModel(string id)
        {
            return await _httpClient.GetNewtonsoftJsonAsync<ApiResponseDto<UserViewModel>>($"api/account/user-view-model/{id}");
        }

        public async Task<UserViewModel> GetUser()
        {
            var apiResponse = await _httpClient.GetNewtonsoftJsonAsync<ApiResponseDto<UserViewModel>>("api/account/get-user");
            return apiResponse.Result;
        }

        public async Task<ApiResponseDto> UpdateUser(UserViewModel userViewModel)
        {
            return await _httpClient.PostJsonAsync<ApiResponseDto>("api/account/update-user", userViewModel);
        }

        public async Task<ApiResponseDto> UpsertUser(UserViewModel userViewModel)
        {
            return await _httpClient.PostJsonAsync<ApiResponseDto>("api/account/UpsertUser", userViewModel);
        }
        public async Task<ApiResponseDto> DeleteUser(string id)
        {
            return await _httpClient.DeleteAsync<ApiResponseDto>($"api/account/{id}");
        }

        public async Task<ApiResponseDto> DeleteMe()
        {
            return await _httpClient.DeleteAsync<ApiResponseDto>("api/account");
        }

        public async Task<ApiResponseDto> AdminUpdateUser(UserViewModel userViewModel)
        {
            return await _httpClient.PostJsonAsync<ApiResponseDto>("api/account/admin-update-user", userViewModel);
        }

        public async Task<UserProfileViewModel> GetUserProfile()
        {
            var apiResponse = await _httpClient.GetNewtonsoftJsonAsync<UserProfileViewModel>("api/admin/user-profile");
            return apiResponse;

        }

        public Task<ApiResponseDto> SendTestEmail(EmailViewModel email)
        {
            throw new NotImplementedException();
        }

        public Task<QueryResult<TenantSettingModels>> GetTenantSettings()
        {
            throw new NotImplementedException();
        }

        public async Task<ApiResponseDto<List<UserViewModel>>> GetUsers(int pageSize, int currentPage, string search)
        {
            return await _httpClient.GetJsonAsync<ApiResponseDto<List<UserViewModel>>>($"api/admin/users?pageSize={pageSize}&pageNumber={currentPage}&search={search}");
        }

        public Task<QueryResult<DbLog>> GetLogs(Expression<Func<DbLog, bool>> predicate = null, int? take = null, int? skip = null)
        {
            throw new NotImplementedException();
        }

        public Task<QueryResult<ApiLogItem>> GetApiLogs(Expression<Func<ApiLogItem, bool>> predicate = null, int? take = null, int? skip = null)
        {
            throw new NotImplementedException();
        }

        //public Task<QueryResult<ApplicationUser>> GetTodoCreators(ToDoFilter filter)
        //{
        //    throw new NotImplementedException();
        //}

        public Task<ApiResponseDto> SendTestEmail(EmailDto email)
        {
            throw new NotImplementedException();
        }

        public async Task<ApiResponseDto<TenantModel>> GetTenant()
        {
            return await _httpClient.GetJsonAsync<ApiResponseDto<TenantModel>>("api/admin/tenant");

        }

        public async Task<ApiResponseDto<List<TenantModel>>> GetListTenant(int pageSize, int pageNumber, string searchText)
        {
            return await _httpClient.GetNewtonsoftJsonAsync<ApiResponseDto<List<TenantModel>>>($"api/admin/tenants?pageSize={pageSize}&pageNumber={pageNumber}&pageSize={searchText}");

        }


        public async Task<ApiResponseDto> CreateNewTenant(TenantModel currentTenant)
        {
            return await _httpClient.PostJsonAsync<ApiResponseDto>("api/admin/create-tenant", currentTenant);

        }

        public async Task<ApiResponseDto> UpdateNewTenant(TenantModel currentTenant)
        {
            return await _httpClient.PutJsonAsync<ApiResponseDto>("api/admin/update-tenant", currentTenant);
        }

        public async Task<ApiResponseDto> DeleteTenant(string name)
        {
            return await _httpClient.DeleteAsync<ApiResponseDto>($"api/admin/tenant/delete/{name}");
        }
        public async Task<ApiResponseDto> DeleteRole(string id)
        {
            return await _httpClient.DeleteAsync<ApiResponseDto>($"api/admin/role/{id}");
        }

        public async Task<ApiResponseDto<List<RoleDto>>> GetRoles(int pageSize, int currentPage, string search)
        {
            return await _httpClient.GetJsonAsync<ApiResponseDto<List<RoleDto>>>($"api/admin/get-roles?pageSize={pageSize}&pageNumber={currentPage}&search={search}");
        }

        //public async Task<ApiResponseDto> UpdateRole(RoleDto request)
        //{
        //    return await _httpClient.PutJsonAsync<ApiResponseDto>("api/account/update-user", request);
        //}

        public async Task<ApiResponseDto> SaveRoleAsync(RoleDto request)
        {
            return await _httpClient.PostJsonAsync<ApiResponseDto>("api/account/update-role", request);
        }

        public async Task<ApiResponseDto<List<string>>> GetAllPermissions()
        {
            return await _httpClient.GetJsonAsync<ApiResponseDto<List<string>>>("api/admin/permissions");
        }

        public async Task<ApiResponseDto<RoleDto>> GetRoleByName(string roleName)
        {
            //Http.GetFromJsonAsync<ApiResponseDto<RoleDto>>($"api/admin/role/{roleName}")
            return await _httpClient.GetJsonAsync<ApiResponseDto<RoleDto>>($"api/admin/role/{roleName}");
        }

        public async Task<UserViewModel> GetUserById(string id)
        {
            var apiResponse = await _httpClient.GetNewtonsoftJsonAsync<ApiResponseDto<UserViewModel>>($"api/admin/user-by-id/{id}");
            return apiResponse.Result;
        }

        public async Task<ApiResponseDto> ToggleUserStatusAsync(ToggleUserStatusRequest request)
        {
            var apiResponse = await _httpClient.PutJsonAsync<ApiResponseDto>($"api/admin/toggle-user-status", request);
            return apiResponse;
        }

        //Task<IResult<UserRolesResponse>> GetRolesAsync(string userId);

        public async Task<ApiResponseDto<UserRolesResponse>> GetRolesByUserId(string userId)
        {
            var apiResponse = await _httpClient.GetJsonAsync<ApiResponseDto<UserRolesResponse>>($"api/admin/user-roles/{userId}");
            return apiResponse;
        }

        public async Task<ApiResponseDto> UpdateUserRolesAsync(UpdateUserRolesRequest request)
        {
            var apiResponse = await _httpClient.PutJsonAsync<ApiResponseDto>($"api/admin/user-roles", request);
            return apiResponse;
        }

        public async Task<ApiResponse<PermissionModel>> GetAllByRoleIdAsync(string roleId)
        {
            var apiResponse = await _httpClient.GetJsonAsync<ApiResponse<PermissionModel>>($"api/admin/get-all-permission-by-role-id/{roleId}");
            return apiResponse;
        }

        //public async Task<ApiResponseDto<RoleDto>> GetRoleByName(string roleName)
        //{
        //    //Http.GetFromJsonAsync<ApiResponseDto<RoleDto>>($"api/admin/role/{roleName}")
        //    return await _httpClient.GetJsonAsync<ApiResponseDto<RoleDto>>($"api/admin/role/{roleName}");
        //}

        //public async Task<ApiResponseDto<List<TenantDto>>> GetListTenant(int pageSize, int pageNumber, string searchText)
        //{
        //    return await _httpClient.GetNewtonsoftJsonAsync<ApiResponseDto<List<TenantDto>>($"api/admin/tenants?pageSize={pageSize}&pageNumber={pageNumber}&pageSize={searchText}");

        //}
    }
}