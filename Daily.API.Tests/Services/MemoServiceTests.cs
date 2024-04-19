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
public class MemoServiceTests
{
    private readonly Mock<IMemoRepoService> _mockRepo;
    private readonly IApiResponseHelper _apiResponseHelper;

    public MemoServiceTests()
    {
        _apiResponseHelper = new ApiResponseHelper();
        _mockRepo = new Mock<IMemoRepoService>();
    }

    #region AddMemoAsync
    [Fact]
    public async Task AddMemoAsync_ReturnApiResponseAndResultCodeIsSuccess_WhenAddMemoIsSuccessfully()
    {
        // Arrange
        _mockRepo.Setup(repo => repo.AddMemoAsync(It.IsAny<MemoInfo>())).ReturnsAsync((true, 0));

        // Act
        var accountService = new MemoService(_apiResponseHelper, _mockRepo.Object);
        var result = await accountService.AddMemoAsync(new MemoInfoDto { Title = "测试标题", Content = "测试内容", Status = 1 });

        // Assert
        Assert.NotNull(result);

        var msg = "添加备忘录成功";

        Assert.Equal(msg, result.Msg);

        Assert.NotNull(result.ResultData);

        var type = result.ResultData.GetType();

        Assert.Equal(typeof(int), type);

        Assert.Equal(ResultCodeEnum.Success, result.ResultCode);
    }

    [Fact]
    public async Task AddMemoAsync_ReturnApiResponseAndResultCodeIsDtoError_WhenTitleOrContentIsEmpty()
    {
        // Arrange
        _mockRepo.Setup(repo => repo.AddMemoAsync(It.IsAny<MemoInfo>())).ReturnsAsync((false, 5));

        var accountService = new MemoService(_apiResponseHelper, _mockRepo.Object);

        // Act
        var result = await accountService.AddMemoAsync(new MemoInfoDto { Title = "", Content = "", Status = 1 });

        // Assert
        Assert.NotNull(result);

        var msg = "备忘录信息不全";

        Assert.Equal(msg, result.Msg);

        Assert.Equal(ResultCodeEnum.DtoNotComplete, result.ResultCode);
    }

    [Fact]
    public async Task AddMemoAsync_ReturnApiResponseAndResultCodeIsServerError_WhenAddMemoIsError()
    {
        // Arrange
        _mockRepo.Setup(repo => repo.AddMemoAsync(It.IsAny<MemoInfo>())).ReturnsAsync((false, 10));

        var accountService = new MemoService(_apiResponseHelper, _mockRepo.Object);

        // Act
        var result = await accountService.AddMemoAsync(new MemoInfoDto { Title = "测试标题", Content = "测试内容", Status = 1 });

        // Assert
        Assert.NotNull(result);

        var msg = "服务器忙，请稍后...";

        Assert.Equal(msg, result.Msg);

        Assert.Equal(ResultCodeEnum.DataBaseError, result.ResultCode);
    }

    [Fact]
    public async Task AddMemoAsync_ReturnApiResponseAndResultCodeIsServerError_WhenAddMemoToDatabaseError()
    {
        // Arrange
        _mockRepo.Setup(repo => repo.AddMemoAsync(It.IsAny<MemoInfo>())).ThrowsAsync(new Exception());

        var accountService = new MemoService(_apiResponseHelper, _mockRepo.Object);

        // Act
        var result = await accountService.AddMemoAsync(new MemoInfoDto { Title = "测试标题", Content = "测试内容", Status = 1 });

        // Assert
        Assert.NotNull(result);

        var msg = "服务器忙，请稍后...";

        Assert.Equal(msg, result.Msg);

        Assert.Equal(ResultCodeEnum.Error, result.ResultCode);
    }
    #endregion

    #region DeleteMemoAsync
    [Fact]
    public async Task DeleteMemoAsync_ReturnApiResponseAndResultCodeIsSuccess_WhenDeleteMemoIsSuccessfully()
    {
        // Arrange
        _mockRepo.Setup(repo => repo.DeleteMemoAsync(It.IsAny<int>())).ReturnsAsync((true,0));

        var accountService = new MemoService(_apiResponseHelper, _mockRepo.Object);

        // Act
        var result = await accountService.DeleteMemoAsync(1);

        // Assert
        Assert.NotNull(result);

        var msg = "删除备忘录成功";

        Assert.Equal(msg, result.Msg);

        Assert.Equal(ResultCodeEnum.Success, result.ResultCode);
    }

