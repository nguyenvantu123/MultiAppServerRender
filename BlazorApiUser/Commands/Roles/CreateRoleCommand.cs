using MediatR;
using System.ComponentModel.DataAnnotations;

namespace BlazorApiUser.Commands.Users;

public record CreateRoleCommand : IRequest<Tuple<int, string>>
{

    public string Name { get; set; }

    //public List<string> Permissions { get; set; }

    //public string FormattedPermissions
    //{
    //    get
    //    {
    //        return String.Join(", ", Permissions.ToArray());
    //    }
    //}

    public Guid Id { get; set; }
}
