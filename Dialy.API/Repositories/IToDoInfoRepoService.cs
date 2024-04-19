using Daily.API.DataModel;

namespace Daily.API.Repositories;

/// <summary>
/// 提供待办事项信息仓储服务接口
/// </summary>
public interface IToDoInfoRepoService
{
    /// <summary>
    /// 提供添加待办事项信息的接口方法
    /// </summary>
    /// <param name="todo">待办事项</param>
    /// <returns>返回一个元组，Success判断成功与否，Code是0-成功；5-待办信息不全；异常-抛出</returns>
    Task<(bool Success,int Code)> AddToDoInfoAsync(ToDoInfo todo);

    /// <summary>
    /// 提供删除待办事项信息的接口方法
    /// </summary>
    /// <param name="id">待办事项id</param>
    /// <returns>返回一个元组，Success判断成功与否，Code是0-成功；5-id非法；10：此id找不到待办事项；异常-抛出</returns>
    Task<(bool Success, int Code)> DeleteToDoInfoAsync(int id);

    /// <summary>
    /// 提供更新待办事项信息的接口方法
    /// </summary>
    /// <param name="todo">待办事项</param>
    /// <returns>返回一个元组，Success判断成功与否，Code是0-成功；5-id非法；10：此id找不到待办事项；15-信息不全;异常-抛出</returns>
    Task<(bool Success, int Code)> UpdateToDoInfoAsync(ToDoInfo todo);

    /// <summary>
    /// 提供根据条件查询待办事项信息的接口方法
    /// </summary>
    /// <param name="status">待办事项状态0-全部；1-待办中事项；2-已完成事项</param>
    /// <param name="searchText">搜索文本</param>
    /// <returns>返回一个元组，toDoInfoList待办事项列表，Success成功与否，Code是0-全部待办事项；5-按条件的全部待办事项；10-全部待办中事项；15-按条件的待办中事项；20-全部已完成事项；25-按条件的已完成事项；30-状态异常</returns>
    Task<(IEnumerable<ToDoInfo> toDoInfoList,bool Success,int Code)> GetConditionQueryToDoListAsync(int status,string? searchText);

    /// <summary>
    /// 提供更新待办事项状态的接口方法
    /// </summary>
    /// <returns>返回一个元组，total是总数，completed是已完成数，Success成功与否，Code是0-成功；5-失败</returns>
    Task<(int total,int completed,bool Success,int Code)> StatisticsToDoAsync();

    /// <summary>
    /// 实现更新待办事项状态的接口方法
    /// </summary>
    /// <param name="id">待办事项id</param>
    /// <returns>返回0-成功；5-id非法；10-找不到该待办事项</returns>
    Task<int> UpdateToDoStatus(int id );
}
