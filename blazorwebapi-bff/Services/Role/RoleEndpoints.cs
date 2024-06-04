using BlazorWebApi.Bff.ApiClients;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace BlazorWebApi.Bff.Services.Role
{
    public static class RoleEndpoints
    {

        public static IEndpointRouteBuilder RoleEndpoint(this IEndpointRouteBuilder builder)
        {
            var user = builder.MapGroup("/role");

            user.MapPost("", async ([FromServices] IMediator mediator, CancellationToken cancellationToken) =>
            {
                var result = await mediator.Send(user, cancellationToken);

                return Results.Ok(result);
            });


            //login.MapPost("/refreshToken", async ([FromBody] RefreshRequest loginRequest, [FromServices] IMediator mediator, CancellationToken cancellationToken) =>
            //{
            //    var result = await mediator.Send(login, cancellationToken);

            //    return Results.Ok(result);
            //});

            return builder;
        }

        //private static async Task<IResult> Login(IIdentityApiClient identityApiClient, LoginRequest loginRequest)
        //{

        //}
    }

}
