using Daily.API.ApiReponses;
using Daily.API.DataModel;
using Daily.API.Dtos;
using Daily.API.Helper;
using Daily.API.Repositories;
using Daily.API.Services;
using Microsoft.EntityFrameworkCore;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Daily.API.Tests.Services;
public class AccountServiceTests
{
    private readonly Mock<IAccountRepoService> _mockRepo;
    private readonly IApiResponseHelper _apiResponseHelper;

    public AccountServiceTests()
    {
        _apiResponseHelper = new ApiResponseHelper();
        _mockRepo = new Mock<IAccountRepoService>();
    }

    [Fact]
    public async Task LoginAsync_ReturnApiResponseAndResultCodeIsSuccess_WhenUserIsLoginedSuccessfully()
    {
        _mockRepo.Setup(repo => repo.LoginAsync(It.IsAny<string>(), It.IsAny<string>())).ReturnsAsync((true, 0,"管理员"));
        
        // Arrange
        var accountService = new AccountService(_apiResponseHelper, _mockRepo.Object);

        // Act
        var result = await accountService.LoginAsync( "Admin", "123");

        // Assert
        Assert.NotNull(result);
        Assert.Equal(ResultCodeEnum.Success, result.ResultCode);
    }

    [Fact]
    public async Task LoginAsync_ReturnApiResponseAndResultCodeIsDtoError_WhenAccountOrPasswordIsEmpty()
    {
        // Arrange
        _mockRepo.Setup(repo => repo.LoginAsync(It.IsAny<string>(), It.IsAny<string>())).ReturnsAsync((false, 10, string.Empty));
        var accountService = new AccountService(_apiResponseHelper, _mockRepo.Object);

        // Act
        var result = await accountService.LoginAsync("", "");

        // Assert
        Assert.NotNull(result);
        Assert.Equal(ResultCodeEnum.DtoError, result.ResultCode);
    }

    [Fact]
    public async Task LoginAsync_ReturnApiResponseAndResultCodeIsNotFound_WhenAccountOrPasswordIsError()
    {
        // Arrange
        _mockRepo.Setup(repo => repo.LoginAsync(It.IsAny<string>(), It.IsAny<string>())).ReturnsAsync((false, 5, "账号或密码不正确"));
        var accountService = new AccountService(_apiResponseHelper, _mockRepo.Object);
        
        // Act
        var result = await accountService.LoginAsync("Admin", "123");

        // Assert
        Assert.NotNull(result);
        Assert.Equal(ResultCodeEnum.NotFound, result.ResultCode);
    }

    [Fact]
    public async Task RegisterAsync_ReturnApiResponseAndResultCodeIsSuccess_WhenRegisterIsSuccessfully()
    {
        // Arrange
        _mockRepo.Setup(repo => repo.AddAsync(It.IsAny<AccountInfo>())).ReturnsAsync((true, 0));

        // Act
        var accountService = new AccountService(_apiResponseHelper, _mockRepo.Object);
        var result = await accountService.RegisterAsync(new AccountInfoDto { Account = "Admin", Pwd = "123" ,Name = "管理员"});

        // Assert
        Assert.NotNull(result);
        Assert.Equal(ResultCodeEnum.Success, result.ResultCode);
    }

    [Fact]
    public async Task RegisterAsync_ReturnApiResponseAndResultCodeIsExist_WhenAccountIsExist()
    {
        // Arrange
        _mockRepo.Setup(repo => repo.AddAsync(It.IsAny<AccountInfo>())).ReturnsAsync((false, 5));
        var accountService = new AccountService(_apiResponseHelper, _mockRepo.Object);

        // Act
        var result = await accountService.RegisterAsync(new AccountInfoDto { Account = "Admin", Pwd = "123", Name = "管理员" });

        // Assert
        Assert.NotNull(result);
        Assert.Equal(ResultCodeEnum.Exist, result.ResultCode);
    }

    [Fact]
    public async Task RegisterAsync_ReturnApiResponseAndResultCodeIsDtoError_WhenAccountInfoIsEmpty()
    {
        // Arrange
        _mockRepo.Setup(repo => repo.AddAsync(It.IsAny<AccountInfo>())).ReturnsAsync((false, 10));
        var accountService = new AccountService(_apiResponseHelper, _mockRepo.Object);

        // Act
        var result = await accountService.RegisterAsync(new AccountInfoDto { Account = "", Pwd = "", Name = "" });

        // Assert
        Assert.NotNull(result);
        Assert.Equal(ResultCodeEnum.DtoError, result.ResultCode);
    }

    [Fact]
    public async Task RegisterAsync_ReturnApiResponseAndResultCodeIsDataBaseError_WhenAddAccountInfoToDatabaseError()
    {
        // Arrange
        _mockRepo.Setup(repo => repo.AddAsync(It.IsAny<AccountInfo>())).ThrowsAsync(new Exception());
        var accountService = new AccountService(_apiResponseHelper, _mockRepo.Object);

        // Act
        var result = await accountService.RegisterAsync(new AccountInfoDto { Account = "Admin", Pwd = "123", Name = "管理员" });

        // Assert
        Assert.NotNull(result);
        Assert.Equal(ResultCodeEnum.Error, result.ResultCode);
    }

    [Fact]
    public async Task RegisterAsync_ReturnApiResponseAndResultCodeIsDtoError_WhenAccountInfoIsNull()
    {
        // Arrange
        _mockRepo.Setup(repo => repo.AddAsync(It.IsAny<AccountInfo>())).ReturnsAsync((false, 10));
        var accountService = new AccountService(_apiResponseHelper, _mockRepo.Object);

        // Act
        var result = await accountService.RegisterAsync(null);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(ResultCodeEnum.Error, result.ResultCode);
    }
}
