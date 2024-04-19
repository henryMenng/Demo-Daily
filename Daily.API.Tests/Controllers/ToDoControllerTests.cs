using Daily.API.DataModel;
using Daily.API.Dtos;
using Microsoft.AspNetCore.Mvc.Testing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;

namespace Daily.API.Tests.Controllers;
public class ToDoControllerTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;

    public ToDoControllerTests(WebApplicationFactory<Program> factory)
    {
        _factory = factory;
    }

    [Fact]
    public async Task GetToDoList_ReturnsSuccess()
    {
        // Arrange
        var client = _factory.CreateClient();

        // Act
        var response = await client.GetAsync("/api/ToDo/GetAllToDoList");

        // Assert
        response.EnsureSuccessStatusCode();
        var responseString = await response.Content.ReadAsStringAsync();
        Assert.Contains("获取全部待办事项成功！", responseString);
    }

    [Fact]
    public async Task AddToDo_ReturnsSuccess()
    {
        // Arrange
        var client = _factory.CreateClient();
        var toDoDto = new AddToDoDto
        {
            Title = "test",
            Content = "test",
            Status = 1
        };

        // Act
        var response = await client.PostAsJsonAsync("/api/ToDo/AddToDo", toDoDto);
        response.EnsureSuccessStatusCode();

        // Assert
        var responseString = await response.Content.ReadAsStringAsync();
        Assert.Contains("添加待办事项成功", responseString);
    }

    [Fact]
    public async Task UpdateToDo_ReturnsSuccess()
    {
        // Arrange
        var client = _factory.CreateClient();
        var toDoDto = new AddToDoDto
        {
            Title = "test",
            Content = "test",
            Status = 1
        };

        // Act
        await client.PostAsJsonAsync("/api/ToDo/AddToDo", toDoDto);

  
        var editToDoDto = new EditToDoDto
        {
            ToDoId = 4,
            Title = "test",
            Content = "test",
            Status = 0
        };

        // Act
        var response = await client.PostAsJsonAsync("/api/ToDo/EditToDo", editToDoDto);
        response.EnsureSuccessStatusCode();

        // Assert
        var responseString = await response.Content.ReadAsStringAsync();
        Assert.Contains("编辑成功", responseString);
    }

    [Fact]
    public async Task DeleteToDo_ReturnsSuccess()
    {
        // Arrange
        var client = _factory.CreateClient();

        // Act
        var response = await client.GetAsync("/api/ToDo/DeleteToDo?id=2");
        response.EnsureSuccessStatusCode();

        // Assert
        var responseString = await response.Content.ReadAsStringAsync();
        Assert.Contains("删除成功", responseString);
    }

    [Fact]
    public async Task DeleteToDo_ReturnsBadRequest()
    {
        // Arrange
        var client = _factory.CreateClient();

        // Act
        var response = await client.GetAsync("/api/ToDo/DeleteToDo?id=0");

        // Assert
        var responseString = await response.Content.ReadAsStringAsync();
        Assert.Contains("服务器忙，请稍后...", responseString);
    }
}
