//xUnit每个测试方法创建新的测试类实例
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

public class AccountRepoServiceTests
{
    // 使用InMemoryDatabase创建一个新的数据库，避免数据库名称重复，每次测试都是一个新的数据库，避免数据污染
    private readonly DbContextOptions<DailyDbContext> _options;

    public AccountRepoServiceTests()
    {
        //用guid创建一个新的数据库名称,避免数据库名称重复,每次测试都是一个新的数据库,避免数据污染
        var dbName = Guid.NewGuid().ToString();
        _options = new DbContextOptionsBuilder<DailyDbContext>()
            .UseInMemoryDatabase(dbName)
            .Options;
    }

    #region AddAsync
    // 返回一个元组，Success判断成功与否，Code是0-成功；5-账号已存在；10-用户信息不全或空；异常-抛出
    [Fact]
    public async Task AddAsync_ReturnTrueAndZero_WhenUserIsAddedSuccessfully()
    {
        //使用事务，避免对数据库造成影响,但是InMemoryDatabase不支持事务
        //using var transaction = await _db.Database.BeginTransactionAsync();

        //初始化数据库上下文，把数据库上下文传入仓储服务
        using var _db = new DailyDbContext(_options);
        var _accountRepoService = new AccountRepoService(_db);

        // Arrange           
        var accountInfo = new AccountInfo
        {
            Account = "admin",
            Pwd = "123456",
            Name = "管理员",
        };
        // Act
        var result = await _accountRepoService.AddAsync(accountInfo);
        // Assert
        var addAccount = await _db.AccountInfo.FirstOrDefaultAsync(u => u.Account == "admin");

        Assert.True(result.Success && result.Code == 0);
        Assert.NotNull(addAccount);
        Assert.Equal("管理员", addAccount.Name);

        // 回滚事务,避免对数据库造成影响,但是InMemoryDatabase不支持事务
        //await transaction.RollbackAsync();
    }

