using Daily.API.DataModel;
using Daily.API.Repositories;
using Microsoft.EntityFrameworkCore;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Daily.API.Tests.Repositories;
public class MemoRepoServiceTests
{
    // 使用InMemoryDatabase创建一个新的数据库，避免数据库名称重复，每次测试都是一个新的数据库，避免数据污染
    private readonly DbContextOptions<DailyDbContext> _options;

    public MemoRepoServiceTests()
    {
        //用guid创建一个新的数据库名称,避免数据库名称重复,每次测试都是一个新的数据库,避免数据污染
        var dbName = Guid.NewGuid().ToString();
        _options = new DbContextOptionsBuilder<DailyDbContext>()
            .UseInMemoryDatabase(dbName)
            .Options;
    }


    #region AddMemoAsync
    [Fact]
    public async Task AddMemoAsync_WhenMemoIsNull_ThrowArgumentNullException()
    {
        using var _db = new DailyDbContext(_options);
        var _memoRepoService = new MemoRepoService(_db);

        // Arrange
        MemoInfo memo = null;

        // Act
        async Task Act() => await _memoRepoService.AddMemoAsync(memo);

        // Assert
        await Assert.ThrowsAsync<ArgumentNullException>(Act);
    }

    [Theory]
    [InlineData("", "content")]
    [InlineData("title", "")]
    public async Task AddMemoAsync_WhenTitleOrContentIsNullOrEmpty_ReturnFalse(string title, string content)
    {
        using var _db = new DailyDbContext(_options);
        var _memoRepoService = new MemoRepoService(_db);

        // Arrange
        MemoInfo memo = new MemoInfo
        {
            Title = title,
            Content = content
        };

        // Act
        var result = await _memoRepoService.AddMemoAsync(memo);

        // Assert
        Assert.False(result.Success);
        Assert.Equal(5, result.Code);
    }

    [Fact]
    public async Task AddMemoAsync_WhenSaveChangesAsyncReturnNotOne_ReturnFalse()
    {
        // Arrange
        var mockDbSet = new Mock<DbSet<MemoInfo>>();
        var mockContext = new Mock<DailyDbContext>(_options);
        mockContext.Setup(m => m.MemoInfo).Returns(mockDbSet.Object);
        mockContext.Setup(m => m.SaveChangesAsync(default)).ReturnsAsync(2); // Mock SaveChangesAsync to return 2

        var _memoRepoService = new MemoRepoService(mockContext.Object);

        MemoInfo memo = new()
        {
            Title = "Title",
            Content = "Content"
        };

        // Act
        var result = await _memoRepoService.AddMemoAsync(memo);

        // Assert
        Assert.False(result.Success);
        Assert.Equal(10, result.Code);
    }


    [Fact]
    public async Task AddMemoAsync_WhenSaveChangesAsyncReturnOne_ReturnTrue()
    {
        using var _db = new DailyDbContext(_options);
        var _memoRepoService = new MemoRepoService(_db);

        // Arrange
        MemoInfo memo = new MemoInfo
        {
            Title = "Title",
            Content = "Content"
        };

        // Act
        var result = await _memoRepoService.AddMemoAsync(memo);

        // Assert
        Assert.True(result.Success);
        Assert.Equal(0, result.Code);
    }
    #endregion

    #region DeleteMemoAsync
    //返回一个元组，Success判断成功与否，Code是0-成功；5-id非法；10-信息不全；15-数据库操作结果不为1
    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public async Task DeleteMemoAsync_WhenIdIsInvalid_ReturnFalse(int id)
    {
        using var _db = new DailyDbContext(_options);
        var _memoRepoService = new MemoRepoService(_db);

        // Arrange
        // Act
        var result = await _memoRepoService.DeleteMemoAsync(id);

        // Assert
        Assert.False(result.Success);
        Assert.Equal(5, result.Code);
    }

