using AutoMapper;
using BlazorIdentity.Files.Application.Queries;
using BlazorIdentity.Files.Constant;
using BlazorIdentity.Files.Entities;
using BlazorIdentity.Files.Response;
using BlazorIdentityFiles.SeedWork;
using Microsoft.EntityFrameworkCore;
using MultiAppServer.ServiceDefaults;

namespace BlazorIdentity.Files.CQRS.Query
{
    public class GetUploadHistoryQueryHandler : IRequestHandler<GetUploadHistoryQuery, ApiResponseDto<List<UploadHistoryResponse>>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public GetUploadHistoryQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<ApiResponseDto<List<UploadHistoryResponse>>> Handle(GetUploadHistoryQuery request, CancellationToken cancellationToken)
        {
            var data = await _unitOfWork.Repository<DocumentsFiles>().GetQuery().Where(x => x.DocumentsTypeId == request.Id).Select(x => new UploadHistoryResponse
            {
                Id = x.Id,
                Name = x.Name,
                IsActive = x.IsActive,
                InsertedAt = x.InsertedAt!.Value,
            }).OrderBy(x => x.IsActive).ToListAsync();


            return new ApiResponseDto<List<UploadHistoryResponse>>(200, "Success", data, 0);
        }
    }
}