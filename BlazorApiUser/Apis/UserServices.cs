

using BlazorIdentity.Users.Models;
using MediatR;
using Microsoft.AspNetCore.Identity;

public class UserServices(
    IMediator mediator,
    ILogger<UserServices> logger,

    UserManager<ApplicationUser> userManager,
    RoleManager<ApplicationRole> roleManager)
{
    public IMediator Mediator { get; set; } = mediator;
    public ILogger<UserServices> Logger { get; } = logger;

    public UserManager<ApplicationUser> UserManager { get; } = userManager;
    public RoleManager<ApplicationRole> RoleManager { get; } = roleManager;
}
