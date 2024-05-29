using Microsoft.AspNetCore.Mvc;
using Refit;
using System.ComponentModel.DataAnnotations;

namespace BlazorWebApi.Bff.ApiClients
{
    public interface IRoleApiClient
    {
        [Post("/role")]
        public Task<object> GetRoleAsync();

    }


    public record class RoleResponse
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
    }

}
