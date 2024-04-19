using Daily.API.DataModel;
using Microsoft.EntityFrameworkCore;
using System.Data.Common;

namespace Daily.API.Repositories;

/// <summary>
/// 实现备忘录信息仓储服务接口，提供添加、删除、查询备忘录等功能
/// </summary>
public class MemoRepoService : IMemoRepoService
{
    /// <summary>
    /// 数据库上下文
    /// </summary>
    private readonly DailyDbContext _db;

    /// <summary>
    /// 备忘录信息仓储服务类的构造函数
    /// </summary>
    /// <param name="db">数据库上下文</param>
    public MemoRepoService(DailyDbContext db)
    {
        _db = db;
    }

    /// <summary>
    /// 实现添加备忘录的仓储层服务
    /// </summary>
    /// <param name="memo">备忘录</param>
    /// <returns>返回一个元组，0-成功添加；5-信息不全；10-操作数据库结果不为1；异常-抛出</returns>
    /// <exception cref="InvalidOperationException"></exception>
    public async Task<(bool Success, int Code)> AddMemoAsync(MemoInfo memo)
    {
        ArgumentNullException.ThrowIfNull(memo);

        try
        {
            if (string.IsNullOrEmpty(memo.Title) || string.IsNullOrEmpty(memo.Content))
                return (false, 5);

            await _db.MemoInfo.AddAsync(memo);

            var operaResult = await _db.SaveChangesAsync();

            return operaResult == 1 ? (true, 0) : (false, 10);
        }
        catch (DbUpdateException ex)
        {
            throw new InvalidOperationException(ex.Message);
        }
    }

    /// <summary>
    /// 删除备忘录
    /// </summary>
    /// <param name="id">备忘录id</param>
    /// <returns>返回一个元组，Success判断成功与否，Code是0-成功；5-id非法；10-找不到该备忘录；15-数据库操作结果不为1</returns>
    /// <exception cref="InvalidOperationException"></exception>
    public async Task<(bool Success,int Code)> DeleteMemoAsync(int id)
    {
        if (id <= 0)
            return (false, 5);

        try
        {
            var memo = await _db.MemoInfo.FindAsync(id);
            if (memo == null)
                return (false, 10);

            _db.MemoInfo.Remove(memo);

            var operaResult = await _db.SaveChangesAsync();

            return operaResult == 1 ? (true, 0) : (false, 15);
        }
        catch (DbException ex)
        {
            throw new InvalidOperationException(ex.Message);
        }
    }

    /// <summary>
    /// 实现获取所有备忘录的仓储层类函数
    /// </summary>
    /// <returns>返回一个元组，包括memoInfoList和Code是0-成功</returns>
    public async Task<(IEnumerable<MemoInfo> memoInfoList,int Code)> GetAllMemosAsync()
    {
        try
        {
            var queryResult = await _db.MemoInfo.AsNoTracking().ToListAsync();

            return (queryResult, 0);
        }
        catch (DbException ex)
        {
            throw new InvalidOperationException("查询备忘录时发生数据库错误", ex);
        }
        
    }

    /// <summary>
    /// 实现搜索备忘录的仓储层服务
    /// </summary>
    /// <param name="searchText">搜索文本</param>
    /// <returns>返回一个元组，包括备忘录列表和Code是0-成功</returns>
    /// <exception cref="InvalidOperationException"></exception>
    public async Task<(IEnumerable<MemoInfo> memoInfoList,int Code)> SearchMemosAsync(string searchText)
    {
        try
        {
            searchText = searchText ?? "";

            var queryResult = await _db.MemoInfo.AsNoTracking().Where(MemoInfo => MemoInfo.Title.Contains(searchText) || MemoInfo.Content.Contains(searchText)).ToListAsync();

            return (queryResult,0);
        }
        catch (DbException ex)
        {
            throw new InvalidOperationException(ex.Message);
        }
    }

    /// <summary>
    /// 实现更新备忘录的仓储层服务
    /// </summary>
    /// <param name="memo">备忘录</param>
    /// <returns>返回0-成功；5-信息不全；10-找不到该备忘录；15-数据库操作结果不为1</returns>
    public async Task<int> UpdateMemoAsync(MemoInfo memo)
    {
        ArgumentNullException.ThrowIfNull(memo);

        if (string.IsNullOrEmpty(memo.Content) || string.IsNullOrEmpty(memo.Title))
            return 5;

        try
        {
            var memoInfo = await _db.MemoInfo.FirstOrDefaultAsync(temp => temp.MemoId == memo.MemoId);

            if (memoInfo == null)
                return 10;

            _db.Entry(memoInfo).CurrentValues.SetValues(memo);

            _db.MemoInfo.Update(memoInfo);

            var result = await _db.SaveChangesAsync();

            return result == 1 ? 0 : 15;
        }
        catch (DbUpdateException ex)
        {
            throw new InvalidOperationException(ex.Message);
        }
    }
}
