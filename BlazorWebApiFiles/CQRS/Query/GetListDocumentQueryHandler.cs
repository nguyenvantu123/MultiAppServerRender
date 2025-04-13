using AutoMapper;
using BlazorIdentity.Files.Application.Queries;
using BlazorIdentity.Files.Constant;
using BlazorIdentity.Files.Entities;
using BlazorIdentity.Files.Response;
using BlazorIdentityFiles.SeedWork;
using Microsoft.EntityFrameworkCore;
using Minio;
using Minio.DataModel.Args;
using MultiAppServer.ServiceDefaults;
using System.IO;

namespace BlazorIdentity.Files.CQRS.Query
{
    public class GetListDocumentQueryHandler : IRequestHandler<GetListDocumentQuery, ApiResponseDto<List<DocumentResponse>>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IMinioClient _minioClient;

        public GetListDocumentQueryHandler(IUnitOfWork unitOfWork, IMapper mapper, IMinioClient minioClient)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _minioClient = minioClient;
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

            List<DocumentsType> documents = await query.Include(dt => dt.DocumentsFiles.Where(df => df.IsActive)).ToListAsync(cancellationToken);

            var data = _mapper.Map<List<DocumentResponse>>(documents);

            foreach (var item in data)
            {
                if (!string.IsNullOrEmpty(item.LinkUrl))
                {
                    PresignedGetObjectArgs presignedGetObjectArgs = new PresignedGetObjectArgs()
                        .WithBucket("multiappserver")
                        .WithObject(item.LinkUrl)
                        .WithExpiry(60 * 60 * 3); // 1 day

                    item.LinkUrl = await _minioClient.PresignedGetObjectAsync(presignedGetObjectArgs);

                }
            }

            return new ApiResponseDto<List<DocumentResponse>>(200, "Success", data, count);
        }
    }
}