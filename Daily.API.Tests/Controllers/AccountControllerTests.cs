using Daily.API;
using Daily.API.ApiReponses;
using Daily.API.DataModel;
using Daily.API.Dtos;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Daily.API.Tests.Controllers;
public class AccountControllerTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;

    private readonly DailyDbContext _context;

    private readonly DbContextOptions<DailyDbContext> _options;

    public AccountControllerTests(WebApplicationFactory<Program> factory)
    {
        _options = new DbContextOptionsBuilder<DailyDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
        _context = new DailyDbContext(_options);

        _factory = factory;
    }

    public void Dispose()
    {
        // 测试结束时，删除数据库
        _context.Database.EnsureDeleted();
        _context.Dispose();
    }

    [Fact]
    public async Task Login_ReturnsSuccess()
    {
        // Arrange
        var client = _factory.CreateClient();
        var logAccount = "admin";
        var logPassword = "202CB962AC59075B964B07152D234B70";

        // Act
        var response = await client.GetAsync($"/api/Account/Login?logAccount={logAccount}&logPassword={logPassword}");

        // Assert
        response.EnsureSuccessStatusCode();
        var responseString = await response.Content.ReadAsStringAsync();
        Assert.Contains("登录成功", responseString);
    }

    [Fact]
    public async Task Register_ReturnsSuccess()
    {
        // Arrange
        var client = _factory.CreateClient();
        var accountInfoDto = new AccountInfoDto
        {
            Account = "test1",
            Name = "test1",
            Pwd = "123456"
        };

        // Act
        var response = await client.PostAsJsonAsync("/api/Account/Register", accountInfoDto);
        response.EnsureSuccessStatusCode();

        // Assert
        var responseString = await response.Content.ReadAsStringAsync();
        Assert.Contains("注册成功", responseString);
    }

    [Fact]
    public async Task Register_ReturnsBadRequest()
    {
        // Arrange
        var client = _factory.CreateClient();
        var accountInfoDto = new AccountInfoDto
        {
            Account = "",
            Name = "test",
            Pwd = "123"
        };

        // Act
        var response = await client.PostAsJsonAsync("/api/Account/Register", accountInfoDto);

        // Assert
        Assert.Equal(System.Net.HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task Register_ReturnsBadRequest_1()
    {
        // Arrange
        var client = _factory.CreateClient();
        var accountInfoDto = new AccountInfoDto
        {
            Account = "test1",
            Name = "",
            Pwd = "123"
        };

        // Act
        var response = await client.PostAsJsonAsync("/api/Account/Register", accountInfoDto);

        // Assert
        Assert.Equal(System.Net.HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task Register_ReturnsBadRequest_2()
    {
        // Arrange
        var client = _factory.CreateClient();
        var accountInfoDto = new AccountInfoDto
        {
            Account = "test1",
            Name = "test3",
            Pwd = ""
        };

        // Act
        var response = await client.PostAsJsonAsync("/api/Account/Register", accountInfoDto);

        // Assert
        Assert.Equal(System.Net.HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task Register_ReturnsBadRequest_3()
    {
        // Arrange
        var client = _factory.CreateClient();
        var accountInfoDto = new AccountInfoDto();

        // Act
        var response = await client.PostAsJsonAsync("/api/Account/Register", accountInfoDto);

        // Assert
        Assert.Equal(System.Net.HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task Register_ReturnsConflict()
    {
        // Arrange
        var client = _factory.CreateClient();
        var accountInfoDto = new AccountInfoDto
        {
            Account = "admin",
            Name = "test",
            Pwd = "123456"
        };

        // Act
        var response = await client.PostAsJsonAsync("/api/Account/Register", accountInfoDto);

        // Assert
        var content = await response.Content.ReadAsStringAsync();

        var apiResponse  = JsonConvert.DeserializeObject<ApiResponse>(content);

        if (apiResponse == null)
            apiResponse = new();

        Assert.Equal("账号已存在", apiResponse.Msg);

        Assert.Equal(System.Net.HttpStatusCode.OK, response.StatusCode);
    }
}
