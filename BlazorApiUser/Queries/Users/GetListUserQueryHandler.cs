
// Regular CommandHandler
using AutoMapper;
using BlazorApiUser.Models;
using BlazorApiUser.Repository;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using MultiAppServer.ServiceDefaults;
using System.Net;
using System.Security.Claims;


namespace BlazorApiUser.Queries.Users;

public class GetListUserQueryHandler : IRequestHandler<GetListUserQuery, Tuple<int, List<UserDataViewModel>>>
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IMapper _autoMapper;

    public GetListUserQueryHandler( UserManager<ApplicationUser> userManager, IMapper autoMapper)
    {
        _userManager = userManager;
        _autoMapper = autoMapper;
    }

    /// <summary>
    /// Handler which processes the command when
    /// customer executes cancel order from app
    /// </summary>
    /// <param name="command"></param>
    /// <returns></returns>
    public async Task<Tuple<int, List<UserDataViewModel>>> Handle(GetListUserQuery command, CancellationToken cancellationToken)
    {
        var userList = _userManager.Users.Include(x => x.UserRoles).AsQueryable();
        var count = userList.Count();
        var listUsers = await userList.OrderBy(x => x.Id).Skip(command.pageNumber * command.pageSize).Take(command.pageSize).ToListAsync();

        var userDtoList = _autoMapper.Map<List<UserDataViewModel>>(listUsers);

        return new Tuple<int, List<UserDataViewModel>>(count, userDtoList);

        //return new Tuple<int, string>(200, $"User {user.UserName} deleted");
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
