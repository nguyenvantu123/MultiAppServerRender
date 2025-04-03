using BlazorIdentity.Files.Application.Commands;
using BlazorIdentity.Files.Entities;
using BlazorIdentityFiles.SeedWork;
using MultiAppServer.ServiceDefaults;

namespace BlazorIdentity.Files.CQRS.Command
{
    public class CreateDocumentTypeCommandHandler : IRequestHandler<CreateDocumentTypeCommand, ApiResponseDto<bool>>
    {
        private readonly IUnitOfWork _unitOfWork;

        public CreateDocumentTypeCommandHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<ApiResponseDto<bool>> Handle(CreateDocumentTypeCommand request, CancellationToken cancellationToken)
        {
            var existingDocumentType = await _unitOfWork.Repository<DocumentsType>()
                   .FindAsync(dt => dt.Name == request.Name);

            if (existingDocumentType != null)
            {
                return new ApiResponseDto<bool>(400, "Document type with the same name already exists", false, 0);
            }

            var documentType = new DocumentsType
            {
                Name = request.Name,
                Description = request.Description!,
                IsActive = request.IsActive,
                Code = ""
            };

            _unitOfWork.Repository<DocumentsType>().Add(documentType);

            var documentsFiles = new DocumentsFiles
            {
                Name = request.Name,
                IsActive = request.FileActive,
                FilePath = request.LinkUrl,
                DocumentsTypeId = documentType.Id,
            };

            _unitOfWork.Repository<DocumentsFiles>().Add(documentsFiles);

            await _unitOfWork.SaveChangesAsync(cancellationToken);


            return new ApiResponseDto<bool>(200, "Document type created successfully", true, 0);
        }
    }
}