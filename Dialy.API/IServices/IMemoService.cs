using Daily.API.ApiReponses;
using Daily.API.Dtos;

namespace Daily.API.IServices;

/// <summary>
/// 提供与备忘录相关的服务接口，如添加、编辑、删除备忘录等
/// </summary>
public interface IMemoService
{
    /// <summary>
    /// 提供获得所有备忘录列表接口
    /// </summary>
    /// <returns>返回一个ApiResponse类型的对象，包括备忘录列表和异常信息等</returns>
    Task<ApiResponse> GetAllMemoListAsync(); 

    /// <summary>
    /// 提供添加备忘录接口
    /// </summary>
    /// <param name="memoInfoDto">备忘录Dto</param>
    /// <returns>返回一个ApiResponse类型的对象，包括添加是否成功和异常信息等</returns>
    Task<ApiResponse> AddMemoAsync(MemoInfoDto memoInfoDto);

    /// <summary>
    /// 提供编辑备忘录接口
    /// </summary>
    /// <param name="editMemoDto">编辑备忘录Dto</param>
    /// <returns>返回一个ApiResponse类型的对象，包括编辑是否成功和异常信息等</returns>
    Task<ApiResponse> EditMemoAsync(EditMemoDto editMemoDto);

    /// <summary>
    /// 提供删除备忘录接口
    /// </summary>
    /// <param name="id">备忘录id，查询字符串</param>
    /// <returns>返回也给ApiResponse类型的对象，包括删除成功与否等</returns>
    Task<ApiResponse> DeleteMemoAsync(int id);

    /// <summary>
    /// 提供根据条件获得备忘录列表接口
    /// </summary>
    /// <param name="searchText">查询条件</param>
    /// <returns>返回一个ApiResponse的对象，包括查询到的备忘录列表和异常信息等</returns>
    Task<ApiResponse> GetConditionQueryMemoListAsync(string searchText);
}
