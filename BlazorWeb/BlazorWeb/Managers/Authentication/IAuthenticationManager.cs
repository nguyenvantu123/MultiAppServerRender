using System.Security.Claims;
using System.Threading.Tasks;

namespace BlazorWeb.Identity
{
    public interface IAuthenticationManager
    {

        Task<bool> Logout();

        Task<string> RefreshToken();

        Task<string> TryRefreshToken();

        Task<string> TryForceRefreshToken();

        Task<ClaimsPrincipal> CurrentUser();

    }
}