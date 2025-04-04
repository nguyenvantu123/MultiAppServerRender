using MultiAppServer.ServiceDefaults;

namespace BlazorIdentity.Files.Application.Commands
{
    public class UpdateDocumentTypeCommand : IRequest<ApiResponseDto<bool>>
    {
        public string Description { get; set; }
        public bool IsActive { get; set; }

        public Guid Id { get; set; }
    }
}
