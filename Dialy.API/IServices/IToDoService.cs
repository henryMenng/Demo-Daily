using Daily.API.ApiReponses;
using Daily.API.Dtos;
using Microsoft.AspNetCore.Mvc;

namespace Daily.API.IServices;

/// <summary>
/// 提供待办事项相关的服务接口，如添加待办事项、获取待办事项列表等
/// </summary>
public interface IToDoService
{
    /// <summary>
    /// 提供获取待办事项统计信息的服务
    /// </summary>
    /// <returns>返回一个ApiResponse对象，包括统计信息等</returns>
    Task<ApiResponse> StatisticsToDoAsync();

    /// <summary>
    /// 提供添加待办事项的服务
    /// </summary>
    /// <param name="addToDoDto">要添加的待办事项Dto</param>
    /// <returns>返回一个ApiResponse对象，包括添加成功与否等</returns>
    Task<ApiResponse> AddToDoAsync(AddToDoDto addToDoDto);

    /// <summary>
    /// 提供获得待办中（未完成）事项列表的服务
    /// </summary>
    /// <returns>返回一个ApiResponse对象，包括待办中的事项等</returns>
    Task<ApiResponse> GetActiveToDoListAsync();

    /// <summary>
    /// 提供获得已完成事项列表的服务
    /// </summary>
    /// <returns>返回一个ApiResponse对象，包括已完成的事项等</returns>
    Task<ApiResponse> GetCompletedToDoListAsync();

    /// <summary>
    /// 提供改变待办事项状态的服务
    /// </summary>
    /// <param name="id">待办事项id</param>
    /// <returns>返回一个ApiResponse对象，包括更改成功与否等</returns>
    Task<ApiResponse> UpdateToDoStatusAsync(int id);

    /// <summary>
    /// 提供编辑待办事项的服务
    /// </summary>
    /// <param name="editToDoDto">要编辑的待办事项Dto</param>
    /// <returns>返回一个ApiResponse对象，包括编辑成功与否等</returns>
    Task<ApiResponse> EditToDoAsync(EditToDoDto editToDoDto);

    /// <summary>
    /// 提供获取所有待办事项列表的服务
    /// </summary>
    /// <returns>返回一个ApiResponse对象，包括所有的的待办事项等</returns>
    Task<ApiResponse> GetAllToDoListAsync();

    /// <summary>
    /// 获取符合条件的待办事项列表
    /// </summary>
    /// <param name="status">待办事项状态</param>
    /// <param name="searchText">搜索条件</param>
    /// <returns>返回一个ApiResponse对象，包括按条件查到的待办事项等</returns>
    Task<ApiResponse> GetConditionQueryToDoListAsync(int status, string? searchText);

    /// <summary>
    /// 提供删除待办事项的服务
    /// </summary>
    /// <param name="id">要删除的待办事项的id</param>
    /// <returns>返回一个ApiResponse对象，包括删除成功与否等</returns>
    Task<ApiResponse> DeleteToDoAsync(int id);

}
