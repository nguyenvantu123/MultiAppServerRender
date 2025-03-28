﻿using BlazorIdentity.Files.Entities;
using BlazorIdentity.Repository;
using BlazorIdentityFiles.SeedWork;

public class FileServices(
    IMediator mediator,
    IFilesQueries queries,
    IIdentityService identityService,
    ILogger<FileServices> logger,
    IUnitOfWork unitOfWork)
{
    public IMediator Mediator { get; set; } = mediator;
    public ILogger<FileServices> Logger { get; } = logger;
    public IFilesQueries Queries { get; } = queries;
    public IIdentityService IdentityService { get; } = identityService;

    public IUnitOfWork UnitOfWork { get; set; } = unitOfWork;
}
