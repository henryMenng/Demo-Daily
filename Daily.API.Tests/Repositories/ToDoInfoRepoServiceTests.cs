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
public class ToDoInfoRepoServiceTests
{
    // 使用InMemoryDatabase创建一个新的数据库，避免数据库名称重复，每次测试都是一个新的数据库，避免数据污染
    private readonly DbContextOptions<DailyDbContext> _options;

    public ToDoInfoRepoServiceTests()
    {
        _options = new DbContextOptionsBuilder<DailyDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
    }

    #region AddToDoInfoAsync

    [Fact]
    public async Task AddToDoInfoAsync_ReturnTrueAndZreo_WhenToDoInfoAddedSuccessfully()
    {
        //初始化数据库上下文，把数据库上下文传入仓储服务
        using var _db = new DailyDbContext(_options);
        var _toDoInfoRepoService = new ToDoInfoRepoService(_db);

        //准备数据
        var todo = new ToDoInfo
        {
            Title = "Test",
            Content = "Test",
            Status = 1
        };

        //执行待测方法
        var (Success, Code) = await _toDoInfoRepoService.AddToDoInfoAsync(todo);

        //断言
        Assert.True(Success && Code == 0);

        //验证
        var result = await _db.ToDoInfo.FirstOrDefaultAsync();
        Assert.NotNull(result);
        Assert.Equal("Test", result.Title);
        Assert.Equal("Test", result.Content);
        Assert.Equal(1, result.Status);
    }

    [Theory]
    [InlineData("", "Test")]
    [InlineData("Test", "")]
    [InlineData(null, "Test")]
    [InlineData("Test", null)]
    public async Task AddToDoInfoAsync_ReturnFalseAndFive_WhenTitleOrContentIsNull(string title, string content)
    {
        //初始化数据库上下文，把数据库上下文传入仓储服务
        using var _db = new DailyDbContext(_options);
        var _toDoInfoRepoService = new ToDoInfoRepoService(_db);

        //准备数据
        var todo = new ToDoInfo
        {
            Title = title,
            Content = content,
            Status = 1
        };

        //执行待测方法
        var (Success, Code) = await _toDoInfoRepoService.AddToDoInfoAsync(todo);

        //断言
        Assert.True(!Success && Code == 5);
    }

    [Fact]
    public async Task AddToDoInfoAsync_ThrowInvalidOperationException_WhenDbUpdateException()
    {
        var mockDb = new Mock<DailyDbContext>(_options);
        mockDb.Setup(db => db.SaveChangesAsync(default))
            .ThrowsAsync(new DbUpdateException());

        var data = new List<ToDoInfo>().AsQueryable();

        var mockSet = new Mock<DbSet<ToDoInfo>>();
        mockSet.As<IQueryable<ToDoInfo>>().Setup(m => m.Provider).Returns(data.Provider);
        mockSet.As<IQueryable<ToDoInfo>>().Setup(m => m.Expression).Returns(data.Expression);
        mockSet.As<IQueryable<ToDoInfo>>().Setup(m => m.ElementType).Returns(data.ElementType);
        mockSet.As<IQueryable<ToDoInfo>>().Setup(m => m.GetEnumerator()).Returns(data.GetEnumerator());

        mockDb.Setup(db => db.ToDoInfo).Returns(mockSet.Object);

        var _toDoInfoRepoService = new ToDoInfoRepoService(mockDb.Object);

        //准备数据
        var todo = new ToDoInfo
        {
            Title = "Test",
            Content = "Test",
            Status = 1
        };

        //执行待测方法和断言
        var ex = await Assert.ThrowsAsync<InvalidOperationException>(() => _toDoInfoRepoService.AddToDoInfoAsync(todo));

        //断言
        Assert.Equal("添加待办事项时发生数据库错误", ex.Message);
    }

    [Fact]
    public async Task AddToDoInfoAsync_ThrowArgumentNullException_WhenDbUpdateException()
    {
        //初始化数据库上下文，把数据库上下文传入仓储服务
        using var _db = new DailyDbContext(_options);
        var _toDoInfoRepoService = new ToDoInfoRepoService(_db);

        //准备数据
        ToDoInfo todo = null;

        //执行待测方法和断言
        await Assert.ThrowsAsync<ArgumentNullException>(() => _toDoInfoRepoService.AddToDoInfoAsync(todo));
    }
    #endregion

