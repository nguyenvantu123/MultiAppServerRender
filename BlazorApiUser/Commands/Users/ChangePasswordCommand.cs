using MediatR;
using System.ComponentModel.DataAnnotations;

namespace BlazorApiUser.Commands.Users
{
    public record ChangePasswordCommand 
    {
        public string UserId { get; set; }
    }
}