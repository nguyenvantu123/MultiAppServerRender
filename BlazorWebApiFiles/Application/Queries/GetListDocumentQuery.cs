using BlazorIdentity.Files.Application.Queries._base;
using BlazorIdentity.Files.Response;
using MultiAppServer.ServiceDefaults;

namespace BlazorIdentity.Files.Application.Queries
{
    public class GetListDocumentQuery : GetListBase, IRequest<ApiResponseDto<List<DocumentResponse>>>
    {

        public string? Name { get; set; }

        public bool? IsActive { get; set; }

    }
}