    [Fact]
    public async Task DeleteMemoAsync_WhenSaveChangesAsyncReturnNotOne_ReturnFalse()
    {
        // Arrange
        var mockDbSet = new Mock<DbSet<MemoInfo>>();
        var mockContext = new Mock<DailyDbContext>(_options);
        mockContext.Setup(m => m.MemoInfo).Returns(mockDbSet.Object);
        mockContext.Setup(m => m.SaveChangesAsync(default)).ReturnsAsync(2); // Mock SaveChangesAsync to return 2

        var memo = new MemoInfo { MemoId = 1 };
        mockContext.Setup(m => m.MemoInfo.FindAsync(1)).ReturnsAsync(memo); // Mock FindAsync to return a memo with ID 1


        var _memoRepoService = new MemoRepoService(mockContext.Object);

        // Act
        var result = await _memoRepoService.DeleteMemoAsync(1);

        // Assert
        Assert.False(result.Success);
        Assert.Equal(15, result.Code);
    }

    [Fact]
    public async Task DeleteMemoAsync_WhenSaveChangesAsyncReturnOne_ReturnTrue()
    {
        using var _db = new DailyDbContext(_options);
        var _memoRepoService = new MemoRepoService(_db);

        // Arrange
        MemoInfo memo = new MemoInfo
        {
            Title = "Title",
            Content = "Content"
        };
        await _db.MemoInfo.AddAsync(memo);
        await _db.SaveChangesAsync();

        // Act
        var result = await _memoRepoService.DeleteMemoAsync(memo.MemoId);

        // Assert
        Assert.True(result.Success);
        Assert.Equal(0, result.Code);

        var deletedMemo = await _db.MemoInfo.FindAsync(memo.MemoId);
        Assert.Null(deletedMemo);
    }

    [Fact]
    public async Task DeleteMemoAsync_WhenMemoIsNull_ReturnFalse()
    {
        using var _db = new DailyDbContext(_options);
        var _memoRepoService = new MemoRepoService(_db);

        // Arrange
        int id = 100;

        // Act
        var result = await _memoRepoService.DeleteMemoAsync(id);

        // Assert
        Assert.False(result.Success);
        Assert.Equal(10, result.Code);
    }


    #endregion

    #region GetAllMemosAsync
    [Fact]
    public async Task GetAllMemosAsync_ReturnMemoInfoList()
    {
        using var _db = new DailyDbContext(_options);
        var _memoRepoService = new MemoRepoService(_db);

        // Arrange
        var memo1 = new MemoInfo { Title = "Title1", Content = "Content1" };
        var memo2 = new MemoInfo { Title = "Title2", Content = "Content2" };
        await _db.MemoInfo.AddRangeAsync(memo1, memo2);
        await _db.SaveChangesAsync();

        // Act
        var result = await _memoRepoService.GetAllMemosAsync();

        // Assert
        Assert.Equal(2, result.memoInfoList.Count());
        Assert.Equal(0, result.Code);
    }

    [Fact]
    public async Task GetAllMemosAsync_WhenMemoInfoListIsEmpty_ReturnEmptyList()
    {
        using var _db = new DailyDbContext(_options);
        var _memoRepoService = new MemoRepoService(_db);

        // Arrange

        // Act
        var result = await _memoRepoService.GetAllMemosAsync();

        // Assert
        Assert.Empty(result.memoInfoList);
        Assert.Equal(0, result.Code);
    }
    #endregion

    #region UpdateMemoAsync
    //返回0-成功；5-信息不全；10-找不到该备忘录；15-数据库操作结果不为1
    [Fact]
    public async Task UpdateMemoAsync_WhenMemoIsNull_ReturnFalse()
    {
        using var _db = new DailyDbContext(_options);
        var _memoRepoService = new MemoRepoService(_db);

        // Arrange
        MemoInfo memo = new()
        {
            Title = "测试标题",
            Content = "测试内容",
            Status  =  1,
            MemoId = 10
           
        };

        // Act
        var result = await _memoRepoService.UpdateMemoAsync(memo);

        // Assert
        Assert.Equal(10, result);
    }

    [Theory]
    [InlineData("", "content")]
    [InlineData("title", "")]
    public async Task UpdateMemoAsync_WhenTitleOrContentIsNullOrEmpty_ReturnFalse(string title, string content)
    {
        using var _db = new DailyDbContext(_options);
        var _memoRepoService = new MemoRepoService(_db);

        // Arrange
        MemoInfo memo = new MemoInfo
        {
            Title = title,
            Content = content
        };

        // Act
        var result = await _memoRepoService.UpdateMemoAsync(memo);

        // Assert
        Assert.Equal(5, result);
    }