    #region DeleteToDoInfoAsync
    [Fact]
    public async Task DeleteToDoInfoAsync_ReturnTrueAndZreo_WhenToDoInfoDeletedSuccessfully()
    {
        //初始化数据库上下文，把数据库上下文传入仓储服务
        using var _db = new DailyDbContext(_options);
        var _toDoInfoRepoService = new ToDoInfoRepoService(_db);

        //准备数据
        var todo = new ToDoInfo
        {
            Title = "Test",
            Content = "Test",
            Status = 1
        };

        await _db.AddAsync(todo);
        await _db.SaveChangesAsync();

        //执行待测方法
        var (Success, Code) = await _toDoInfoRepoService.DeleteToDoInfoAsync(todo.ToDoId);

        //断言
        Assert.True(Success && Code == 0);

        //验证
        var result = await _db.ToDoInfo.FirstOrDefaultAsync();
        Assert.Null(result);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public async Task DeleteToDoInfoAsync_ReturnFalseAndFive_WhenIdIsZeroOrNegative(int id)
    {
        //初始化数据库上下文，把数据库上下文传入仓储服务
        using var _db = new DailyDbContext(_options);
        var _toDoInfoRepoService = new ToDoInfoRepoService(_db);

        //执行待测方法
        var (Success, Code) = await _toDoInfoRepoService.DeleteToDoInfoAsync(id);

        //断言
        Assert.True(!Success && Code == 5);
    }

    [Fact]
    public async Task DeleteToDoInfoAsync_ReturnFalseAndTen_WhenIdIsNotExist()
    {
        //初始化数据库上下文，把数据库上下文传入仓储服务
        using var _db = new DailyDbContext(_options);
        var _toDoInfoRepoService = new ToDoInfoRepoService(_db);

        //准备数据
        var todo = new ToDoInfo
        {
            Title = "Test",
            Content = "Test",
            Status = 1
        };

        await _db.AddAsync(todo);
        await _db.SaveChangesAsync();

        //执行待测方法
        var (Success, Code) = await _toDoInfoRepoService.DeleteToDoInfoAsync(todo.ToDoId + 1);

        //断言
        Assert.True(!Success && Code == 10);
    }

    [Fact]
    public async Task DeleteToDoInfoAsync_ThrowInvalidOperationException_WhenDbUpdateException()
    {
        //初始化数据库上下文，把数据库上下文传入仓储服务
        using var _db = new DailyDbContext(_options);

        var mockDb = new Mock<DailyDbContext>(_options);
        mockDb.Setup(db => db.FindAsync(default))
    .ThrowsAsync(new DbUpdateException());


        var data = new List<ToDoInfo>().AsQueryable();

        var mockSet = new Mock<DbSet<ToDoInfo>>();
        mockSet.As<IQueryable<ToDoInfo>>().Setup(m => m.Provider).Returns(data.Provider);
        mockSet.As<IQueryable<ToDoInfo>>().Setup(m => m.Expression).Returns(data.Expression);
        mockSet.As<IQueryable<ToDoInfo>>().Setup(m => m.ElementType).Returns(data.ElementType);
        mockSet.As<IQueryable<ToDoInfo>>().Setup(m => m.GetEnumerator()).Returns(data.GetEnumerator());

        mockDb.Setup(db => db.ToDoInfo).Returns(mockSet.Object);

        var _toDoInfoRepoService = new ToDoInfoRepoService(mockDb.Object);

        //准备数据
        var todo = new ToDoInfo
        {
            Title = "Test",
            Content = "Test",
            Status = 1
        };

        await _db.AddAsync(todo);
        await _db.SaveChangesAsync();

        //执行待测方法和断言
        var ex = await Assert.ThrowsAsync<InvalidOperationException>(() => _toDoInfoRepoService.DeleteToDoInfoAsync(todo.ToDoId));
    }
    #endregion

    #region GetConditionQueryToDoListAsync
    [Theory]
    [InlineData(0, null)]
    [InlineData(0, "")]
    public async Task GetConditionQueryToDoListAsync_ReturnAllToDoInfoListAndZreo_WhenStatusIsZeroAndTextIsNullOrEmpty(int status, string searchText)
    {
        //初始化数据库上下文，把数据库上下文传入仓储服务
        using var _db = new DailyDbContext(_options);
        var _toDoInfoRepoService = new ToDoInfoRepoService(_db);

        //准备数据
        var todo1 = new ToDoInfo
        {
            Title = "Test1",
            Content = "Test1",
            Status = 1
        };

        var todo2 = new ToDoInfo
        {
            Title = "Test2",
            Content = "Test2",
            Status = 0
        };

        var todo3 = new ToDoInfo
        {
            Title = "Test3",
            Content = "Test3",
            Status = 1
        };

        await _db.AddRangeAsync(todo1, todo2, todo3);
        await _db.SaveChangesAsync();

        //执行待测方法
        var (toDoInfoList, Success, Code) = await _toDoInfoRepoService.GetConditionQueryToDoListAsync(status, searchText);

        //断言
        Assert.True(Success && Code == 0);
        Assert.Equal(3, toDoInfoList.Count());
    }

    [Fact]
    public async Task GetConditionQueryToDoListAsync_ReturnAllToDoInfoListAndFive_WhenStatusIsZreoAndTextIsNotNull()
    {
        //初始化数据库上下文，把数据库上下文传入仓储服务
        using var _db = new DailyDbContext(_options);
        var _toDoInfoRepoService = new ToDoInfoRepoService(_db);

        //准备数据
        var todo1 = new ToDoInfo
        {
            Title = "Test1",
            Content = "Test1",
            Status = 1
        };

        var todo2 = new ToDoInfo
        {
            Title = "Test2",
            Content = "Test2",
            Status = 0
        };

        var todo3 = new ToDoInfo
        {
            Title = "Test3",
            Content = "Test3",
            Status = 1
        };

        await _db.AddRangeAsync(todo1, todo2, todo3);
        await _db.SaveChangesAsync();

        //执行待测方法
        var (toDoInfoList, Success, Code) = await _toDoInfoRepoService.GetConditionQueryToDoListAsync(0, "Test");

        //断言
        Assert.True(Success && Code == 5);
        Assert.Equal(3, toDoInfoList.Count());
    }

    [Theory]
    [InlineData(1, null)]
    [InlineData(1, "")]
    public async Task GetConditionQueryToDoListAsync_ReturnAllToDoInfoListAndTen_WhenStatusIsOneAndTextIsNullOrEmpty(int status, string searchText)
    {
        //初始化数据库上下文，把数据库上下文传入仓储服务
        using var _db = new DailyDbContext(_options);
        var _toDoInfoRepoService = new ToDoInfoRepoService(_db);

        //准备数据
        var todo1 = new ToDoInfo
        {
            Title = "Test1",
            Content = "Test1",
            Status = 1
        };

        var todo2 = new ToDoInfo
        {
            Title = "Test2",
            Content = "Test2",
            Status = 0
        };

        var todo3 = new ToDoInfo
        {
            Title = "Test3",
            Content = "Test3",
            Status = 0
        };

        await _db.AddRangeAsync(todo1, todo2, todo3);
        await _db.SaveChangesAsync();

        //执行待测方法
        var (toDoInfoList, Success, Code) = await _toDoInfoRepoService.GetConditionQueryToDoListAsync(status, searchText);

        //断言
        Assert.True(Success && Code == 10);
        Assert.Equal(2, toDoInfoList.Count());
    }

    [Fact]
    public async Task GetConditionQueryToDoListAsync_ReturnAllToDoInfoListAndFifteen_WhenStatusIsOneAndTextIsNotNull()
    {
        //初始化数据库上下文，把数据库上下文传入仓储服务
        using var _db = new DailyDbContext(_options);
        var _toDoInfoRepoService = new ToDoInfoRepoService(_db);

        //准备数据
        var todo1 = new ToDoInfo
        {
            Title = "Test1",
            Content = "Test1",
            Status = 1
        };

        var todo2 = new ToDoInfo
        {
            Title = "Test2",
            Content = "Test2",
            Status = 0
        };

        var todo3 = new ToDoInfo
        {
            Title = "Test3",
            Content = "Test3",
            Status = 0
        };

        await _db.AddRangeAsync(todo1, todo2, todo3);
        await _db.SaveChangesAsync();

        //执行待测方法
        var (toDoInfoList, Success, Code) = await _toDoInfoRepoService.GetConditionQueryToDoListAsync(1, "Test");

        //断言
        Assert.True(Success && Code == 15);
        Assert.Equal(2, toDoInfoList.Count());
    }

    [Theory]
    [InlineData(2, null)]
    [InlineData(2, "")]
    public async Task GetConditionQueryToDoListAsync_ReturnAllToDoInfoListAndTwenty_WhenStatusIsTwoAndTextIsNullOrEmpty(int status, string searchText)
    {
        //初始化数据库上下文，把数据库上下文传入仓储服务
        using var _db = new DailyDbContext(_options);
        var _toDoInfoRepoService = new ToDoInfoRepoService(_db);

        //准备数据
        var todo1 = new ToDoInfo
        {
            Title = "Test1",
            Content = "Test1",
            Status = 1
        };

        var todo2 = new ToDoInfo
        {
            Title = "Test2",
            Content = "Test2",
            Status = 0
        };

        var todo3 = new ToDoInfo
        {
            Title = "Test3",
            Content = "Test3",
            Status = 0
        };

        await _db.AddRangeAsync(todo1, todo2, todo3);
        await _db.SaveChangesAsync();

        //执行待测方法
        var (toDoInfoList, Success, Code) = await _toDoInfoRepoService.GetConditionQueryToDoListAsync(status, searchText);

        //断言
        Assert.True(Success && Code == 20);
        Assert.Equal(1, toDoInfoList.Count());
    }

    [Fact]
    public async Task GetConditionQueryToDoListAsync_ReturnAllToDoInfoListAndTwentyFive_WhenStatusIsTwoAndTextIsNotNull()
    {
        //初始化数据库上下文，把数据库上下文传入仓储服务
        using var _db = new DailyDbContext(_options);
        var _toDoInfoRepoService = new ToDoInfoRepoService(_db);

        //准备数据
        var todo1 = new ToDoInfo
        {
            Title = "Test1",
            Content = "Test1",
            Status = 1
        };

        var todo2 = new ToDoInfo
        {
            Title = "Test2",
            Content = "Test2",
            Status = 0
        };

        var todo3 = new ToDoInfo
        {
            Title = "Test3",
            Content = "Test3",
            Status = 1
        };

        await _db.AddRangeAsync(todo1, todo2, todo3);
        await _db.SaveChangesAsync();

        //执行待测方法
        var (toDoInfoList, Success, Code) = await _toDoInfoRepoService.GetConditionQueryToDoListAsync(2, "Test");

        //断言
        Assert.True(Success && Code == 25);
        Assert.Equal(2, toDoInfoList.Count());
    }

    //Code = 30
    [Fact]
    public async Task GetConditionQueryToDoListAsync_ReturnAllToDoInfoListAndThirty_WhenStatusIsThree()
    {
        //初始化数据库上下文，把数据库上下文传入仓储服务
        using var _db = new DailyDbContext(_options);
        var _toDoInfoRepoService = new ToDoInfoRepoService(_db);

        //准备数据
        var todo1 = new ToDoInfo
        {
            Title = "Test1",
            Content = "Test1",
            Status = 1
        };

        var todo2 = new ToDoInfo
        {
            Title = "Test2",
            Content = "Test2",
            Status = 0
        };

        var todo3 = new ToDoInfo
        {
            Title = "Test3",
            Content = "Test3",
            Status = 1
        };

        await _db.AddRangeAsync(todo1, todo2, todo3);
        await _db.SaveChangesAsync();

        //执行待测方法
        var (toDoInfoList, Success, Code) = await _toDoInfoRepoService.GetConditionQueryToDoListAsync(3, "Test");

        //断言
        Assert.True(!Success && Code == 30);
        Assert.Null(toDoInfoList);
    }

    [Fact]
    public async Task GetConditionQueryToDoListAsync_ThrowInvalidOperationException_WhenDbUpdateException()
    {
        var mockDb = new Mock<DailyDbContext>(_options);
        mockDb.Setup(db => db.SaveChangesAsync(default))
            .ThrowsAsync(new DbUpdateException());

        var data = new List<ToDoInfo>().AsQueryable();

        var mockSet = new Mock<DbSet<ToDoInfo>>();
        mockSet.As<IQueryable<ToDoInfo>>().Setup(m => m.Provider).Returns(data.Provider);
        mockSet.As<IQueryable<ToDoInfo>>().Setup(m => m.Expression).Returns(data.Expression);
        mockSet.As<IQueryable<ToDoInfo>>().Setup(m => m.ElementType).Returns(data.ElementType);
        mockSet.As<IQueryable<ToDoInfo>>().Setup(m => m.GetEnumerator()).Returns(data.GetEnumerator());

        mockDb.Setup(db => db.ToDoInfo).Returns(mockSet.Object);

        var _toDoInfoRepoService = new ToDoInfoRepoService(mockDb.Object);

        //执行待测方法和断言
        var ex = await Assert.ThrowsAsync<InvalidOperationException>(() => _toDoInfoRepoService.GetConditionQueryToDoListAsync(0, null));
    }
    #endregion

    #region UpdateToDoInfoAsync
    //返回一个元组，Success判断成功与否，Code是0-成功；5-id非法；10：此id找不到待办事项；异常-抛出
    [Fact]
    public async Task UpdateToDoInfoAsync_ReturnTrueAndZreo_WhenToDoInfoUpdatedSuccessfully()
    {
        //初始化数据库上下文，把数据库上下文传入仓储服务
        using var _db = new DailyDbContext(_options);
        var _toDoInfoRepoService = new ToDoInfoRepoService(_db);

        //准备数据
        var todo = new ToDoInfo
        {
            Title = "Test",
            Content = "Test",
            Status = 1
        };

        await _db.AddAsync(todo);
        await _db.SaveChangesAsync();

        //执行待测方法
        todo.Title = "TestUpdate";
        todo.Content = "TestUpdate";
        todo.Status = 0;

        var (Success, Code) = await _toDoInfoRepoService.UpdateToDoInfoAsync(todo);

        //断言
        Assert.True(Success && Code == 0);

        //验证
        var result = await _db.ToDoInfo.FirstOrDefaultAsync();
        Assert.NotNull(result);
        Assert.Equal("TestUpdate", result.Title);
        Assert.Equal("TestUpdate", result.Content);
        Assert.Equal(0, result.Status);
    }

    [Fact]
    public async Task UpdateToDoInfoAsync_ReturnFalseAndFive_WhenIdIsZeroOrNegative()
    {
        //初始化数据库上下文，把数据库上下文传入仓储服务
        using var _db = new DailyDbContext(_options);
        var _toDoInfoRepoService = new ToDoInfoRepoService(_db);

        //准备数据
        var todo = new ToDoInfo
        {
            Title = "Test",
            Content = "Test",
            Status = 1
        };

        await _db.AddAsync(todo);
        await _db.SaveChangesAsync();

        //执行待测方法
        todo.ToDoId = 0;
        var (Success, Code) = await _toDoInfoRepoService.UpdateToDoInfoAsync(todo);

        //断言
        Assert.True(!Success && Code == 5);
    }

    [Fact]
    public async Task UpdateToDoInfoAsync_ReturnFalseAndTen_WhenIdIsNotExist()
    {
        //初始化数据库上下文，把数据库上下文传入仓储服务
        using var _db = new DailyDbContext(_options);
        var _toDoInfoRepoService = new ToDoInfoRepoService(_db);

        //准备数据
        var todo = new ToDoInfo
        {
            Title = "Test",
            Content = "Test",
            Status = 1
        };

        await _db.AddAsync(todo);
        await _db.SaveChangesAsync();

        //执行待测方法
        todo.ToDoId = todo.ToDoId + 1;
        var (Success, Code) = await _toDoInfoRepoService.UpdateToDoInfoAsync(todo);

        //断言
        Assert.True(!Success && Code == 10);
    }

    [Fact]
    public async Task UpdateToDoInfoAsync_ThrowInvalidOperationException_WhenDbUpdateException()
    {
        //初始化数据库上下文，把数据库上下文传入仓储服务
        using var _db = new DailyDbContext(_options);

        var mockDb = new Mock<DailyDbContext>(_options);
        mockDb.Setup(db => db.SaveChangesAsync(default))
            .ThrowsAsync(new DbUpdateException());

        var data = new List<ToDoInfo>().AsQueryable();

        var mockSet = new Mock<DbSet<ToDoInfo>>();
        mockSet.As<IQueryable<ToDoInfo>>().Setup(m => m.Provider).Returns(data.Provider);
        mockSet.As<IQueryable<ToDoInfo>>().Setup(m => m.Expression).Returns(data.Expression);
        mockSet.As<IQueryable<ToDoInfo>>().Setup(m => m.ElementType).Returns(data.ElementType);
        mockSet.As<IQueryable<ToDoInfo>>().Setup(m => m.GetEnumerator()).Returns(data.GetEnumerator());

        mockDb.Setup(db => db.ToDoInfo).Returns(mockSet.Object);

        var _toDoInfoRepoService = new ToDoInfoRepoService(mockDb.Object);

        //准备数据
        var todo = new ToDoInfo
        {
            Title = "Test",
            Content = "Test",
            Status = 1
        };

        await _db.AddAsync(todo);
        await _db.SaveChangesAsync();

        //执行待测方法和断言
        var ex = await Assert.ThrowsAsync<InvalidOperationException>(() => _toDoInfoRepoService.UpdateToDoInfoAsync(todo));
    }

    [Fact]
    public async Task UpdateToDoInfoAsync_ThrowArgumentNullException_WhenToDoInfoIsNull()
    {
        //初始化数据库上下文，把数据库上下文传入仓储服务
        using var _db = new DailyDbContext(_options);
        var _toDoInfoRepoService = new ToDoInfoRepoService(_db);

        //准备数据
        ToDoInfo todo = null;

        //执行待测方法和断言
        await Assert.ThrowsAsync<ArgumentNullException>(() => _toDoInfoRepoService.UpdateToDoInfoAsync(todo));
    }

    [Theory]
    [InlineData("", "TestUpdate")]
    [InlineData("TestUpdate", "")]
    [InlineData(null, "TestUpdate")]
    [InlineData("TestUpdate", null)]
    public async Task UpdateToDoInfoAsync_ReturnFalseAndFifteen_WhenTitleOrContentIsNull(string title, string content)
    {
        //初始化数据库上下文，把数据库上下文传入仓储服务
        using var _db = new DailyDbContext(_options);
        var _toDoInfoRepoService = new ToDoInfoRepoService(_db);

        //准备数据
        var todo = new ToDoInfo
        {
            Title = "Titel",
            Content = "content",
            Status = 1
        };

        await _db.AddAsync(todo);
        await _db.SaveChangesAsync();

        //执行待测方法
        todo.Title = title;
        todo.Content = content;
        todo.Status = 0;

        var (Success, Code) = await _toDoInfoRepoService.UpdateToDoInfoAsync(todo);

        //断言
        Assert.True(!Success && Code == 15);
    }

    #endregion

    #region UpdateToDoStatusAsync
    //返回一个元组，total是总数，completed是已完成数，Success成功与否，Code是0-成功；5-失败

    [Fact]
    public async Task UpdateToDoStatusAsync_ReturnTotalAndCompletedAndTrueAndZreo_WhenUpdateStatusSuccessfully()
    {
        //初始化数据库上下文，把数据库上下文传入仓储服务
        using var _db = new DailyDbContext(_options);
        var _toDoInfoRepoService = new ToDoInfoRepoService(_db);

        //准备数据
        var todo1 = new ToDoInfo
        {
            Title = "Test1",
            Content = "Test1",
            Status = 1
        };

        var todo2 = new ToDoInfo
        {
            Title = "Test2",
            Content = "Test2",
            Status = 0
        };

        var todo3 = new ToDoInfo
        {
            Title = "Test3",
            Content = "Test3",
            Status = 1
        };

        await _db.AddRangeAsync(todo1, todo2, todo3);
        await _db.SaveChangesAsync();

        //执行待测方法
        var result = await _toDoInfoRepoService.UpdateToDoStatus(1);

        //断言
        Assert.True(result == 0);
    }
    #endregion
}
