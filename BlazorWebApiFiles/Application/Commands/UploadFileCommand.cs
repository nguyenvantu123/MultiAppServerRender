using BlazorWebApi.Files.Constant;

namespace BlazorWebApiFiles.Application.Commands;

public record UploadFileCommand(IFormFile FormFile, FileType FileType) : IRequest<bool>;

