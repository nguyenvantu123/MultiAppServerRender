﻿using BlazorWebApi.Users.Domain.Models;
using BlazorWebApi.Users.Helper;
using BlazorWebApi.Users.Response.Identity;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using BlazorWebApi.Users.Request.Identity;
using ServiceDefaults;

namespace BlazorWebApi.Users.Controller
{

    [Authorize]
    public class IdentityController(
        UserManager<User> userManager,
        RoleManager<Role> roleManager,
        IOptions<AppConfiguration> appConfig,
        SignInManager<User> signInManager)
        : BaseApiController<IdentityController>
    {
        private readonly AppConfiguration _appConfig = appConfig.Value;
        private readonly SignInManager<User> _signInManager = signInManager;

        //private readonly IStringLocalizer<IdentityControllerBase> _localizer;

        //,IStringLocalizer<IdentityControllerBase> localizer
        //_localizer = localizer;

        /// <summary>
        /// Login admin page.
        /// </summary>
        /// <returns></returns>
        [AllowAnonymous]
        [HttpPost]
        [Route("/identity/token")]
        public async Task<ResultBase<TokenResponse>> LoginAsync([FromBody] TokenRequest model)
        {
            var user = await userManager.FindByNameAsync(model.UserName);

            var result = new ResultBase<TokenResponse>();

            if (user == null)
            {
                result.ErrorMessages.Add("User Not Found.");
                return result;
            }

            if (!user.IsActive)
            {
                result.ErrorMessages.Add("User Not Active. Please contact the administrator.");
                return result;
            }

            //if (!user.EmailConfirmed)
            //{
            //    result.ErrorMessages.Add("User Not Active. Please contact the administrator.");
            //    return result;
            //}
            var passwordValid = await userManager.CheckPasswordAsync(user, model.Password);
            if (!passwordValid)
            {
                result.ErrorMessages.Add("Invalid Credentials.");
                return result;
            }

            user.RefreshToken = GenerateRefreshToken();
            user.RefreshTokenExpiryTime = DateTime.Now.AddDays(7);
            await userManager.UpdateAsync(user);

            DateTime expireTime = DateTime.UtcNow.AddHours(3);
            var token = await GenerateJwtAsync(user, expireTime);
            var response = new TokenResponse
            { AccessToken = token, RefreshToken = user.RefreshToken, ExpireIn = expireTime };

            result.Success = true;
            result.Result = response;

            return result;
        }

        [NonAction]
        private async Task<string> GenerateJwtAsync(User user, DateTime expireTime)
        {
            var token = GenerateEncryptedToken(GetSigningCredentials(), await GetClaimsAsync(user), expireTime);
            return token;
        }

        [HttpPost]
        [Route("/identity/refreshToken")]
        public async Task<ResultBase<TokenResponse>> RefreshTokenAsync([FromBody] RefreshTokenRequest model)
        {

            var result = new ResultBase<TokenResponse>();

            if (model is null)
            {
                //result.ErrorMessages.Add("Invalid Client Token.");
                result.ErrorMessages.Add("Invalid Client Token.");
            }

            if (model != null)
            {
                var userPrincipal = GetPrincipalFromExpiredToken(model.AccessToken);
                var userEmail = userPrincipal.FindFirstValue(ClaimTypes.Email);
                if (userEmail != null)
                {
                    var user = await userManager.FindByEmailAsync(userEmail);
                    if (user == null)
                        result.ErrorMessages.Add("User Not Found.");
                    //return await ResultBase<TokenResponse>.FailAsync("User Not Found.");
                    if (user != null && (user.RefreshToken != model.RefreshToken ||
                                         user.RefreshTokenExpiryTime <= DateTime.Now))
                        //return await ResultBase<TokenResponse>.FailAsync("Invalid Client Token.");
                        result.ErrorMessages.Add("Invalid Client Token.");

                    if (result.ErrorMessages.Any())
                    {
                        result.Success = false;

                        return result;
                    }

                    DateTime expireTime = DateTime.UtcNow.AddHours(1);
                    var token = GenerateEncryptedToken(GetSigningCredentials(), await GetClaimsAsync(user), expireTime);
                    user.RefreshToken = GenerateRefreshToken();
                    await userManager.UpdateAsync(user);

                    var response = new TokenResponse
                    {
                        AccessToken = token,
                        RefreshToken = user.RefreshToken,
                        ExpireIn = user.RefreshTokenExpiryTime.Value
                    };

                    result.Success = true;
                    result.Result = response;
                }
            }

            return result;

        }

        [NonAction]
        private string GenerateEncryptedToken(SigningCredentials signingCredentials, IEnumerable<Claim> claims,
            DateTime expireTime)
        {
            var token = new JwtSecurityToken(
                claims: claims,
                expires: expireTime,
                signingCredentials: signingCredentials);
            var tokenHandler = new JwtSecurityTokenHandler();
            var encryptedToken = tokenHandler.WriteToken(token);
            return encryptedToken;
        }

        [NonAction]
        private ClaimsPrincipal GetPrincipalFromExpiredToken(string token)
        {
            var tokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_appConfig.Secret)),
                ValidateIssuer = false,
                ValidateAudience = false,
                RoleClaimType = ClaimTypes.Role,
                ClockSkew = TimeSpan.Zero,
                ValidateLifetime = false
            };
            var tokenHandler = new JwtSecurityTokenHandler();
            var principal = tokenHandler.ValidateToken(token, tokenValidationParameters, out var securityToken);
            if (securityToken is not JwtSecurityToken jwtSecurityToken || !jwtSecurityToken.Header.Alg.Equals(
                    SecurityAlgorithms.HmacSha256,
                    StringComparison.InvariantCultureIgnoreCase))
            {
                throw new SecurityTokenException("Invalid token");
            }

            return principal;
        }

        [NonAction]
        private string GenerateRefreshToken()
        {
            var randomNumber = new byte[32];
            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(randomNumber);
            return Convert.ToBase64String(randomNumber);
        }

        [NonAction]
        private SigningCredentials GetSigningCredentials()
        {
            var secret = Encoding.UTF8.GetBytes(_appConfig.Secret);
            return new SigningCredentials(new SymmetricSecurityKey(secret), SecurityAlgorithms.HmacSha256);
        }

        [NonAction]
        private async Task<IEnumerable<Claim>> GetClaimsAsync(User user)
        {
            var userClaims = await userManager.GetClaimsAsync(user);
            var roles = await userManager.GetRolesAsync(user);
            var roleClaims = new List<Claim>();
            var permissionClaims = new List<Claim>();
            foreach (var role in roles)
            {
                roleClaims.Add(new Claim(ClaimTypes.Role, role));
                var thisRole = await roleManager.FindByNameAsync(role);
                var allPermissionsForThisRoles = await roleManager.GetClaimsAsync(thisRole!);
                permissionClaims.AddRange(allPermissionsForThisRoles);
            }

            var claims = new List<Claim>
                    {
                        new(ClaimTypes.NameIdentifier, user.Id.ToString()),
                        new(ClaimTypes.Email, user.Email!),
                        new(ClaimTypes.Name, user.FirstName),
                        new(ClaimTypes.Surname, user.LastName),
                        new(ClaimTypes.MobilePhone, user.PhoneNumber ?? string.Empty)
                    }
                .Union(userClaims)
                .Union(roleClaims)
                .Union(permissionClaims);

            return claims;
        }
    }
}
