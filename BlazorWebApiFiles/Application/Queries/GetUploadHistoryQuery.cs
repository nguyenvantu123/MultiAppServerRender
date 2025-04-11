using BlazorIdentity.Files.Application.Queries._base;
using BlazorIdentity.Files.Response;
using MultiAppServer.ServiceDefaults;

namespace BlazorIdentity.Files.Application.Queries
{
    public class GetUploadHistoryQuery : GetListBase, IRequest<ApiResponseDto<List<UploadHistoryResponse>>>
    {
        public Guid Id { get; set; }
    }
}

