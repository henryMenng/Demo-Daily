using Daily.API.ApiReponses;
using Daily.API.DataModel;
using Daily.API.Dtos;
using Daily.API.Helper;
using Daily.API.Repositories;
using Daily.API.Services;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Daily.API.Tests.Services;
public class ToDoServiceTests
{
    private readonly Mock<IToDoInfoRepoService> _mockRepoService;
    private readonly ToDoService _service;
    private readonly IApiResponseHelper _apiResponseHelper;

    public ToDoServiceTests()
    {
        _apiResponseHelper = new ApiResponseHelper();
        _mockRepoService = new Mock<IToDoInfoRepoService>();
        _service = new ToDoService(_apiResponseHelper,_mockRepoService.Object);
    }

    #region AddToDoInfoAsync
    [Fact]
    public async Task AddToDoInfoAsync_WhenToDoIsNull_ThrowArgumentNullException()
    {
        // Arrange
        AddToDoDto todo = null;

        // Act
        async Task act() => await _service.AddToDoAsync(todo);

        // Assert
        await Assert.ThrowsAsync<ArgumentNullException>(act);

    }

    [Fact]
    public async Task AddToDoInfoAsync_WhenToDoIsAddedSuccessfully_ReturnTureAndZero()
    {
        // Arrange
        AddToDoDto todo = new()
        {
            Title = "Test",
            Content = "Test"
        };

        _mockRepoService.Setup(x => x.AddToDoInfoAsync(It.IsAny<ToDoInfo>())).ReturnsAsync((true, 0));

        // Act
        var result = await _service.AddToDoAsync(todo);

        //Assert
        Assert.NotNull(result.ResultData);
        Assert.Equal(0, (int)result.ResultData);

        // Assert
        Assert.Equal(ResultCodeEnum.Success, result.ResultCode);
    }

    [Theory]
    [InlineData("", "Test")]
    [InlineData("Test", "")]
    public async Task AddToDoInfoAsync_WhenTitleOrContentIsEmpty_ReturnFalseAndFive(string title, string content)
    {
        // Arrange
        AddToDoDto todo = new()
        {
            Title = title,
            Content = content
        };

        _mockRepoService.Setup(x => x.AddToDoInfoAsync(It.IsAny<ToDoInfo>())).ReturnsAsync((false, 5));

        // Act
        var result = await _service.AddToDoAsync(todo);

        //Assert
        Assert.NotNull(result.ResultData);

        //Assert
        Assert.Equal(5, (int)result.ResultData);

        // Assert
        Assert.Equal(ResultCodeEnum.Error, result.ResultCode);
    }
    #endregion

    #region DeleteToDoInfoAsync
    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public async Task DeleteToDoInfoAsync_WhenIdIsInvalid_ReturnFalseAndFive(int id)
    {
        // Arrange
        _mockRepoService.Setup(x => x.DeleteToDoInfoAsync(It.IsAny<int>())).ReturnsAsync((false, 5));

        // Act
        var result = await _service.DeleteToDoAsync(id);

        //Assert
        Assert.NotNull(result.ResultData);

        //Assert
        Assert.Equal(5, (int)result.ResultData);

        // Assert
        Assert.Equal(ResultCodeEnum.DtoError, result.ResultCode);
    }

    [Fact]
    public async Task DeleteToDoInfoAsync_WhenIdIsNotFound_ReturnFalseAndTen()
    {
        // Arrange
        _mockRepoService.Setup(x => x.DeleteToDoInfoAsync(It.IsAny<int>())).ReturnsAsync((false, 10));

        // Act
        var result = await _service.DeleteToDoAsync(1);

        //Assert
        Assert.NotNull(result.ResultData);

        //Assert
        Assert.Equal(10, (int)result.ResultData);

        // Assert
        Assert.Equal(ResultCodeEnum.NotFound, result.ResultCode);
    }

    [Fact]
    public async Task DeleteToDoInfoAsync_WhenIdIsDeletedSuccessfully_ReturnTrueAndZero()
    {
        // Arrange
        _mockRepoService.Setup(x => x.DeleteToDoInfoAsync(It.IsAny<int>())).ReturnsAsync((true, 0));

        // Act
        var result = await _service.DeleteToDoAsync(1);

        //Assert
        Assert.NotNull(result.ResultData);

        //Assert
        Assert.Equal(0, (int)result.ResultData);

        // Assert
        Assert.Equal(ResultCodeEnum.Success, result.ResultCode);
    }
    #endregion

