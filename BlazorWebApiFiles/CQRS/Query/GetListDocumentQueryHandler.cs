using AutoMapper;
using BlazorIdentity.Files.Application.Queries;
using BlazorIdentity.Files.Entities;
using BlazorIdentity.Files.Response;
using BlazorIdentityFiles.SeedWork;
using Microsoft.EntityFrameworkCore;
using MultiAppServer.ServiceDefaults;

namespace BlazorIdentity.Files.CQRS.Query
{
    public class GetListDocumentQueryHandler : IRequestHandler<GetListDocumentQuery, ApiResponseDto<List<DocumentResponse>>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public GetListDocumentQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<ApiResponseDto<List<DocumentResponse>>> Handle(GetListDocumentQuery request, CancellationToken cancellationToken)
        {
            var query = _unitOfWork.Repository<DocumentsType>().GetQuery();

            if (!string.IsNullOrEmpty(request.Name))
            {
                query = query.Where(x => x.Name.Contains(request.Name));
            }

            if (request.IsActive.HasValue)
            {
                query = query.Where(x => x.IsActive == request.IsActive);
            }

            var count = await query.CountAsync(cancellationToken);

            if (request.PageNumber > 0)
            {
                query = query.Skip((request.PageNumber.Value - 1) * request.PageSize!.Value).Take(request.PageSize!.Value);
            }

            var documents = await query.ToListAsync(cancellationToken);
            var data = _mapper.Map<List<DocumentResponse>>(documents);

            return new ApiResponseDto<List<DocumentResponse>>(200, "Success", data, count);
        }
    }
}