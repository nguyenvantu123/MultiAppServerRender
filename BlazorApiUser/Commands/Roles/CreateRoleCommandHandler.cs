﻿
// Regular CommandHandler
using BlazorApiUser.Extensions;
using BlazorApiUser.Models;
using BlazorApiUser.Repository;
using MediatR;
using Microsoft.AspNetCore.Identity;
using System.Net;
using System.Security.Claims;


namespace BlazorApiUser.Commands.Users;

public class CreateRoleCommandHandler : IRequestHandler<CreateRoleCommand, Tuple<int, string>>
{
    private readonly RoleManager<ApplicationRole> _roleManager;

    public CreateRoleCommandHandler(RoleManager<ApplicationRole> roleManager)
    {
        _roleManager = roleManager;
    }

    /// <summary>
    /// Handler which processes the command when
    /// customer executes cancel order from app
    /// </summary>
    /// <param name="command"></param>
    /// <returns></returns>
    public async Task<Tuple<int, string>> Handle(CreateRoleCommand command, CancellationToken cancellationToken)
    {

        if (_roleManager.Roles.Any(r => r.Name == command.Name))
        {
            return new Tuple<int, string>(400, $"Role {command.Name} already exists");
        }

        var result = await _roleManager.CreateAsync(new ApplicationRole(command.Name, command.Description));

        if (!result.Succeeded)
        {
            var msg = result.GetErrors();
            return new Tuple<int, string>(400, msg);
        }

        return new Tuple<int, string>(200, $"Role {command.Name} created");

    }
}


//// Use for Idempotency in Command process
//public class CancelOrderIdentifiedCommandHandler : IdentifiedCommandHandler<CancelOrderCommand, bool>
//{
//    public CancelOrderIdentifiedCommandHandler(
//        IMediator mediator,
//        IRequestManager requestManager,
//        ILogger<IdentifiedCommandHandler<CancelOrderCommand, bool>> logger)
//        : base(mediator, requestManager, logger)
//    {
//    }

//    protected override bool CreateResultForDuplicateRequest()
//    {
//        return true; // Ignore duplicate requests for processing order.
//    }
//}
