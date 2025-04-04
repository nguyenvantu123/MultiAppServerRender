using BlazorIdentity.Files.Constant;
using MultiAppServer.ServiceDefaults;

namespace BlazorIdentityFiles.Application.Commands;

public record UploadFileCommand : IRequest<ApiResponseDto<string>>
{
    public MultipartFormDataContent Content { get; init; }
}

