namespace BlazorIdentityFiles.Exceptions;

/// <summary>
/// Exception type for domain exceptions
/// </summary>
public class UsersDomainException : Exception
{
    public UsersDomainException()
    { }

    public UsersDomainException(string message)
        : base(message)
    { }

    public UsersDomainException(string message, Exception innerException)
        : base(message, innerException)
    { }
}