    [Fact]
    public async Task UpdateMemoAsync_WhenMemoInfoIsNull_ReturnFalse()
    {
        using var _db = new DailyDbContext(_options);
        var _memoRepoService = new MemoRepoService(_db);

        // Arrange
        MemoInfo memo = new MemoInfo
        {
            Title = "Title",
            Content = "Content"
        };

        // Act
        var result = await _memoRepoService.UpdateMemoAsync(memo);

        // Assert
        Assert.Equal(10, result);
    }

  

    [Fact]
    public async Task UpdateMemoAsync_WhenSaveChangesAsyncReturnOne_ReturnTrue()
    {
        using var _db = new DailyDbContext(_options);
        var _memoRepoService = new MemoRepoService(_db);

        // Arrange
        MemoInfo memo = new MemoInfo
        {
            Title = "Title",
            Content = "Content"
        };
        await _db.MemoInfo.AddAsync(memo);
        await _db.SaveChangesAsync();

        memo.Title = "New Title";
        memo.Content = "New Content";

        // Act
        var result = await _memoRepoService.UpdateMemoAsync(memo);

        // Assert
        Assert.Equal(0, result);

        var updatedMemo = await _db.MemoInfo.FindAsync(memo.MemoId);
        Assert.Equal("New Title", updatedMemo.Title);
        Assert.Equal("New Content", updatedMemo.Content);
    }
    #endregion

    #region SearchMemosAsync
    // <returns>返回一个元组，包括备忘录列表和Code是0-成功</returns>
    [Theory]
    [InlineData("Title1")]
    [InlineData("Content1")]
    public async Task SearchMemosAsync_ReturnMemoInfoList(string searchText)
    {
        using var _db = new DailyDbContext(_options);
        var _memoRepoService = new MemoRepoService(_db);

        // Arrange
        var memo1 = new MemoInfo { Title = "Title1", Content = "Content1" };
        var memo2 = new MemoInfo { Title = "Title2", Content = "Content2" };
        await _db.MemoInfo.AddRangeAsync(memo1, memo2);
        await _db.SaveChangesAsync();

        // Act
        var result = await _memoRepoService.SearchMemosAsync(searchText);

        // Assert
        Assert.Single(result.memoInfoList);
        Assert.Equal(0, result.Code);
    }

    [Theory]
    [InlineData("")]
    [InlineData(null)]
    public async Task SearchMemosAsync_WhenSearchTextIsNullOrEmpty_ReturnEmptyList(string searchText)
    {
        using var _db = new DailyDbContext(_options);
        var _memoRepoService = new MemoRepoService(_db);

        // Arrange
        var memo1 = new MemoInfo { Title = "Title1", Content = "Content1" };
        var memo2 = new MemoInfo { Title = "Title2", Content = "Content2" };
        await _db.MemoInfo.AddRangeAsync(memo1, memo2);
        await _db.SaveChangesAsync();

        // Arrange


        // Act
        var result = await _memoRepoService.SearchMemosAsync(searchText);

        // Assert
        Assert.True(result.memoInfoList.Count() == 2);
        Assert.Equal(0, result.Code);
    }

    [Fact]
    public async Task SearchMemosAsync_WhenMemoInfoListIsEmpty_ReturnEmptyList()
    {
        using var _db = new DailyDbContext(_options);
        var _memoRepoService = new MemoRepoService(_db);

        // Arrange

        // Act
        var result = await _memoRepoService.SearchMemosAsync("Title");

        // Assert
        Assert.Empty(result.memoInfoList);
        Assert.Equal(0, result.Code);
    }

    [Fact]
    public async Task SearchMemosAsync_WhenMemoInfoListIsEmpty_ReturnEmptyList2()
    {
        using var _db = new DailyDbContext(_options);
        var _memoRepoService = new MemoRepoService(_db);

        // Arrange

        // Act
        var result = await _memoRepoService.SearchMemosAsync("Content");

        // Assert
        Assert.Empty(result.memoInfoList);
        Assert.Equal(0, result.Code);
    }
    #endregion
}