    #region EditToDo
    [Fact]
    public async Task EditToDoAsync_ThrowArgumentNullException_WhenEditToDoIsNull()
    {
        // Arrange
        EditToDoDto todo = null;

        // Act
        async Task act() => await _service.EditToDoAsync(todo);

        // Assert
        await Assert.ThrowsAsync<ArgumentNullException>(act);
    }

    [Fact]
    public async Task EditToDoAsync_ReturnSuccess_WhenEditedSuccessfully()
    {
        _mockRepoService.Setup(x => x.UpdateToDoInfoAsync(It.IsAny<ToDoInfo>())).ReturnsAsync((true, 0));

        // Arrange
        EditToDoDto todo = new()
        {
            ToDoId = 1,
            Title = "Test",
            Content = "Test"
        };

        // Act
        var result = await _service.EditToDoAsync(todo);

        //Assert
        Assert.NotNull(result.ResultData);
        Assert.Equal(0, (int) result.ResultData);
        Assert.Equal(ResultCodeEnum.Success,result.ResultCode);
    }

    [Fact]
    public async Task EditToDoAsync_ReturnError_WhenTitleOrContentIsEmpty()
    {
        // Arrange
        EditToDoDto todo = new()
        {
            ToDoId = -1,
            Title = "title",
            Content = "content"
        };

        _mockRepoService.Setup(x => x.UpdateToDoInfoAsync(It.IsAny<ToDoInfo>())).ReturnsAsync((false, 5));

        // Act
        var result = await _service.EditToDoAsync(todo);

        //Assert
        Assert.NotNull(result.ResultData);

        //Assert
        Assert.Equal(5, (int)result.ResultData);

        // Assert
        Assert.Equal(ResultCodeEnum.DtoError, result.ResultCode);
    }

    [Fact]
    public async Task EditToDoAsync_ReturnNotFound_WhenIdIsNotFound()
    {
        // Arrange
        EditToDoDto todo = new()
        {
            ToDoId = 1,
            Title = "title",
            Content = "content"
        };

        _mockRepoService.Setup(x => x.UpdateToDoInfoAsync(It.IsAny<ToDoInfo>())).ReturnsAsync((false, 10));

        // Act
        var result = await _service.EditToDoAsync(todo);

        //Assert
        Assert.NotNull(result.ResultData);

        //Assert
        Assert.Equal(10, (int)result.ResultData);

        // Assert
        Assert.Equal(ResultCodeEnum.NotFound, result.ResultCode);
    }

    [Fact]
    public async Task EditToDoAsync_ReturnError_WhenUpdateFailed()
    {
        // Arrange
        EditToDoDto todo = new()
        {
            ToDoId = 1,
            Title = "title",
            Content = "content"
        };

        _mockRepoService.Setup(x => x.UpdateToDoInfoAsync(It.IsAny<ToDoInfo>())).ReturnsAsync((false, 1));

        // Act
        var result = await _service.EditToDoAsync(todo);

        //Assert
        Assert.NotNull(result.ResultData);

        // Assert
        Assert.Equal(ResultCodeEnum.Error, result.ResultCode);
    }

    [Theory]
    [InlineData("", "content")]
    [InlineData("title", "")]
    [InlineData(null, "content")]
    [InlineData("title", null)]
    public async Task EditToDoAsync_ReturnDtoNotComplete_WhenTitleOrContentIsNullOrEmpty(string title,string content)
    {
        // Arrange
        EditToDoDto todo = new()
        {
            ToDoId = 1,
            Title = title,
            Content = content
        };

        _mockRepoService.Setup(x => x.UpdateToDoInfoAsync(It.IsAny<ToDoInfo>())).ReturnsAsync((false, 15));

        // Act
        var result = await _service.EditToDoAsync(todo);

        //Assert
        Assert.NotNull(result.ResultData);

        // Assert
        Assert.Equal(ResultCodeEnum.DtoNotComplete, result.ResultCode);
    }
    #endregion

    #region GetAllToDoListAsync
    [Fact]
    public async Task GetAllToDoListAsync_RetrunToDoDtoList_WhenGetAllToDoListSuccessfully()
    {
        // Arrange
        _mockRepoService.Setup(x => x.GetConditionQueryToDoListAsync(It.IsAny<int>(), It.IsAny<string>())).ReturnsAsync((new List<ToDoInfo>(), true, 0));

        // Act
        var result = await _service.GetAllToDoListAsync();

        //Assert
        Assert.NotNull(result.ResultData);
  
        // Assert
        Assert.Equal(ResultCodeEnum.Success, result.ResultCode);
    }

