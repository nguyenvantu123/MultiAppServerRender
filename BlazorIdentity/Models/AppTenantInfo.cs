using Finbuckle.MultiTenant.Abstractions;

namespace BlazorIdentity.Users.Models;

public class AppTenantInfo : ITenantInfo
{
    public string Id { get; set; }
    public string Identifier { get; set; }
    public string Name { get; set; }
    public string ConnectionString { get; set; }
}