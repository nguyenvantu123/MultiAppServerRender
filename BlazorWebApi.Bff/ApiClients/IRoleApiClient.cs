using Microsoft.AspNetCore.Mvc;
using Refit;
using System.ComponentModel.DataAnnotations;

namespace BlazorWebApi.Bff.ApiClients
{
    public interface IRoleApiClient
    {
        [Post("/user/roles")]
        public Task<dynamic> GetRoleAsync();

    }


    public record class RoleResponse
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
    }

}
