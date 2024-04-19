using Daily.API.DataModel;

namespace Daily.API.Repositories;

/// <summary>
/// 提供AccountInfo仓储的接口
/// </summary>
public interface IAccountRepoService
{
    /// <summary>
    /// 提供根据id查找用户的接口方法
    /// </summary>
    /// <param name="id">用户 id</param>
    /// <returns>返回用户信息实例、Success判断成功与否，Code是0-成功；5-不存在的id；10-id为0和负数；异常-抛出</returns>
    Task<(AccountInfo? accountInfo,bool Success,int Code)> GetByIdAsync(int id);

    /// <summary>
    /// 提供获得所有的用户信息的接口方法
    /// </summary>
    /// <param name="account">账号</param>
    /// <param name="password">密码</param>
    /// <returns>返回一个元组，Success判断成功与否，Name返回用户名，Code是0-成功；5-账号或密码错误；10-用户信息不全或空；异常-抛出</returns>
    Task<(bool Success, int Code, string Name)> LoginAsync(string account, string password);

    /// <summary>
    /// 提供添加用户信息的接口方法
    /// </summary>
    /// <param name="accountInfo">用户信息</param>
    /// <returns>返回一个元组，Success判断成功与否，Code是0-成功；5-账号已存在；10-用户信息不全或空；异常-抛出</returns>
    Task<(bool Success,int Code)> AddAsync(AccountInfo accountInfo);


}
