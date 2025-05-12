using MultiAppServer.ServiceDefaults;

namespace BlazorIdentity.Files.Application.Commands
{
    public class UploadAgainCommand : IRequest<ApiResponseDto<string>>
    {
        public Guid Id { get; set; }
       
        public string FilePath { get; set; }
    }
}
