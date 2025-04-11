using BlazorIdentity.Files.Application.Commands;
using BlazorIdentity.Files.Entities;
using BlazorIdentityFiles.Application.Commands;
using BlazorIdentityFiles.SeedWork;
using Minio;
using Minio.DataModel.Args;
using MultiAppServer.ServiceDefaults;
using Shared;

namespace BlazorIdentity.Files.CQRS.Command
{
    public class UploadAgainCommandHandler : IRequestHandler<UploadAgainCommand, ApiResponseDto<string>>
    {
        private readonly IUnitOfWork _unitOfWork;

        public UploadAgainCommandHandler(IUnitOfWork unitOfWork, IMinioClient minioClient)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<ApiResponseDto<string>> Handle(UploadAgainCommand request, CancellationToken cancellationToken)
        {

            List<DocumentsFiles> documentsFiles = _unitOfWork.Repository<DocumentsFiles>().GetQuery().Where(x => x.DocumentsTypeId == request.Id).ToList();


            foreach (var item in documentsFiles)
            {
                item.IsActive = false;

                _unitOfWork.Repository<DocumentsFiles>().Update(item);

            }

            DocumentsFiles documentsFilesNew = new DocumentsFiles
            {
                DocumentsTypeId = request.Id,
                Name = request.FilePath,
                FilePath = request.FilePath,
                IsActive = true,
            };

            _unitOfWork.Repository<DocumentsFiles>().Add(documentsFilesNew);

            await _unitOfWork.SaveChangesAsync();

            return new ApiResponseDto<string>(200, request.FilePath, "");
        }
    }
}
