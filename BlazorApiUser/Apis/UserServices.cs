

using AutoMapper;
using BlazorApiUser.Repository;
using BlazorIdentity.Users.Data;
using BlazorIdentity.Users.Models;
using MediatR;
using Microsoft.AspNetCore.Identity;

public class UserServices(
    IMediator mediator,
    ILogger<UserServices> logger,

    UserManager<ApplicationUser> userManager,
    RoleManager<ApplicationRole> roleManager,
    TenantStoreDbContext tenantStoreDbContext,
    IMapper mapper,
    IHttpContextAccessor httpContextAccessor)
{
    public IMediator Mediator { get; set; } = mediator;
    public ILogger<UserServices> Logger { get; } = logger;

    public UserManager<ApplicationUser> UserManager { get; } = userManager;
    public RoleManager<ApplicationRole> RoleManager { get; } = roleManager;

    public TenantStoreDbContext TenantStoreDbContext { get; } = tenantStoreDbContext;

    public IMapper Mapper { get; set; } = mapper;
    public IHttpContextAccessor HttpContextAccessor { get; set; } = httpContextAccessor;

}
