using Daily.API.Dtos;
using Microsoft.AspNetCore.Mvc.Testing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;

namespace Daily.API.Tests.Controllers;
public class MemoControllerTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;

    public MemoControllerTests(WebApplicationFactory<Program> factory)
    {
        _factory = factory;
    }

    [Fact]
    public async Task GetAllMemoList_ReturnsSuccess()
    {
        // Arrange
        var client = _factory.CreateClient();

        // Act
        var response = await client.GetAsync("/api/Memo/GetAllMemoList");

        // Assert
        response.EnsureSuccessStatusCode();
        var responseString = await response.Content.ReadAsStringAsync();
        Assert.Contains("获取所有备忘录成功", responseString);
    }

    [Fact]
    public async Task GetConditionQueryMemoList_ReturnsSuccess()
    {
        // Arrange
        var client = _factory.CreateClient();
        var searchText = "test";

        // Act
        var response = await client.GetAsync($"/api/Memo/GetConditionQueryMemoList?searchText={searchText}");

        // Assert
        response.EnsureSuccessStatusCode();
        var responseString = await response.Content.ReadAsStringAsync();
        Assert.Contains("获取查询的备忘录成功", responseString);
    }

    [Fact]
    public async Task DeleteMemo_ReturnsSuccess()
    {
        // Arrange
        var client = _factory.CreateClient();
        var memoInfoDto = new MemoInfoDto
        {
            Title = "test",
            Content = "test",
            Status = 1
        };

        // Act
        await client.PostAsJsonAsync("/api/Memo/AddMemo", memoInfoDto);

        var id = 2;
        // Act
        var response = await client.GetAsync($"/api/Memo/DeleteMemo?id={id}");

        // Assert
        response.EnsureSuccessStatusCode();
        var responseString = await response.Content.ReadAsStringAsync();
        Assert.Contains("删除备忘录成功", responseString);
    }

    [Fact]
    public async Task AddMemo_ReturnsSuccess()
    {
        // Arrange
        var client = _factory.CreateClient();
        var memoInfoDto = new MemoInfoDto
        {
            Title = "test",
            Content = "test",
            Status = 1
        };

        // Act
        var response = await client.PostAsJsonAsync("/api/Memo/AddMemo", memoInfoDto);
        response.EnsureSuccessStatusCode();

        // Assert
        var responseString = await response.Content.ReadAsStringAsync();
        Assert.Contains("添加备忘录成功", responseString);
    }

    [Fact]
    public async Task EditMemo_ReturnsSuccess()
    {
        // Arrange
        var client = _factory.CreateClient();
        var memoInfoDto = new MemoInfoDto
        {
            Title = "添加测试标题",
            Content = "添加测试内容",
            Status = 1
        };
        var res = await client.PostAsJsonAsync("/api/Memo/AddMemo", memoInfoDto);

        res.EnsureSuccessStatusCode();

        var editMemoDto = new EditMemoDto
        {
            MemoId = 15,
            Title = "测试标题",
            Content = "测试内容",
            Status = 1
        };

        // Act
        var response = await client.PostAsJsonAsync("/api/Memo/EditMemo", editMemoDto);
        response.EnsureSuccessStatusCode();

        // Assert
        var responseString = await response.Content.ReadAsStringAsync();
        Assert.Contains("编辑备忘录成功", responseString);
    }

    [Fact]
    public async Task EditMemo_ReturnsBadRequest()
    {
        // Arrange
        var client = _factory.CreateClient();
        var editMemoDto = new EditMemoDto
        {
            MemoId = 1,
            Title = "",
            Content = "test",
            Status = 1
        };

        // Act
        var response = await client.PostAsJsonAsync("/api/Memo/EditMemo", editMemoDto);

        // Assert
        Assert.Equal(System.Net.HttpStatusCode.BadRequest, response.StatusCode);
    }
}
