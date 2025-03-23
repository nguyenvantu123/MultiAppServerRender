

using AutoMapper;
using BlazorApiUser.Db;
using BlazorApiUser.Models;
using BlazorApiUser.Repositories;
using BlazorApiUser.Repository;

using MediatR;
using Microsoft.AspNetCore.Identity;

public class UserServices(
    IMediator mediator,
    ILogger<UserServices> logger,

    UserManager<ApplicationUser> userManager,
    RoleManager<ApplicationRole> roleManager,
    SignInManager<ApplicationUser> signInManager,

    TenantStoreDbContext tenantStoreDbContext,
    IMapper mapper,
    IHttpContextAccessor httpContextAccessor,
    RedisUserRepository redisUserRepository,
    ApplicationDbContext applicationDbContext)
{
    public IMediator Mediator { get; set; } = mediator;
    public ILogger<UserServices> Logger { get; } = logger;

    public UserManager<ApplicationUser> UserManager { get; } = userManager;
    public RoleManager<ApplicationRole> RoleManager { get; } = roleManager;
    public SignInManager<ApplicationUser> SignInManager { get; } = signInManager;

    public TenantStoreDbContext TenantStoreDbContext { get; } = tenantStoreDbContext;

    public IMapper Mapper { get; set; } = mapper;
    public IHttpContextAccessor HttpContextAccessor { get; set; } = httpContextAccessor;

    public RedisUserRepository RedisUserRepository { get; set; } = redisUserRepository;

    public ApplicationDbContext ApplicationDbContext { get; set; } = applicationDbContext;

}
