using Daily.API.ApiReponses;
using Daily.API.DataModel;
using Daily.API.Dtos;
using Daily.API.IServices;
using Microsoft.AspNetCore.Mvc;

namespace Daily.API.Controllers;
/// <summary>
/// 备忘录接口
/// </summary>
[Route("api/[controller]/[action]")]
[ApiController]
public class MemoController : ControllerBase
{
    private readonly IMemoService _memoService;


    /// <summary>
    /// 备忘录接口的构造函数，注入服务，备忘录服务
    /// </summary>
    /// <param name="memoService">备忘录服务</param>
    public MemoController(IMemoService memoService)
    {
        _memoService = memoService;
    }

    /// <summary>
    /// 获取备忘录所有列表数据，解决总数和展示数据
    /// </summary>
    /// <returns>1:获取成功；-99异常</returns>
    [HttpGet]
    public async Task<ActionResult<ApiResponse>> GetAllMemoList() => Ok(await _memoService.GetAllMemoListAsync());

    /// <summary>
    /// 根据条件查询备忘录
    /// </summary>
    /// <param name="searchText">查询条件，是查询字符串</param>
    /// <returns></returns>
    [HttpGet]
    public async Task<ActionResult<ApiResponse>> GetConditionQueryMemoList(string searchText) => Ok(await _memoService.GetConditionQueryMemoListAsync(searchText));

    /// <summary>
    /// 删除备忘录
    /// </summary>
    /// <param name="id">备忘录id，通过查询字符串接收</param>
    /// <returns>1：成功；-1：Id异常；-99：异常</returns>
    [HttpGet]
    public async Task<ActionResult<ApiResponse>> DeleteMemo(int id) => Ok(await _memoService.DeleteMemoAsync(id));

    /// <summary>
    /// 添加备忘录
    /// </summary>
    /// <param name="memoInfoDto">备忘录信息Dto</param>
    /// <returns>1:添加成功；-99：异常；-1：添加失败</returns>
    [HttpPost]
    public async Task<ActionResult<ApiResponse>> AddMemo(MemoInfoDto memoInfoDto) => Ok(await _memoService.AddMemoAsync(memoInfoDto));
    

    /// <summary>
    /// 编辑备忘录方法
    /// </summary>
    /// <param name="editMemoDto">编辑备忘录Dto</param>
    /// <returns>1：编辑成功；-1：Dto异常；-99：异常</returns>
    [HttpPost]
    public async Task<ActionResult<ApiResponse>> EditMemo(EditMemoDto editMemoDto) => Ok(await _memoService.EditMemoAsync(editMemoDto));
}
