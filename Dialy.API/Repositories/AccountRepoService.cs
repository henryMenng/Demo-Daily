using Daily.API.DataModel;
using Microsoft.EntityFrameworkCore;
using System.Data.Common;
using System.Xml.Linq;

namespace Daily.API.Repositories;

/// <summary>
/// 实现AccountInfo仓储接口的类
/// </summary>
public class AccountRepoService : IAccountRepoService
{
    /// <summary>
    /// 数据库上下文
    /// </summary>
    private readonly DailyDbContext _db;

    /// <summary>
    /// 用户信息仓储的构造函数
    /// </summary>
    /// <param name="db">数据库上下文</param>
    public AccountRepoService(DailyDbContext db) => _db = db;

    /// <summary>
    ///  实现根据id查找用户的接口方法
    /// </summary>
    /// <param name="id">用户 id</param>
    /// <returns>返回用户信息实例、Success判断成功与否，Code是0-成功；5-不存在的id；10-id为0和负数；异常-抛出</returns>
    public async Task<(AccountInfo? accountInfo, bool Success, int Code)> GetByIdAsync(int id)
    {
        try
        {
            if (id <= 0)
                return (null, false, 10);

            var accountInfo = await _db.AccountInfo.AsNoTracking().FirstOrDefaultAsync(temp => temp.AccountId == id);

            if (accountInfo == null)
                return (null, false, 5);

            return (accountInfo, true, 0);
        }
        catch (DbUpdateException ex)
        {
            throw new InvalidOperationException("查找用户信息时发生数据库查询错误", ex);
        }

    }

    /// <summary>
    /// 实现添加用户信息的接口方法
    /// </summary>
    /// <param name="accountInfo">用户信息</param>
    /// <returns>返回一个元组，Success判断成功与否，Code是0-成功；5-账号已存在；10-用户信息不全或空；异常-抛出</returns>
    public async Task<(bool Success,int Code)> AddAsync(AccountInfo accountInfo)
    {
        ArgumentNullException.ThrowIfNull(accountInfo);

        if (string.IsNullOrEmpty(accountInfo.Account) || string.IsNullOrEmpty(accountInfo.Name) || string.IsNullOrEmpty(accountInfo.Pwd))
            return (false,10);

        try
        {
            // 查询数据库中是否已经存在该账号，不跟踪查询结果，因为只是查询,提高性能
            var existingUser = await _db.AccountInfo.AsNoTracking()
                                .FirstOrDefaultAsync(u => u.Account == accountInfo.Account);
            if (existingUser != null)
                return (false,5);

            // 将新的用户信息对象添加到数据库中
            await _db.AddAsync(accountInfo);

            // 保存更改
            await _db.SaveChangesAsync();
            return (true,0);
        }
        catch (DbUpdateException ex)
        {
            throw new InvalidOperationException("添加用户信息时发生数据库错误", ex);
        }
    }

    /// <summary>
    /// 实现账号登录的接口方法
    /// </summary>
    /// <param name="account">账号</param>
    /// <param name="password">密码</param>
    /// <returns>返回一个元组，Success判断成功与否，Name返回用户名，Code是0-成功；5-账号或密码错误；10-用户信息不全或空；异常-抛出</returns>
    /// <exception cref="InvalidOperationException"></exception>
    public async Task<(bool Success, int Code,string Name)> LoginAsync(string account, string password)
    {
        try
        {
            if (string.IsNullOrEmpty(account) || string.IsNullOrEmpty(password))
                return (false, 10,string.Empty); // 用户信息不全或空

            var accountInfo = await _db.AccountInfo.AsNoTracking()
                .FirstOrDefaultAsync(u => u.Account == account && u.Pwd == password);

            if (accountInfo == null)
                return (false, 5,string.Empty); // 账号或密码错误

            return (true, 0,accountInfo.Name); // 登录成功
        }
        catch (DbException ex)
        {
            throw new InvalidOperationException("登录时发生数据库查询错误", ex);
        }
    }

}

