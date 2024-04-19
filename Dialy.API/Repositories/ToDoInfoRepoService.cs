using Daily.API.DataModel;
using Microsoft.EntityFrameworkCore;
using System.Data.Common;

namespace Daily.API.Repositories;

/// <summary>
/// 实现待办事项信息仓储服务接口
/// </summary>
public class ToDoInfoRepoService : IToDoInfoRepoService
{
    private readonly DailyDbContext _db;

    /// <summary>
    /// 办事项信息仓储服务类的构造函数
    /// </summary>
    /// <param name="db">数据库上下文</param>
    public ToDoInfoRepoService(DailyDbContext db)
    {
        _db = db;
    }

    /// <summary>
    /// 实现添加待办事项信息的接口方法
    /// </summary>
    /// <param name="todo">待办事项</param>
    /// <returns>返回一个元组，Success判断成功与否，Code是0-成功；5-待办信息不全；异常-抛出</returns>
    public async Task<(bool Success, int Code)> AddToDoInfoAsync(ToDoInfo todo)
    {
        ArgumentNullException.ThrowIfNull(todo);

        if (string.IsNullOrEmpty(todo.Title) || string.IsNullOrEmpty(todo.Content))
            return (false, 5);

        try
        {
            await _db.AddAsync(todo);
            await _db.SaveChangesAsync();
            return (true, 0);
        }
        catch (DbUpdateException ex)
        {
            throw new InvalidOperationException("添加待办事项时发生数据库错误", ex);
        }
    }

    /// <summary>
    /// 实现删除待办事项信息的接口方法
    /// </summary>
    /// <param name="id">待办事项id</param>
    /// <returns>返回一个元组，Success判断成功与否，Code是0-成功；5-id非法；10：此id找不到待办事项；异常-抛出</returns>
    public async Task<(bool Success, int Code)> DeleteToDoInfoAsync(int id)
    {
        if(id <= 0)
            return (false, 5);

        try
        {
            var queryResult = await _db.ToDoInfo.FirstOrDefaultAsync(temp => temp.ToDoId == id);
            if(queryResult == null)
                return (false, 10);

            _db.Remove(queryResult);

            await _db.SaveChangesAsync();

            return (true, 0);

        }
        catch(DbUpdateException ex)
        {
            throw new InvalidOperationException("删除待办事项时发生数据库错误", ex);
        }
    }

    /// <summary>
    /// 实现根据条件查询待办事项信息的接口方法
    /// </summary>
    /// <param name="status">待办事项状态0-全部；1-待办中事项；2-已完成事项</param>
    /// <param name="searchText">搜索文本</param>
    /// <returns>返回一个元组，toDoInfoList待办事项列表，Success成功与否，Code是0-全部待办事项；5-按条件的全部待办事项；10-全部待办中事项；15-按条件的待办中事项；20-全部已完成事项；25-按条件的已完成事项；30-状态异常</returns>
    public async Task<(IEnumerable<ToDoInfo> toDoInfoList, bool Success, int Code)> GetConditionQueryToDoListAsync(int status, string? searchText)
    {
        try
        {
            switch (status)
            {
                case 0 when (string.IsNullOrEmpty(searchText) || searchText.Equals(null)):
                    return (await _db.ToDoInfo.AsNoTracking().ToListAsync(), true, 0);
                case 0:
                    return (await _db.ToDoInfo
                        .AsNoTracking()
                        .Where(temp => 
                        temp.Title.Contains(searchText) ||
                        temp.Content.Contains(searchText))
                        .ToListAsync(), true, 5);
                case 1 when string.IsNullOrEmpty(searchText):
                    return (await _db.ToDoInfo
                        .AsNoTracking()
                        .Where(temp => temp.Status == 0)
                        .ToListAsync(), true, 10);
                case 1:
                    return (await _db.ToDoInfo
                        .AsNoTracking()
                        .Where(temp => temp.Status == 0 &&                  (temp.Title.Contains(searchText) 
                        || temp.Content.Contains(searchText)))
                        .ToListAsync(), true, 15);
                case 2 when string.IsNullOrEmpty(searchText):
                    return (await _db.ToDoInfo.AsNoTracking().Where(temp => temp.Status == 1).ToListAsync(), true, 20);
                case 2:
                    return (await _db.ToDoInfo.AsNoTracking().Where(temp => temp.Status == 1 && (temp.Title.Contains(searchText) || temp.Content.Contains(searchText))).ToListAsync(), true, 25);
                default:
                    return (null!, false,30);
            }
        }
        catch (DbException ex)
        {
            throw new InvalidOperationException("查询待办事项时发生数据库错误", ex);
        }
    }

    /// <summary>
    /// 实现更新待办事项信息的接口方法
    /// </summary>
    /// <param name="todo">待办事项</param>
    /// <returns>返回一个元组，Success判断成功与否，Code是0-成功；5-id非法；10：此id找不到待办事项；15-信息不全;异常-抛出</returns>
    public async Task<(bool Success, int Code)> UpdateToDoInfoAsync(ToDoInfo todo)
    {
        ArgumentNullException.ThrowIfNull(todo);

        if(string.IsNullOrEmpty(todo.Content) || string.IsNullOrEmpty(todo.Title))
            return (false, 15);

        if (todo.ToDoId <= 0)
            return (false, 5);
        try
        {
            var queryResult = await _db.ToDoInfo.FirstOrDefaultAsync(temp => temp.ToDoId == todo.ToDoId);
            if (queryResult == null)
                return (false, 10);

            queryResult.Title = todo.Title;
            queryResult.Content = todo.Content;
            queryResult.Status = todo.Status;

            await _db.SaveChangesAsync();

            return (true, 0);

        }
        catch (DbUpdateException ex)
        {
            throw new InvalidOperationException("更新待办事项时发生数据库错误", ex);
        }

    }

    /// <summary>
    /// 实现更新待办事项状态的接口方法
    /// </summary>
    /// <returns>返回一个元组，total是总数，completed是已完成数，Success成功与否，Code是0-成功；5-失败</returns>
    public async Task<(int total, int completed, bool Success, int Code)> StatisticsToDoAsync()
    {
        try
        {
            var queryResult = await _db.ToDoInfo.ToListAsync();

            int total = queryResult.Count;
            int completed = queryResult.Count(temp => temp.Status == 1);
            return (total, completed, true, 0);
        }
        catch (DbException ex)
        {
            throw new InvalidOperationException("更新待办事项状态时发生数据库错误", ex);
        }

    }

    /// <summary>
    /// 实现更新待办事项状态的接口方法
    /// </summary>
    /// <param name="id">待办事项id</param>
    /// <returns>返回0-成功；5-id非法；10-找不到该待办事项</returns>
    public async Task<int> UpdateToDoStatus(int id)
    {
        if (id <= 0)
            return 5;
        var result = await _db.ToDoInfo.FirstOrDefaultAsync(temp => temp.ToDoId == id);

        if (result == null)
            return 10;

        result.Status = result.Status == 1 ? 0 : 1;

        _db.Update(result);

        await _db.SaveChangesAsync();

        return 0;

    }
}