    [Fact]
    public async Task GetAllToDoListAsync_ReturnError_WhenGetAllToDoListFailed()
    {
        // Arrange
        _mockRepoService.Setup(x => x.GetConditionQueryToDoListAsync(It.IsAny<int>(), It.IsAny<string>())).ReturnsAsync((null!, false, 30));

        // Act
        var result = await _service.GetAllToDoListAsync();

        //Assert
        Assert.NotNull(result.ResultData);

        // Assert
        Assert.Equal(ResultCodeEnum.Error, result.ResultCode);
    }
    #endregion

    #region GetConditionQueryToDoListAsync
    [Theory]
    [InlineData(0, "")]
    [InlineData(0, null)]
    public async Task GetConditionQueryToDoListAsync_ReturnToDoDtoList_WhenGetConditionQueryToDoListSuccessfully(int status, string searchText)
    {
        // Arrange
        _mockRepoService.Setup(x => x.GetConditionQueryToDoListAsync(It.IsAny<int>(), It.IsAny<string>())).ReturnsAsync((new List<ToDoInfo>(), true, 0));

        // Act
        var result = await _service.GetConditionQueryToDoListAsync(status, searchText);

        //Assert
        Assert.NotNull(result.ResultData);

        var type = result.ResultData.GetType();

        Assert.Equal(typeof(List<ToDoInfoDto>), type);

        var msg = "获取全部待办事项成功！";

        Assert.Equal(msg,result.Msg);

        // Assert
        Assert.Equal(ResultCodeEnum.Success, result.ResultCode);
    }

    [Theory]
    [InlineData(0, "test")]
    public async Task GetConditionQueryToDoListAsync_ReturnError_WhenGetConditionQueryToDoListFailed(int status, string searchText)
    {
        // Arrange
        _mockRepoService.Setup(x => x.GetConditionQueryToDoListAsync(It.IsAny<int>(), It.IsAny<string>())).ReturnsAsync((new List<ToDoInfo>(), true, 5));

        // Act
        var result = await _service.GetConditionQueryToDoListAsync(status, searchText);

        //Assert
        Assert.NotNull(result.ResultData);

        var type = result.ResultData.GetType();

        Assert.Equal(typeof(List<ToDoInfoDto>), type);

        var msg = $"获取全部且内容或标题有{searchText}的待办事项成功！";

        Assert.Equal(msg, result.Msg);

        // Assert
        Assert.Equal(ResultCodeEnum.Success, result.ResultCode);
    }

    [Theory]
    [InlineData(1, "")]
    [InlineData(1,null)]
    public async Task GetConditionQueryToDoListAsync_ReturnToDoDtoList_WhenGetActiveToDoListSuccessfully(int status, string searchText)
    {
        // Arrange
        _mockRepoService.Setup(x => x.GetConditionQueryToDoListAsync(It.IsAny<int>(), It.IsAny<string>())).ReturnsAsync((new List<ToDoInfo>(), true, 10));

        // Act
        var result = await _service.GetConditionQueryToDoListAsync(status, searchText);

        //Assert
        Assert.NotNull(result.ResultData);

        var type = result.ResultData.GetType();

        Assert.Equal(typeof(List<ToDoInfoDto>), type);

        var msg = "获取全部待办中事项成功！";

        Assert.Equal(msg, result.Msg);

        // Assert
        Assert.Equal(ResultCodeEnum.Success, result.ResultCode);
    }

    [Fact]
    public async Task GetConditionQueryToDoListAsync_ReturnError_WhenGetActiveToDoListFailed()
    {
        // Arrange
        _mockRepoService.Setup(x => x.GetConditionQueryToDoListAsync(It.IsAny<int>(), It.IsAny<string>())).ReturnsAsync((new List<ToDoInfo>(), true, 15));

        // Act
        var result = await _service.GetConditionQueryToDoListAsync(1, "test");

        //Assert
        Assert.NotNull(result.ResultData);

        var type = result.ResultData.GetType();

        Assert.Equal(typeof(List<ToDoInfoDto>), type);

        var msg = $"获取待办中且内容或标题有test的待办事项成功！";

        Assert.Equal(msg, result.Msg);

        // Assert
        Assert.Equal(ResultCodeEnum.Success, result.ResultCode);
    }