    [Fact]
    public async Task AddAsync_ReturnFalseAndFive_WhenUserIsExist()
    {
        //初始化数据库上下文，把数据库上下文传入仓储服务
        using var _db = new DailyDbContext(_options);
        var _accountRepoService = new AccountRepoService(_db);

        //在数据库添加一个用户
        await _db.AccountInfo.AddAsync(new AccountInfo
        {
            Account = "admin",
            Pwd = "123456",
            Name = "管理员",
        });

        await _db.SaveChangesAsync();

        //Arrange
        //在仓储服务添加一个已存在的用户
        var accountInfo = new AccountInfo
        {
            Account = "admin",
            Pwd = "123456",
            Name = "管理员",
        };

        //Act
        var result = await _accountRepoService.AddAsync(accountInfo);

        //Assert
        Assert.True(!result.Success && result.Code == 5);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")] // 可以添加其他空白字符串
    public async Task AddAsync_ReturnFalseAndTen_WhenAccountIsNullOrEmpty(string account)
    {
        //初始化数据库上下文，把数据库上下文传入仓储服务
        using var _db = new DailyDbContext(_options);
        var _accountRepoService = new AccountRepoService(_db);

        // Arrange
        var accountInfo = new AccountInfo
        {
            Account = account,
            Pwd = "123456",
            Name = "管理员",
        };

        // Act
        var result = await _accountRepoService.AddAsync(accountInfo);

        // Assert
        Assert.False(result.Success);
        Assert.Equal(10, result.Code);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    public async Task AddAsync_ReturnFalseAndTen_WhenNameIsNullOrEmpty(string name)
    {
        //初始化数据库上下文，把数据库上下文传入仓储服务
        using var _db = new DailyDbContext(_options);
        var _accountRepoService = new AccountRepoService(_db);

        // Arrange
        var accountInfo = new AccountInfo
        {
            Account = "admin",
            Pwd = "123456",
            Name = name,
        };

        // Act
        var result = await _accountRepoService.AddAsync(accountInfo);

        // Assert
        Assert.False(result.Success);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    public async Task AddAsync_ReturnFalseAndTen_WhenPwdIsNullOrEmpty(string pwd)
    {
        //初始化数据库上下文，把数据库上下文传入仓储服务
        using var _db = new DailyDbContext(_options);
        var _accountRepoService = new AccountRepoService(_db);

        // Arrange
        var accountInfo = new AccountInfo
        {
            Account = "admin",
            Pwd = pwd,
            Name = "管理员",
        };

        // Act
        var result = await _accountRepoService.AddAsync(accountInfo);

        // Assert
        Assert.False(result.Success);
    }

    [Fact]
    public async Task AddAsync_ThrowsArgumentNullException_WhenAccountInfoIsNull()
    {
        //初始化数据库上下文，把数据库上下文传入仓储服务
        using var _db = new DailyDbContext(_options);
        var _accountRepoService = new AccountRepoService(_db);

        // Arrange
        AccountInfo? accountInfo = null;

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentNullException>(async () => await _accountRepoService.AddAsync(accountInfo));
    }

    [Fact]
    public async Task AddAsync_ThrowsInvalidOperationException_WhenDatabaseUpdateError()
    {
        // Arrange
        var mockDb = new Mock<DailyDbContext>(_options);
        mockDb.Setup(db => db.SaveChangesAsync(default))
            .ThrowsAsync(new DbUpdateException());

        var data = new List<AccountInfo>().AsQueryable();

        var mockSet = new Mock<DbSet<AccountInfo>>();
        mockSet.As<IQueryable<AccountInfo>>().Setup(m => m.Provider).Returns(data.Provider);
        mockSet.As<IQueryable<AccountInfo>>().Setup(m => m.Expression).Returns(data.Expression);
        mockSet.As<IQueryable<AccountInfo>>().Setup(m => m.ElementType).Returns(data.ElementType);
        mockSet.As<IQueryable<AccountInfo>>().Setup(m => m.GetEnumerator()).Returns(data.GetEnumerator());

        mockDb.Setup(db => db.AccountInfo).Returns(mockSet.Object);

        var _accountRepoService = new AccountRepoService(mockDb.Object);

        // Arrange
        var accountInfo = new AccountInfo
        {
            Account = "admin",
            Pwd = "123456",
            Name = "管理员",
        };

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(async () =>
        {
            await _accountRepoService.AddAsync(accountInfo);
        });
    }
    #endregion

    #region GetByIdAsync
    //返回用户信息实例、Success判断成功与否，Code是0-成功；5-不存在的id；10-id为0和负数；异常-抛出
    [Fact]
    public async Task GetByIdAsync_ReturnTrueAccuontInfoAndZero_WhenIdIsValid()
    {
        //初始化数据库上下文，把数据库上下文传入仓储服务
        using var _db = new DailyDbContext(_options);
        var _accountRepoService = new AccountRepoService(_db);

        //在数据库添加一个用户
        await _db.AccountInfo.AddAsync(new AccountInfo
        {
            Account = "admin",
            Pwd = "123456",
            Name = "管理员",
        });

        //保存更改
        await _db.SaveChangesAsync();

        //Arrange
        var id = 1;

        //Act
        var (accountInfo, Success, Code) = await _accountRepoService.GetByIdAsync(id);
        Assert.NotNull(accountInfo);
        Assert.True(Success && Code == 0);
        Assert.Equal("admin", accountInfo.Account);
    }

    [Fact]
    public async Task GetByIdAsync_ReturnFalseNullAndFive_WhenIdNotExist()
    {
        //初始化数据库上下文，把数据库上下文传入仓储服务
        using var _db = new DailyDbContext(_options);
        var _accountRepoService = new AccountRepoService(_db);

        //Arrange
        var id = 1;

        //Act
        var (accountInfo, Success, Code) = await _accountRepoService.GetByIdAsync(id);

        //Assert
        Assert.Null(accountInfo);
        Assert.True(!Success && Code == 5);
    }

    [Theory]
    [InlineData(-1)]
    [InlineData(0)]
    public async Task GetByIdAsync_ReturnFalseNullAndTen_WhenIdLessThanOrEqualZero(int id)
    {
        //初始化数据库上下文，把数据库上下文传入仓储服务
        using var _db = new DailyDbContext(_options);
        var _accountRepoService = new AccountRepoService(_db);

        //Act & Arrange
        var (accountInfo, Success, Code) = await _accountRepoService.GetByIdAsync(id);

        //Assert
        Assert.Null(accountInfo);
        Assert.True(!Success && Code == 10);
    }

    [Fact]
    public async Task GetByIdAsync_ThrowsInvalidOperationException_WhenDatabaseError()
    {
        // Arrange
        var mockDb = new Mock<DailyDbContext>(_options);
        mockDb.Setup(db => db.AccountInfo.FindAsync(default))
            .ThrowsAsync(new DbUpdateException());

        var data = new List<AccountInfo>().AsQueryable();

        var mockSet = new Mock<DbSet<AccountInfo>>();
        mockSet.As<IQueryable<AccountInfo>>().Setup(m => m.Provider).Returns(data.Provider);
        mockSet.As<IQueryable<AccountInfo>>().Setup(m => m.Expression).Returns(data.Expression);
        mockSet.As<IQueryable<AccountInfo>>().Setup(m => m.ElementType).Returns(data.ElementType);
        mockSet.As<IQueryable<AccountInfo>>().Setup(m => m.GetEnumerator()).Returns(data.GetEnumerator());

        mockDb.Setup(db => db.AccountInfo).Returns(mockSet.Object);

        var _accountRepoService = new AccountRepoService(mockDb.Object);

        // Arrange
        var id = 1;

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(async () =>
        {
            await _accountRepoService.GetByIdAsync(id);
        });
    }
    #endregion

    #region LoginAsync
    //返回一个元组，Success判断成功与否，Code是0-成功；5-账号或密码错误；10-用户信息不全或空；异常-抛出
    [Fact]
    public async Task LoginAsync_ReturnTrueAndZreo_WhenUserIsLoginedSuccessfully()
    {
        //初始化数据库上下文，把数据库上下文传入仓储服务
        using var _db = new DailyDbContext(_options);
        var _accountRepoService = new AccountRepoService(_db);

        //在数据库添加一个用户
        await _db.AccountInfo.AddAsync(new AccountInfo
        {
            Account = "admin",
            Pwd = "123456",
            Name = "管理员",
        });

        //保存更改
        await _db.SaveChangesAsync();

        //Arrange
        var account = "admin";

        //Act
        var (Success, Code, Name) = await _accountRepoService.LoginAsync(account, "123456");

        //Assert    
        Assert.True(Success && Code == 0 && Name.Equals( "管理员"));
    }

    [Fact]
    public async Task LoginAsync_ReturnFalseAndFive_WhenAccountOrPasswordIsError()
    {
        //初始化数据库上下文，把数据库上下文传入仓储服务
        using var _db = new DailyDbContext(_options);
        var _accountRepoService = new AccountRepoService(_db);

        //在数据库添加一个用户
        await _db.AccountInfo.AddAsync(new AccountInfo
        {
            Account = "admin",
            Pwd = "123456",
            Name = "管理员",
        });

        //保存更改
        await _db.SaveChangesAsync();

        //Arrange
        var account = "admin";

        //Act
        var (Success, Code, Name) = await _accountRepoService.LoginAsync(account, "1234567");

        //Assert
        Assert.True(!Success && Code == 5 && string.IsNullOrEmpty(Name));
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    public async Task LoginAsync_ReturnFalseAndTen_WhenAccountIsNullOrEmpty(string account)
    {
        //初始化数据库上下文，把数据库上下文传入仓储服务
        using var _db = new DailyDbContext(_options);
        var _accountRepoService = new AccountRepoService(_db);

        //Arrange
        //Act
        var (Success, Code, Name) = await _accountRepoService.LoginAsync(account, "123456");

        //Assert
        Assert.True(!Success && Code == 10 && string.IsNullOrEmpty(Name));
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    public async Task LoginAsync_ReturnFalseAndTen_WhenPasswordIsNullOrEmpty(string password)
    {
        //初始化数据库上下文，把数据库上下文传入仓储服务
        using var _db = new DailyDbContext(_options);
        var _accountRepoService = new AccountRepoService(_db);

        //Arrange
        //Act
        var (Success, Code, Name) = await _accountRepoService.LoginAsync("admin", password);

        //Assert
        Assert.True(!Success && Code == 10 && string.IsNullOrEmpty(Name));
    }

    [Fact]
    public async Task LoginAsync_ThrowsInvalidOperationException_WhenDatabaseError()
    {
        // Arrange
        var mockDb = new Mock<DailyDbContext>(_options);
        mockDb.Setup(db => db.AccountInfo.FindAsync(It.IsAny<CancellationToken>()))
                                                    .ReturnsAsync(new AccountInfo());


        var data = new List<AccountInfo>().AsQueryable();

        var mockSet = new Mock<DbSet<AccountInfo>>();
        mockSet.As<IQueryable<AccountInfo>>().Setup(m => m.Provider).Returns(data.Provider);
        mockSet.As<IQueryable<AccountInfo>>().Setup(m => m.Expression).Returns(data.Expression);
        mockSet.As<IQueryable<AccountInfo>>().Setup(m => m.ElementType).Returns(data.ElementType);
        mockSet.As<IQueryable<AccountInfo>>().Setup(m => m.GetEnumerator()).Returns(data.GetEnumerator());

        mockDb.Setup(db => db.AccountInfo).Returns(mockSet.Object);

        var _accountRepoService = new AccountRepoService(mockDb.Object);

        // Arrange
        var account = "admin";

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(async () =>
        {
            await _accountRepoService.LoginAsync(account, "123456");
        });
    } 
    #endregion

}
