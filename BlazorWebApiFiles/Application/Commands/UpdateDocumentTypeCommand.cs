using MultiAppServer.ServiceDefaults;

namespace BlazorIdentity.Files.Application.Commands
{
    public class UpdateDocumentTypeCommand : IRequest<ApiResponseDto<bool>>
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public bool IsActive { get; set; }
    }
}
