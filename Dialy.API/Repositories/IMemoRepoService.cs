using Daily.API.DataModel;

namespace Daily.API.Repositories;

/// <summary>
/// 提供备忘录信息仓储服务接口,提供添加、删除、查询备忘录等功能
/// </summary>
public interface IMemoRepoService
{
    /// <summary>
    /// 提供添加备忘录的仓储层服务
    /// </summary>
    /// <param name="memo">备忘录</param>
    /// <returns>返回一个元组，0-成功添加；5-信息不全；10-操作数据库结果不为1；异常-抛出</returns>
    Task<(bool Success,int Code)> AddMemoAsync(MemoInfo memo);

    /// <summary>
    /// 提供删除备忘录
    /// </summary>
    /// <param name="id">备忘录id</param>
    /// <returns>返回一个元组，Success判断成功与否，Code是0-成功；5-id非法；10-找不到该备忘录；15-数据库操作结果不为1</returns>
    Task<(bool Success, int Code)> DeleteMemoAsync(int id);

    /// <summary>
    /// 提供获取所有备忘录的仓储层类函数
    /// </summary>
    /// <returns>返回一个元组，包括memoInfoList和Code是0-成功</returns>
    Task<(IEnumerable<MemoInfo> memoInfoList,int Code)> GetAllMemosAsync();

    /// <summary>
    /// 提供搜索备忘录的仓储层服务
    /// </summary>
    /// <param name="searchText">搜索文本</param>
    /// <returns>返回一个元组，包括备忘录列表和Code是0-成功</returns>
    Task<(IEnumerable<MemoInfo> memoInfoList, int Code)> SearchMemosAsync(string searchText);

    /// <summary>
    /// 提供更新备忘录的仓储层服务
    /// </summary>
    /// <param name="memo">备忘录</param>
    /// <returns>返回0-成功；5-信息不全；10-找不到该备忘录；15-数据库操作结果不为1</returns>
    Task<int> UpdateMemoAsync(MemoInfo memo);



}