    [Theory]
    [InlineData(2, "")]
    [InlineData(2, null)]
    public async Task GetConditionQueryToDoListAsync_ReturnToDoDtoList_WhenGetCompletedToDoListSuccessfully(int status, string searchText)
    {
        // Arrange
        _mockRepoService.Setup(x => x.GetConditionQueryToDoListAsync(It.IsAny<int>(), It.IsAny<string>())).ReturnsAsync((new List<ToDoInfo>(), true, 20));

        // Act
        var result = await _service.GetConditionQueryToDoListAsync(status, searchText);

        //Assert
        Assert.NotNull(result.ResultData);

        var type = result.ResultData.GetType();

        Assert.Equal(typeof(List<ToDoInfoDto>), type);

        var msg = "获取全部已完成事项成功！";

        Assert.Equal(msg, result.Msg);

        // Assert
        Assert.Equal(ResultCodeEnum.Success, result.ResultCode);
    }

    [Fact]
    public async Task GetConditionQueryToDoListAsync_ReturnError_WhenGetCompletedToDoListFailed()
    {
        // Arrange
        _mockRepoService.Setup(x => x.GetConditionQueryToDoListAsync(It.IsAny<int>(), It.IsAny<string>())).ReturnsAsync((new List<ToDoInfo>(), true, 25));

        // Act
        var result = await _service.GetConditionQueryToDoListAsync(2, "test");

        //Assert
        Assert.NotNull(result.ResultData);

        var type = result.ResultData.GetType();

        Assert.Equal(typeof(List<ToDoInfoDto>), type);

        var msg = $"获取已完成且内容或标题有test的待办事项成功！";

        Assert.Equal(msg, result.Msg);

        // Assert
        Assert.Equal(ResultCodeEnum.Success, result.ResultCode);
    }

    #endregion

    #region GetActiveToDoListAsync
    [Fact]
    public async Task GetActiveToDoListAsync_ReturnToDoDtoList_WhenGetActiveToDoListSuccessfully()
    {
        // Arrange
        _mockRepoService.Setup(x => x.GetConditionQueryToDoListAsync(It.IsAny<int>(), It.IsAny<string>())).ReturnsAsync((new List<ToDoInfo>(), true, 10));

        // Act
        var result = await _service.GetActiveToDoListAsync();

        //Assert
        Assert.NotNull(result.ResultData);

        var type = result.ResultData.GetType();

        Assert.Equal(typeof(List<ToDoInfoDto>), type);

        var msg = "获取待办中的事项成功！";

        Assert.Equal(msg, result.Msg);

        // Assert
        Assert.Equal(ResultCodeEnum.Success, result.ResultCode);
    }

    [Fact]
    public async Task GetActiveToDoListAsync_ReturnError_WhenGetActiveToDoListFailed()
    {
        // Arrange
        _mockRepoService.Setup(x => x.GetConditionQueryToDoListAsync(It.IsAny<int>(), It.IsAny<string>())).ReturnsAsync((new List<ToDoInfo>(), true, 15));

        // Act
        var result = await _service.GetActiveToDoListAsync();

        //Assert
        Assert.NotNull(result.ResultData);

        var type = result.ResultData.GetType();

        Assert.Equal(typeof(object), type);

        var msg = $"获取全部待办中事项成功！";

        Assert.NotEqual(msg, result.Msg);

        // Assert
        Assert.Equal(ResultCodeEnum.Error, result.ResultCode);
    }


    #endregion

    #region GetCompletedToDoListAsync
    [Fact]
    public async Task GetCompletedToDoListAsync_ReturnToDoDtoList_WhenGetCompletedToDoListSuccessfully()
    {
        // Arrange
        _mockRepoService.Setup(x => x.GetConditionQueryToDoListAsync(It.IsAny<int>(), It.IsAny<string>())).ReturnsAsync((new List<ToDoInfo>(), true, 20));

        // Act
        var result = await _service.GetCompletedToDoListAsync();

        //Assert
        Assert.NotNull(result.ResultData);

        var type = result.ResultData.GetType();

        Assert.Equal(typeof(List<ToDoInfoDto>), type);

        var msg = "获取已完成的事项成功！";

        Assert.Equal(msg, result.Msg);

        // Assert
        Assert.Equal(ResultCodeEnum.Success, result.ResultCode);
    }

