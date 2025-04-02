using BlazorIdentity.Files.Response;
using MultiAppServer.ServiceDefaults;

namespace BlazorIdentity.Files.Application.Commands
{
    public class CreateDocumentTypeCommand : IRequest<ApiResponseDto<bool>>
    {
        public string Name { get; set; }
        public string? Description { get; set; }

        public bool IsActive { get; set; }

        public string LinkUrl { get; set; }

        public bool FileActive { get; set; }
        // Add other properties as needed
    }
}
