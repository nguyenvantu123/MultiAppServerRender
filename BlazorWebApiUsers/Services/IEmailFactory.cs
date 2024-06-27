using BlazorBoilerplate.Constants;
using BlazorWebApi.Users.Models;
using BlazorWebApi.Users.Models.Email;

namespace BlazorWebApi.Users.Services
{
    public interface IEmailFactory
    {
        EmailMessageDto BuildTestEmail(string recipient);
        EmailMessageDto GetPlainTextTestEmail(DateTime date);
        EmailMessageDto BuildNewUserConfirmationEmail(string fullName, string userName, string callbackUrl);
        EmailMessageDto BuildNewUserEmail(string fullName, string userName, string emailAddress, string password);
        EmailMessageDto BuilNewUserNotificationEmail(string creator, string name, string userName, string company, string roles);
        EmailMessageDto BuildForgotPasswordEmail(string name, string callbackUrl, string token);
        EmailMessageDto BuildPasswordResetEmail(string userName);

        Task<ApiResponse> SendTestEmail(EmailDto parameters);
        Task<ApiResponse> Receive();
        Task<ApiResponse> QueueEmail(EmailMessageDto emailMessage, EmailType emailType);
        Task<ApiResponse> SendEmail(EmailMessageDto emailMessage);
        List<EmailMessageDto> ReceiveEmail(int maxCount = 10);
        Task<ApiResponse> ReceiveMailImapAsync();
        Task<ApiResponse> ReceiveMailPopAsync(int min = 0, int max = 0);
    }
}