    [Fact]
    public async Task GetCompletedToDoListAsync_ReturnError_WhenGetCompletedToDoListFailed()
    {
        // Arrange
        _mockRepoService.Setup(x => x.GetConditionQueryToDoListAsync(It.IsAny<int>(), It.IsAny<string>())).ReturnsAsync((new List<ToDoInfo>(), true, 25));

        // Act
        var result = await _service.GetCompletedToDoListAsync();

        //Assert
        Assert.NotNull(result.ResultData);

        var type = result.ResultData.GetType();

        Assert.Equal(typeof(object), type);

        var msg = $"获取全部已完成事项成功！";

        Assert.NotEqual(msg, result.Msg);

        // Assert
        Assert.Equal(ResultCodeEnum.Error, result.ResultCode);
    }

    #endregion

    #region StatisticsToDoAsync
    [Fact]
    public async Task StatisticsToDoAsync_ReturnStatisticsInfo_WhenStatisticsToDoSuccessfully()
    {
        // Arrange
        _mockRepoService.Setup(x => x.StatisticsToDoAsync()).ReturnsAsync((2, 1, true, 0));

        // Act
        var result = await _service.StatisticsToDoAsync();

        //Assert
        Assert.NotNull(result.ResultData);

        var type = result.ResultData.GetType();

        Assert.Equal(typeof(StatisticsToDoDto), type);

        var msg = "统计待办事项成功";

        Assert.Equal(msg, result.Msg);

        // Assert
        Assert.Equal(ResultCodeEnum.Success, result.ResultCode);
    }

    [Fact]
    public async Task StatisticsToDoAsync_ReturnError_WhenStatisticsToDoFailed()
    {
        // Arrange
        _mockRepoService.Setup(x => x.StatisticsToDoAsync()).ReturnsAsync((0, 0, false, 5));

        // Act
        var result = await _service.StatisticsToDoAsync();

        //Assert
        Assert.NotNull(result.ResultData);

        var type = result.ResultData.GetType();

        Assert.Equal(typeof(int), type);

        var msg = "统计待办事项失败";

        Assert.Equal(msg, result.Msg);

        // Assert
        Assert.Equal(ResultCodeEnum.Error, result.ResultCode);
    }

    #endregion

    #region UpdateToDoStatusAsync
    [Fact]
    public async Task UpdateToDoStatusAsync_ReturnSuccess_WhenUpdateToDoStatusSuccessfully()
    {
        // Arrange
        _mockRepoService.Setup(x => x.UpdateToDoInfoAsync(It.IsAny<ToDoInfo>())).ReturnsAsync((true,0));

        // Act
        var result = await _service.UpdateToDoStatusAsync(1);

        //Assert
        Assert.NotNull(result.ResultData);

        var type = result.ResultData.GetType();

        Assert.Equal(typeof(int), type);

        var msg = "更新待办事项状态成功";

        Assert.Equal(msg, result.Msg);

        // Assert
        Assert.Equal(ResultCodeEnum.Success, result.ResultCode);
    }

    [Fact]
    public async Task UpdateToDoStatusAsync_ReturnError_WhenUpdateToDoStatusFailed()
    {
        // Arrange
        _mockRepoService.Setup(x => x.UpdateToDoStatus(It.IsAny<int>())).ReturnsAsync((0));

        // Act
        var result = await _service.UpdateToDoStatusAsync(1);

        //Assert
        Assert.NotNull(result.ResultData);

        var type = result.ResultData.GetType();

        Assert.Equal(typeof(int), type);

        var msg = "更新待办事项状态成功";

        Assert.Equal(msg, result.Msg);

        // Assert
        Assert.Equal(ResultCodeEnum.Success, result.ResultCode);
    }

    [Fact]
    public async Task UpdateToDoStatusAsync_ReturnNotFound_WhenIdIsNotFound()
    {
        // Arrange
        _mockRepoService.Setup(x => x.UpdateToDoStatus(It.IsAny<int>())).ReturnsAsync( 10);

        // Act
        var result = await _service.UpdateToDoStatusAsync(1);

        //Assert
        Assert.NotNull(result.ResultData);

        var type = result.ResultData.GetType();

        Assert.Equal(typeof(int), type);

        var msg = "未找到该待办事项";

        Assert.Equal(msg, result.Msg);

        // Assert
        Assert.Equal(ResultCodeEnum.NotFound, result.ResultCode);
    }
    #endregion

}
