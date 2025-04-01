using BlazorIdentity.Files.Application.Commands;
using BlazorIdentity.Files.Entities;
using BlazorIdentityFiles.SeedWork;
using MultiAppServer.ServiceDefaults;

namespace BlazorIdentity.Files.CQRS.Command
{
    public class UpdateDocumentTypeCommandHandler : IRequestHandler<UpdateDocumentTypeCommand, ApiResponseDto<bool>>
    {
        private readonly IUnitOfWork _unitOfWork;

        public UpdateDocumentTypeCommandHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<ApiResponseDto<bool>> Handle(UpdateDocumentTypeCommand request, CancellationToken cancellationToken)
        {
            var documentType = await _unitOfWork.Repository<DocumentsType>().FindAsync(dt => dt.Id == request.Id);

            if (documentType == null)
            {
                return new ApiResponseDto<bool>(404, "Document type not found", false, 0);
            }

            documentType.Description = request.Description;
            documentType.IsActive = request.IsActive;

            _unitOfWork.Repository<DocumentsType>().Update(documentType);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return new ApiResponseDto<bool>(200, "Document type updated successfully", true, 0);
        }
    }
}