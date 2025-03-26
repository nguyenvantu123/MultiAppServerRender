using Xunit;
using Moq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using BlazorApiUser.Models;
using BlazorApiUser.Repositories;
using BlazorApiUser.Commands.Users;
using BlazorApiUser.Queries.Users;
using BlazorApiUser.Constants;
using BlazorApiUser.Extensions;
using BlazorApiUser.Queries.Roles;
using AutoMapper;
using System.Security.Claims;
using System.Collections.Generic;
using Microsoft.AspNetCore.Http;
using System.Linq;
using Finbuckle.MultiTenant.Abstractions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using StackExchange.Redis;
using BlazorApiUser.Db;
using Finbuckle.MultiTenant;
using Finbuckle.MultiTenant.EntityFrameworkCore;

namespace UserApiTest;

public class UsersApiTests
{
    private readonly Mock<UserManager<ApplicationUser>> _userManagerMock;
    private readonly Mock<RoleManager<ApplicationRole>> _roleManagerMock;
    private readonly Mock<IMediator> _mediatorMock;
    private readonly Mock<IMapper> _mapperMock;
    private readonly Mock<IHttpContextAccessor> _httpContextAccessorMock;
    private readonly Mock<RedisUserRepository> _redisUserRepositoryMock;
    private readonly Mock<ApplicationDbContext> _applicationDbContextMock;
    private readonly Mock<TenantStoreDbContext> _tenantStoreDbContextMock;
    private readonly Mock<IMultiTenantContextAccessor> _multiTenantContextAccessorMock;
    private readonly UserServices _userServices;

    private readonly Mock<SignInManager<ApplicationUser>> _signInManagerMock;


    public UsersApiTests()
    {
        _userManagerMock = new Mock<UserManager<ApplicationUser>>(
             Mock.Of<IUserStore<ApplicationUser>>(), null, null, null, null, null, null, null, null);
        _roleManagerMock = new Mock<RoleManager<ApplicationRole>>(
            Mock.Of<IRoleStore<ApplicationRole>>(), null, null, null, null);
        // mock for  SignInManager 
        _signInManagerMock = new Mock<SignInManager<ApplicationUser>>(
           Mock.Of<IUserStore<ApplicationUser>>(), null, null, null, null);
        _mediatorMock = new Mock<IMediator>();
        _mapperMock = new Mock<IMapper>();
        _httpContextAccessorMock = new Mock<IHttpContextAccessor>();
        _redisUserRepositoryMock = new Mock<RedisUserRepository>(Mock.Of<ILogger<RedisUserRepository>>(), Mock.Of<IConnectionMultiplexer>());
        _multiTenantContextAccessorMock = new Mock<IMultiTenantContextAccessor>();
        var multiTenantContext = new Mock<IMultiTenantContext>();
        multiTenantContext.Setup(m => m.TenantInfo).Returns(new AppTenantInfo { Id = DefaultTenant.DefaultTenantId, Identifier = DefaultTenant.DefaultTenantId, Name = DefaultTenant.DefaultTenantId });
        _multiTenantContextAccessorMock.Setup(m => m.MultiTenantContext).Returns(multiTenantContext.Object);
        _applicationDbContextMock = new Mock<ApplicationDbContext>(_multiTenantContextAccessorMock.Object, new DbContextOptions<ApplicationDbContext>());
        _tenantStoreDbContextMock = new Mock<TenantStoreDbContext>(new DbContextOptions<TenantStoreDbContext>());

        _userServices = new UserServices(
            _mediatorMock.Object,
            Mock.Of<ILogger<UserServices>>(),
            _userManagerMock.Object,
            _roleManagerMock.Object,
            _signInManagerMock.Object,
            _tenantStoreDbContextMock.Object,
            _mapperMock.Object,
            _httpContextAccessorMock.Object,
            _redisUserRepositoryMock.Object,
            _applicationDbContextMock.Object
        );
    }

    [Fact]
    public async Task GetUsers_ReturnsUsers()
    {
        // Arrange
        var query = new GetListUserQuery();
        var users = new List<UserDataViewModel> { new UserDataViewModel { UserName = "testuser" } };
        _mediatorMock.Setup(m => m.Send(query, default)).ReturnsAsync(Tuple.Create(1, users));

        // Act
        var result = await UsersApi.GetUsers(query, _userServices);

        // Assert
        Assert.Equal(200, result.StatusCode);
        Assert.Equal("1 users fetched", result.Message);
        Assert.Equal(users, result.Result);
    }

    [Fact]
    public async Task Create_ReturnsSuccess()
    {
        // Arrange
        var command = new CreateUserCommand
        {
            UserName = "testuser",
            Email = "test@example.com",
            Password = "Password123!",
            FirstName = "Test",
            LastName = "User",
            PhoneNumber = "1234567890",
            ActivateUser = true,
            AutoConfirmEmail = true
        };
        _userManagerMock.Setup(um => um.FindByNameAsync(command.UserName)).ReturnsAsync((ApplicationUser)null);
        _userManagerMock.Setup(um => um.FindByEmailAsync(command.Email)).ReturnsAsync((ApplicationUser)null);
        _userManagerMock.Setup(um => um.CreateAsync(It.IsAny<ApplicationUser>(), command.Password)).ReturnsAsync(IdentityResult.Success);

        // Act
        var result = await UsersApi.Create(command, _userServices);

        // Assert
        Assert.Equal(200, result.StatusCode);
        Assert.Equal($"User {command.UserName} created", result.Message);
    }

    [Fact]
    public async Task AdminUpdateUser_ReturnsSuccess()
    {
        // Arrange
        var id = "test-id";
        var command = new AdminUpdateUserCommand
        {
            FirstName = "Updated",
            LastName = "User",
            PhoneNumber = "0987654321"
        };
        var user = new ApplicationUser { UserName = "testuser" };
        _userManagerMock.Setup(um => um.FindByIdAsync(id)).ReturnsAsync(user);
        _userManagerMock.Setup(um => um.UpdateAsync(user)).ReturnsAsync(IdentityResult.Success);

        // Act
        var result = await UsersApi.AdminUpdateUser(id, command, _userServices);

        // Assert
        Assert.Equal(200, result.StatusCode);
        Assert.Equal($"{command.UserName} update successfully!!!", result.Message);
    }

    [Fact]
    public async Task AdminDelete_ReturnsSuccess()
    {
        // Arrange
        var id = "test-id";
        var user = new ApplicationUser { UserName = "testuser" };
        _userManagerMock.Setup(um => um.FindByIdAsync(id)).ReturnsAsync(user);
        _userManagerMock.Setup(um => um.UpdateAsync(user)).ReturnsAsync(IdentityResult.Success);
        _applicationDbContextMock.Setup(db => db.SaveChangesAsync(default)).ReturnsAsync(1);

        // Act
        var result = await UsersApi.AdminDelete(id, _userServices);

        // Assert
        Assert.Equal(200, result.StatusCode);
        Assert.Equal($"{user.UserName} delete successfully!!!", result.Message);
    }
}
