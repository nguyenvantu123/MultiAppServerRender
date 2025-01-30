namespace BlazorIdentityFiles.Exceptions;

/// <summary>
/// Exception type for domain exceptions
/// </summary>
public class FilesDomainException : Exception
{
    public FilesDomainException()
    { }

    public FilesDomainException(string message)
        : base(message)
    { }

    public FilesDomainException(string message, Exception innerException)
        : base(message, innerException)
    { }
}
