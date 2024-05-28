using MediatR;

namespace BlazorWebApi.Users.Request.User
{
    public class CreateUserRequest
    {
    }

    public class RegisterUserRequest
    {
        public string Email { get; init; }

        public string UserName { get; init; }

        public string FirstName { get; init; }

        public string LastName { get; init; }

        public string Password { get; init; }

        public string VerifyPassword { get; init; }

        public string PhoneNumber { get; init; }

    }
}