    [Fact]
    public async Task DeleteMemoAsync_ReturnApiResponseAndResultCodeIsServerError_WhenDeleteMemoIsError()
    {
        // Arrange
        _mockRepo.Setup(repo => repo.DeleteMemoAsync(It.IsAny<int>())).ReturnsAsync((false, 10));

        var accountService = new MemoService(_apiResponseHelper, _mockRepo.Object);

        // Act
        var result = await accountService.DeleteMemoAsync(1);

        // Assert
        Assert.NotNull(result);

        var msg = "找不到该备忘录";

        Assert.Equal(msg, result.Msg);

        Assert.Equal(ResultCodeEnum.DataBaseError, result.ResultCode);
    }

    [Fact]
    public async Task DeleteMemoAsync_ReturnApiResponseAndResultCodeIsServerError_WhenDeleteMemoToDatabaseError()
    {
        // Arrange
        _mockRepo.Setup(repo => repo.DeleteMemoAsync(It.IsAny<int>())).ThrowsAsync(new Exception());

        var accountService = new MemoService(_apiResponseHelper, _mockRepo.Object);

        // Act
        var result = await accountService.DeleteMemoAsync(1);

        // Assert
        Assert.NotNull(result);

        var msg = "服务器忙，请稍后...";

        Assert.Equal(msg, result.Msg);

        Assert.Equal(ResultCodeEnum.Error, result.ResultCode);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public async Task DeleteMemoAsync_ReturnApiResponseAndResultCodeIsDtoError_WhenIdIsZero(int id)
    {
        // Arrange
        _mockRepo.Setup(repo => repo.DeleteMemoAsync(It.IsAny<int>())).ReturnsAsync((false, 5));

        var accountService = new MemoService(_apiResponseHelper, _mockRepo.Object);

        // Act
        var result = await accountService.DeleteMemoAsync(id);

        // Assert
        Assert.NotNull(result);

        var msg = "服务器忙，请稍后...";

        Assert.Equal(msg, result.Msg);

        Assert.Equal(ResultCodeEnum.DtoError, result.ResultCode);
    }
    #endregion

    #region GetAllMemoList
    [Fact]
    public async Task GetAllMemoList_ReturnApiResponseAndResultCodeIsSuccess_WhenGetAllMemoListIsSuccessfully()
    {
        // Arrange
        _mockRepo.Setup(repo => repo.GetAllMemosAsync()).ReturnsAsync((new List<MemoInfo>(),0));

        var _memoService = new MemoService(_apiResponseHelper, _mockRepo.Object);

        // Act
        var result = await _memoService.GetAllMemoListAsync();

        // Assert
        Assert.NotNull(result);

        var msg = "获取所有备忘录成功";

        Assert.Equal(msg, result.Msg);

        Assert.NotNull(result.ResultData);

        var type = result.ResultData.GetType();

        Assert.Equal(typeof(List<MemoInfo>), type);

        Assert.Equal(ResultCodeEnum.Success, result.ResultCode);
    }

    [Fact]
    public async Task GetAllMemoList_ReturnApiResponseAndResultCodeIsServerError_WhenGetAllMemoListIsError()
    {
        // Arrange
        _mockRepo.Setup(repo => repo.GetAllMemosAsync()).ThrowsAsync(new Exception());

        var _memoService = new MemoService(_apiResponseHelper, _mockRepo.Object);

        // Act
        var result = await _memoService.GetAllMemoListAsync();

        // Assert
        Assert.NotNull(result);

        var msg = "服务器忙，请稍后...";

        Assert.Equal(msg, result.Msg);

        Assert.Equal(ResultCodeEnum.Error, result.ResultCode);
    }
    #endregion

    #region EditMemoAsync
    [Fact]
    public async Task EditMemoAsync_ReturnApiResponseAndResultCodeIsSuccess_WhenEditMemoIsSuccessfully()
    {
        // Arrange
        _mockRepo.Setup(repo => repo.UpdateMemoAsync(It.IsAny<MemoInfo>())).ReturnsAsync(0);

        var _memoService = new MemoService(_apiResponseHelper, _mockRepo.Object);

        // Act
        var result = await _memoService.EditMemoAsync(new EditMemoDto { MemoId = 1, Title = "测试标题", Content = "测试内容", Status = 1 });

        // Assert
        Assert.NotNull(result);

        var msg = "编辑备忘录成功";

        Assert.Equal(msg, result.Msg);

        Assert.NotNull(result.ResultData);

        var type = result.ResultData.GetType();

        Assert.Equal(typeof(int), type);

        Assert.Equal(ResultCodeEnum.Success, result.ResultCode);
    }

    [Theory]
    [InlineData("Tiltle","")]
    [InlineData("","Content")]
    [InlineData("","")]
    public async Task EditMemoAsync_ReturnApiResponseAndResultCodeIsDtoError_WhenEditMemoDtoIsError(string title,string content)
    {
        // Arrange
        _mockRepo.Setup(repo => repo.UpdateMemoAsync(It.IsAny<MemoInfo>())).ReturnsAsync(5);

        var _memoService = new MemoService(_apiResponseHelper, _mockRepo.Object);

        // Act
        var result = await _memoService.EditMemoAsync(new EditMemoDto { MemoId = 1, Title = title, Content = content, Status = 1 });

        // Assert
        Assert.NotNull(result);

        var msg = "备忘录信息不全";

        Assert.Equal(msg, result.Msg);

        Assert.Equal(ResultCodeEnum.DtoNotComplete, result.ResultCode);
    }

    [Fact]
    public async Task EditMemoAsync_ReturnApiResponseAndResultCodeIsServerError_WhenEditMemoIsError()
    {
        // Arrange
        _mockRepo.Setup(repo => repo.UpdateMemoAsync(It.IsAny<MemoInfo>())).ReturnsAsync(10);

        var _memoService = new MemoService(_apiResponseHelper, _mockRepo.Object);

        // Act
        var result = await _memoService.EditMemoAsync(new EditMemoDto { MemoId = 1, Title = "测试标题", Content = "测试内容", Status = 1 });

        // Assert
        Assert.NotNull(result);

        var msg = "找不到备忘录";

        Assert.Equal(msg, result.Msg);

        Assert.Equal(ResultCodeEnum.DataBaseError, result.ResultCode);
    }

    [Fact]
    public async Task EditMemoAsync_ReturnApiResponseAndResultCodeIsServerError_WhenEditMemoToDatabaseError()
    {
        // Arrange
        _mockRepo.Setup(repo => repo.UpdateMemoAsync(It.IsAny<MemoInfo>())).ThrowsAsync(new Exception());

        var _memoService = new MemoService(_apiResponseHelper, _mockRepo.Object);

        // Act
        var result = await _memoService.EditMemoAsync(new EditMemoDto { MemoId = 1, Title = "测试标题", Content = "测试内容", Status = 1 });

        // Assert
        Assert.NotNull(result);

        var msg = "服务器忙，请稍后...";

        Assert.Equal(msg, result.Msg);

        Assert.Equal(ResultCodeEnum.Error, result.ResultCode);
    }
    #endregion

    #region GetConditionQueryMemoList
    [Theory]
    [InlineData("测试")]
    [InlineData("")]
    [InlineData(null)]

    public async Task GetConditionQueryMemoList_ReturnApiResponseAndResultCodeIsSuccess_WhenGetConditionQueryMemoListIsSuccessfully(string searchTest)
    {
        // Arrange
        _mockRepo.Setup(repo => repo.SearchMemosAsync(It.IsAny<string>())).ReturnsAsync((new List<MemoInfo>(), 0));

        var _memoService = new MemoService(_apiResponseHelper, _mockRepo.Object);

        // Act
        var result = await _memoService.GetConditionQueryMemoListAsync(searchTest);

        // Assert
        Assert.NotNull(result);

        var msg = "获取查询的备忘录成功";

        Assert.Equal(msg, result.Msg);

        Assert.NotNull(result.ResultData);

        var type = result.ResultData.GetType();

        Assert.Equal(typeof(List<MemoInfo>), type);

        Assert.Equal(ResultCodeEnum.Success, result.ResultCode);
    }

    [Fact]
    public async Task GetConditionQueryMemoList_ReturnApiResponseAndResultCodeIsServerError_WhenGetConditionQueryMemoListIsError()
    {
        // Arrange
        _mockRepo.Setup(repo => repo.SearchMemosAsync(It.IsAny<string>())).ThrowsAsync(new Exception());

        var _memoService = new MemoService(_apiResponseHelper, _mockRepo.Object);

        // Act
        var result = await _memoService.GetConditionQueryMemoListAsync("测试");

        // Assert
        Assert.NotNull(result);

        var msg = "服务器忙，请稍后...";

        Assert.Equal(msg, result.Msg);

        Assert.Equal(ResultCodeEnum.Error, result.ResultCode);
    }
    #endregion

}
