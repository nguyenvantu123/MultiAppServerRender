using BlazorWebApi.Users.Models;
using Breeze.Sharp;
using MultiAppServer.ServiceDefaults;
using System.Linq.Expressions;
using WebApp.Components.Pages.Admin;
using WebApp.DataModels;
using WebApp.Models;
using WebApp.Settings;

namespace WebApp.Interfaces
{
    public interface IAccountApiClient
    {
        Task<ApiResponseDto<LoginViewModel>> BuildLoginViewModel(string returnUrl);
        Task<ApiResponseDto<LoginResponseModel>> Login(LoginInputModel parameters);
        Task<ApiResponseDto> LoginWith2fa(LoginWith2faInputModel parameters);
        Task<ApiResponseDto> LoginWithRecoveryCode(LoginWithRecoveryCodeInputModel parameters);
        Task<ApiResponseDto> CreateUser(RegisterViewModel parameters);
        Task<ApiResponseDto<LoginResponseModel>> Register(RegisterViewModel parameters);
        Task<ApiResponseDto> ForgotPassword(ForgotPasswordViewModel parameters);
        Task<ApiResponseDto> ResetPassword(ResetPasswordViewModel parameters);
        Task<ApiResponseDto> UpdatePassword(UpdatePasswordViewModel parameters);
        Task<ApiResponseDto> AdminChangePassword(ChangePasswordViewModel parameters);
        Task<ApiResponseDto> Logout(string returnUrl = null);
        Task<ApiResponseDto> ConfirmEmail(ConfirmEmailViewModel parameters);
        Task<UserViewModel> GetUserViewModel();
        Task<ApiResponseDto<UserViewModel>> GetUserViewModel(string id);
        Task<ApiResponseDto> UpdateUser(UserViewModel userViewModel);
        Task<ApiResponseDto> UpsertUser(UserViewModel userViewModel);
        Task<ApiResponseDto> DeleteUser(string id);
        Task<ApiResponseDto> DeleteMe();
        Task<ApiResponseDto> AdminUpdateUser(UserViewModel userViewModel);
        Task<UserViewModel> GetUser();

        Task<ApiResponseDto<UserViewModel>> EnableAuthenticator(AuthenticatorVerificationCodeViewModel parameters);
        Task<ApiResponseDto<UserViewModel>> DisableAuthenticator();
        Task<ApiResponseDto<UserViewModel>> ForgetTwoFactorClient();
        Task<ApiResponseDto<UserViewModel>> Enable2fa(string userId = null);
        Task<ApiResponseDto<UserViewModel>> Disable2fa(string userId = null);

        Task<UserProfileViewModel> GetUserProfile();
        Task<QueryResult<TenantSettingModels>> GetTenantSettings();
        Task<QueryResult<ApplicationUser>> GetUsers(Expression<Func<ApplicationUser, bool>> predicate = null, int? take = null, int? skip = null);
        Task<QueryResult<ApplicationRole>> GetRoles(Expression<Func<ApplicationRole, bool>> predicate = null, int? take = null, int? skip = null);

        Task<QueryResult<DbLog>> GetLogs(Expression<Func<DbLog, bool>> predicate = null, int? take = null, int? skip = null);
        Task<QueryResult<ApiLogItem>> GetApiLogs(Expression<Func<ApiLogItem, bool>> predicate = null, int? take = null, int? skip = null);

        //Task<QueryResult<Todo>> GetToDos(ToDoFilter filter, int? take = null, int? skip = null);
        //Task<QueryResult<ApplicationUser>> GetTodoCreators(ToDoFilter filter);
        //Task<QueryResult<ApplicationUser>> GetTodoEditors(ToDoFilter filter);

        Task<ApiResponseDto> SendTestEmail(EmailDto email);

        Task<ApiResponseDto<TenantDto>> GetTenant();

        Task<ApiResponseDto<List<TenantDto>>> GetListTenant(int pageSize, int pageNumber, string searchText);

        Task<ApiResponseDto> CreateNewTenant(TenantDto currentTenant);

        Task<ApiResponseDto> UpdateNewTenant(TenantDto currentTenant);

        Task<ApiResponseDto> DeleteTenant(string name);
    }
}