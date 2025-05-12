using BlazorIdentityFiles.Application.Commands;
using Minio;
using Minio.DataModel.Args;
using MultiAppServer.ServiceDefaults;
using Shared;

namespace BlazorIdentity.Files.CQRS.Command
{
    public class UploadFileCommandHandler : IRequestHandler<UploadFileCommand, ApiResponseDto<string>>
    {
        private readonly HttpClient _httpClient;
        private readonly IMinioClient _minioClient;

        public UploadFileCommandHandler(HttpClient httpClient, IMinioClient minioClient)
        {
            _httpClient = httpClient;
            _minioClient = minioClient;
        }

        public async Task<ApiResponseDto<string>> Handle(UploadFileCommand request, CancellationToken cancellationToken)
        {
            var memoryStream = new MemoryStream();
            await request.Content.CopyToAsync(memoryStream);
            memoryStream.Position = 0;

            var fileName = SharedExtensions.GetFileNameFromMultipartContent(request.Content);
            var contentType = SharedExtensions.GetContentType(fileName);

            PutObjectArgs putObjectArgs = new PutObjectArgs()
                                      .WithBucket("multiappserver")
                                      .WithStreamData(memoryStream)
                                      .WithObject(fileName)
                                      .WithObjectSize(memoryStream.Length)
                                      .WithContentType(contentType);


            var dataUpload = await _minioClient.PutObjectAsync(putObjectArgs);

            if (string.IsNullOrEmpty(dataUpload.Etag))
            {
                return new ApiResponseDto<string>(400, "File uploaded failed!!!", fileName);
            }
            return new ApiResponseDto<string>(200, "File upload success!!!", fileName);
        }
    }
}
